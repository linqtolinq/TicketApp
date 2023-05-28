
<template>
    <div style="width: 100%;">
        <el-row>

            <el-col :span="10">
                <div class="m-4">
                    检索公司：&nbsp;
                    <el-select style="width:80%;" v-model="selectcom" multiple filterable remote reserve-keyword
                        placeholder="请输入公司名称" remote-show-suffix :remote-method="remoteMethod" :loading="loading"
                        @change="testcheng">
                        <el-option :key="0" :label="'全部'" :value="'0'"></el-option>
                        <el-option v-for="item in comoptions" :key="item.id" :label="item.name" :value="item.id" />
                    </el-select>
                </div>
            </el-col>
            <el-col :span="10">
                <el-input @keyup.enter="search()" clearable v-model="keywords" placeholder="请输入检索关键字"
                    class="input-with-select" style="width:100%">
                    <template #prepend>
                        <el-select v-model="searchtype" disabled placeholder="选择检索类型" style="width: 130px">
                            <el-option label="按名称" value="1" />
                            <!-- <el-option label="按负责人" value="2" /> -->
                        </el-select>
                    </template>
                    <template #append>
                        <el-button type="primary" id="companysearch" icon="Search" @click="search()" />
                    </template>
                </el-input></el-col>
            <!-- <el-col :span="4">
                <el-button @click="onAddItem">添加公司</el-button>
            </el-col> -->
        </el-row>


    </div>
    <el-divider />
    <div>
        <el-table :data="tableData" style="width: 100%">
            <el-table-column fixed prop="name" label="名称" width="200" />
            <el-table-column prop="managerName" label="管理员" width="120" />
            <el-table-column prop="id" label="ID" width="200" />
            <el-table-column prop="createTime" label="创建时间" width="150">
                <template #default="scope">
                    {{timeformate(scope.row.createTime)}}
                </template>
            </el-table-column>
            <!-- <el-table-column prop="registeredLocation" label="注册地" width="300" /> -->
            <el-table-column prop="remark" label="备注" />
            <el-table-column fixed="right" label="操作" width="180">
                <template #default="scope">

                    <el-button link type="primary" size="small" @click="updatecomfun(scope.row)">修改</el-button>
                    <el-button link type="danger" size="small" @click="deletecomfun(scope.row)">删除</el-button>
                    <el-button link type="primary" size="small" @click="detailyewufun(scope.row)">详情</el-button>

                </template>
            </el-table-column>
        </el-table>
        <el-pagination style="margin-top: 15px;" v-model:current-page="currentPage" v-model:page-size="pageSize"
            :page-sizes="[30, 50, 100, 200, 300, 400]" :small="false" :disabled="false" :hide-on-single-page="false"
            layout="total, sizes, prev, pager, next, jumper" :total="total" @size-change="handleSizeChange"
            @current-change="handleCurrentChange" />
    </div>



    <el-drawer v-model="updatecom" title="修改业务">
        <!-- :before-close="handleClose" -->
        <el-form ref="comupdateRef" :model="updateyewumodel" status-icon :rules="rules" label-width="120px"
            class="demo-ruleForm">
            <el-form-item label="业务名称" prop="name">
                <el-input v-model="updateyewumodel.name" clearable autocomplete="off" />
            </el-form-item>
            <el-form-item label="负责人" prop="managerName">
                <el-input v-model="updateyewumodel.managerName" clearable autocomplete="off" />
            </el-form-item>

            <el-form-item label="备注" prop="remark">
                <el-input clearable v-model="updateyewumodel.remark" :rows="4" type="textarea" />
            </el-form-item>

            <el-form-item>
                <el-button type="primary" @click="submitupdateForm()">修改</el-button>
                <el-button @click="resetupdateForm()">重置</el-button>
            </el-form-item>

        </el-form>
    </el-drawer>


    <el-drawer v-model="detailyewu_v" title="业务详情" size="70%">
    
        <el-descriptions class="margin-top" title="公司信息" :column="2" border>
            <!-- <template #extra>
                <el-button type="primary">Operation</el-button>
            </template> -->
            <el-descriptions-item>
                <template #label>
                    <div class="cell-item">
                        <el-icon>
                            <user />
                        </el-icon>
                        &nbsp;&nbsp;&nbsp;姓名
                    </div>
                </template>
                {{ judragnullstring(detailcommodel.name) }}
            </el-descriptions-item>
            <el-descriptions-item>
                <template #label>
                    <div class="cell-item">
                        <el-icon >
                            <iphone />
                        </el-icon>
                        &nbsp;&nbsp;&nbsp;手机
                    </div>
                </template>
                {{ judragnullstring(detailcommodel.phoneNumber) }}
            </el-descriptions-item>
            <el-descriptions-item>
                <template #label>
                    <div class="cell-item">
                        <el-icon >
                            <UserFilled />
                        </el-icon>
                        &nbsp;&nbsp;&nbsp;负责人
                    </div>
                </template>
                {{ judragnullstring(detailcommodel.managerName) }}
            </el-descriptions-item>
            <el-descriptions-item>
                <template #label>
                    <div class="cell-item">
                        <el-icon>
                            <tickets />
                        </el-icon>
                        &nbsp;&nbsp;&nbsp;备注
                    </div>
                </template>
                {{ judragnullstring(detailcommodel.remark) }}
            </el-descriptions-item>
            <el-descriptions-item>
                <template #label>
                    <div class="cell-item">
                        <el-icon>
                            <location />
                        </el-icon>
                        &nbsp;&nbsp;注册地
                    </div>
                </template>
                {{ judragnullstring(detailcommodel.registeredLocation) }}
            </el-descriptions-item>

            <el-descriptions-item>
                <template #label>
                    <div class="cell-item">
                        <el-icon>
                            <Aim />
                        </el-icon>
                        &nbsp;&nbsp;注册时间
                    </div>
                </template>
                {{ timeformate(judragnullstring(detailcommodel.createTime)) }}
            </el-descriptions-item>

        </el-descriptions>
        <div style="height: 15px;"></div>

        <el-descriptions class="margin-top" title="业务信息" :column="2" border>
            <!-- <template #extra>
                <el-button type="primary">Operation</el-button>
            </template> -->
            <el-descriptions-item>
                <template #label>
                    <div class="cell-item">
                        <el-icon>
                            <user />
                        </el-icon>
                        &nbsp;&nbsp;&nbsp;姓名
                    </div>
                </template>
                {{ judragnullstring(detailyewumodel.name) }}
            </el-descriptions-item>
           
            <el-descriptions-item>
                <template #label>
                    <div class="cell-item">
                        <el-icon>
                            <UserFilled />
                        </el-icon>
                        &nbsp;&nbsp;&nbsp;负责人
                    </div>
                </template>
                {{ judragnullstring(detailyewumodel.managerName) }}
            </el-descriptions-item>
            <el-descriptions-item>
                <template #label>
                    <div class="cell-item">
                        <el-icon>
                            <tickets />
                        </el-icon>
                        &nbsp;&nbsp;&nbsp;备注
                    </div>
                </template>
                {{ judragnullstring(detailyewumodel.remark) }}
            </el-descriptions-item>
            

            <el-descriptions-item>
                <template #label>
                    <div class="cell-item">
                        <el-icon>
                            <Aim />
                        </el-icon>
                        &nbsp;&nbsp;注册时间
                    </div>
                </template>
                {{ timeformate(judragnullstring(detailyewumodel.createTime)) }}
            </el-descriptions-item>

        </el-descriptions>
        <el-divider>
            <el-tag>归属项目</el-tag>
        </el-divider>

        <el-row>
            <el-col :span="16">
                <div class="form">
    <label for="search">
        <input required="" @keyup.enter="searchpro" v-model="pro_keywords" autocomplete="off" placeholder="输入检索项目名称（回车检索）" id="search" type="text">
        <div class="icon">
            <svg stroke-width="2" stroke="currentColor" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg" class="swap-on">
                <path d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" stroke-linejoin="round" stroke-linecap="round"></path>
            </svg>
            <svg stroke-width="2" stroke="currentColor" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg" class="swap-off">
                <path d="M10 19l-7-7m0 0l7-7m-7 7h18" stroke-linejoin="round" stroke-linecap="round"></path>
            </svg>
        </div>
        <button @click="pro_keywords = ''" type="reset" class="close-btn">
            <svg viewBox="0 0 20 20" class="h-5 w-5" xmlns="http://www.w3.org/2000/svg">
                <path clip-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" fill-rule="evenodd"></path>
            </svg>
        </button>
    </label>
</div>
            </el-col>
            <el-col :span="8">
                <button type="button" class="button" @click="addpro_vi = true">
                    <span class="button__text">添加项目</span>
                    <span class="button__icon"><svg xmlns="http://www.w3.org/2000/svg" width="24" viewBox="0 0 24 24"
                            stroke-width="2" stroke-linejoin="round" stroke-linecap="round" stroke="currentColor"
                            height="24" fill="none" class="svg">
                            <line y2="19" y1="5" x2="12" x1="12"></line>
                            <line y2="12" y1="12" x2="19" x1="5"></line>
                        </svg></span>
                </button>
            </el-col>
        </el-row>
        <el-divider></el-divider>
        <el-empty :image-size="200" style="width: 90%;height: 50%;" description="无数据"  v-if="detailproject.length == 0"/>
        <el-table :data="detailproject" border v-if="detailproject.length > 0">
                        <el-table-column fixed label="名称" prop="name"  width="200"/>
                        <el-table-column label="ID" prop="id"  width="220"/>
                        <el-table-column label="负责人" prop="managerName" width="100" />
                        <el-table-column label="项目编号" prop="projectNumber" width="200" />
                        <el-table-column label="合同金额" prop="contractAmount" width="200" >
                            <template #default="scope">
                                {{'￥'+judragnullstring(scope.row.contractAmount)}}
                            </template>
                        </el-table-column>
                        <el-table-column label="到账金额" prop="receivedAmount" width="200">
                            <template #default="scope">
                                {{'￥'+judragnullstring(scope.row.receivedAmount)}}
                            </template>
                        </el-table-column>
                        <el-table-column label="创建时间" prop="createTime" width="200">
                            <template #default="scope">
                                {{timeformate(scope.row.createTime)}}
                            </template>
                        </el-table-column>
                        <el-table-column label="备注" prop="remark" width="400"/>
        </el-table>

        <el-drawer v-model="addpro_vi" title="添加新项目" :append-to-body="true">
            <el-form ref="addprojectref" :model="newpro" status-icon :rules="rules" label-width="120px" class="demo-ruleForm">
                <el-form-item label="所属公司">
                    <el-input :readonly="true" :value="detailcommodel.name" clearable autocomplete="off" />
                </el-form-item>
                <el-form-item label="所属业务">
                    <el-input :readonly="true" :value="detailyewumodel.name" clearable autocomplete="off" />
                </el-form-item>
                <el-form-item label="项目名称" prop="Name">
                    <el-input v-model="newpro.Name" clearable autocomplete="off" />
                </el-form-item>
                <el-form-item label="项目负责人" prop="ManagerName">
                    <el-input v-model="newpro.ManagerName" clearable autocomplete="off" />
                </el-form-item>
                <el-form-item label="项目编号" prop="ProjectNumber">
                    <el-input v-model="newpro.ProjectNumber" clearable autocomplete="off" />
                </el-form-item>
                <el-form-item label="项目合同金额" prop="ContractAmount">
                    <el-input v-model="newpro.ContractAmount" clearable autocomplete="off" />
                </el-form-item>
                <!-- <el-form-item label="项目到账金额" prop="ReceivedAmount">
                    <el-input v-model="newpro.ReceivedAmount" clearable autocomplete="off" />
                </el-form-item> -->
                <el-form-item label="简介" prop="Remark">
                    <el-input clearable v-model="newpro.Remark" :rows="4" type="textarea" />
                </el-form-item>

                <el-form-item>
                    <el-button type="primary" @click="addnewprofunc()">添加</el-button>
                    <el-button @click="resetyenepro()">重置</el-button>
                </el-form-item>

            </el-form>
        </el-drawer>

    </el-drawer>

   

</template>
 
<script>
import { log } from '../extension/log.js'
import { desolveresponse, desolvererror } from '../extension/response.js'
import { istelephonenumber,judragnullstring ,isFloat,isInt} from '../extension/static.js'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getcompanys, deleteyewu, updateyewu,jiansuo_yewu_project,addnewproject,getyewulist ,GetOneBusinessDepartmentAllInfo} from '../request/api.js'

// import { Local } from "../jwt/jwt.js";
export default {
    name: 'BusinessTab',
    data() {
        return {
            now: new Date(),
            tableData: [
                // {
                //     createTime: "",
                //     id: "",
                //     managerName: "",
                //     name: "",
                //     phoneNumber: "",
                //     registeredLocation: "",
                //     remark: ""
                // }
            ],
            keywords: "",
            searchtype: "1",
            addpro_vi:false,
            updateyewumodel: {
                "id": 0,
                "name": "",
                "managerName": "",
                "remark": "",
                createTime:""
            },
            detailyewu_v:false,
            detailyewumodel:{
                "id": 0,
                "name": "",
                "managerName": "",
                "remark": "",
            },
            detailcommodel:{
                "id": 0,
                "name": "",
                "managerName": "",
                "remark": "",
            },
            newpro:{
                Name:"",
                ManagerName:"",
                ProjectNumber:"",
                Remark:"",
                BusinessDepartmentId:"",
                ContractAmount:"",
                ReceivedAmount:"",
            },
            detailproject:[],
            rules: {
                Name: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                ManagerName: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                RegisteredLocation: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                PhoneNumber: [{ required: true, validator: this.istelephonenumber, trigger: ['blur', 'change'] }],
                name: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                managerName: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                ProjectNumber: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                ReceivedAmount: [{ required: true, validator: this.isFloatorfloat, trigger: ['blur', 'change'] }],
                ContractAmount: [{ required: true, validator: this.isFloatorfloat, trigger: ['blur', 'change'] }],

            },
            updatecom: false,
            currentPage: 1,
            pageSize: 30,
            total: 100,
            comoptions: [],
            loading: false,
            selectcom: [],
            pro_keywords:""
        }
    },
    methods: {
        addnewprofunc(){
            this.$refs.addprojectref.validate((valid) => {
                if (valid) {
                    this.$loadingon()
                    this.newpro.BusinessDepartmentId = this.detailyewumodel.id
                    addnewproject(this.newpro).then(res => {
                        if (res.status == true) {
                            ElMessage({
                                message: res.message,
                                type: 'success',
                                duration: 1500
                            })
                            this.addpro_vi = false
                            this.resetyenepro()
                            this.searchpro()
                        } else {
                            desolveresponse(res)
                        }
                        this.$loadingoff()
                    }).catch(e => {
                        desolvererror(e)
                        this.$loadingoff()
                    })
                } else {
                    ElMessage({
                        message: '表单填写不正确',
                        type: 'error',
                        duration: 1500
                    })
                    return false
                }
            })
        },
        //检索业务项目
        searchpro(){
            jiansuo_yewu_project(this.detailyewumodel.id,this.pro_keywords == "" ? "#" : this.pro_keywords).then(res => {
                if (res.status == true) {
                   this.detailproject = res.data.projects
                    log(res.data.projects)
                } 
                else    
                {
                    desolveresponse(res)
                }
            }).catch((er) => {
                desolvererror(er)
            })
        },
        resetyenepro(){
            this.$refs.addprojectref.resetFields()
            this. newpro={
                Name:"",
                ManagerName:"",
                ProjectNumber:"",
                ProjectId:"",
                Remark:"",
                BusinessDepartmentId:"",
                ContractAmount:"",
                ReceivedAmount:"",
            }
        },
        isFloatorfloat(rule, val, callback){
            if (!isInt(val) && !isFloat(val)) {
                callback(new Error('数据格式不合法，请检查数据格式'))
                return
            }
            callback()
        },
        judragnullstring(e) {
            return judragnullstring(e)
        },
        detailyewufun(e){
            this.$loadingon()
            GetOneBusinessDepartmentAllInfo(e.id).then(res => {
                if (res.status == true) {
                    this.detailyewumodel = JSON.parse(JSON.stringify(e))
                    this.detailyewu_v =true
                    this.detailcommodel = res.data.company
                    this.detailproject = res.data.projects
                    log(res.data.projects)
                } 
                else    
                {
                    desolveresponse(res)
                }
                this.$loadingoff()
            }).catch((er) => {
                desolvererror(er)
                this.$loadingoff()
            })
            
        },
        testcheng() {
            log(this.selectcom)
            if(this.selectcom.filter(s=>s == "0").length > 0 && this.selectcom.length != 1)
            {
                this.selectcom=["0"]
                ElMessage({
                                message: "选择全部择不能选择其他",
                                type: 'warning',
                                duration: 1500
                            })
            }
        },
        timeformate(time) {
            var oldTime = (new Date(time)).getTime();
            var curTime = new Date(oldTime).format("yyyy年MM月dd日");
            return (curTime);
        },
        // 远程检索
        remoteMethod(query) {
            this.loading = true

            getcompanys(10, 1, query == "" ? "#" : query, "1").then(res => {
                if (res.status == true) {
                    this.comoptions = res.data.tables.filter(s => s.id != "0")
                    this.loading = false
                } else {
                    this.comoptions = []
                    this.loading = false
                }

            }).catch(() => {
                this.comoptions = []
                this.loading = false
            })
        },
        updatecomfun(e) {
            log(e)
            this.updateyewumodel = JSON.parse(JSON.stringify(e))
            this.updatecom = true
        },
        istelephonenumber(rule, val, callback) {
            if (!istelephonenumber(val)) {
                callback(new Error('手机号不合法，请检查手机号格式'))
                return
            }
            callback()
        },
        deletecomfun(e) {
            log(e)
            ElMessageBox.confirm('确定删除吗？', {
                distinguishCancelAndClose: true,
                confirmButtonText: '确定',
                type: 'warning',
                cancelButtonText: '取消',
            })
                .then(() => {
                    this.$loadingon()
                    deleteyewu(e.id).then(res => {
                        if (res.status == true) {
                            this.tableData = this.tableData.filter(s => s.id != e.id)
                            this.total = this.total - 1
                            ElMessage({
                                message: res.message,
                                type: 'success',
                                duration: 1500
                            })
                        } else {
                            desolveresponse(res)
                        }
                        this.$loadingoff()
                    }).catch(er => {
                        desolvererror(er)
                        this.$loadingoff()
                    })
                })
                .catch(() => {
                    // catch error
                })

        },
        submitupdateForm() {
            this.$refs.comupdateRef.validate((valid) => {
                if (valid) {
                    this.$loadingon()
                    updateyewu(this.updateyewumodel).then(res => {
                        if (res.status == true) {
                            ElMessage({
                                message: res.message,
                                type: 'success',
                                duration: 1500
                            })
                            this.updatecom = false
                            this.updateyewumodel = {
                                "id": 0,
                                "name": "",
                                "managerName": "",
                                "remark": "",
                            }
                            this.resetupdateForm()
                            this.search()
                        } else {
                            desolveresponse(res)
                        }
                        this.$loadingoff()
                    }).catch(e => {
                        desolvererror(e)
                        this.$loadingoff()
                    })
                } else {
                    ElMessage({
                        message: '表单填写不正确',
                        type: 'error',
                        duration: 1500
                    })
                    return false
                }
            })
        },
        resetupdateForm() {
            this.$refs.comupdateRef.resetFields()
            this.updateyewumodel = {
                                "id": 0,
                                "name": "",
                                "managerName": "",
                                "remark": "",
                            }
        },
        search() {
            getyewulist(JSON.stringify(this.selectcom), this.currentPage,  this.pageSize,this.keywords != "" ? this.keywords : "#").then(res => {
                
                if (res.status == true) {
                    this.tableData = res.data.tables.filter(s => s.id != "0")
                    this.total = Number(res.data.counts)-1
                    log(res.data)
                } else {
                    desolveresponse(res)
                }

            }).catch(er => {
                desolvererror(er)
            })
        },
        init() {
            this.search()
            //   document.getElementById("companysearch").addEventListener("click", ()=>{
            // this.search()
            //   })
        },
        handleSizeChange() {
            this.search()
        },
        handleCurrentChange() {
            this.search()
        },
    },
    mounted() {
        this.init()
    }
}
</script>
 
 <!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
/* From uiverse.io by @satyamchaudharydev */
/* this button is inspired by -- whatsapp input */
/* == type to see fully interactive and click the close buttom to remove the text  == */

.form {
  --input-bg: #FFf;
 /*  background of input */
  --padding: 1.5em;
  --rotate: 80deg;
 /*  rotation degree of input*/
  --gap: 2em;
  /*  gap of items in input */
  --icon-change-color: #007acc;
 /*  when rotating changed icon color */
  --height: 40px;
 /*  height */
  width: 80%;
  padding-inline-end: 1em;
 /*  change this for padding in the end of input */
  background: var(--input-bg);
  position: relative;
  border-radius: 4px;
}

.form label {
  display: flex;
  align-items: center;
  width: 100%;
  height: var(--height);
}

.form input {
  width: 100%;
  padding-inline-start: calc(var(--padding) + var(--gap));
  outline: none;
  background: none;
  border: 0;
}
/* style for both icons -- search,close */
.form svg {
  /* display: block; */
  color: #111;
  transition: 0.3s cubic-bezier(.4,0,.2,1);
  position: absolute;
  height: 15px;
}
/* search icon */
.icon {
  position: absolute;
  left: var(--padding);
  transition: 0.3s cubic-bezier(.4,0,.2,1);
  display: flex;
  justify-content: center;
  align-items: center;
}
/* arrow-icon*/
.swap-off {
  transform: rotate(-80deg);
  opacity: 0;
  visibility: hidden;
}
/* close button */
.close-btn {
  /* removing default bg of button */
  background: none;
  border: none;
  right: calc(var(--padding) - var(--gap));
  box-sizing: border-box;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #111;
  padding: 0.1em;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  transition: 0.3s;
  opacity: 0;
  transform: scale(0);
  visibility: hidden;
}

.form input:focus ~ .icon {
  transform: rotate(var(--rotate)) scale(1.3);
}

.form input:focus ~ .icon .swap-off {
  opacity: 1;
  transform: rotate(-80deg);
  visibility: visible;
  color: var(--icon-change-color);
}

.form input:focus ~ .icon .swap-on {
  opacity: 0;
  visibility: visible;
}

.form input:valid ~ .icon {
  transform: scale(1.3) rotate(var(--rotate))
}

.form input:valid ~ .icon .swap-off {
  opacity: 1;
  visibility: visible;
  color: var(--icon-change-color);
}

.form input:valid ~ .icon .swap-on {
  opacity: 0;
  visibility: visible;
}

.form input:valid ~ .close-btn {
  opacity: 1;
  visibility: visible;
  transform: scale(1);
  transition: 0s;
}

.button {
    position: relative;
    width: 150px;
    height: 40px;
    cursor: pointer;
    display: flex;
    align-items: center;
    border: 1px solid #409eff;
    background-color: #409eff;
}

.button,
.button__icon,
.button__text {
    transition: all 0.3s;
}

.button .button__text {
    transform: translateX(30px);
    color: #fff;
    font-weight: 600;
}

.button .button__icon {
    position: absolute;
    transform: translateX(109px);
    height: 100%;
    width: 39px;
    background-color: #409eff;
    display: flex;
    align-items: center;
    justify-content: center;
}

.button .svg {
    width: 30px;
    stroke: #fff;
}

.button:hover {
    background: #409eff;
}

.button:hover .button__text {
    color: transparent;
}

.button:hover .button__icon {
    width: 148px;
    transform: translateX(0);
}

.button:active .button__icon {
    background-color: #409eff;
}

.button:active {
    border: 1px solid #409eff;
}
</style>
 