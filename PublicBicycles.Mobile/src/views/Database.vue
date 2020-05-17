<template>
  <div class="container">
    <el-button @click="generateTestDatas()">生成测试数据</el-button>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import Cookies from "js-cookie";
import {
  withToken,
  getUrl,
  showError,
  showNotify
} from "../common";
export default Vue.extend({
  name: "Database",
  data() {
    return {};
  },
  methods: {
    generateTestDatas() {
      showNotify("请稍后");
      Vue.axios
        .post(getUrl("Admin", "Fake"), withToken({}))
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