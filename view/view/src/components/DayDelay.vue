<template>
    <div class="request-status flex-display">
        <div id="chart2"></div>
        <span class="flex-1"></span>
        <div class="detail">
            <ul>
                <li v-for="(item,index) in detail" :key="index">
                    <dl>
                        <dt class="flex-display"><span>{{item.Count}}</span> <span class="flex-1"></span> <span>{{item.Percent}}%</span></dt>
                        <dd>{{item.Url}}</dd>
                    </dl>
                </li>
            </ul>
        </div>
    </div>
</template>
<script>
import echarts from 'echarts'
import {get,post} from '../request/index'
export default {
    name:'DayDelay'
    ,data(){
        return {
            chart:null,
            detail:[],
            timer:0
        }
    }
    ,mounted(){
        this.chart = echarts.init(document.getElementById('chart2'));
        this.chart.on("click", this.chartClick);
       this.loadData();
    },
    beforeDestroy(){
        clearTimeout(this.timer);
    },
    methods:{
        chartClick(e){
            get('GetDelayTopUrl',{params:{ name: e.name, index: e.data.value[1], count: 20 }}).then((res)=>{
               this.detail = res.data.Data;
            });
        },
        loadData(){
            clearTimeout(this.timer);
            get('GetDayDelay').then((res)=>{
                this.draw(res.data.Data);
                this.timer = setTimeout(() => {
                    this.loadData();
                }, 60*1000);
            });
        },
        draw(data){
            var _max = 0;
            var _data = [];
            for (var i = 0; i < data.Data.length; i++) {
                var item = data.Data[i];
                for (var k = 0; k < item.length; k++) {
                    var value = item[k];
                    if (value > _max)
                        _max = value;
                    var im = {
                        value: [i, k, value], itemStyle: { color: 'blue', opacity: 1, borderWidth: 2, borderColor: 'rgba(255,255,255,1)' }
                    };
                    if (!im.value[2])
                        im.itemStyle.color = 'rgba(0,0,0,0)';
                    else {
                        switch (k) {
                            case 0:
                                im.itemStyle.color = '#00A505';
                                break;
                            case 1:
                                im.itemStyle.color = '#2CDD00';
                                break;
                            case 2:
                                im.itemStyle.color = '#99FF00';
                                break;
                            case 3:
                                im.itemStyle.color = '#FFCC00';
                                break;
                            case 4:
                                im.itemStyle.color = '#FF9900';
                                break;
                            case 5:
                                im.itemStyle.color = '#FF6600';
                                break;
                            case 6:
                                im.itemStyle.color = '#FF3300';
                                break;
                            case 7:
                                im.itemStyle.color = '#771800';
                                break;
                        }
                    }
                    _data.push(im);
                }
            }
            let option = {
                tooltip: {
                    position: 'top',
                    formatter: function (p) {
                        if (p.data.value[2])
                            return '<p style="text-align:center;">' + data.XName[p.data.value[0]] + "<br/>" + data.YName[p.data.value[1]] + "<br/>" + p.data.value[2] + '<p/>';
                        else
                            return '';
                    }
                },
                animation: false,
                grid: {
                    top: 10,
                    bottom: 30,
                    right: 10,
                    left: 10
                },
                xAxis: {
                    type: 'category',
                    data: data.XName,
                    splitArea: {
                        show: true,
                        areaStyle: {
                            color: '#F9F7F7'
                        }
                    },
                    splitLine: {
                        show: false,
                        lineStyle: {
                            color: '#000'
                        }
                    }
                },
                yAxis: {
                    type: 'category',
                    show: false,
                    data: data.YName,
                    axisLine: {
                        show: true,
                    },
                    axisLabel: {
                        show: false,
                    },
                    splitArea: {
                        show: true,
                    },
                    splitLine: {
                        show: true,
                        lineStyle: {
                           // type: 'dashed'
                        }
                    }
                },
                series: [{
                    type: 'heatmap',
                    data: _data,
                    itemStyle: {
                        borderWidth: 1,
                        borderColor: 'red',
                        emphasis: {
                            shadowBlur: 0,
                            shadowColor: 'rgba(0, 0, 0, 0.2)'
                        }
                    }
                }]
            };
            this.chart.setOption(option);
        }
    }
}
</script>
<style scoped>
.request-status>h3{text-align: center; font-size: 16px;color:#e8fbf6;margin-bottom: 20px;}
#chart2{background-color: rgba(255,255,255,0.2);border-radius: 4px;}

#chart2{height:500px;width:800px;}
.detail{height: 500px;width: 390px;background-color: rgba(255,255,255,0.2);border-radius: 4px;padding:10px;overflow: auto;}
.detail li{border-bottom: 1px solid rgba(0,0,0,0.1);}

</style>