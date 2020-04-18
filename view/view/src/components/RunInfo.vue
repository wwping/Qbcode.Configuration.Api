<template>
    <div>
        <Modal
            v-model="visible"
            :title=" name + ':运行信息'" :footer-hide="true" :width="1200">
            <div>
                <div class="table-wrap">
                    <div class="head flex-display">
                        <Button type="default" @click="loadData" size="small">更新</Button>
                        <span class="flex-1"></span>
                    </div>
                    <div class="table">
                        <Table border ref="table" :height="500"  :columns="columns" :data="data.Data"></Table>
                    </div>
                    <div class="page t-c">
                        <Page show-total :total="data.Count" :current="data.PageIndex" :page-size="data.PageSize" @on-change="changePage1"  prev-text="上一页" next-text="下一页" />
                    </div>
                </div>
            </div>
        </Modal>
    </div>
</template>
<script>
import {get,post} from '../request/index'
export default {
    props:{
        value:{ type:Boolean, default:true },
        name:{ type:String, default:`` }
    },
    name:'RunInfo',
    data(){
        return {
            visible:this.value,
            loading:false,
            columns:[],
            data:{
                PageIndex:1,
                PageSize:10,
                Count:0,
                Data:[]
            }
        }
    },
    watch:{
        value(val) {
            this.visible = val;
        },
        visible(val){
            this.$emit('on-visible-change',val);
            if(this.name){
                this.loadData();
            }
        }
    },
    mounted(){
        
    },
    methods:{
        changePage1(index){
            this.data.PageIndex = index;
            this.loadData();
        },
        loadData(){
            get(`RunInfo`,{name:this.name,p:this.data.PageIndex,ps:this.data.PageSize}).then((res)=>{
                this.columns = res.data.Data.Columns.map(c=>{
                    return {
                        key:c.Key,
                        title:c.Name
                    };
                })
                this.data = res.data.Data.Data;
            });
        }
    }
}
</script>
<style>
    .jsoneditor-vue{height:400px;}
</style>
<style scoped>

</style>