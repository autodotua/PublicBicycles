<template>
  <div>
    <div v-title data-title="借车记录 - 公共自行车"></div>
    <h2 class="header-title" style="margin-left:12px">借车记录</h2>
    <el-table :data="records" style="width: 100%">
      <el-table-column prop="hireTime" label="借车时间" width="160"></el-table-column>
      <el-table-column prop="returnTime" label="还车时间" width="160"></el-table-column>
      <el-table-column prop="hireStation.name" label="借车站点" width="160"></el-table-column>
      <el-table-column prop="returnStation.name" label="还车站点" width="160"></el-table-column>
    </el-table>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { withToken, getUrl, showError, formatDateTime } from "../common";
export default Vue.extend({
  name: "Home",
  data() {
    return {
      records: []
    };
  },
  methods: {},
  computed: {},
  components: {},
  mounted: function() {
    this.$nextTick(function() {
      //获取所有的借车记录
      Vue.axios
        .post(getUrl("User", "Records"), withToken({}))
        .then(response => {
          for (const record of response.data.data) {
            record.hireTime = formatDateTime(record.hireTime as string);
            record.returnTime = formatDateTime(record.returnTime as string);
          }
          this.records = response.data.data;
        })
        .catch(showError);
    });
  }
});
</script>

<style scoped>
.el-table .cell {
  white-space: pre-line;
  word-wrap: break-word;
}

.cell .el-button {
  margin-right: 6px;
}
</style>
