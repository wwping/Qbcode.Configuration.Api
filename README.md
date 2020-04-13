# Qbcode.Configuration.Api
Bumblebee网关的可视化管理的 api 和 vue界面，仿照作者的 配置插件所改，
## 加载插件
```
g = new Gateway();
....省略一万个字...
 g.LoadPlugin(
  typeof(Qbcode.Configuration.Api.Plugin).Assembly
  );
```
## 此插件默认开启，并且不能关闭，不需要什么配置
