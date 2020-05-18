<template>
  <div class="container">
    <map-view ref="map" @select="stationSelected" map-type="normal" @gotStations="gotStations"></map-view>

    <el-autocomplete
      class="search"
      :style="searchStyle"
      v-model="searchContent"
      :fetch-suggestions="querySearch"
      placeholder="请输入内容"
      @select="searchSelect"
    ></el-autocomplete>
    <el-drawer
      title
      :visible.sync="drawerDetail"
      :with-header="false"
      direction="btt"
      size="360px"
      class="bicycles"
    >
      <a class="station-title" style="float:left">
        <b>{{station?station.name:""}}</b>
        {{station?station.bicycleCount:""}}/{{station?station.count:""}}
      </a>

      <el-dropdown class="station-menu" @command="handleDropdownCommand">
        <span class="el-dropdown-link station-menu">
          操作
          <i class="el-icon-arrow-down el-icon--right"></i>
        </span>
        <el-dropdown-menu slot="dropdown">
          <el-dropdown-item command="deleteStation">删除车站</el-dropdown-item>
          <el-dropdown-item command="addBicycle">新增自行车</el-dropdown-item>
        </el-dropdown-menu>
      </el-dropdown>

      <el-table :data="bicycles" style="width: 100%" height="280">
        <el-table-column prop="id" label="ID" width="160"></el-table-column>
        <el-table-column fixed="right" label="操作" width="100">
          <template slot-scope="scope">
            <el-button @click="deleteBicycle(scope.row)" type="text" size="small">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-drawer>
    <el-drawer
      title
      :visible.sync="showAddBicycle"
      :with-header="false"
      size="180px"
      class="bicycles"
    >
      <a class="station-title" style="float:left">
        <b>添加自行车</b>
      </a>
      <div v-show="showAddBicycle" style="margin: 48px 12px 0 12px">
        <a>自行车ID：</a>
        <br />
        <el-input v-model="bicycle.bicycleID" size="small"></el-input>
        <br />
        <br />
        <el-button type="primary" @click="addBicycle" size="small">确定</el-button>
        <el-button @click="showAddBicycle=false" size="small">取消</el-button>
      </div>
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
      bicycle: { bicycleID: 0 },
      bicycles: [],
      map: new Map({}),
      drawerDetail: false,
      stations: [],
      station: undefined,
      searchContent: "",
      showAddBicycle: false
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
    handleDropdownCommand(cmd) {
      setTimeout(() => {
        switch (cmd) {
          case "deleteStation":
            break;
          case "deleteBicycle":
            break;
          case "addBicycle":
            this.showAddBicycle = true;
            this.drawerDetail = false;
            break;
        }
      }, 100);
    },
    addBicycle() {
      console.log("");
    },
    deleteBicycle(id) {
      console.log(id);
    },
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

.bicycles .el-table {
  margin: 12px;
}
.bicycles h2 {
  margin-left: 12px;
}

.station-menu {
  float: right;
  margin-top: 8px;
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