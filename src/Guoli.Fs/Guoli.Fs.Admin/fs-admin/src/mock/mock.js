import axios from 'axios';
import MockAdapter from 'axios-mock-adapter';
import { DefaultUser } from '../mockdata/user'

let loginUser = DefaultUser;

export default {

  bootstrap() {
    let mock = new MockAdapter(axios);

    mock.onPost('/login').reply(config => {
      let {account, password} = JSON.parse(config.data);
      return new Promise((resolve, refect) => {
        setTimeout(function() {
          if (account === 'test') {
            resolve([200, {msg: '服务器内部错误', code: 500, user: null}]);
          } else if (account === loginUser.account && password === loginUser.password) {
            resolve([200, {msg: '登录成功', code: 200, user: loginUser}]);
          } else {
            resolve([200, {msg: '用户或者密码错误', code: 200, user: null}]);
          }
        }, 2000);
      });
    });
  }

} // end export
