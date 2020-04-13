<template>
    <div>
        <Modal
            v-model="visible"
            title="插件配置" :footer-hide="true" :width="800">
            <div>
                <Form ref="addForm" :model="data" :label-width="80">
                    <FormItem label="插件" prop="Host"> 
                        <Input v-model="data.Info.Name" readonly></Input>
                    </FormItem>
                    <FormItem label="" prop="Json"> 
                        <div style="height:400px">
                            <vue-json-editor ref="jsoneditor" v-model="data.Setting" style="height:400px" mode="code" :exapndedOnStart="true" :showBtns="false"></vue-json-editor>
                        </div>
                    </FormItem>
                    <FormItem style="t-c">
                        <div class="t-c">
                            <Button type="primary" :loading="loading" @click="handleSubmit('addForm')">确定保存</Button>
                        </div>
                    </FormItem>
                </Form>
            </div>
        </Modal>
    </div>
</template>
<script>
import vueJsonEditor from 'vue-json-editor'
import {get,post} from '../request/index'
export default {
    props:{
        value:{ type:Boolean, default:true },
        name:{ type:String, default:`` }
    },
    name:'PluginJson',
    components:{vueJsonEditor},
    data(){
        return {
            data:{
                Info:{
                    Name:''
                },
                Setting:''
            },
            default:{},
            visible:this.value,
            loading:false,
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
        loadData(){
            get(`GetPlugin`,{name:this.name}).then((res)=>{
                res.data.Data.Setting = JSON.parse(res.data.Data.Setting || '{}');
                this.data = res.data.Data;
            });
        },
        handleSubmit(){
            this.loading = true;
            post(`SavePluginSetting?name=${this.data.Info.Name}&setting=${JSON.stringify(this.data.Setting)}`).then(()=>{
                this.loading = false;
                this.$Message.success('操作成功');
            }).catch(()=>{
                this.$Message.error('操作失败');
                this.loading = false;
            })
        }
    }
}
</script>
<style>
    .jsoneditor-vue{height:400px;}
</style>
<style scoped>

</style>