<template>
    <div class="request-status">
        <div id="chart" style="height:500px;width:100%;"></div>
    </div>
</template>
<script>
import echarts from 'echarts'
import {get,post} from '../request/index'
export default {
    name:'DayStatus'
    ,data(){
        return {
            chart:null,
            timer:0
        }
    }
    ,mounted(){
        this.chart = echarts.init(document.getElementById('chart'));
        this.loadData();
    },
    beforeDestroy(){
        clearTimeout(this.timer);
    },
    methods:{
        loadData(){
            clearTimeout(this.timer);
            get('GetDayStatus').then((res)=>{
                this.draw(res.data.Data.XName,res.data.Data.YName,res.data.Data.Data);
                this.timer = setTimeout(() => {
                    this.loadData();
                }, 2500);
            });
        },
        draw(xNames,yNames,data){

            let series = [],colors = ['#fff','green','red','yellow'];
            yNames.map((c,index)=>{
                series[index] = {
                    name: c,
                    type: 'line',
                    data: [],
                    lineStyle:{
                        normal:{
                            color:colors[index]
                        }
                    }
                };
            }) ;   
            
            for(let i = 0,len = data.length; i < len; i++){
                for(let j = 0; j < yNames.length; j++){
                    series[j].data.push(data[i][j]);
                }
            }

            this.chart.setOption( {
                title: {
                    text: ''
                },
                tooltip: {
                    trigger: 'axis'
                },
                color:colors,
                legend: {
                    data: yNames
                },
                grid: {
                    left: '3%',
                    right: '4%',
                    bottom: '3%',
                    top:'10%',
                    containLabel: true
                },
                toolbox: {
                    feature: {
                        //saveAsImage: {}
                    }
                },
                xAxis: {
                    type: 'category',
                    boundaryGap: false,
                    data: xNames
                },
                yAxis: {
                    type: 'value'
                },
                series:series
            })
        }
    }
}
</script>
<style scoped>
.request-status>h3{text-align: center; font-size: 16px;color:#e8fbf6;margin-bottom: 20px;}
#chart{background-color: rgba(255,255,255,0.2);border-radius: 4px;}
</style>