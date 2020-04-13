<template>
    <div>
        <div class="page-wrap">
            <div id="chart" style="height:800px;width:100%;"></div>
        </div>
    </div>
</template>
<script>
import {get,post} from '../request/index'
import echarts from 'echarts'
export default {
    name:'Request'
    ,data(){
        return {
            timer:0
        };
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
            get('ServiceStatus').then((res)=>{
                this.show(res.data.Data);
                this.timer = setTimeout(() => {
                    this.loadData();
                }, 1500);
            })
        },
        show(data){
            let legendData = []
            ,seriesData2xx = []
            ,seriesData5xx = []
            ,seriesData4xx = []
            ,seriesData = [];

            for(let i = 0; i < data.length; i++){
                let name = `${data[i].Category || 'None'}  ${data[i].Host}`;
                legendData.push(name);
                seriesData.push({
                    name:name,
                    value:data[i].Count,
                })
                seriesData2xx.push({
                    name:name,
                    value:data[i]._2xCount,
                })
                seriesData5xx.push({
                    name:name,
                    value:data[i]._5xCount,
                })
                seriesData4xx.push({
                    name:name,
                    value:data[i]._4xCount,
                })
            }

            let option = {
                title:{

                },
                tooltip: {
                    trigger: 'item',
                    formatter: '{a} <br/>{b} <br/> {c} ({d}%)'
                },
                legend: {
                    type: 'scroll',
                    orient: 'vertical',
                    right: 10,
                    top: 20,
                    bottom: 20,
                    data: legendData
                },
                series: [
                    {
                        name: '服务',
                        type: 'pie',
                        radius: '55%',
                        center: ['40%', '50%'],
                        data: seriesData,
                        emphasis: {
                            itemStyle: {
                                shadowBlur: 10,
                                shadowOffsetX: 0,
                                shadowColor: 'rgba(0, 0, 0, 0.5)'
                            }
                        }
                    }
                ]
            };
            this.chart.setOption(option);

        }
    }
}
</script>
<style scoped>

</style>