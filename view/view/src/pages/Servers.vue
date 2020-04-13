<template>
    <div>
        <div class="page-wrap">
            <div class="table-wrap">
                <div class="head">
                    <Button type="success" @click="add" size="small">新增一个服务</Button>
                    <span style="margin-left:20px">
                        <Checkbox v-model="update">自动更新列表</Checkbox>
                    </span>
                </div>
                <div class="table">
                    <Table border ref="table" :height="750"  :columns="columns" :data="data"></Table>
                </div>
            </div>
        </div>
        <Modal
            v-model="isAdd"
            title="编辑服务" :footer-hide="true">
            <div>
                <Form ref="addForm" :model="addForm" :rules="ruleValidate" :label-width="80">
                    <FormItem label="主机" prop="Host"> 
                        <Input v-model="addForm.Host"></Input>
                    </FormItem>
                    <FormItem label="分类">
                        <Select v-model="addForm.Category" filterable allow-create @on-create="handleCreate">
                            <Option v-for="item in categorys" :value="item" :key="item">{{ item }}</Option>
                        </Select>
                    </FormItem>
                    <FormItem label="连接数">
                        <InputNumber :max="100000" :min="1" v-model="addForm.MaxConnections"></InputNumber>
                    </FormItem>
                    <FormItem>
                        <Button type="primary" @click="handleSubmit('addForm')">确定保存</Button>
                    </FormItem>
                </Form>
            </div>
        </Modal>
    </div>
</template>
<script>
import {get,post} from '../request/index'
export default {
    name:'Servers',
    data(){
        return {
            data:[],
            columns:[
                {
                    type: 'index',
                    width: 60,
                    align: 'center'
                },
                {
                    title:'主机',
                    key:'Host',
                    width:200,
                    render:(h,params)=>{
                        if(params.row.Available){
                            return h('strong',{style:{color:'green'}},params.row.Host)
                        }
                        return h('strong',{style:{color:'red'}},params.row.Host)
                    }
                },
                {
                    title:'分类',
                    key:'Category',
                    width:120
                },
                {
                    title:'最大连接数',
                    key:'MaxConnections',
                    width:110
                },
                {
                    title:'等待队列',
                    key:'WaitQueue',
                    width:100
                },
                {
                    title:'状态',
                    key:'Status',
                    minWidth:200,
                    render:(h,params)=>{
                        
                        return h('div',[
                            h('span',{style:{color:'green',marginRight:'10px'}},'2xx : '+params.row.Status._2xRps+'R/'+params.row.Status._2xCount+'C'),
                            h('span',{style:{color:'black',marginRight:'10px'}},'5xx : '+params.row.Status._5xRps+'R/'+params.row.Status._5xCount+'C'),
                            h('span',{style:{color:'red',marginRight:'10px'}},'4xx : '+params.row.Status._4xRps+'R/'+params.row.Status._4xCount+'C'),
                            h('span',{style:{color:'#002bff'}},params.row.Status.Rps+'R/'+params.row.Status.Count+'C'),
                        ])
                        
                    }
                },
                {
                    title:'操作',
                    key:'controller',
                    fixed: 'right',
                    width:130,
                    render:(h,params)=>{
                        let btns = [];
                        btns.push(h('Button', {
                            props: {
                                type: 'primary',
                                size: 'small'
                            },
                            style: {
                                marginRight: '5px'
                            },
                            on: {
                                click: () => {
                                    this.add(params.row);
                                }
                            }
                        }, '编辑'));
                        btns.push(h('Button', {
                                props: {
                                    type: 'error',
                                    size: 'small'
                                },
                                on: {
                                    click: () => {
                                        this.delete(params.row.Host);
                                    }
                                }
                            }, '删除'));
                        return btns;
                    }
                }
            ],
            timer:0,
            update:true,
            categorys:[],
            isAdd:false,
            addForm:{
                Host:'',
                Category:'None',
                MaxConnections:1
            },
            ruleValidate:{
                Host:[
                    { required: true, message: '必填', trigger: 'blur' }
                ]
            }
        }
    },
    mounted(){
        this.loadList();
        this.loadCategorys();
    },
    beforeDestroy(){
        clearTimeout(this.timer);
    },
    methods:{
        handleSubmit(name) {
            this.$refs[name].validate((valid) => {
                if (!valid) {
                    return;
                } else {
                    this.$axios.post('AddServer',this.addForm).then((res)=>{
                        this.isAdd = false;
                        this.$refs[name].resetFields();
                        this.loadList();
                    });
                }
            })
        },
        handleCreate(val){
            this.categorys.push(val);
        },
        add(row){
            if(row){
               this.addForm = JSON.parse(JSON.stringify(row)); 
            }
            this.isAdd = true;
        },
        delete(name){
            this.$Modal.confirm({
                title:'确认?',
                content:'删除不可逆，是否确认？',
                onOk:()=>{
                    post('RemoveServer?server='+name).then((res)=>{
                        this.loadList();
                    });
                }
            });
        },
        loadCategorys(){
            get('ListServerCategories').then((res)=>{
                this.categorys = res.data.Data.map(c=>{
                    return c == null ?'None':c
                });
            })
        },
        loadList(){
            clearTimeout(this.timer);
            if(this.update){
                get('ListServer').then((res)=>{
                    this.data = res.data.Data;
                    this.timer = setTimeout(() => {
                        this.loadList();
                    }, 2000);
                })
            }else{
                this.timer = setTimeout(() => {
                    this.loadList();
                }, 2000);
            }
        }
    }
}
</script>
<style scoped>

</style>