import { createApp } from 'vue'
import App from './App.vue'
// import Storage from 'vue-ls';
import router from './router/router.js'
import ElementPlus from 'element-plus'
import 'element-plus/theme-chalk/index.css'
import * as ElementPlusIconsVue from '@element-plus/icons-vue'
import mitt from 'mitt'

import { h } from 'vue';
import {  ElNotification } from "element-plus";
import LoadingV from './components/LoadingV.vue'

Date.prototype.format = function(fmt) { 
  var o = { 
     "M+" : this.getMonth()+1,                 //月份 
     "d+" : this.getDate(),                    //日 
     "h+" : this.getHours(),                   //小时 
     "m+" : this.getMinutes(),                 //分 
     "s+" : this.getSeconds(),                 //秒 
     "q+" : Math.floor((this.getMonth()+3)/3), //季度 
     "S"  : this.getMilliseconds()             //毫秒 
 }; 
 if(/(y+)/.test(fmt)) {
         fmt=fmt.replace(RegExp.$1, (this.getFullYear()+"").substr(4 - RegExp.$1.length)); 
 }
  for(var k in o) {
     if(new RegExp("("+ k +")").test(fmt)){
          fmt = fmt.replace(RegExp.$1, (RegExp.$1.length==1) ? (o[k]) : (("00"+ o[k]).substr((""+ o[k]).length)));
      }
  }
 return fmt; 
}  
 
const app = createApp(App)
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component)
}

let lo
const mymitt =mitt()
app.config.globalProperties.$mybus = mymitt
app.config.globalProperties.$loadingon =()=>{
  lo = ElNotification({
    title: '正在处理，请稍后...',
    message: h(LoadingV,()=>{}),
    showClose: false,
    duration:0
  })
}

app.config.globalProperties.$loadingoff = ()=>{
  lo.close()
}
mymitt.on("loading",()=>{console.log("emit ok")})


app.use(ElementPlus).use(router).mount('#app')


