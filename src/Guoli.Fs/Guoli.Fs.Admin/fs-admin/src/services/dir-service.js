import service from './base-service';

let controller = 'directory';

export default {
  add: params => service.post(controller, params),
  rename: (id, newName) => service.post(`${controller}/${id}/${newName}`, {}),
  remove: id => service.delete(`${controller}/${id}`, {})
};
