# Qbcode.Configuration.Api
Bumblebee网关的可视化管理的 api 和 vue界面，仿照作者的 配置插件所改，
### 加载插件
```
g = new Gateway();
....省略一万个字...
 g.LoadPlugin(
  typeof(Qbcode.Configuration.Api.Plugin).Assembly
  );
```
### 插件api没有做什么限制
- 如果有登录限制需求 可以配合 Qbcode.Cors 插件使用 https://github.com/wwping/Qbcode.IDS4
- 如果有跨域需求，还可以配合 Qbcode.Cors插件 https://github.com/wwping/Qbcode.Cors

### 前端由vue 2.5.2 所写
在 /view/view/ 目录下
- 在 /config/ 下配置一些信息 
- 在 /src/request/index.js 配置 axios
- 在 /src/store/modules/Login/token.js 配置你的token设置和获取的方式
- 在 /src/apis/login.js 配置一下登录信息格式
```
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
```
### 如果你觉得有帮助
- alipay
![image](https://github.com/wwping/Qbcode.Configuration.Api/blob/master/alipay.png?raw=true)
