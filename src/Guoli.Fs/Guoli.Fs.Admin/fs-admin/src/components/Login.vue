<template>
  <el-form :model="loginUser" :rules="validator" ref="loginUser" label-position="left" label-width="0px" class="demo-ruleForm login-container">
    <h3 class="title">系统登录</h3>
    <el-form-item prop="account">
      <el-input type="text" v-model="loginUser.account" auto-complete="off" placeholder="账号"></el-input>
    </el-form-item>
    <el-form-item prop="password">
      <el-input type="password" v-model="loginUser.password" auto-complete="off" placeholder="密码"></el-input>
    </el-form-item>
    <el-checkbox v-model="checked" checked class="remember">记住密码</el-checkbox>
    <el-form-item class="w100p">
      <el-button type="primary" class="w100p" @click.native.prevent="login" :loading="logining">登 录</el-button>
    </el-form-item>
  </el-form>
</template>

<script>
  import NProgress from 'nprogress';
  import LoginService from '../services/login-service';
  import {config, store} from '../global';

  export default {
    data () {
      return {
        loginUser: {
          account: '',
          password: ''
        },

        validator: {
          account: { required: true, message: '请输入账号', trigger: 'change' },
          password: [{ required: true, message: '请输入密码', trigger: 'change' },
            { min: 6, max: 16, message: '密码长度必须在6~16之间', trigger: 'change' }]
        },

        checked: true,
        logining: false
      }
    },

    methods: {
      // 登录
      login () {
        this.$refs.loginUser.validate(valid => {
          if (valid) {
            this.logining = true;
            NProgress.start();

            LoginService.login(this.loginUser).then(res => {
              this.logining = false;
              NProgress.done();

              let {msg, code, data} = res;
              if (code !== 200) {
                // 登录失败
                this.$notify({
                  title: '提示',
                  message: msg,
                  type: 'error'
                });
              } else {
                console.info(data);
                // 登录成功
                // 将当前登录用户的信息保存到localStorage中
                store.setLoginUser(data.User);
                store.setLoginAccount(this.loginUser.account);
                store.setLoginToken(data.Token);

                let _this = this;
                _this.$notify({
                  title: '提示',
                  message: '登录成功',
                  type: 'success',
                  duration: 2000,
                  onClose: () => {
                    let backPath = _this.$route.query.backPath || _this.$route.params.backPath || '/';
                    _this.$router.replace(backPath);
                    // _this.$router.push(backPath);
                  }
                });
              }
            });
          } else {
            // do nothing
          }
        });
      }

    } // end methods

  } // end export
</script>

<style lang="scss" scoped>
  .login-container {
  /*box-shadow: 0 0px 8px 0 rgba(0, 0, 0, 0.06), 0 1px 0px 0 rgba(0, 0, 0, 0.02);*/
  -webkit-border-radius: 5px;
  border-radius: 5px;
  -moz-border-radius: 5px;
  background-clip: padding-box;
  margin-bottom: 20px;
  background-color: #F9FAFC;
  margin: 180px auto;
  border: 2px solid #8492A6;
  width: 350px;
  padding: 35px 35px 15px 35px;
  .title {
  margin: 0px auto 40px auto;
  text-align: center;
  color: #505458;
  }
  .remember {
  margin: 0px 0px 35px 0px;
  }
  .w100p {
  width: 100%;
  }
  }
</style>
