import ol,{} from "openlayers"
ol.layer.ClusterLayer = function (options) {

    // eslint-disable-next-line @typescript-eslint/no-this-alias
    const self= this;
    self.styleFunc = function (feat) {
        const attribute = feat.get("attribute");
        let count = attribute.cluster.length;
        if (count < 1) {
            const name = attribute.data.name;
            return new ol.style.Style({
                image: new ol.style.Icon(/** @type {olx.style.IconOptions} */({
                    anchor: [0.5, 60],
                    anchorOrigin: 'top-right',
                    anchorXUnits: 'fraction',
                    anchorYUnits: 'pixels',
                    offsetOrigin: 'top-right',
                    offset: [0, 1],//偏移量设置
                    scale: 0.7,  //图标缩放比例
                    opacity: 0.75,  //透明度
                    src: 'data/marker-icon.png'//图标的url
                })),
                text: new ol.style.Text({
                    text: name,
                    fill: new ol.style.Fill({
                        color: '#000000'
                    }),
                    textAlign: "left",
                    offsetX: 5,
                    textBaseline: "middle"
                })
            })
        } else {
            let _smallCorlor;
            let _bigCorlor;
            if (count < 100) {
                if (count > 50) {
                    _smallCorlor = "#f0cd41";
                    _bigCorlor = "#f5de8b";
                }
                else {
                    _smallCorlor = "#94d769";
                    _bigCorlor = "#cde7b1";
                }
            }
            else {
                _smallCorlor = '#f1964d';
                _bigCorlor = "#f9bda2";
            }
             count++;
            count = count.toString();
            let smallRadius = count.length * 10;
            smallRadius = smallRadius < 10 ? 12 : smallRadius ;
            const bigRadius = smallRadius + 5;
            return [
                new ol.style.Style({
                    image: new ol.style.Circle({
                        radius: bigRadius,
                        fill: new ol.style.Fill({
                            color: _bigCorlor
                        })
                    }),
                }),
                new ol.style.Style({
                    image: new ol.style.Circle({
                        radius: smallRadius,
                        fill: new ol.style.Fill({
                            color: _smallCorlor
                        })
                    }),
                    text: new ol.style.Text({
                        text: count,
                        fill: new ol.style.Fill({
                            color: '#620022'
                        }),
                        textAlign: "center",
                        textBaseline: "middle"
                    })
                }),
            ]
        }
    }
    const defaults = {
        map: null,
        clusterField: "",
        zooms: [2, 4, 8, 12],
        distance: 256,
        data: [],
        style: self.styleFunc,
    };
    //将default和options合并
    self.options = {
        map: options.map,
        clusterField: options.clusterField,
        zooms: (options.zooms.length > 0 ? options.zooms : defaults.zooms),
        distance: (options.distance > 0 ? options.distance : defaults.distance),
        data: options.data,
        style:(options.style!=null?options.style:defaults.style)
    }
    
    self.proj = self.options.map.getView().getProjection();
    
    self.vectorSource = new ol.source.Vector({
        features: []
    });
    self.vectorLayer = new ol.layer.Vector({
        source: self.vectorSource,
        style: self.options.style
    });
    self.clusterData = [];
    //判断该点是否聚合
    self._clusterTest = function (data, dataCluster) {
        let _flag = false;
    
        const _cField = self.options.clusterField;
        if (_cField != "") {
            _flag = data[_cField] === dataCluster[_cField];
        } else {
            //将地理坐标转换成屏幕坐标，进行距离判断
            const _dataCoord = self._getCoordinate(data.lon, data.lat),
                _cdataCoord = self._getCoordinate(dataCluster.lon, dataCluster.lat);
            const _dataScrCoord = self.options.map.getPixelFromCoordinate(_dataCoord),
                _cdataScrCoord = self.options.map.getPixelFromCoordinate(_cdataCoord);
    
            const _distance = Math.sqrt(
                Math.pow((_dataScrCoord[0] - _cdataScrCoord[0]), 2) +
                Math.pow((_dataScrCoord[1] - _cdataScrCoord[1]), 2)
            );
            _flag = _distance <= self.options.distance;
        }
        //如果超过最大的缩放级别，数据全部展示
        const _zoom = self.options.map.getView().getZoom(),
            _maxZoom = self.options.zooms[self.options.zooms.length - 1];
        if (_zoom > _maxZoom) _flag = false;
        return _flag;
    };
    //坐标转换
    self._getCoordinate = function (lon, lat) {
        return ol.proj.transform([parseFloat(lon), parseFloat(lat)],
            "EPSG:4326",
            self.proj
        );
    };
    //添加数据到聚合图
    self._add2CluserData = function (index, data) {
        self.clusterData[index].cluster.push(data);
    };
    
    self._clusterCreate = function (data) {
        self.clusterData.push({
            data: data,
            cluster: []
        });
    };
    //展示数据
    self._showCluster = function () {
        self.vectorSource.clear();
        const _features = [];
        for (let i = 0, len = self.clusterData.length; i < len; i++) {
            const _cdata = self.clusterData[i];
            const _coord = self._getCoordinate(_cdata.data.lon, _cdata.data.lat);
            const _feature = new ol.Feature({
                geometry: new ol.geom.Point(_coord),
                attribute: _cdata
            });
            //如果聚合点里面没有数据就显示该点数据
            if (_cdata.cluster.length === 0) _feature.attr = _feature.data;
            _features.push(_feature);
        }
        self.vectorSource.addFeatures(_features);
    };
    
    self._clusterFeatures = function () {
        self.clusterData = [];
        //可视域处理
        const _viewExtent = self.options.map.getView().calculateExtent();
        //声明一个矩形，范围就是屏幕的四至
        const _viewGeom = new ol.geom.Polygon.fromExtent(_viewExtent);
        for (let i = 0, ilen = self.options.data.length; i < ilen; i++) {
            const _data = self.options.data[i];
            const _coord = self._getCoordinate(_data.lon, _data.lat);
            if (_viewGeom.intersectsCoordinate(_coord)) {
                //当前点是否聚合，默认是false
                let _clustered = false;
                for (let j = 0, jlen = self.clusterData.length; j < jlen; j++) {
                    const _cdata = self.clusterData[j];
                    if (self._clusterTest(_data, _cdata.data)) {
                        self._add2CluserData(j, _data);
                        _clustered = true;
                        break;
                    }
                }
                if (!_clustered) {
                    self._clusterCreate(_data);
                }
            }
        }
        self.vectorSource.clear();
        self._showCluster();
    };
    self.init = function () {
        self._clusterFeatures();
        self.options.map.on("moveend", function () {
            self._clusterFeatures();
        });
    };
    self.init();
    
    return self.vectorLayer;
    };
    
    ol.inherits(ol.layer.ClusterLayer, ol.layer.Vector);