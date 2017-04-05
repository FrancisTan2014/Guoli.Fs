import axios from 'axios';
import service from './base-service';

let controller = 'login';

export default {
  login: params => service.post(controller, params)
  // login: params => service.post(`${controller}/Index`, params)
  ,contact: params => service.post('contact', params)
}
