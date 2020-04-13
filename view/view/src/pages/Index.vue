<template>
    <div class="index">
        <Status></Status>
        <div class="status">
            <Tabs value="dayDelay">
                <TabPane label="请求状态" name="dayStatus">
                    <DayStatus></DayStatus>
                </TabPane>
                <TabPane label="请求延迟" name="dayDelay">
                    <DayDelay></DayDelay>
                </TabPane>
            </Tabs>
        </div>
    </div>
</template>
<script>
import Status from '../components/Status'
import DayStatus from '../components/DayStatus'
import DayDelay from '../components/DayDelay'
export default {
    name:'Index',
    data(){
        return {
            ws:null
        }
    },
    components:{Status,DayStatus,DayDelay},
    beforeDestroy(){
       //this.ws && this.ws.close();
    },
    mounted(){
        // this.ws = new WebSocket("ws://localhost:8082");
        // this.ws.onopen = (evt)=> {
        //     this.ws.send("Hello WebSockets!");
        // };

        // this.ws.onmessage = (event)=> {
        //     try{
        //         let json = JSON.parse(event.data);
        //         if(json.Data.type == 'status'){
        //             this.$root.$emit('on'+json.Data.message.type,json.Data.message.data);
        //         }
        //     }catch(e){

        //     }
        // };
    }
}
</script>
<style>
    .index .ivu-tabs-nav .ivu-tabs-tab-active{color:#fff;}
    .index .ivu-tabs-ink-bar{background-color: #3ab397;}
</style>
<style scoped>
.status{padding-top: 50px;}
</style>