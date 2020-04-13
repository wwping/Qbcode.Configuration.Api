<template>
    <div class="logs">
        <div class="page-wrap">
            <div class="table-wrap">
                <div class="head flex-display">
                    <Button type="default" @click="loadData" size="small">更新</Button>
                    <span class="flex-1"></span>
                </div>
                <div class="table">
                    <Table border ref="table" :height="710"  :columns="columns" :data="page.Items"></Table>
                </div>
                <div class="page t-c">
                    <Page show-total :total="page.Count" :current="page.PageIndex" :page-size="page.PageSize" @on-change="changePage1"  prev-text="上一页" next-text="下一页" />
                </div>
            </div>
        </div>
    </div>
</template>
<script>
import {get,post} from '../request/index'
export default {
    name:'Logs',
    data(){
        return {
            page:{
                PageIndex:1,
                PageSize:14,
                Count:0,
                Items:[]
            },
            columns:[
                {
                    type: 'index',
                    width: 60,
                    align: 'center'
                },
                {
                    title:'时间',
                    key:'Time',
                    width:100
                },
                {
                    title:'类型',
                    key:'Type',
                    width:100,
                    render:(h,params)=>{
                        return h('strong',{
                            style:{
                                color:this.colors[params.row.Type] || '#000'
                            }
                        },params.row.Type)
                    }
                },
                {
                    title:'日志',
                    key:'Message',
                    render: (h, params) => {
                        let ps = params.row.Message.split('\\n').map(c=>{
                           return h('p',c)
                        })
                        return h('Tooltip', {
                            props: { placement: 'top',maxWidth:900 },
                            style:{maxHeight:'300px'}
                        }, [
                            params.row.Message? params.row.Message.substring(0,150):'',
                            h('div', { slot: 'content', style: { whiteSpace: 'normal', wordBreak: 'break-all' } },ps)
                        ])
                    }
                },
            ],
            colors:{
                'Trace':'rgb(181,189,200)',
                'Debug':'rgb(22, 234, 255)',
                'Warring':'rgb(243, 186, 0)',
                'Error':'rgb(251, 61, 0)',
                'Fatal':'rgb(186, 0, 46)',
                'Info':'rgba(128,194,23,1)',
            },
            update:false,
        };
    },
    mounted(){
        this.loadData();
    },
    methods:{
        changePage1(index){
            this.page.PageIndex = index;
            this.loadData();
        },
        loadData(){
            get(`ListLog?p=${this.page.PageIndex}&ps=${this.page.PageSize}`).then((res)=>{
                this.page = res.data.Data;
            });
        }
    }
}
</script>
<style lang="">
    .logs .ivu-tooltip-inner{max-height: 300px; overflow: auto;}
</style>
<style scoped>

</style>