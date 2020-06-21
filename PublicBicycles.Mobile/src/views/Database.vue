<template>
  <div class="container">
    <div v-title data-title="数据库 - 公共自行车"></div>
    <el-input v-model="days"></el-input>
    <br />
    <el-button @click="generateTestDatas()">生成测试数据</el-button>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import Cookies from "js-cookie";
import { withToken, getUrl, showError, showNotify } from "../common";
export default Vue.extend({
  name: "Database",
  data() {
    return { days: 5 };
  },
  methods: {
    generateTestDatas() {
      showNotify("请稍后");
      Vue.axios
        .post(getUrl("Admin", "Fake"), withToken({ days: this.days }))
        .then(response => {
          showNotify("操作成功");
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