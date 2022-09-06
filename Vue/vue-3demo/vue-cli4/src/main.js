import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import './assets/font/iconfont.css'
import VueCompositionApi from '@vue/composition-api';

Vue.use(VueCompositionApi)
Vue.config.productionTip = false    //组织生产模式消息

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')

/* render:function(createElement){
	createElement(App)
} */
/* 
./同级 ../上级 @/ 根目录 */