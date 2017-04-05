import axios from 'axios';
import { config, store } from '../global';
import router from '@/router';

let { base } = config;

// 处理登录已过期
let loginHanler = data => {
  let { code, msg } = data;
  if (code === 40317) {
    // alert('登录已过期，请重新登录');
    store.clearLoginStatus();
    // let href = location.href;
    // let index = href.indexOf(`${config.base}/#`) + config.base.length + 2;
    // let backPath = href.substr(index);
    // router.push({ path: '/login', query: { backPath: backPath }});
    router.push('/login');
    // 让返回数据立即失效，阻止后续逻辑的执行
    data = null;
  } else if (code === 403) {
    router.push('/403');
    data = null;
  }
};

let executeHanlers = data => {
  loginHanler(data);
  return data;
}

let plusToken = url => `${url}?app_token=${store.getLoginToken()}`;

// 所有的请求都应该经过下面的四个方法
// 以便对服务器返回的数据进行统一处理
export default {
  get: (controller, params, config) => axios.get(
    plusToken(`${base}/${controller}`), // 请求地址
    params, // 参数
    config || {}) // axios配置信息
    .then(res => executeHanlers(res.data)),

  post: (controller, params, config) => axios.post(
    plusToken(`${base}/${controller}`),
    params,
    config || {})
    .then(res => executeHanlers(res.data)),

  put: (controller, params, config) => axios.put(
    plusToken(`${base}/${controller}`),
    params,
    config || {})
    .then(res => executeHanlers(res.data)),

  delete: (controller, params, config) => axios.delete(
    plusToken(`${base}/${controller}`),
    params,
    config || {})
    .then(res => executeHanlers(res.data))
}
