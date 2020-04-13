<template>
    <div>
        <div class="header flex-display">
            <a href="/" class="logo">
                <img :src="defaultLogo" alt="奇贝少儿编程">
            </a>
            <ul class="menu flex-display">
                <li v-for="(item,index) in $router.options.routes" :key="index">
                    <router-link :to="{name:item.name}">{{item.meta.display}}</router-link>
                    <div class="icon" v-if="$route.name == item.name && item.name != 'Index'"></div>
                </li>
            </ul>
            <span class="flex-1"></span>
            <Dropdown class="login">
		        <a href="javascript:void(0)">
		            <img :src="avatar" class="avatar" :alt="nickname">
		            <span>{{ nickname }}</span>
		            <Icon type="ios-arrow-down"></Icon>
		        </a>
		        <DropdownMenu slot="list" v-if="loginInfo.auth.islogin">
		            <DropdownItem><p @click="logout">退出登录</p></DropdownItem>
		        </DropdownMenu>
		    </Dropdown>
        </div>
    </div>
</template>
<script>
import jsurl from 'wwp-js-url-helper'
import { createNamespacedHelpers } from 'vuex'
const { mapState,mapActions, mapGetters, mapMutations } = createNamespacedHelpers('LoginInfo')
export default {
    name:'Header',
    data(){
        return {
            static:process.env.STATIC_URL,
            defaultLogo:process.env.LOGO_URL
        }
    },
    computed:{
        ...mapState(['loginInfo']),
        ...mapActions(['setOut']),
        avatar:function(){ 
            return this.loginInfo.auth.islogin && this.loginInfo.user.avatar.length > 0 ?
                this.static + this.loginInfo.user.avatar:
                this.defaultLogo;
        },
        nickname:function(){
            return this.loginInfo.auth.islogin && this.loginInfo.user.nickname.length > 0 ?
            this.loginInfo.user.nickname:
            '匿名';
        }
    },
    mounted(){
    },
    methods:{
        logout(){
            this.setOut();
            window.location.reload();
        }
    }
}
</script>
<style scoped>
.login a{font-size:14px;color:#fff; line-height: 50px; }
.login li a{line-height: normal;}
.login .avatar{height: 32px;vertical-align: middle;margin: -2px 2px 0 0;border: 1px solid #fff;border-radius: 50%;}
.login .ivu-dropdown-item{font-size:14px !important;}


.header{padding: 20px 0;}
.header>.logo{margin-right: 20px;}
.header>.logo img{height: 50px;}
.header>.menu li{line-height: 50px;margin-right: 10px;position: relative;}
.header>.menu li div.icon{position: absolute; left: 50%;margin-left: -20px; top: 40px; border-width: 20px; border-style: solid;border-color: transparent transparent #fff transparent;}
.header>.menu a{padding: 6px 20px;border-radius: 4px;transition: .3s; color: #bffbeb;text-shadow: 0 1px 1px #28866e; font-size: 14px;}
.header>.menu a.router-link-exact-active,.header>.menu a:hover{color:#fff;background-color: rgba(0,0,0,0.5);}
</style>