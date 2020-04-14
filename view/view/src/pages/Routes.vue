<template>
    <div>
        <div class="page-wrap">
            <div class="table-wrap">
                <div class="head flex-display">
                    <Button type="default" @click="loadData" size="small" :loading="loading">更新</Button>
                    <span class="flex-1"></span>
                    <Input v-model="url" size="small" placeholder="route url ([abc.beetlex.com;doc.beetlex.com|]/.*) or (/users/.*)" style="width: 500px;" />
                    <Button type="success" @click="addRoute" size="small">新增路由</Button>
                    <span class="flex-1"></span>
                </div>
                <div class="table">
                    <Table border ref="table" :height="750" :columns="columns" :data="data"></Table>
                </div>
            </div>
        </div>        
    </div>
</template>
<script>
import {get,post} from '../request/index'
export default {
    name:'Routes',
    data(){
        var that = this;
        return {
            data:[],
            columns:[
                // {
                //     type: 'index',
                //     width: 60,
                //     align: 'center'
                // },
                {
                    type:'expand',
                    width:60,
                    render:(h,params)=>{
                        if(params.row._edit){
                            return h('Row',[
                                h('Col',{props:{span:24},style:{'margin-bottom':'10px'}},[
                                   
                                    h('span','Vary : '),
                                    h('Input',{
                                        props:{ value:params.row.Vary,min:0,size:'small' },
                                        style:{width:'100px'},
                                        on:{
                                            'on-change':(event)=>{
                                                that.changeServerValue(event,params.row._index,-1,'Vary');
                                            }
                                        }
                                    }),
                                    h('span',{style:{'margin-left':'20px'}},'MaxAge : '),
                                    h('InputNumber',{
                                        props:{ value:params.row.MaxAge,min:0,size:'small' },
                                        style:{width:'60px'},
                                        on:{
                                            'on-change':(event)=>{
                                                that.changeServerValue(event,params.row._index,-1,'MaxAge');
                                            }
                                        }
                                    }),
                                    h('Checkbox',{
                                        props:{ value:params.row.AllowCredentials },
                                        style:{'margin-left':'20px'},
                                        on:{
                                            'on-change':(event)=>{
                                                that.changeServerValue(event,params.row._index,-1,'AllowCredentials');
                                            }
                                        }
                                    },'AllowCredentials'),
                                ]),
                                h('Col',{props:{span:12}},[
                                    h('span','AllowOrigin : '),
                                    h('Select',{
                                        props:{ value:(params.row.AllowOrigin||'').split(','),size:'small',multiple:true,filterable:true,'allow-create':true },
                                        style:{width:'400px'},
                                        on:{
                                            'on-change':(event)=>{
                                                console.log(event);
                                                that.changeServerValue(event.join(','),params.row._index,-1,'AllowOrigin');
                                            },
                                            'on-create':(event)=>{
                                               this.origins.push(event);
                                            }
                                        }
                                    },this.origins.map(d=>{
                                        return h('Option',{
                                            props:{
                                                value:d,
                                                label:d
                                            }
                                        })
                                    })),
                                ]),
                                h('Col',{props:{span:12}},[
                                    h('span','AllowHeaders : '),
                                    h('Select',{
                                        props:{ value:(params.row.AllowHeaders||'').split(','),size:'small',multiple:true,filterable:true,'allow-create':true },
                                        style:{width:'400px'},
                                        on:{
                                            'on-change':(event)=>{
                                                that.changeServerValue(event.join(','),params.row._index,-1,'AllowHeaders');
                                            },
                                            'on-create':(event)=>{
                                               this.headers.push(event);
                                            }
                                        }
                                    },this.headers.map(d=>{
                                        return h('Option',{
                                            props:{
                                                value:d,
                                                label:d
                                            }
                                        })
                                    })),
                                ]),
                            ]);
                        }else{
                            return h('Row',[
                                h('Col',{props:{span:24}},[
                                    h('span','Vary : ' + (params.row.Vary || '')),
                                    h('span',{style:{'margin-left':'20px'}},'MaxAge : ' + (params.row.MaxAge || '')),
                                    h('span',{style:{'margin-left':'20px'}},'AllowCredentials : ' + params.row.AllowCredentials),
                                ]),
                                h('Col',{props:{span:12}},'AllowOrigin : ' + (params.row.AllowOrigin || '')),
                                h('Col',{props:{span:12}},'AllowHeaders : ' + (params.row.AllowHeaders || ''))
                            ]);
                        }
                    }
                },
                {
                    title:'路由',
                    key:'Url',
                    width:200
                },
                {
                    title:'超时/s',
                    key:'TimeOut',
                    width:100,
                    render:(h,params)=>{
                        if(params.row._edit){
                            return  h('InputNumber',{
                                    props:{ value:params.row.TimeOut,min:0,size:'small' },
                                    style:{width:'60px'},
                                    on:{
                                        'on-change':(event)=>{
                                            that.changeServerValue(event,params.row._index,-1,'TimeOut');
                                        }
                                    }
                                })
                        }
                        return h('span',params.row.TimeOut);
                    }
                },
                {
                    title:'服务',
                    key:'Servers',
                    minWidth:640,
                    render(h,params) {
                        
                        let servers = params.row.Servers,res = [];
                        for(let i = 0; i < servers.length; i++){
                            let c = servers[i];
                            if(params.row._edit){
                                res.push(h('p',{
                                    style:{
                                        'margin':'10px 0'
                                    }
                                },[
                                    h('Select',{
                                        props:{ value:c.Host,size:'small' },
                                        style:{ color:c.Available?'green':'red','margin-right':'10px','width':'200px' },
                                        on:{
                                            'on-change':(event)=>{
                                                that.changeServerValue(event,params.row._index,i,'Host');
                                            }
                                        }
                                    },that.servers.map(d=>{
                                        return h('Option',{
                                            props:{
                                                value:d.Host,
                                                label:d.Host
                                            }
                                        })
                                    })),
                                    h('span','MaxRps : '),
                                    h('InputNumber',{
                                        props:{ value:c.MaxRps,min:0,size:'small' },
                                        style:{width:'60px','margin-right':'10px'},
                                        on:{
                                            'on-change':(event)=>{
                                                that.changeServerValue(event,params.row._index,i,'MaxRps');
                                            }
                                        }
                                    },'MaxRps'),
                                    h('span','Weight : '),
                                    h('InputNumber',{
                                        props:{ value:c.Weight,min:0,size:'small' },
                                        style:{width:'60px','margin-right':'10px'},
                                        on:{
                                            'on-change':(event)=>{
                                                that.changeServerValue(event,params.row._index,i,'Weight');
                                            }
                                        }
                                    },'Weight'),
                                    h('Checkbox',{
                                        props:{ value:c.Standby },
                                        on:{
                                            'on-change':(event)=>{
                                                that.changeServerValue(event,params.row._index,i,'Standby');
                                            }
                                        }
                                    },'Standby'),
                                    h('Button',{
                                        props:{ type: 'default',size: 'small' },
                                        on:{
                                            click: () => {
                                                that.delServer(params.row,c,i);
                                            }
                                        }
                                    },'删除'),
                                ]));
                            }else{
                                res.push(h('p',[
                                    h('span',{
                                        style:{ color:c.Available?'green':'red'},
                                    },c.Host),
                                    h('span',`-- MaxRps : ${c.MaxRps} -- Weight : ${c.Weight} -- Standby : ${c.Standby}`)
                                ]))
                            }
                        }
                        return res;
                    },
                },
                {
                    title:'操作',
                    key:'controller',
                    width:200,
                    fixed: 'right',
                    render:(h,params)=>{
                        let btns = [];
                        if(this.listLoading[params.row.Url] == true){
                            btns.push(h('Spin'));
                        }else{

                            if(params.row._edit){
                                btns.push(h('Button', {
                                    props: {
                                        type: 'default',
                                        size: 'small'
                                    },
                                    style:{
                                        'margin-right':'4px'
                                    },
                                    on: {
                                        click: () => {
                                            this.$set(this.data[params.row._index],'_edit',false);
                                            this.$set(this.data[params.row._index],'_expanded',false);
                                        }
                                    }
                                }, '取消'));
                                btns.push(h('Button', {
                                    props: {
                                        type: 'default',
                                        size: 'small'
                                    },
                                    style:{
                                        'margin-right':'4px'
                                    },
                                    on: {
                                        click: () => {
                                            this.addServer(params.row);
                                        }
                                    }
                                }, '+服务'));
                                btns.push(h('Button', {
                                    props: {
                                        type: 'success',
                                        size: 'small'
                                    },
                                    style:{
                                        'margin-right':'4px'
                                    },
                                    on: {
                                        click: () => {
                                            this.saveRouteServer(params.row);
                                        }
                                    }
                                }, '保存'));
                            }else{
                                btns.push(h('Button', {
                                    props: {
                                        type: 'default',
                                        size: 'small'
                                    },
                                    style:{
                                        'margin-right':'4px'
                                    },
                                    on: {
                                        click: () => {
                                            this.$set(this.data[params.row._index],'_edit',true);
                                            this.$set(this.data[params.row._index],'_expanded',true);
                                        }
                                    }
                                }, '编辑'));
                            }
                            if(params.row.Url != '*' && !params.row._edit){
                                btns.push(h('Button', {
                                    props: {
                                        type: 'error',
                                        size: 'small'
                                    },
                                    on: {
                                        click: () => {
                                            this.deleteRoute(params.row);
                                        }
                                    }
                                }, '删除'));
                            }
                        }
                        return btns;
                    }
                }
            ],
            listLoading:{},
            url:'',
            loading:false,
            servers:[],
            headers:[],
            origins:[],
        }
    },
    mounted(){
        this.loadServers();
        this.loadData();
    },
    methods:{
        addRoute(){
            if(!this.url){
                return false;
            }
            this.loading = true;
            post(`SetRoute?url=${this.url}`).then((res)=>{
                this.loading = false;
                this.url = '';
                this.loadData();
            }).catch(()=>{
                this.loading = false;
            });
        },
        changeServerValue(event,rindex,sindex,key){
            let value = typeof event == 'object' ? event.target.value : event;
            if(sindex == -1){
                this.$set(this.data[rindex],key,value);
            }else{
                this.$set(this.data[rindex].Servers[sindex],key,value);
            }
        },
        delServer(row,server,index){
            if(server.New == true){
                this.data[row._index].Servers.splice(index,1);
                return;
            }
            this.$set(this.listLoading,row.Url,false);
            this.$Modal.confirm({
                title:'确认?',
                content:'删除不可逆，是否确认？',
                onOk:()=>{
                    post(`RemoveRouteServer?url=${row.Url}&server=${server.Host}`).then((res)=>{
                        this.$set(this.listLoading,row.Url,false);
                        this.loadData();
                    }).catch(()=>{
                        this.$set(this.listLoading,row.Url,false);
                    });
                }
            });
        },
        saveRouteServer(row){
            this.$set(this.listLoading,row.Url,true);
            post(`RouteSetting`,row).then((res)=>{
                this.$Message.success('保存成功！');
                this.$set(this.listLoading,row.Url,false);
                this.loadData();
            }).catch(()=>{
                this.$Message.error('保存失败！');
                this.$set(this.listLoading,row.Url,false);
            });
        },
        addServer(row){
            this.data[row._index].Servers.push({
               Host:'',
               Weight:0,
               Available:false,
               MaxRps:0,
               Standby:false,
               New:true,
               _edit:true
           })
        },
        deleteRoute(row){
            this.$set(this.listLoading,row.Url,true);
            this.$Modal.confirm({
                title:'确认?',
                content:'删除不可逆，是否确认？',
                onOk:()=>{
                    post('RemoveRoute?url='+row.Url).then((res)=>{
                        this.$set(this.listLoading,row.Url,false);
                        this.loadData();
                    }).catch(()=>{
                        this.$set(this.listLoading,row.Url,false);
                    });
                }
            });
        },
        loadData(){
            this.loading = true;
            get('ListRoute').then((res)=>{
                this.loading = false;
                this.data = res.data.Data;

                this.headers = this.data.reduce((arr,val)=>{
                    arr.push(val.AllowHeaders);
                    return arr;
                },[]).join(',').split(',').filter(c=>c.length > 0);
                this.origins = this.data.reduce((arr,val)=>{
                    arr.push(val.AllowOrigin);
                    return arr;
                },[]).join(',').split(',').filter(c=>c.length > 0);

            }).catch(()=>{
                this.loading = true;
            });
        },
        loadServers(){
            get('ListServer').then((res)=>{
                this.servers = res.data.Data;
            });
        }
    }
}
</script>
<style scoped>

</style>