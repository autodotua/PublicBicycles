<template>
  <div class="container">
    <div v-title data-title="排行榜 - 公共自行车"></div>
    <h3 style="margin-left:1rem">近{{days}}日统计信息</h3>
    <div class="chart" id="hireStations"></div>
    <div class="chart" id="returnStations"></div>
  </div>
</template>

<script >
import Vue from "vue";
import { withToken, getUrl, showError, formatDateTime } from "../common";
import * as echarts from "echarts";
export default Vue.extend({
  name: "Home",
  data() {
    return {
      days: 100
    };
  },
  methods: {
    initChart(id, title, xs, values) {
      const chart = echarts.init(document.getElementById(id));
      chart.setOption({
        title: {
          text: title
        },
        tooltip: {},
        legend: {
          data: ["数量"]
        },
        xAxis: {
          data: xs,
          axisLabel: {
            interval: 0,
            rotate: 40
          }
        },
        yAxis: {},
        series: [
          {
            name: "数量",
            type: "bar",
            data: values
          }
        ]
      });
    }
  },
  computed: {},
  components: {},
  mounted: function() {
    this.$nextTick(function() {
      //获取所有的借车记录
      Vue.axios
        .post(getUrl("Analysis", "Leaderboard"), withToken({ days: this.days }))
        .then(response => {
          const hireStations = response.data.data.hireStations;
          let xs = hireStations.map(p => p.name);
          let values = hireStations.map(p => p.count);
          this.initChart("hireStations", "借出站点", xs, values);
          const returnStations = response.data.data.returnStations;
          xs = returnStations.map(p => p.name);
          values = returnStations.map(p => p.count);
          this.initChart("returnStations", "归还站点", xs, values);
        })
        .catch(showError);
    });
  }
});
</script>

<style scoped>
.chart {
  width: 100%;
  height: 40%;
  padding: 5%;
}
.container {
  height: 100%;
  width: 100%;
}
.container {
  overflow: hidden;
}
</style>
