import Vue from 'vue'
import Vuex from 'vuex'
import LoginInfo from './modules/Login/index'
Vue.use(Vuex);

const store = new Vuex.Store({
    modules:{
        LoginInfo
    }
});

export default store;