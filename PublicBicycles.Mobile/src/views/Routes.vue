<template>
  <div class="container">
    <map-view ref="map" map-type="routes" :enableSearch="true" @gotStations="gotStations"></map-view>
  </div>
</template>
<script>
import Vue from "vue";
import "../map/ClusterLayer";
import Cookies from "js-cookie";
import Map from "../components/Map";
export default Vue.extend({
  name: "Routes",
  data() {
    return {
      bicycles: [],
      map: new Map({}),
      drawerDetail: false,
      stations: [],
      station: undefined,
      currentHire: undefined,
      searchContent: ""
    };
  },
  components: {
    "map-view": Map,
  },
  computed: {},
  methods: {
    searchSelected(e) {
      this.$refs.map.panTo([e.lng, e.lat]);
    },
    gotStations(e) {
      this.stations = e;
    },
  },
  mounted: function() {
    this.$nextTick(function() {
      if (Cookies.get("userID") == undefined) {
        return;
      }
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

.return-btn {
  float: right;
  margin-top: 16px;
  margin-right: 8px;
}
.station-title {
  margin-top: 8px;
  margin-left: 12px;
  font-size: 1.25em;
}
</style>