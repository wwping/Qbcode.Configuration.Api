<template>
    <div>
        <div class="status">
            <h3>服务版本</h3>
            <ul>
                <li>BeetleX : <span>{{version.BeetleX}}</span></li>
                <li>Bumblebee : <span>{{version.Bumblebee}}</span></li>
                <li>Configuration : <span>{{version.Configuration}}</span></li>
            </ul>
            <h3>服务器状态</h3>
            <ul>
                <li>Time : <span>{{status.RunTime}}</span></li>
                <li>CPU : <span>{{status.Cpu}}</span>%</li>
                <li>Memory : <span>{{status.TotalMemory}}KB</span></li>
                <li>Connections : <span>{{status.CurrentConnectinos}}</span></li>
                <li>Queue : <span>{{status.Queue}}</span></li>
                <li>Request : <span>{{status.RequestPer}} / {{status.TotalRequest}}</span></li>
                <li>Network : <span>W : {{status.SendBytesPer}}MB / R : {{status.ReceiveBytesPer}}MB</span></li>
                <!-- <li>Buffers : <span>{{status.Buffers}} / {{status.BufferSize}}</span></li> -->
            </ul>
        </div>
    </div>
</template>
<script>
import {get,post} from '../request/index'
export default {
    name:'Status',
    data(){
        return {
            status:{
                Host:'',
                Port:'',
                RunTime:'',
                TotalRequest:0,
                RequestPer:0,
                TotalMemory:0, //KB
                CurrentConnectinos:0,
                Cpu:0,
                Queue:0,
                Buffers:0,
                BufferSize:0,
                SendBytesPer:0,
                ReceiveBytesPer:0,
            },
            timer:0,
            version:{
                BeetleX:'',
                FastHttpApi:'',
                Bumblebee:'',
                Configuration:''
            }
        }
    },
    mounted(){
        this.loadData();
        this.loadVersion();
    },
    beforeDestroy(){
        clearTimeout(this.timer);
    },
    methods:{
        loadData(){
            clearTimeout(this.timer);
            get('Status').then((res)=>{
                this.setData(res.data.Data);
                this.timer = setTimeout(() => {
                    this.loadData();
                }, 1300);
            });
        },
        setData(data){
            this.status.Host = data.Status.Host;
            this.status.Port = data.Status.Port;
            this.status.RunTime = data.Status.RunTime;
            this.status.TotalRequest = data.Status.TotalRequest;
            this.status.RequestPer = data.Status.RequestPer;
            this.status.TotalMemory = data.Status.TotalMemory;
            this.status.CurrentConnectinos = data.Status.CurrentConnectinos;
            this.status.Cpu = data.Status.Cpu;
            this.status.SendBytesPer = data.Status.SendBytesPer;
            this.status.ReceiveBytesPer = data.Status.ReceiveBytesPer;
            this.status.Queue = data.Queue;
            this.status.Buffers = data.Buffers;
            this.status.BufferSize = data.BufferSize;
        },
        loadVersion(){
            get('GetVersions').then((res)=>{
                this.version.BeetleX = res.data.Data.BeetleX;
                this.version.FastHttpApi = res.data.Data.FastHttpApi;
                this.version.Bumblebee = res.data.Data.Bumblebee;
                this.version.Configuration = res.data.Data.Configuration;
            })
        }
    }
}
</script>
<style scoped>
.status{padding-top: 10px;}
.status >h3{text-align: center; font-size: 16px;color:#e8fbf6;margin: 20px 0;}
.status ul{text-align: center;}
li{display: inline-block; border: 1px solid #3c9280;color: #f6f6f6; background-color: rgba(0,0,0,0.1); padding: 6px;border-radius: 4px;margin-right: 10px;}
li span{color: #56d2bb;padding: 2px 6px;border-radius: 10px; background-color: rgba(0,0,0,0.2);}
</style>