
<template>
  <div class="container">
    <link
      rel="stylesheet"
      href="https://cdn.jsdelivr.net/gh/openlayers/openlayers.github.io@master/en/v6.3.1/css/ol.css"
      type="text/css"
    />
    <div id="map"></div>
    <!-- 搜索框 -->
    <!-- <el-autocomplete
      class="search"
      :style="searchStyle"
      v-model="searchContent"
      :fetch-suggestions="querySearch"
      placeholder="请输入内容"
      @select="searchSelect"
    ></el-autocomplete>-->
    <search-bar
      :style="searchBarStyle"
      class="search"
      :stations="stations"
      @select="searchSelected"
      v:show="enableSearch"
    ></search-bar>
  </div>
</template>
<script >
import Vue from "vue";
import "../map/ClusterLayer";
import Cookies from "js-cookie";
import ol, { Map, View, proj, Feature } from "openlayers";
import SearchBar from "../components/SearchBar";
import { withToken, getUrl, showError, jump, formatDateTime } from "../common";
export default Vue.component("map-view", {
  data() {
    return {
      stations: [],
      selecting: false,
      markerLayer: undefined,
      map: new Map({}),
      stationLayer: undefined,
      clusterLayer: undefined,
      routesLayer: undefined
    };
  },
  component: {
    "search-bar": SearchBar
  },
  props: {
    mapType: {
      default: "normal"
    },
    enableClick: {
      default: false
    },
    enableSearch: {
      default: true
    },
    searchBarStyle: {
      default: "top:72px"
    }
  },
  computed: {},
  methods: {
    searchSelected(e) {
      this.panTo([e.lng, e.lat]);
    },
    jump: jump,
    /**
     * 新增一个瓦片图层
     */
    addLayer(url) {
      this.map.addLayer(
        new ol.layer.Tile({
          source: new ol.source.XYZ({
            url: url
          })
        })
      );
    },
    /**
     * 获取指定要素的样式
     */
    getStyle(feature, selected) {
      return new ol.style.Style({
        image: new ol.style.Icon({
          src: `../img/bicycle${selected ? "_selected" : ""}.png`,
          scale: 1.0 / 5
        }),
        // 标签格式
        text: new ol.style.Text({
          offsetY: 32,
          fill: new ol.style.Fill({
            color: "#000"
          }),
          stroke: new ol.style.Stroke({
            color: "#fff",
            width: 3
          }),
          scale:1.5,
          text: feature.getProperties().object.name
        })
      });
    },
    /**
     * 处理地图点击事件
     */
    handleClickEvent(event) {
      setTimeout(() => {
        //延迟100毫秒，让选择时间先响应
        if (this.selecting) {
          //如果已经被选择了，那么就不需要响应单击事件了
          this.selecting = false;
          return;
        }
        const coord = ol.proj.transform(
          event.coordinate,
          "EPSG:3857",
          "EPSG:4326"
        );
        const point = new ol.geom.Point(event.coordinate);
        const feature = new Feature({
          geometry: point
        });
        //生成一个标记点击点的标志
        if (this.markerLayer) {
          this.map.removeLayer(this.markerLayer);
        } else {
          this.markerLayer = new ol.layer.Vector({
            name: "stations",
            maxResolution: 6, //越小越晚出现
            source: new ol.source.Vector({
              features: [feature]
            }),
            style: () => {
              return new ol.style.Style({
                image: new ol.style.Icon({
                  src: `../img/marker.png`,
                  scale: 1.0 / 3
                })
              });
            }
          });
          this.map.addLayer(this.markerLayer);
          this.panTo(coord);
          //向上层发射信号
          this.$emit("click", coord);
        }
      }, 100);
    },
    /**
     * 初始化地图
     */
    initMap() {
      this.map = new ol.Map({
        target: "map",
        layers: [],
        controls: ol.control.defaults({
          //删除不需要的控件
          attribution: false,
          zoom: false
        }),
        view: new View({
          center: proj.fromLonLat([121.56, 29.86]),
          zoom: 12
        })
      });
      this.addLayer(
        "http://t0.tianditu.com/vec_w/wmts?service=WMTS&request=GetTile&version=1.0.0&layer=vec&style=default&TILEMATRIXSET=w&format=tiles&height=256&width=256&tilematrix={z}&tilerow={y}&tilecol={x}&tk=9396357d4b92e8e197eafa646c3c541d"
      );
      this.addLayer(
        "http://t0.tianditu.com/cva_w/wmts?service=WMTS&request=GetTile&version=1.0.0&layer=cva&style=default&TILEMATRIXSET=w&format=tiles&height=256&width=256&tilematrix={z}&tilerow={y}&tilecol={x}&tk=9396357d4b92e8e197eafa646c3c541d"
      );
      //如果需要接收点击事件，那么注册一个事件
      if (this.enableClick) {
        this.map.on("click", this.handleClickEvent);
      }

      const selection = new ol.interaction.Select({
        condition: ol.events.condition.click,
        layers: layer => {
          return layer.get("name") == "stations";
        },
        style: feature => this.getStyle(feature, true)
      });
      this.map.addInteraction(selection);
      selection.on("select", e => {
        if (e.selected.length > 0) {
          const station = e.selected[0].getProperties().object;
          if (!station) {
            return;
          }
          this.selecting = true;
          setTimeout(() => {
            if (this.mapType == "routes") {
              this.loadRoutes(station);
            } else {
              //这个事件触发太早了，不延迟的话，抽屉会识别到外部点击事件然后马上收回
              this.$emit("select", station);
            }
          }, 200);
        }
      });
    },
    /**
     * 加载聚类图层
     */
    loadCluster(stations) {
      const data = [];
      for (const station of stations) {
        data.push({ lat: station.lat, lon: station.lng });
      }
      const cluster = ol.layer.ClusterLayer({
        map: this.map,
        clusterField: "",
        zooms: [15], //表示到15级以后就隐藏了
        distance: 100,
        data: data,
        style: null
      });
      this.clusterLayer = cluster;
      this.map.addLayer(cluster);
    },
    /**
     * 加载租赁点图层
     */
    loadStations(features) {
      const layer = new ol.layer.Vector({
        name: "stations",
        maxResolution: 6, //越小越晚出现
        source: new ol.source.Vector({
          features: features
        }),
        style: feature => this.getStyle(feature)
      });
      this.stationLayer = layer;
      this.map.addLayer(layer);
    },
    /**
     * 加载热力图图层
     */
    loadHeatmap(features) {
      const heatmap = new ol.layer.Heatmap({
        source: new ol.source.Vector({
          features: features
        }),
        radius: 10
      });
      this.map.addLayer(heatmap);
    },
    /**
     * 加载某一个租赁点的路线
     */
    loadRoutes(station) {
      Vue.axios
        .post(
          getUrl("Analysis", "Routes"),
          withToken({ stationID: station.id, days: 100 })
        )
        .then(response => {
          const currentCoord = ol.proj.fromLonLat([station.lng, station.lat]);
          const routes = response.data.data;
          const features = [];
          /**
           * 将路线集合加入到要素集合中
           */
          const addToFeatures = (items, type) => {
            for (const stationID of Object.keys(items)) {
              //目标租赁点
              const targetStation = this.stations.find(p => p.id == stationID);
              //目标租赁点的坐标，WGS84
              const targetCoord = ol.proj.fromLonLat([
                targetStation.lng,
                targetStation.lat
              ]);
              const feature = new Feature({
                geometry: new ol.geom.LineString([currentCoord, targetCoord]),
                weight: items[stationID], //路线的宽度使用该路线的发生次数来表示
                type
              });
              features.push(feature);
            }
          };
          //分别对归还和借出调用函数
          addToFeatures(routes.in, "in");
          addToFeatures(routes.out, "out");
          if (this.routesLayer) {
            this.map.removeLayer(this.routesLayer);
          }
          this.routesLayer = new ol.layer.Vector({
            source: new ol.source.Vector({
              features: features
            }),
            style: feature => {
              const t = feature.getProperties()["type"];
              return new ol.style.Style({
                stroke: new ol.style.Stroke({
                  color: t == "in" ? "#33FF55" : "#FF0000", //in用绿色，out用红色
                  width: feature.getProperties()["weight"] * 3
                })
              });
            }
          });
          this.map.addLayer(this.routesLayer);
        })
        .catch(showError);
    },
    /**
     * 平移到，并放大
     */
    panTo(loc) {
      this.map.getView().animate({
        duration: 300,
        rotation: 0,
        zoom: 16,
        center: ol.proj.fromLonLat(loc)
      });
    },
    /**
     * 加载基础数据
     */
    loadDatas() {
      if (this.clusterLayer) {
        this.map.removeLayer(this.clusterLayer);
      }
      if (this.stationLayer) {
        this.map.removeLayer(this.stationLayer);
      }
      Vue.axios
        .get(getUrl("Map", "Stations"))
        .then(response => {
          const features = [];
          this.stations = response.data.data;
          //向上级发送获取到的租赁点信息
          this.$emit("gotStations", response.data.data);
          for (const station of response.data.data) {
            const point = new ol.geom.Point(
              ol.proj.fromLonLat([station.lng, station.lat])
            );
            const feature = new Feature({
              geometry: point,
              object: station
            });
            features.push(feature);
          }
          switch (this.mapType) {
            case "normal":
            case "routes":
              //对于普通或路线类型，加载聚类图层和租赁点图层
              this.loadCluster(response.data.data);
              this.loadStations(features);
              break;
            case "heatmap":
              //this.loadCluster(response.data.data);
              this.loadStations(features);
              //对于热力图，还需要记载热力图图层
              this.loadHeatmap(features);
              break;
          }
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
      //页面加载后，需要初始化地图并加载数据
      this.initMap();
      this.loadDatas();
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
  margin-top: -12px;
  margin-left: 12px;
}

.search {
  position: absolute;
  /* top: 120px; */
  left: 24px;
  right: 24px;
}

</style>