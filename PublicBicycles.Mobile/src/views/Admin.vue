<template>
  <div class="container">
    <div v-title data-title="管理 - 公共自行车"></div>
    <map-view
      ref="map"
      @select="stationSelected"
      map-type="normal"
      :enableClick="true"
      @gotStations="gotStations"
      @click="mapClick"
      :enableSearch="true"
    ></map-view>

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
          <el-dropdown-item command="deleteStation">删除租赁点</el-dropdown-item>
          <el-dropdown-item command="editStation">修改租赁点</el-dropdown-item>
          <el-dropdown-item command="addOrEditBicycle">新增自行车</el-dropdown-item>
        </el-dropdown-menu>
      </el-dropdown>

      <el-table :data="bicycles" style="width: 100%" height="280">
        <el-table-column prop="bicycleID" label="ID" width="160"></el-table-column>
        <el-table-column fixed="right" label="操作" width="100">
          <template slot-scope="scope">
            <el-button type="text" size="small" @click="editBicycle(scope.row)">修改</el-button>

            <el-popconfirm title="确认删除？" @onConfirm="deleteBicycle(scope.row)">
              <el-button slot="reference" type="text" size="small">删除</el-button>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>
    </el-drawer>
    <el-drawer
      title
      :visible.sync="showSidePanel"
      :with-header="false"
      size="180px"
      class="bicycles"
    >
      <a class="station-title" style="float:left">
        <b>{{addingBicycle?"自行车":"租赁点"}}</b>
      </a>
      <div v-show="addingBicycle" class="add-form">
        <a>自行车ID：</a>
        <el-input v-model="bicycle.bicycleID" size="small"></el-input>
        <a>是否可借：</a>
        <el-switch
          style="display: block"
          v-model="bicycle.canHire"
          active-color="#13ce66"
          inactive-color="#ff4949"
        ></el-switch>
        <el-button type="primary" @click="addOrEditBicycle" size="small">确定</el-button>
        <el-button @click="addingBicycle=false" size="small">取消</el-button>
      </div>
      <div v-show="addingStation" class="add-form">
        <a>名称：</a>
        <el-input v-model="station.name" size="small"></el-input>
        <a>地址：</a>
        <el-input v-model="station.address" size="small"></el-input>
        <a>桩位：</a>
        <el-input-number v-model="station.count" size="small"></el-input-number>
        <a>经度：</a>
        <el-input v-model="station.lng" size="small" type="number"></el-input>
        <a>纬度：</a>
        <el-input v-model="station.lat" size="small" type="number"></el-input>
        <el-button type="primary" @click="addOrEditStation" size="small">确定</el-button>
        <el-button @click="addingStation=false" size="small">取消</el-button>
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
      bicycle: { bicycleID: 0, canHire: true },
      operation: "",
      bicycles: [],
      map: new Map({}),
      drawerDetail: false,
      stations: [],
      station: {
        name: "",
        address: "",
        count: 25,
        lng: 0,
        lat: 0
      },
      searchContent: "",
      addingBicycle: false,
      addingStation: false
    };
  },
  components: {
    "map-view": Map
  },
  computed: {
    showSidePanel() {
      return this.addingBicycle || this.addingStation;
    }
  },
  methods: {
    /**
     * 地图单击
     */
    mapClick(coord) {
      //开始新增租赁点
      this.addingStation = true;
      this.station = {
        name: "",
        address: "",
        count: 25,
        lng: coord[0],
        lat: coord[1]
      };
      this.operation = "add";
    },
    /**
     * 增加或编辑租赁点，单击“确定”按钮触发
     */
    addOrEditStation() {
      Vue.axios
        .post(
          getUrl("Admin", "Station"),
          withToken({
            item: this.station,
            type: this.operation
          })
        )
        .then(response => {
          if (response.data.succeed) {
            showNotify((this.operation == "edit" ? "编辑" : "添加") + "成功");
            this.$refs.map.loadDatas();
            this.addingStation = false;
            this.stations.slice(this.stations.indexOf(this.station), 1);
          } else {
            showError(response.data.message);
          }
        })
        .catch(showError);
    },
    /**
     * 处理自行车列表抽屉右上角的下拉框的单击事件
     */
    handleDropdownCommand(cmd) {
      setTimeout(() => {
        switch (cmd) {
          case "deleteStation":
            this.deleteStation();
            break;
          case "editStation":
            this.addingStation = true;
            this.operation = "edit";
            break;
          case "addOrEditBicycle":
            this.addingBicycle = true;
            this.drawerDetail = false;
            this.operation = "add";
            break;
        }
      }, 100);
    },
    /**
     * 删除站点
     */
    deleteStation() {
      this.$confirm("是否删除该站点?", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      }).then(() => {
        Vue.axios
          .post(
            getUrl("Admin", "Station"),
            withToken({
              item: { id: this.station.id },
              type: "delete"
            })
          )
          .then(response => {
            if (response.data.succeed) {
              showNotify("删除成功");
              this.$refs.map.loadDatas();
              this.drawerDetail = false;
              this.stations.slice(this.stations.indexOf(this.station), 1);
            } else {
              showError(response.data.message);
            }
          })
          .catch(showError);
      });
    },
    /**
     * 开始编辑自行车，单击表格上的按钮触发
     */
    editBicycle(bicycle) {
      this.bicycle = bicycle;
      this.operation = "edit";
      this.addingBicycle = true;
      this.drawerDetail = false;
    },
    /**
     * 增加或编辑自行车，单击“确定”按钮触发
     */
    addOrEditBicycle() {
      Vue.axios
        .post(
          getUrl("Admin", "Bicycle"),
          withToken({
            item: {
              id: this.bicycle.id,
              bicycleID: this.bicycle.bicycleID,
              canHire: this.bicycle.canHire,
              station: { id: this.station.id }
            },
            type: this.operation
          })
        )
        .then(response => {
          if (response.data.succeed) {
            showNotify("新建成功");
            this.addingBicycle = false;
          } else {
            showError(response.data.message);
          }
        })
        .catch(showError);
    },
    /**
     * 删除自行车
     */
    deleteBicycle(bicycle) {
      Vue.axios
        .post(
          getUrl("Admin", "Bicycle"),
          withToken({
            item: { id: bicycle.id, bicycleID: 0 },
            type: "delete"
          })
        )
        .then(response => {
          if (response.data.succeed) {
            showNotify("删除成功");
            this.bicycles.splice(this.bicycles.indexOf(bicycle), 1);
          } else {
            showError(response.data.message);
          }
        })
        .catch(showError);
    },
    searchSelected(e) {
      this.$refs.map.panTo([e.lng, e.lat]);
    },
    gotStations(e) {
      this.stations = e;
    },
    formatDateTime: formatDateTime,
    jump: jump,

    /**
     * 租赁点被选中的事件
     */
    stationSelected(station) {
      this.drawerDetail = true;
      this.station = station;
      //获取该站点下的自行车
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
.add-form {
  margin: 48px 12px 0 12px;
}
.add-form a {
  display: block;
  margin-top: 6px;
  margin-bottom: 6px;
}
.add-form .el-input,
.el-switch {
  display: block;
  margin-top: 6px;
  margin-bottom: 12px;
}
.cell .el-button {
  margin-left: 6px;
  margin-right: 6px;
}
</style>