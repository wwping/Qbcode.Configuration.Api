<template>
    <div>
        <div class="page-wrap">
            <div class="table-wrap">
                <div class="head">
                    <Button type="default" @click="loadData" :loading="loading" size="small">刷新</Button>
                </div>
                <div class="table">
                    <Table border ref="table" :height="750"  :columns="columns" :data="data"></Table>
                </div>
            </div>
        </div>
        <PluginJson v-model="isEdit" :name="name" @on-visible-change="(val)=>{ isEdit = val;}"></PluginJson>
    </div>
</template>
<script>
import PluginJson from '../components/PluginJson'
import {get,post} from '../request/index'
export default {
    name:'Plugins',
    data(){
        return {
            loading:false,
            data:[],
            enabledLoadings:{},
            columns:[
                {
                    type: 'index',
                    width: 60,
                    align: 'center'
                },
                {
                    title:'名称',
                    key:'Name',
                    minWidth:260
                },
                {
                    title:'类型',
                    key:'Type',
                    sortable:true,
                    width:120
                },
                {
                    title:'级别',
                    key:'Level',
                    sortable:true,
                    width:100
                },
                {
                    title:'版本',
                    key:'Version',
                    sortable:true,
                    width:100
                },
                {
                    title:'程序集',
                    key:'Assembly',
                    minWidth:250
                },
                {
                    title:'说明',
                    key:'Description',
                    minWidth:250
                },
                {
                    title:'作者信息',
                    key:'Copyright',
                    minWidth:440,
                    render: (h, params) => {
                        return h('Tooltip', {
                            props: { placement: 'top' }
                        }, [
                            params.row.Copyright? params.row.Copyright.substring(0,50):'',
                            h('span', { slot: 'content', style: { whiteSpace: 'normal', wordBreak: 'break-all' } },params.row.Copyright)
                        ])
                    }
                },
                {
                    title:'状态',
                    key:'status',
                    fixed: 'right',
                    width:100,
                    render:(h,params)=>{
                        return h('i-switch',{
                            props:{
                                size:"large",
                                value:params.row.Enabled,
                                disabled:!params.row.Status,
                                loading:this.enabledLoadings[params.row.Name] == true
                            },
                            scopedSlots:{
                                open:()=>h('span','启用'),
                                close:()=>h('span','禁用')
                            },
                            on:{
                                'on-change':()=>{
                                    this.Enabled(params.row);
                                }
                            }
                        });
                    }
                }
                ,
                {
                    title:'操作',
                    key:'controller',
                    fixed: 'right',
                    width:85,
                    render:(h,params)=>{
                        let btns = [];
                        btns.push(h('Button', {
                            props: {
                                type: 'primary',
                                size: 'small'
                            },
                            on: {
                                click: () => {
                                    this.name = params.row.Name;
                                    this.isEdit = true;
                                }
                            }
                        }, '配置'));
                        return btns;
                    }
                }
            ],
            isEdit:false,
            name:''
        };
    },
    components:{PluginJson},
    mounted(){
        this.loadData();
    },
    methods:{
        Enabled(row){
            this.$set(this.enabledLoadings,row.Name,true);
            post(`PluginEnabled?name=${row.Name}&enabled=${!row.Enabled}`).then((res)=>{
                this.$set(this.enabledLoadings,row.Name,false);
                this.$Message.success('操作成功');
            }).catch(()=>{
                this.$Message.error('操作失败！');
                this.$set(this.enabledLoadings,row.Name,false);
            });
        },
        loadData(){
            this.loading = true;
            get('ListPluginInfos').then((res)=>{
                this.data = res.data.Data;
                this.loading = false;
                this.$Message.success('加载数据成功');
            }).catch(()=>{
                this.$Message.error('加载数据失败！');
                this.loading = false;
            });
        }
    }
}
</script>
<style scoped>

</style>