import Vue from 'vue'
import Router from 'vue-router'

Vue.use(Router)

export default new Router({
    mode: 'history',
    routes: [
    {
      path: '/',
      name: 'Index',
      meta:{
          display:'首页',
          ismenu:true,
      },
      component: ()=>import('@/pages/Index')
    },
    {
        path: '/servers.html',
        name: 'Servers',
        meta:{
            display:'服务列表',
            ismenu:true,
        },
        component: ()=>import('@/pages/Servers')
    },
    {
        path: '/request.html',
        name: 'Request',
        meta:{
            display:'请求占比',
            ismenu:true,
        },
        component: ()=>import('@/pages/Request')
    },
    {
        path: '/routes.html',
        name: 'Routes',
        meta:{
            display:'路由设置',
            ismenu:true,
        },
        component: ()=>import('@/pages/Routes')
    },
    {
        path: '/plugins.html',
        name: 'Plugins',
        meta:{
            display:'插件设置',
            ismenu:true,
        },
        component: ()=>import('@/pages/Plugins')
    },
    {
        path: '/setting.html',
        name: 'Setting',
        meta:{
            display:'网关设置',
            ismenu:true,
        },
        component: ()=>import('@/pages/Setting')
    },
    {
        path: '/logs.html',
        name: 'Logs',
        meta:{
            display:'网关日志',
            ismenu:true,
        },
        component: ()=>import('@/pages/Logs')
    }
  ]
})
