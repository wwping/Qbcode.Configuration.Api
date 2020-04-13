<template>
    <div>
        <div class="page-wrap">
            <div class="table-wrap">
                <div class="head flex-display">
                    <Button type="default" @click="loadData" size="small">更新</Button>
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
                    title:'路由',
                    key:'Url',
                    width:200
                },
                {
                    title:'超时/s',
                    key:'TimeOut',
                    width:100,
                    render:(h,params)=>{
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
                },
                {
                    title:'服务',
                    key:'Servers',
                    minWidth:640,
                    render(h,params) {
                        let servers = params.row.Servers,res = [];
                        for(let i = 0; i < servers.length; i++){
                            let c = servers[i];
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
                        }
                        return res;
                    },
                },
                {
                    title:'操作',
                    key:'controller',
                    width:220,
                    fixed: 'right',
                    render:(h,params)=>{
                        let btns = [];
                        if(this.listLoading[params.row.Url] == true){
                            btns.push(h('Spin'));
                        }else{
                            btns.push(h('Button', {
                                props: {
                                    type: 'default',
                                    size: 'small'
                                },
                                style:{
                                    'margin-right':'10px'
                                },
                                on: {
                                    click: () => {
                                        this.addServer(params.row);
                                    }
                                }
                            }, '加个服务'));
                            btns.push(h('Button', {
                                props: {
                                    type: 'success',
                                    size: 'small'
                                },
                                style:{
                                    'margin-right':'10px'
                                },
                                on: {
                                    click: () => {
                                        this.saveRouteServer(params.row);
                                    }
                                }
                            }, '保存'));
                            if(params.row.Url != '*'){
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
            servers:[]
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
               New:true
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
            get('ListRoute').then((res)=>{
                this.data = res.data.Data;
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