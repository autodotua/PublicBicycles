<template>
  <div class="container">
    <map-view @select="stationSelected" map-type="normal"></map-view>
    <div id="hire-bar" class="bar" v-show="currentHire">
      <a>从{{currentHire?formatDateTime(currentHire.hireTime):""}}起借车</a>
    </div>
    <el-drawer
      title
      :visible.sync="drawerDetail"
      :with-header="false"
      direction="btt"
      size="360px"
      class="bicycles"
    >
      <h2 style="float:left">{{station?station.name:""}}</h2>

      <el-button
        class="return-btn"
        plain
        size="small"
        style="float:right"
        v-show="currentHire"
        @click="returnBicycle"
      >还车</el-button>
      <el-table :data="bicycles" style="width: 100%">
        <el-table-column prop="id" label="ID" width="160"></el-table-column>
        <el-table-column fixed="right" label="操作" width="100">
          <template slot-scope="scope">
            <el-button @click="hireBicycle(scope.row)" type="text" size="small">借车</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-drawer>
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
  showNotify
} from "../common";
import Map from "../components/Map";
export default Vue.extend({
  name: "Home",
  data() {
    return {
      bicycles: [],
      map: new Map({}),
      drawerDetail: false,
      station: undefined,
      currentHire: undefined
    };
  },
  comments: {
    "map-view": Map
  },
  computed: {},
  methods: {
    formatDateTime: formatDateTime,
    jump: jump,
    getImageUrl(id) {
      return getUrl("Home", "PublicBicyclesImage") + "/" + id;
    },
    returnBicycle() {
      console.log(this.currentHire)
      Vue.axios
        .post(
          getUrl("User", "Return"),
          withToken({
            bicycleID: this.currentHire.bicycle.id,
            stationID: this.station.id
          })
        )
        .then(response => {
          if (response.data.succeed) {
            showNotify("还车成功");
            this.currentHire =undefined;
          } else {
            showError(response.data.message);
          }
        })
        .catch(showError);
    },
    hireBicycle(item) {
      Vue.axios
        .post(
          getUrl("User", "Hire"),
          withToken({
            bicycleID: item.id,
            stationID: this.station.id
          })
        )
        .then(response => {
          if (response.data.succeed) {
            showNotify("借车成功");
            this.currentHire = response.data.data;
          } else {
            showError(response.data.message);
          }
        })
        .catch(showError);
    },

    stationSelected(station) {
      this.drawerDetail = true;
      this.station = station;
      Vue.axios
        .get(getUrl("Map", "Bicycles") + `/${station.id}`)
        .then(response => {
          this.bicycles = response.data.data;
        })
        .catch(showError);
    }
  },
  components: {},
  mounted: function() {
    this.$nextTick(function() {
      if (Cookies.get("userID") == undefined) {
        return;
      }
      Vue.axios
        .post(getUrl("User", "Status"), withToken({}))
        .then(response => {
          this.currentHire = response.data.data;
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
  margin-left: 12px;
}

.bar {
  width: 100%;
  height: 48px;
  position: absolute;
  top: 60px;
  left: 0;
  background: orange;
  line-height: 48px;
}
.bar a {
  margin-left: 12px;
  color: #ffffff;
}

.return-btn {
  float: right;
  margin-top: 16px;
  margin-right: 8px;
}
</style>