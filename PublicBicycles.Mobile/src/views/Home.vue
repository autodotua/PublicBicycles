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
import ol, { Map, View, proj, Feature } from "openlayers";
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
    getStyle(feature: ol.Feature | ol.render.Feature,scale=1): ol.style.Style {
      return      new ol.style.Style({
        image: new ol.style.Icon({
          src: "../img/bicycle.svg",
          scale: scale
        }),

        text: new ol.style.Text({
          offsetY: 18+scale*6,
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
        style: feature => this.getStyle(feature,2)
      });
      this.map.addInteraction(selection);
      selection.on("select", e => {
        console.log(e);
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
          console.log(response);

          const features = new Array<Feature>();
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

          const layer = new ol.layer.Vector({
            source: new ol.source.Vector({
              features: features
            }),
            style:feature=>this.getStyle(feature)
          });
          this.stationLayer = layer;
          this.map.addLayer(layer);
        })
        .catch(showError);
    });
  }
});
</script>
