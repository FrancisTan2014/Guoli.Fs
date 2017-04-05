import Vue from 'vue'
import Router from 'vue-router'
import Home from '@/components/Home'
import Login from '@/components/Login'
import NotFound from '@/components/NotFound'
import Department from '@/components/Department'
import Person from '@/components/Person'
import Forbidden from '@/components/Forbidden'
import Directory from '@/components/Directory'
import Files from '@/components/Files'

import {vue} from '../global';

Vue.use(Router)

export default new Router({
  // 不配置此项的url像这样：http://localhost:18082/#/login
  // 配置之后像这样：http://localhost:18082/login
  // mode: 'history',
  routes: [
    {
      path: '/',
      name: '账户管理',
      component: Home,
      iconCls: 'el-icon-menu',
      children: [
        { path: '/department', name: '部门管理', component: Department, super: true },
        { path: '/person', name: '人员管理', component: Person }
      ]
    },
    {
      path: '/',
      name: '文件管理',
      component: Home,
      iconCls: 'el-icon-menu',
      children: [
        { path: '/files', name: '文件列表', component: Files }
      ]
    },
    {
      path: '/login',
      name: 'Login',
      hidden: true,
      component: Login
    },
    {
      path: '/404',
      hidden: true,
      component: NotFound
    },
    {
      path: '/403',
      hidden: true,
      component: Forbidden
    },
    {
      path: '*',
      hidden: true,
      redirect: { path: '/404' }
    }
  ]
});
