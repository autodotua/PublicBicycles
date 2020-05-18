<template>
  <div class="container">
    <link
      rel="stylesheet"
      href="https://cdn.jsdelivr.net/gh/openlayers/openlayers.github.io@master/en/v6.3.1/css/ol.css"
      type="text/css"
    />
    <div id="map"></div>
  </div>
</template>
<script >
import Vue from "vue";
import "../map/ClusterLayer";
import Cookies from "js-cookie";
import ol, { Map, View, proj, Feature } from "openlayers";
import { withToken, getUrl, showError, jump, formatDateTime } from "../common";
export default Vue.component("map-view", {
  data() {
    return {
      stations: [],
      map: new Map({}),
      stationLayer: new ol.layer.Vector({
        source: new ol.source.Vector({
          features: []
        })
      }),
      routesLayer: undefined
    };
  },
  props: {
    mapType: {
      default: "normal"
    }
  },
  computed: {},
  methods: {
    jump: jump,
    addLayer(url) {
      this.map.addLayer(
        new ol.layer.Tile({
          source: new ol.source.XYZ({
            url: url
          })
        })
      );
    },

    getStyle(feature,selected) {
      return new ol.style.Style({
        image: new ol.style.Icon({
          src: `../img/bicycle${selected?"_selected":""}.png`,
          scale: 1.0 / 6
        }),

        text: new ol.style.Text({
          offsetY: 24,
          fill: new ol.style.Fill({
            color: "#000"
          }),
          stroke: new ol.style.Stroke({
            color: "#fff",
            width: 3
          }),
          text: feature.getProperties().object.name
        })
      });
    },
    initMap() {
      this.map = new ol.Map({
        target: "map",
        layers: [],
        controls: ol.control.defaults({
          attribution: false,
          zoom: false
        }),
        view: new View({
          center: proj.fromLonLat([121.56, 29.86]),
          zoom: 12
        })
      });
      this.addLayer(
        "http://t0.tianditu.com/vec_w/wmts?service=WMTS&request=GetTile&version=1.0.0&layer=vec&style=default&TILEMATRIXSET=w&format=tiles&height=256&width=256&tilematrix={z}&tilerow={y}&tilecol={x}&tk=9396357d4b92e8e197eafa646c3c541d"
      );
      this.addLayer(
        "http://t0.tianditu.com/cva_w/wmts?service=WMTS&request=GetTile&version=1.0.0&layer=cva&style=default&TILEMATRIXSET=w&format=tiles&height=256&width=256&tilematrix={z}&tilerow={y}&tilecol={x}&tk=9396357d4b92e8e197eafa646c3c541d"
      );
      this.map.addLayer(this.stationLayer);
      const selection = new ol.interaction.Select({
        condition: ol.events.condition.click,
        style: feature => this.getStyle(feature, true)
      });
      this.map.addInteraction(selection);
      selection.on("select", e => {
        console.log(e);
        if (e.selected.length > 0) {
          const station = e.selected[0].getProperties().object;
          //this.stationName = (e as any).selected[0].getProperties().object.name;
          setTimeout(() => {
            if (this.mapType == "routes") {
              this.loadRoutes(station);
            } else {
              //这个事件触发太早了，不延迟的话，抽屉会识别到外部点击事件然后马上收回
              this.$emit("select", station);
            }
          }, 200);
        }
      });
    },
    loadCluster(stations) {
      const data = [];
      for (const station of stations) {
        data.push({ lat: station.lat, lon: station.lng });
      }
      const cluster = ol.layer.ClusterLayer({
        map: this.map,
        clusterField: "",
        zooms: [15],
        distance: 100,
        data: data,
        style: null
      });
      this.map.addLayer(cluster);
    },
    loadStations(features) {
      const layer = new ol.layer.Vector({
        maxResolution: 6, //越小越晚出现
        source: new ol.source.Vector({
          features: features
        }),
        style: feature => this.getStyle(feature)
      });
      this.stationLayer = layer;
      this.map.addLayer(layer);
    },
    loadHeatmap(features) {
      const heatmap = new ol.layer.Heatmap({
        source: new ol.source.Vector({
          features: features
        }),
        radius: 10
      });
      this.map.addLayer(heatmap);
    },
    loadRoutes(station) {
      Vue.axios
        .post(
          getUrl("Analysis", "Routes"),
          withToken({ stationID: station.id, days: 5 })
        )
        .then(response => {
          const currentCoord = ol.proj.fromLonLat([station.lng, station.lat]);
          const routes = response.data.data;
          const features = [];
          const addToFeatures = (items, type) => {
            for (const stationID of Object.keys(items)) {
              const targetStation = this.stations.find(p => p.id == stationID);
              const targetCoord = ol.proj.fromLonLat([
                targetStation.lng,
                targetStation.lat
              ]);
              const feature = new Feature({
                geometry: new ol.geom.LineString([currentCoord, targetCoord]),
                weight: items[stationID],
                type
              });
              features.push(feature);
            }
          };
          addToFeatures(routes.in, "in");
          addToFeatures(routes.out, "out");
          if (this.routesLayer) {
            this.map.removeLayer(this.routesLayer);
          }
          this.routesLayer = new ol.layer.Vector({
            //maxResolution: 6, //越小越晚出现
            source: new ol.source.Vector({
              features: features
            }),
            style: feature => {
              const t=feature.getProperties()["type"];
              return new ol.style.Style({
                stroke: new ol.style.Stroke({
                  color: t=="in"?"#33FF55":"#FF0000",
                  width: feature.getProperties()["weight"]*3,
                })
              });
            }
          });
          this.map.addLayer(this.routesLayer);
        })
        .catch(showError);
    },
    panTo(loc) {
      this.map.getView().animate({
        duration: 300,
        rotation: 0,
        zoom: 16,
        center: ol.proj.fromLonLat(loc)
      });
    }
  },
  components: {},
  mounted: function() {
    this.$nextTick(function() {
      if (Cookies.get("userID") == undefined) {
        return;
      }
      this.initMap();
      Vue.axios
        .get(getUrl("Map", "Stations"))
        .then(response => {
          const features = [];
          this.stations = response.data.data;
          this.$emit("gotStations", response.data.data);
          for (const station of response.data.data) {
            const point = new ol.geom.Point(
              ol.proj.fromLonLat([station.lng, station.lat])
            );
            const feature = new Feature({
              geometry: point,
              object: station
            });
            features.push(feature);
          }
          switch (this.mapType) {
            case "normal":
            case "routes":
              this.loadCluster(response.data.data);
              this.loadStations(features);
              break;
            case "heatmap":
              this.loadHeatmap(features);
              break;
          }
        })
        .catch(showError);
    });
  }
});
</script>
<style scoped>
#map {
  width: 100%;
  height: 100%;
}
.container {
  height: 100%;
  width: 100%;
}

.bicycles .el-table {
  margin: 12px;
}
.bicycles h2 {
  margin-top: -12px;
  margin-left: 12px;
}
</style>