import {get} from '../request/index'

export const loadLoginInfo = ()=>{
    return new Promise((resolve,reject)=>{
        get(process.env.INFO_URL).then((res)=>{
            resolve({
                auth:{
                    islogin:res.data.auth.islogin,
                    issuccess:res.data.auth.islogin && (res.data.auth.role||[]).indexOf('superadmin') >=0
                },
                user:{
                    avatar:res.data.user.avatar,
                    nickname:res.data.user.nickname,
                }
            });
        }).catch((err)=>{
            reject(err);
        });
    }) 
}