import service from './base-service';

let controller = 'person';

export default {
  getPage: params => service.post(`${controller}/getpage`, params),

  add: params => service.post(controller, params),
  update: params => service.put(`${controller}/${params.Id}`, params),
  delete: id => service.delete(`${controller}/${id}`, {}),
  userNameExists: params => service.post(`${controller}/username_exists/${params.personId}/${params.username}`, {})
};
