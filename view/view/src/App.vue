<template>
    <div id="app">
        <div class="w">
            <Header></Header>
            <router-view/>
            <Footer></Footer>
        </div>
    </div>
</template>

<script>
import Header from './components/Header'
import Footer from './components/Footer'
export default {
    name: 'App',
    components:{Header,Footer},
    mounted(){
        
    }
}
</script>
<style>
    .w{width: 1200px;margin: auto;}
    .page-wrap{min-height: 600px;background-color: #fff;border-radius: 4px;}
    .table-wrap{padding: 10px;}
    .table-wrap .head{margin-bottom: 10px;}
    .table-wrap .page{text-align: center; padding: 10px 0 0 0;}
</style>
<style scoped>
#app{position: absolute;left:0; top: 0;right: 0;bottom: 0; overflow: auto; background-image: url(../static/images/main_bg.png);background-size: 100% 100%;background-position: left top;background-repeat: no-repeat;}
</style>
