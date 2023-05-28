<template>
    <div></div>
 </template>
 
 <script>
 import { desolveresponse, desolvererror } from '../extension/response.js'
// import { judragnullstring } from '../extension/static.js'
// import { ElMessageBox, ElMessage } from 'element-plus'
import { log } from '../extension/log.js'
import { GetOwnInfo} from '../request/api.js'
 export default {
   name: 'EmptyPage',
   data(){
       return {
        
       }
   },
   methods:{
 
   },
   mounted(){
    GetOwnInfo().then(re => {
                if (re.status == true) {
                    this.user = re.data
                    log(this.user)
                    if(this.user.role == 0)
                    {
                        this.$router.push('/Company')
                    }
                    else if(this.user.role == 1)
                    {
                        this.$router.push('/MyProject')
                    }
                    else
                    {
                        this.$router.push('/404')
                    }
                }
                else {
                    desolveresponse(re)
                }
            }).catch(e => {
                desolvererror(e)
            })
   }
 }
 </script>
 
 <!-- Add "scoped" attribute to limit CSS to this component only -->
 <style scoped>
 h3 {
   margin: 40px 0 0;
 }
 ul {
   list-style-type: none;
   padding: 0;
 }
 li {
   display: inline-block;
   margin: 0 10px;
 }
 a {
   color: #42b983;
 }
 </style>
 