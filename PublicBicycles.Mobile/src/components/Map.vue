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
<script>
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
      })
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
    getImageUrl(id) {
      return getUrl("Home", "PublicBicyclesImage") + "/" + id;
    },
    hire(id) {
      console.log(id);
    },
    addLayer(url) {
      this.map.addLayer(
        new ol.layer.Tile({
          source: new ol.source.XYZ({
            url: url
          })
        })
      );
    },
    showBicycles(station) {
      Vue.axios
        .get(getUrl("Map", "Bicycles") + `/${station.id}`)
        .then(response => {
          this.bicycles = response.data.data;
        })
        .catch(showError);
    },
    getStyle(feature, scale = 1) {
      return new ol.style.Style({
        image: new ol.style.Icon({
          src: "../img/bicycle.png",
          scale: scale / 6
        }),

        text: new ol.style.Text({
          offsetY: 18 + scale * 6,
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
        style: feature => this.getStyle(feature, 2)
      });
      this.map.addInteraction(selection);
      selection.on("select", e => {
        console.log(e);
        if (e.selected.length > 0) {
          this.stationName = e.selected[0].getProperties().object.name;
          setTimeout(() => {
            //这个事件触发太早了，不延迟的话，抽屉会识别到外部点击事件然后马上收回
            this.$emit("select", e.selected[0].getProperties().object);
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
        loadWhileAnimating: false,
        radius: 10
      });
      this.map.addLayer(heatmap);
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