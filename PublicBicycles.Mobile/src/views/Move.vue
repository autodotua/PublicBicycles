<template>
  <div class="container">
    <div v-title data-title="车辆平衡 - 公共自行车"></div>
    <map-view
      ref="map"
      map-type="no-cluster"
      @gotStations="gotStations"
      :autoLoad="false"
      :filter="filter"
      @select="select"
    ></map-view>
    <div class="bottom-bar">  
        <el-switch
      class="fullOrEmpty"
      active-color="#50a0fc"
      inactive-color="#50a0fc"
      v-model="full"
      active-text="车辆过多"
      inactive-text="车辆过少"
    ></el-switch>
    <a class="detail">{{bikes}} / {{count}}    </a>
    </div>

  </div>
</template>
<script>
import Vue from "vue";
import "../map/ClusterLayer";
import Cookies from "js-cookie";
import Map from "../components/Map";
import { withToken, getUrl, showError, formatDateTime } from "../common";
export default Vue.extend({
  name: "Move",
  data() {
    return {
      filter: [],
      full: false,
      emptys: [],
      fulls: [],
      bikes:'剩余车辆',
      count:'桩位总数'
    };
  },
  components: {
    "map-view": Map
  },
  computed: {},
  methods: {
    searchSelected(e) {
      this.$refs.map.panTo([e.lng, e.lat]);
    },
    gotStations(e) {
      this.stations = e;
    },
    select(station){
      this.bikes=station.bicycleCount;
      this.count=station.count;
    }
  },
  watch: {
    full: function() {
      this.filter = this.full ? this.fulls : this.emptys;
      console.log(this.filter);
      
      this.$refs.map.loadDatas();
    }
  },
  mounted: function() {
    this.$nextTick(function() {
      if (Cookies.get("userID") == undefined) {
        return;
      }
      Vue.axios
        .post(getUrl("Analysis", "Move"), withToken({ days: this.days }))
        .then(response => {
          this.fulls = response.data.data.full;
          this.emptys = response.data.data.empty;
          this.filter = this.emptys;
          this.$refs.map.loadDatas();
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

.bottom-bar {
  position: absolute;
  padding: 12px;
  bottom: 0;
  left: 0;
  right: 0;
  background: #dddddd88;
}
.fullOrEmpty{
margin-left: 12px;
}
.detail{
  float: right;
  margin-right: 12px;
}
</style>