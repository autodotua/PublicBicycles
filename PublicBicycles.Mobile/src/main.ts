import Vue from 'vue'
import App from './App.vue'
import './registerServiceWorker'
import router from './router'
import ElementUI from 'element-ui';
import 'element-ui/lib/theme-chalk/index.css';
import axios from 'axios'
import VueAxios from 'vue-axios'

Vue.use(VueAxios, axios)
Vue.config.productionTip = false
Vue.directive('title', {//单个修改标题
  inserted: function (el, binding) {
    if(el.dataset.title)
    {
    document.title = el.dataset.title
    }
  }
})
Vue.use(ElementUI);
new Vue({
  router,
  render: h => h(App)
}).$mount('#app')
