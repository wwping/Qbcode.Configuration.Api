
import {getToken,setToken,removeToken} from './token'
import {encode,decode,getKey} from './encode'
import sessionstorage from 'sessionstorage'

const CACHE_KEY = 'loginInfo';

const _default  = {
    auth:{
        islogin:false,
        issuccess:false,
    },
    user:{
        avatar:'',
        nickname:'我'
    }
};

const getData = ()=>{
    try{
        let str = sessionstorage.getItem(CACHE_KEY);
        if(!str){
            return JSON.stringify(_default);
        }
        const key = str.substring(0,16);
        const iv = str.substring(16,32);
        return decode(str.substr(32),key,iv);
    }catch(e){
        return JSON.stringify(_default);
    }
}
const setData = (data)=>{
    const key = getKey(),iv = getKey();
    sessionstorage.setItem(CACHE_KEY,key+iv+encode(JSON.stringify(data),key,iv));
}
const removeData = ()=>{
    sessionstorage.removeItem(CACHE_KEY);
}

const LoginInfo ={
    namespaced: true,
    state:{
        loginInfo:JSON.parse(getData())
    },
    getters:{
        getLoginInfo:(state)=>()=>{
            return state.loginInfo;
        },
        getToken:(state)=>()=>{
            return getToken();
        },
        isSuccess:(state)=>()=>{
            let info = state.loginInfo;
            return info.auth && info.auth.issuccess;
        }
    },
    mutations:{
        setLoginInfo(state,info){
            state.loginInfo = info;
            setData(info);
        },
        setToken(context,token){
            setToken(token);
        },
        setOut(context){
            removeData();
            removeToken();
        }
    },
    actions:{
        setLoginInfo(context,info){
            if(!info){
                info = JSON.parse(JSON.stringify(_default));
            }
            info.auth = info.auth  || {};
            info.auth.role = info.auth.role  || [];
            context.commit('setLoginInfo',info);
        },
        setToken(context,token){
            context.commit("setToken",token);
            
        },
        setOut(context){
            context.commit("setOut");
        }
    }
};

setInterval(() => {
    const str = getData();
    if(!str){
        return;
    }
    setData(JSON.parse(str));
}, 5000);

export default LoginInfo;