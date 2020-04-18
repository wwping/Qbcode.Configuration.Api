// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import router from './router'
import store from './store'

Vue.config.productionTip = false

import ViewUI from 'view-design'
import 'view-design/dist/styles/iview.css'
Vue.use(ViewUI);

import jsurl from 'wwp-js-url-helper'

const tologin = ()=>{
    if(process.env.NODE_ENV == 'production'){
        store.dispatch('LoginInfo/setOut');
        let url = process.env.LOGIN_URL;
        url = jsurl.updateSearch('from',encodeURIComponent(jsurl.removeSearch('from',window.location.href)),url);
        jsurl.updateSearch('out',new Date().getTime(),url);
        window.location.href = url;
    }
}

import {loadLoginInfo} from './apis/login'

setInterval(() => {
    loadLoginInfo();
}, 6000);

router.beforeEach((to, from,next) => {
    let issuccess = store.getters['LoginInfo/isSuccess']();
    if( issuccess ){
        loadLoginInfo();
        next();
    }else{
        loadLoginInfo().then((data)=>{
            store.dispatch('LoginInfo/setLoginInfo',data);
            issuccess = store.getters['LoginInfo/isSuccess']();
            if(issuccess){
                next();
            }else{
                tologin();
                next();
            }
        }).catch((err)=>{
            console.log(err);
            tologin();
            next();
        })
    }
});

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,store,
  components: { App },
  template: '<App/>'
})
