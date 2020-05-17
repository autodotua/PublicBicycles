<template>
  <div id="app">
    <el-container>
      <el-header class="header" v:show="showHeader">
        <el-button type="text" icon="el-icon-menu" style="float:left" @click="menu=true"></el-button>
        <el-button type="text" style="float:right" @click="clickUsername">{{username}}</el-button>
        <h3 style="float:left;margin-left:12px" @click="jump('')">公共自行车</h3>
        <slot name="header"></slot>
      </el-header>
      <el-main>
        <router-view></router-view>
      </el-main>
      <el-drawer title :visible.sync="menu" :with-header="false" direction="ltr" size="240px">
        <el-menu router="true" @select="menuSelect">
          <el-menu-item index="home">
            <i class="el-icon-menu"></i>
            <span slot="title">地图</span>
          </el-menu-item>
          <el-menu-item index="records">
            <i class="el-icon-document"></i>
            <span slot="title">借车记录</span>
          </el-menu-item>
        </el-menu>
      </el-drawer>
    </el-container>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import Cookies from "js-cookie";
import { jump } from "./common";
export default Vue.extend({
  name: "App",
  data: function() {
    return {
      showHeader: true,
      menu: false
    };
  },
  computed: {
    username() {
      return Cookies.get("username");
    }
  },
  mounted: function() {
    this.$nextTick(function() {
      const url = window.location.href;
      if (url.indexOf("login") >= 0) {
        this.showHeader = false;
      } else {
        if (Cookies.get("userID") == undefined) {
          jump("login");
        }
      }
    });
  },
  methods: {
    jump: jump,
    menuSelect(index: number) {
      console.log(index);
      this.menu = false;
      // switch (index) {
      //   case 1:
      //     jump("map");
      //     break;
      //   case 2:
      //     jump("records");
      //     break;
      // }
    },
    clickUsername() {
      this.$confirm("是否退出账户？", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      }).then(() => {
        Cookies.remove("username");
        Cookies.remove("userID");
        Cookies.remove("token");
        location.reload();
      });
    }
  }
});
</script>
<style >
.header-title {
  float: left;
  margin-top: 0px;
}
.el-message-box {
  width: auto !important;
}

html,
body,
#app,
.el-container,
.el-main {
  overflow-x: hidden;
  width: 100%;
  height: 100%;
  padding: 0px !important;
  margin: 0px;
}
</style>
<style scoped>
/* #app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}
header a {
  text-decoration: none;
} */

.header {
  height: 48px;
  margin-left: -12px;
  margin-right: -12px;
  /* margin-top: -12px; */
  background: #ebeef5;
  color: #606266;
}

.header button {
  color: #606266;
  margin-top: 12px;
}
</style>
