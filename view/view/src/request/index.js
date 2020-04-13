import axios from 'axios'
import store from '@/store'

axios.defaults.withCredentials = true;
axios.defaults.timeout = 5000;
axios.defaults.baseURL = process.env.BASE_URL;

axios.interceptors.request.use(config=> {
    let _token = store.getters['LoginInfo/getToken']();
    config.headers["Authorization"] = _token.token_type + ' ' + _token.access_token;
    return config;
},(err)=>{
    return Promise.reject(err);
});
axios.interceptors.response.use(res=> {
    return res;
}, err=> {
    if(err.response){
        if(err.response.status == 401 || err.response.status == 403) {
            //tologin();
        }
    }
    return Promise.reject(err);
});

const get = (url,params = {})=>{
    return axios.get(url,{params:params});
} 

const post = (url,data)=>{
    return axios.post(url,data);
}

export {get,post};