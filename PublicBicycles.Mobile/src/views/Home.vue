<template>
  <div>
    <link
      rel="stylesheet"
      href="https://cdn.jsdelivr.net/gh/openlayers/openlayers.github.io@master/en/v6.3.1/css/ol.css"
      type="text/css"
    />

    <div id="map"></div>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import Cookies from "js-cookie";
import ol, { Map, View, source, layer, proj, Feature } from "openlayers";
import { withToken, getUrl, showError, jump, formatDateTime } from "../common";
export default Vue.extend({
  name: "Home",
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
  computed: {},
  methods: {
    jump: jump,
    getImageUrl(id: number): string {
      return getUrl("Home", "PublicBicyclesImage") + "/" + id;
    },
    addLayer(url: string) {
      this.map.addLayer(
        new ol.layer.Tile({
          source: new ol.source.XYZ({
            url: url
          })
        })
      );
    },
    initMap() {
      this.map = new ol.Map({
        target: "map",
        layers: [],
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
          console.log(response);

          const features = new Array<Feature>();
          for (const station of response.data.data) {
            const point = new ol.geom.Point(
              ol.proj.fromLonLat([station.lng, station.lat])
            );
            const feature = new Feature(point);
            features.push(feature);
          }

          const layer = new ol.layer.Vector({
            source: new ol.source.Vector({
              features: features
            }),
            style: function(feature) {
              const style = new ol.style.Style({
                image: new ol.style.Circle({
                  radius: 7,
                  fill: new ol.style.Fill({ color: "green" }),
                  stroke: new ol.style.Stroke({
                    color: [255, 0, 0,255],
                    width: 2
                  })
                })
              });
              return style;
            }
          });
          this.map.addLayer(layer);
        })
        .catch(showError);
    });
  }
});
</script>
