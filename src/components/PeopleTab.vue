<template>
    <div style="width: 60%;">
        <el-row>
            <el-col :span="18">
                <el-input @keyup.enter="search()" clearable v-model="keywords" placeholder="请输入检索关键字"
                    class="input-with-select" style="width:100%">
                    <template #prepend>
                        <el-select disabled v-model="searchtype" placeholder="选择检索类型" style="width: 130px">
                            <el-option label="按姓名 / 昵称" value="1" />
                            <el-option label="按昵称" value="2" />
                        </el-select>
                    </template>
                    <template #append>
                        <el-button type="primary" id="peoplesearch" icon="Search" @click="search()" />
                    </template>
                </el-input>
            </el-col>
            <el-col :span="6">
                <el-button @click="onAddItem">添加人员</el-button>
            </el-col>
        </el-row>


    </div>
    <el-divider></el-divider>

    <div>
        <el-table :data="tableData" style="width: 100%">

            <el-table-column fixed prop="name" label="姓名" width="120" />
            <el-table-column prop="id" label="ID" width="180" />
            <el-table-column prop="userName" label="账号" width="120" />
            <el-table-column label="身份" width="100">
                <template #default="scope">
                    <el-tag>{{ scope.row.role == 0 ? '管理员' : '用户' }}</el-tag>
                </template>
            </el-table-column>
            <el-table-column prop="email" label="邮箱" width="200" />
            <el-table-column prop="address" label="地址" width="200" />
            <el-table-column prop="createTime" label="创建时间" width="150">
                <template #default="scope">
                    {{ timeformate(scope.row.createTime) }}
                </template>
            </el-table-column>
            <el-table-column prop="ip" label="上一次登录IP" width="200" />
            <el-table-column prop="nick" label="昵称" width="200" />
            <el-table-column prop="remark" label="描述" width="350" />
            <el-table-column fixed="right" label="操作" width="100">
                <template #default="scope">
                    <el-button link type="primary" size="small" @click="detailcomfun(scope.row)">详情</el-button>
                    <el-button link type="danger" size="small" @click="deletecomfun(scope.row)">删除</el-button>
                </template>
            </el-table-column>
        </el-table>
        <el-pagination style="margin-top: 15px;" v-model:current-page="currentPage" v-model:page-size="pageSize"
            :page-sizes="[30, 50, 100, 200, 300, 400]" :small="false" :disabled="false" :hide-on-single-page="false"
            layout="total, sizes, prev, pager, next, jumper" :total="total" @size-change="handleSizeChange"
            @current-change="handleCurrentChange" />
    </div>

    <el-drawer v-model="addcom" title="添加人员">
        <!-- :before-close="handleClose" -->
        <el-form ref="userupdateRef" :model="newpeople" status-icon :rules="rules" label-width="120px"
            class="demo-ruleForm">
            <el-form-item label="账号" prop="UserName">
                <el-input v-model="newpeople.UserName" clearable autocomplete="off" />
            </el-form-item>
            <el-form-item label="密码" prop="Password">
                <el-input type="password" v-model="newpeople.Password" clearable autocomplete="off" />
            </el-form-item>

            <el-form-item label="姓名" prop="Name">
                <el-input clearable v-model="newpeople.Name" />
            </el-form-item>
            <el-form-item label="昵称" prop="Nick">
                <el-input clearable v-model="newpeople.Nick" />
            </el-form-item>
            <el-form-item label="邮箱" prop="Email">
                <el-input clearable v-model="newpeople.Email" type="Email" />
            </el-form-item>
            <el-form-item label="身份" prop="Role">
                <el-select v-model="newpeople.Role" clearable placeholder="Select">
                    <el-option label="管理员" :value="0" />
                    <el-option label="用户" :value="1" />
                </el-select>
            </el-form-item>

            <el-form-item label="地址" prop="Address">
                <el-input clearable v-model="newpeople.Address" :rows="2" type="textarea" />
            </el-form-item>
            <el-form-item label="备注" prop="Remark">
                <el-input clearable v-model="newpeople.Remark" :rows="2" type="textarea" />
            </el-form-item>
            <el-form-item>
                <el-button type="primary" @click="submitForm()">添加</el-button>
                <el-button @click="resetForm()">重置</el-button>
            </el-form-item>

        </el-form>
    </el-drawer>



    <el-drawer v-model="butable_v" title="人员详情" size="50%">

        <!-- <el-descriptions class="margin-top" title="人员信息" :column="2" border>
                <el-descriptions-item>
                    <template #label>
                        <div class="cell-item">
                            <el-icon>
                                <user />
                            </el-icon>
                            &nbsp;&nbsp;&nbsp;名称
                        </div>
                    </template>
                    {{ judragnullstring(detailcommodel.name) }}
                </el-descriptions-item>
                <el-descriptions-item>
                    <template #label>
                        <div class="cell-item">
                            <el-icon>
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
                            <el-icon>
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

            </el-descriptions> -->
        <el-divider content-position="left">
            <el-tag>添加部门</el-tag>
        </el-divider>
        <center>
            <el-input @keyup.enter="bu_change" @change="bu_change" v-model="bu_keywords" style="width:50%" class="w-50 m-2" size="large"
                placeholder="输入业务部名称检索（回车检索）" suffix-icon="Search" />
                &nbsp;  &nbsp;  &nbsp;  &nbsp; <el-button style="width: 100px;height:40px;" @click="submit_up_bu" type="primary">提交</el-button>
        </center>
        <rl-row style="height: 20px;"></rl-row>
        <!-- <el-transfer v-model="selected_bu" style="text-align: left; display: inline-block;margin-top: 20px;"
            :left-default-checked="[]" :right-default-checked="[]" :titles="['可选择', '已选择']" :button-texts="['移除', '选择']"
            :data="bu_se_optios" @change="selected_bu_change"
           > -->
        <!-- </el-transfer> -->
        <el-row style="padding-top: 20px;">
            <el-col :span="8">
                <center><el-tag style="font-size:20px;height: 40px;">选择区</el-tag></center>
                <el-card class="box-card">
                    <el-empty description="无数据" v-if="bu_se_optios.length == 0"/>
                    <el-checkbox v-model="i.selected" v-for="(i,index) in bu_se_optios" :key="index" :label="i.label" size="large" />
                </el-card>
            </el-col>
            <el-col :span="8">
                <el-button style="width: 100px;height:40px;margin-top:50%" @click="add_bu" type="primary">添加&nbsp;<el-icon><DArrowRight /></el-icon> </el-button>
                <el-button style="width: 100px;height:40px;margin-top:50%" @click="jian_bu" type="success"> <el-icon><DArrowLeft /></el-icon>&nbsp;解除</el-button>
            </el-col>
            <el-col :span="8">
                <center><el-tag style="font-size:20px;height: 40px;">已选择</el-tag></center>
                <el-card class="box-card">
                    <el-empty description="未选择任何" v-if="selected_bu.length == 0"/>
                    <el-checkbox v-model="i.selected" v-for="(i,index) in selected_bu" :key="index" :label="i.label" size="large" />
                </el-card>
            </el-col>
        </el-row>



        <el-divider content-position="left">
            <el-tag>负责部门</el-tag>
        </el-divider>
        <el-table :data="butable" border>
            <el-table-column fixed label="名称" prop="name" width="200" />
            <el-table-column label="ID" prop="id" width="220" />
            <el-table-column label="负责人" prop="managerName" width="100" />
           
            
            <el-table-column label="创建时间" prop="createTime" width="200">
                <template #default="scope">
                    {{ timeformate(scope.row.createTime) }}
                </template>
            </el-table-column>
            <el-table-column label="备注" prop="remark" width="400" />
            <el-table-column fixed="right" label="操作" width="180">
                <template #default="scope">
                    <el-button link type="danger" size="small" @click="deleterelationfun(scope.row)">解除</el-button>
                </template>
            </el-table-column>
        </el-table>
    </el-drawer>
</template>
 
<script>
import { log } from '../extension/log.js'
import { desolveresponse, desolvererror } from '../extension/response.js'
import { ElMessage, ElMessageBox } from 'element-plus'
import { istelephonenumber,judragnullstring } from '../extension/static.js'
import {
    useradd, GetUserLists, getyewulist, deleteuser,UpdateAuserAllBusinessDepartment, getAuserAllBusinessDepartment, DeletAuserAllBusinessDepartment
} from '../request/api.js'

// import { Local } from "../jwt/jwt.js";
export default {
    name: 'PeopleTab',
    data() {
        return {
            now: new Date(),
            tableData: [
                // {
                //     UserName: "",
                //     Name: "",
                //     Remark: "",
                //     Nick: "",
                //     Email: "",
                //     Address: "",
                // }
            ],
            bu_keywords: "",
            butable: [],
            keywords: "",
            searchtype: "1",
            newpeople: {
                UserName: "",
                Password: "",
                Name: "",
                Remark: "",
                Nick: "",
                Email: "",
                Address: "",
                Role: 1
            },
            rules: {
                // UserName: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                Password: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                Name: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                UserName: [{ required: true, validator: this.istelephonenumber, trigger: ['blur', 'change'] }],
                Email: [{ required: false, type: "email", message: "邮箱格式错误", trigger: ['blur', 'change'] }],
                // Remark: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                // Address: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                // Nick: [{ required: false, message: "该字段必填", trigger: ['blur', 'change'] }],
            },
            addcom: false,
            updatecom: false,
            butable_v: false,
            currentPage: 1,
            pageSize: 30,
            total: 100,
            userdetailmodel: {},
            selected_bu: [],
            bu_se_optios: []
        }
    },
    methods: {
        judragnullstring(e) {
            return judragnullstring(e)
        },
        submit_up_bu() {
            this.$loadingon()
            UpdateAuserAllBusinessDepartment(this.userdetailmodel.id,JSON.stringify(this.selected_bu.map(s=>s.key))).then(res => {
                        if (res.status == true) {
                            ElMessage({
                                message: res.message,
                                type: 'success',
                                duration: 1500
                            })
                            // this. selected_bu= []
                            this.bu_se_optios= []
                            this.bu_search()
                        } else {
                            desolveresponse(res)
                        }
                        this.$loadingoff()
                    }).catch(e => {
                        desolvererror(e)
                        this.$loadingoff()
                    })
        },
        add_bu(){
            this.bu_se_optios.filter(s=>s.selected == true).forEach(s=>{
                if(this.selected_bu.filter(ss=>ss.key == s.key).length == 0)
                {
                    s.selected = false;
                    this.selected_bu.push(JSON.parse(JSON.stringify(s)))
                }
            })
            // this.bu_se_optios.forEach(s=>s.selected = false)
        },
        jian_bu(){
            this.selected_bu = this.selected_bu.filter(s=>s.selected == false).map(s=>{
                var i = s
                s.selected = false
                return i
            })

        },
        renderFunc(h, option) {
            return h('span', null, option.label)
        },
        istelephonenumber(rule, val, callback) {
            if (!istelephonenumber(val)) {
                callback(new Error('手机号不合法，请检查手机号格式'))
                return
            }
            callback()
        },
        timeformate(time) {
            var oldTime = (new Date(time)).getTime();
            var curTime = new Date(oldTime).format("yyyy年MM月dd日");
            return (curTime);
        },
        onAddItem() {
            this.addcom = true
        },
        resetForm() {
            this.$refs.userupdateRef.resetFields()
            this.newpeople = {
                UserName: "",
                Password: "",
                Name: "",
                Remark: "",
                Nick: "",
                Email: "",
                Address: "",
                Role: 1
            }
        },
        submitForm() {
            this.$refs.userupdateRef.validate((valid) => {
                if (valid) {
                    this.$loadingon()
                    useradd(this.newpeople).then(res => {
                        if (res.status == true) {
                            ElMessage({
                                message: res.message,
                                type: 'success',
                                duration: 1500
                            })
                            this.addcom = false
                            this.newpeople = {
                                UserName: "",
                                Password: "",
                                Name: "",
                                Remark: "",
                                Nick: "",
                                Email: "",
                                Address: "",
                            }
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
                    deleteuser(e.id).then(res => {
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
        search() {
            GetUserLists(this.currentPage, this.pageSize, this.keywords != "" ? this.keywords : "#", this.searchtype).then(res => {
                if (res.status == true) {
                    this.tableData = res.data.tables.filter(s => s.id != "0")
                    this.total = Number(res.data.counts) - 1
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
        detailcomfun(e) {
            this.userdetailmodel = JSON.parse(JSON.stringify(e))
            // this.$loadingon()
            getAuserAllBusinessDepartment(e.id).then(res => {
                this.butable_v = true
                this.bu_change()
                if (res.status == true) {
                    if (res.data != null) {
                        this.butable = res.data.filter(s => s.id != "0")
                        this.selected_bu =res.data.filter(s => s.id != "0").map(s => {
                        var i = {
                            key: s.id,
                            label: s.name,
                            selected: false,
                        }
                        return i
                    })
                    }
                    else {
                        this.butable = []
                    }
                    log(res.data)
                    // this.$loadingon()
                } else {
                    desolveresponse(res)
                    // this.$loadingon()
                }
                // this.$loadingoff()
            }).catch(er => {
                desolvererror(er)
                // this.$loadingoff()
            })
        },
        deleterelationfun(e) {
            ElMessageBox.confirm('确定解除关系吗？', {
                distinguishCancelAndClose: true,
                confirmButtonText: '确定',
                type: 'warning',
                cancelButtonText: '取消',
            })
                .then(() => {
                    this.$loadingon()
                    DeletAuserAllBusinessDepartment(this.userdetailmodel.id, `[${e.id}]`).then(res => {
                        if (res.status == true) {
                            this.butable = this.butable.filter(s => s.id != e.id)
                            this.selected_bu = this.selected_bu.filter(s => s.key != e.id)
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
                        this.$loadingoff()
                        desolvererror(er)
                    })
                })
                .catch(() => {
                    // catch error
                })
        },
        bu_change() {
            this.$loadingon()
            getyewulist("[0]", 1, 10, this.bu_keywords != "" ? this.bu_keywords : "#").then(res => {
                if (res.status == true) {
                    this.bu_se_optios = res.data.tables.filter(s => s.id != "0").map(s => {
                        var i = {
                            key: s.id,
                            label: s.name,
                            selected: false,
                        }
                        return i
                    })
                } else {
                    this.bu_se_optios = []
                }
            this.$loadingoff()
            }).catch(() => {
                this.bu_se_optios = []
                this.$loadingoff()

            })
        },
        bu_search(){
            getAuserAllBusinessDepartment(this.userdetailmodel.id).then(res => {
                this.butable_v = true
                this.bu_change()
                if (res.status == true) {
                    if (res.data != null) {
                        this.butable = res.data.filter(s => s.id != "0")
                    }
                    else {
                        this.butable = []
                    }
                    log(res.data)
                } else {
                    desolveresponse(res)
                }

            }).catch(er => {
                desolvererror(er)
            })
        }
    },
    mounted() {
        this.init()
    }
}
</script>
 
 <!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
/* .input-with-select .el-input-group__prepend {
  background-color: var(--el-fill-color-blank);
} */
.box-card{
    height: 360px;
    /* overflow: scroll; */
    margin: 20px;
    border-radius: 10px;
}
</style>
 