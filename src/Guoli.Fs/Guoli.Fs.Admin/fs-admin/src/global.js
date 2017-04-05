let config =  {
  base: 'http://192.168.1.108:8016/api',
  // base: 'http://192.168.1.108:8011/api',

  fileServer: 'http://192.168.1.108:8012',

  loginTokenCookie: 'app_token',
  loginCookieExpires: 7, // 7 days
};

let [userKey, accountKey, tokenKey] = ['login_user', 'login_account', 'login_token'];
// 使用localStorage统一管理本地缓存
let store = {
  setLoginUser: user => localStorage.setItem(userKey, JSON.stringify(user)),
  getLoginUser: () => JSON.parse(localStorage.getItem(userKey)),
  removeLoginUser: () => localStorage.removeItem(userKey),

  setLoginAccount: account => localStorage.setItem(accountKey, account),
  getLoginAccount: () => localStorage.getItem(accountKey),
  removeLoginAccount: () => localStorage.removeItem(accountKey),

  setLoginToken: token => localStorage.setItem(tokenKey, token),
  getLoginToken: () => localStorage.getItem(tokenKey),
  removeLoginToken: () => localStorage.removeItem(tokenKey),

  clearLoginStatus: () => {
    store.removeLoginUser();
    store.removeLoginAccount();
    store.removeLoginToken();
  },

  isSuperAdminLogin: () => (store.getLoginUser() || {}).UserType === 0
};

export { config, store };
