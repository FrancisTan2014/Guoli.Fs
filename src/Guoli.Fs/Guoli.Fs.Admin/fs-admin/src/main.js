// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import ElementUI from 'element-ui'
import router from './router'
import Mock from './mock'
import VueCookie from 'vue-cookie'
import VueMoment from 'vue-moment'

import 'element-ui/lib/theme-default/index.css'
import 'font-awesome/css/font-awesome.min.css'
import 'nprogress/nprogress.css'

Vue.use(ElementUI)
Vue.use(VueCookie)
Vue.use(VueMoment)

// Mock.bootstrap()

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  template: '<App/>',
  components: { App }
})
