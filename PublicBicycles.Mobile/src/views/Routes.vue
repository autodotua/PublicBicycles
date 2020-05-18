<template>
  <div class="container">
    <map-view ref="map" @select="stationSelected" map-type="routes" @gotStations="gotStations"></map-view>

    <el-autocomplete
      class="search"
      :style="searchStyle"
      v-model="searchContent"
      :fetch-suggestions="querySearch"
      placeholder="请输入内容"
      @select="searchSelect"
    ></el-autocomplete>
  </div>
</template>
<script>
import Vue from "vue";
import "../map/ClusterLayer";
import Cookies from "js-cookie";
import {
  withToken,
  getUrl,
  showError,
  jump,
  formatDateTime,
  showNotify,
} from "../common";
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
    "map-view": Map
  },
  computed: {
    searchStyle() {
      if (this.currentHire == null) {
        return "top:72px;";
      }
      return "top:120px;";
    }
  },
  methods: {
    searchSelect(e) {
      console.log("选择到了", e);
      this.$refs.map.panTo([e.lng, e.lat]);
    },
    gotStations(e) {
      this.stations = e;
    },
    formatDateTime: formatDateTime,
    jump: jump,
    querySearch(queryString, callback) {
      if (this.searchContent) {
        const result = this.stations.filter(
          p => p.name.indexOf(this.searchContent) >= 0
        );
        const values = [];
        for (const station of result) {
          values.push(Object.assign({ value: station.name }, station));
        }
        callback(values);
      }
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
.search {
  position: absolute;
  /* top: 120px; */
  left: 24px;
  right: 24px;
}
</style>