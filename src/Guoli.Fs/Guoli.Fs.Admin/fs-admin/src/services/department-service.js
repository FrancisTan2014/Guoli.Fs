import service from './base-service';

let controller = 'department';

export default {
  get: params => service.get(controller, params),
  getAll: () => service.get(controller, {}),
  // getPage: params => service.get(`${controller}/${params.page || 0}/${params.size || 0}`, params),
  getPage: params => service.post(`${controller}/getpage`, params),

  exists: params => service.post(`${controller}/exists/${params.name}`, params),
  add: params => service.post(controller, params),
  update: params => service.put(`${controller}/${params.id}`, params),
  delete: params => service.delete(`${controller}/${params.id}`)
}
