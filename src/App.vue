<template>
   <el-config-provider :locale="locale">
    <router-view></router-view>
  </el-config-provider>
 
</template>

<script>
import zhCn from "element-plus/lib/locale/lang/zh-cn"
import English from "element-plus/lib/locale/lang/en"
import { ElConfigProvider } from 'element-plus'
export default {
  name: 'App',
  components:{
    ElConfigProvider
  },
  data() {
    return {
      locale: zhCn,
      En1:English
    }
  },
  methods:{
    
  },
  mounted(){
   
  }
}
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  width: 100vw;
  height: 100vh;
  float: left;
        display: flex;
        justify-content: center;
        align-items: center;
        flex-direction: column;
}
:root {
        --backgroundcolor: #FFFFFF;
        --forcecolor: #0f67bc;
    }

    * {
        padding: 0;
        position: relative;
        margin: 0;
    }

    body {
        background: url('@/assets/static/loginbackgroud.jpg') no-repeat center center;
        background-size: cover;
        background-attachment: fixed;
        width: 100vw;
        min-height: 100vh;
        /* 弹性布局，水平居中排列，垂直居中排列 ,排布方向按列拍*/
        display: flex;
        justify-content: center;
        align-items: center;
        flex-direction: column;

    }
</style>
