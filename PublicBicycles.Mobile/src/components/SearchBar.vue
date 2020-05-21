
<template>
  <div>
    <el-autocomplete
      style="width:100%"
      v-model="searchContent"
      :fetch-suggestions="querySearch"
      placeholder="搜索站点"
      @select="searchSelect"
    >
      <template slot-scope="{ item }">
        <div class="name" style="line-height:20px;margin-top:4px">{{ item.name }}</div>
        <span class="address" style="color:#b4b4b4;font-size:12px;line-height:16px">{{ item.address }}</span>
      </template>
    </el-autocomplete>
  </div>
</template>
<script >
import Vue from "vue";
import "../map/ClusterLayer";
import Cookies from "js-cookie";
export default Vue.component("search-bar", {
  data() {
    return {
      searchContent: ""
    };
  },
  props: {
    stations: {
      default: []
    },
    select: {
      default: undefined
    }
  },
  computed: {},
  methods: {
    searchSelect(e) {
      this.$emit("select", e);
    },
    querySearch(queryString, callback) {
      if (this.searchContent &&  this.stations) {
        const result = this.stations.filter(
          p =>
            p.name.indexOf(this.searchContent) >= 0// ||
            // p.address.indexOf(this.searchContent)
        );
        const values = [];
        for (const station of result) {
          values.push(Object.assign({ value: station.name }, station));
        }
        callback(values);
      }
    }
  },
  components: {},
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
</style>