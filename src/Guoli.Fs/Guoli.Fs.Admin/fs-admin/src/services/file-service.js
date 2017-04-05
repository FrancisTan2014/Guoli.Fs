import service from './base-service';

let controller = 'file';

export default {
  get: dirId => service.get(`${controller}/${dirId}`, {}),
  rename: (id, newName) => service.put(`${controller}/rename/${id}/${newName}`, {}),
  remove: id => service.delete(`${controller}/${id}`, {})
};
