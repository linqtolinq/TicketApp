<template>
    <div class="common-layout">
        <el-container class="container">

            <el-aside :class="isCollapse ? 'jian' : 'jia'" :width=sidebar_width style="background-color: #156bc0;">
                <!-- <div style="width:100%;background-color: #156bc0;height: 60px; "> -->
                        <!-- <div style="display: flex;padding-left: 10%;">
                            <div><el-icon style="font-size: 30px;padding-top: 9px;color: white;width: auto;">
                                    <HomeFilled />
                                </el-icon></div>
                            <div style="color: white;font-size: 22px;font-weight: 800;margin-top: 10px;width: 55%;">票据管理
                            </div>
                        </div>
                    </div> -->
                    <el-menu :default-active="index" class="el-menu-vertical-demo" :collapse="isCollapse">
                        <el-menu-item style="background-color: #007acc;" index="0">
                            <el-icon style="font-size: 30px;padding-top: 0px;color: white;width: auto;"><HomeFilled /></el-icon>
                            <template #title> <center style="font-size: 20px;color: white;">&nbsp;票据管理</center></template>
                        </el-menu-item>

                        <el-sub-menu index="1">
                            <template #title>
                                <el-icon>
                                    <DataLine />
                                </el-icon>
                                <span>{{user.role == 0 ?'公司管理' :'业务管理'}}</span>
                            </template>
                            <el-menu-item-group>
                                <el-menu-item index="1-1" id="menu-1-1" v-if="user.role == 0" @click="this.$router.push('/Company') & tabchange('Company')">公司管理</el-menu-item>
                                <el-menu-item index="1-2" id="menu-1-2" v-if="user.role == 0" @click="this.$router.push('/Business') & tabchange('Business')">业务管理</el-menu-item>
                                <el-menu-item index="1-3" id="menu-1-3" v-if="user.role == 0" @click="this.$router.push('/Project') & tabchange('Project')">项目管理</el-menu-item>
                                
                                
                                <el-menu-item index="1-4" id="menu-1-4" v-if="user.role == 1" @click="this.$router.push('/MyBusiness') & tabchange('MyBusiness')">我的业务</el-menu-item>
                                <el-menu-item index="1-5" id="menu-1-5" v-if="user.role == 1" @click="this.$router.push('/MyProject') & tabchange('MyProject')">项目管理</el-menu-item>
                                
                            </el-menu-item-group>
                        </el-sub-menu>
                        <el-menu-item index="2-1" v-if="user.role == 0" id="menu-2-1" @click="this.$router.push('/FinancialStaff') & tabchange('FinancialStaff')">
                            <el-icon><Avatar /></el-icon>
                            <template #title>人员管理</template>
                        </el-menu-item>
                        <el-sub-menu index="3" >
                            <template #title>
                                <el-icon>
                                    <Memo />
                                </el-icon>
                                <span>财务管理</span>
                            </template>
                            <el-menu-item-group>
                                <el-menu-item index="3-1" id="menu-3-1"  @click="this.$router.push('/FluvialCategory') & tabchange('FluvialCategory')">流水类别</el-menu-item>
                                <el-menu-item index="3-2" id="menu-3-2" v-if="user.role == 0" @click="this.$router.push('/Flow') & tabchange('Flow')">流水管理</el-menu-item>
                                <el-menu-item index="3-3"  id="menu-3-3" v-if="user.role == 1" @click="this.$router.push('/MyFlow') & tabchange('MyFlow')">流水管理</el-menu-item>
                                <!-- <el-menu-item index="3-3">年月汇总</el-menu-item> -->
                            </el-menu-item-group>
                        </el-sub-menu>
                        <el-menu-item index="4-1" id="menu-4-1"  v-show="user.role == 0"  @click="this.$router.push('/SystemMessage') & tabchange('SystemMessage')">
                            <el-icon><Setting /></el-icon>
                            <template #title>系统消息</template>
                        </el-menu-item>
                    </el-menu>

                </el-aside>
            <el-container style="padding: 0;">
                <el-header style="padding: 0;">
                <div style="display: flex;">
                    
                    <div style="width:100%;padding: 0px 0 0 20px;">
                        <el-page-header :icon="isCollapse ? 'DArrowRight' : 'DArrowLeft'" :title="!isCollapse ? '收起' : '展开'"
                            @back="isCollapse = ~isCollapse">

                            <template #content>
                                <div class="flex items-center">
                                    <el-breadcrumb separator="/">
                                        <el-breadcrumb-item>{{indexrouter.split('|')[0]}}</el-breadcrumb-item>
                                        <el-breadcrumb-item>{{indexrouter.split('|')[1]}}</el-breadcrumb-item>
                                    </el-breadcrumb>
                                </div>
                            </template>
                            <template #extra>
                                <div class="flex items-center" style="padding-top: 10px;">
                                    <el-avatar :size="45">
                                        <img src="@/assets/static/admin.png" />
                                    </el-avatar>
                                    <el-dropdown :hide-on-click="false"
                                        style="margin-top: 8%;margin-left: 15px;margin-right: 20px;">
                                        <span class="el-dropdown-link">
                                            您好，{{ judragnullstring(user.name) }}<el-icon
                                                class="el-icon--right"><arrow-down /></el-icon>
                                        </span>
                                        <template #dropdown>
                                            <el-dropdown-menu>
                                                <el-dropdown-item divided @click="usercenterinfo()">个人中心</el-dropdown-item>
                                                <el-dropdown-item divided @click="userpwd()">修改密码</el-dropdown-item>
                                                <el-dropdown-item divided @click="signout()">退出登录</el-dropdown-item>
                                            </el-dropdown-menu>
                                        </template>
                                    </el-dropdown>
                                </div>
                            </template>
                        </el-page-header>
                    </div>
                </div>


            </el-header>
                <el-container style="background-color: white;">
                    <el-main style="height:82vh;overflow: scroll;">
                        <router-view></router-view>
                    </el-main>
                    <el-footer style="padding: 0;"> 
                        <div style="background-color: #156bc0;width :100%;height: 100%;">
                            <center style="color: white;height: 20px;">
                                <div></div>
                            </center>
                            <center style="color: white;">
                                <div>版权所有@</div>
                            </center>
                        </div>
                    </el-footer>
                </el-container>
            </el-container>
        </el-container>

        <el-drawer v-model="usercenter" title="用户中心">
            <!-- :before-close="handleClose" -->
            <el-form ref="userupdateRef" :model="userupdate" status-icon :rules="rules" label-width="120px"
                class="demo-ruleForm">
                <el-form-item label="姓名" prop="name">
                    <el-input v-model="userupdate.name" clearable  autocomplete="off" />
                </el-form-item>
                <el-form-item label="昵称" prop="nick">
                    <el-input v-model="userupdate.nick" clearable  autocomplete="off" />
                </el-form-item>
                <el-form-item label="账号" prop="userName">
                    {{ userupdate.userName }}
                </el-form-item>
                <el-form-item label="邮箱" prop="email">
                    <el-input clearable  v-model="userupdate.email" />
                </el-form-item>
                <el-form-item label="创建时间" prop="createTime">
                    {{ timeformate(String(userupdate.createTime)) }}
                </el-form-item>
                <el-form-item label="账号级别" prop="role">
                    <el-tag>{{ userupdate.role == 0 ? '管理员' : '用户' }}</el-tag>

                </el-form-item>
                <el-form-item label="地址" prop="address">
                    <el-input clearable  v-model="userupdate.address" :rows="4" type="textarea" />
                </el-form-item>
                <el-form-item label="备注" prop="remark">
                    <el-input clearable  v-model="userupdate.remark" :rows="4" type="textarea" />
                </el-form-item>

                <el-form-item>
                    <el-button type="primary" @click="submitForm(ruleFormRef)">修改</el-button>
                    <el-button @click="resetForm(ruleFormRef)">重置</el-button>
                </el-form-item>

            </el-form>
        </el-drawer>


        <el-dialog v-model="pwdupdate" title="密码修改" width="30%" center>
            <el-form ref="pwdRef" :model="pwdform" status-icon :rules="rules" label-width="120px"
                class="demo-ruleForm">
                <el-form-item label="旧密码" prop="OldPwd">
                    <el-input show-password clearable  v-model="pwdform.OldPwd" type="password" autocomplete="off" />
                </el-form-item>
                <el-form-item label="新密码" prop="NewPwd">
                    <el-input show-password clearable  v-model="pwdform.NewPwd" type="password" autocomplete="off" />
                </el-form-item>
                <el-form-item label="确认密码" prop="NewConfirmPwd">
                    <el-input show-password clearable  v-model="pwdform.NewConfirmPwd" type="password" autocomplete="off" />
                </el-form-item>
            </el-form>
            <template #footer>
                <span class="dialog-footer">
                    <el-button type="primary" @click="submitpwd()">提交</el-button>
                    <el-button @click="resetpwd()">重置</el-button>
                </span>
            </template>
        </el-dialog>
    </div>
</template>
  
<script>
import { log } from '../extension/log.js'
import { desolveresponse, desolvererror } from '../extension/response.js'
import { judragnullstring } from '../extension/static.js'
import { ElMessageBox, ElMessage } from 'element-plus'
import { GetOwnInfo, userupdate ,userupdatepwd} from '../request/api.js'
import { Local } from "../jwt/jwt.js";

export default {
    name: 'HomeIndex',
    data() {
        return {
            index:"1-1",
            indexrouter:" | ",
            sidebar_width: '200px',
            isCollapse: false,
            pwdupdate: false,
            user: {
                address: "",
                createTime: "",
                email: "",
                icon: "",
                id: "",
                ip: "",
                name: "",
                nick: "",
                remark: "",
                role: "",
                userName: ""
            },
            userupdate: {
                address: "",
                createTime: "",
                email: "",
                icon: "",
                id: "",
                ip: "",
                name: "",
                nick: "",
                remark: "",
                role: "",
                userName: ""
            },
            usercenter: false,
            rules: {
                name: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                address: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                // createTime:[{ required: true, message: "该字段必填", trigger:  ['blur','change']}],
                email: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] },
                { type: "email", message: "非法邮箱", trigger: ['blur', 'change'] }],
                OldPwd:[{ required: true, message: "该字段必填", trigger:  ['blur','change']}],
                NewPwd:[{ required: true, message: "该字段必填", trigger:  ['blur','change']}],
                NewConfirmPwd:[{ required: true, message: "该字段必填", trigger:  ['blur','change']}],
                // nick: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                // remark: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                // role:[{ required: true, message: "该字段必填", trigger:  ['blur','change']}],
                userName: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
            },
            pwdform:{
                OldPwd:"",
                NewPwd:"",
                NewConfirmPwd:""
            }
        }
    },
    watch: {

    },
    methods: {
        tabchange(e){
            switch(e)
            {
                case "Company":
                    // document.getElementById('menu-1-1').click()
                    this.index = "1-1"
                    this.indexrouter = "公司管理|公司管理"
                    break;
                case "Business":
                    // document.getElementById('menu-1-2').click()
                    this.indexrouter = "公司管理|业务管理"
                    this.index = "1-2"
                    break;
                case "Project":
                    // document.getElementById('menu-1-3').click()
                    this.indexrouter = "公司管理|项目管理"
                    this.index = "1-3"
                    break;
                case "MyBusiness":
                    // document.getElementById('menu-1-4').click()
                    this.indexrouter = "业务管理|我的业务"
                    this.index = "1-4"
                    break;
                case "MyProject":
                    // document.getElementById('1-5').click()
                    this.indexrouter = "业务管理|项目管理"
                    this.index = "1-5"
                    break;
                case "FinancialStaff":
                    // document.getElementById('menu-2-1').click()
                    this.indexrouter = "人员管理|人员管理"
                    this.index = "2-1"
                    break;
                 case "FluvialCategory":
                    // document.getElementById('menu-3-1').click()
                    this.indexrouter = "财务管理|流水类别"
                    this.index = "3-1"
                    break;
                 case "Flow":
                    // document.getElementById('menu-3-2').click()
                    this.indexrouter = "财务管理|流水管理"
                    this.index = "3-2"
                    break;
                    case "MyFlow":
                    // document.getElementById('menu-3-3').click()
                    this.indexrouter = "财务管理|流水管理"
                    this.index = "3-3"
                    break;
                    case "SystemMessage":
                    // document.getElementById('menu-4-1').click()
                    this.indexrouter = "系统信息|系统信息"
                    this.index = "4-1"
                    break;
                default:
                    break;
            }
            // document.getElementById('menu-'+e.index).click()
        },
        // 修改密码
        userpwd() {
            this.pwdupdate = true
            this.pwdform ={
                OldPwd:"",
                NewPwd:"",
                NewConfirmPwd:""
            }
        },
        timeformate(time) {
            var oldTime = (new Date(time)).getTime();
            var curTime = new Date(oldTime).format("yyyy年MM月dd日 hh时mm分ss秒");
            return (curTime);
        },
        usercenterinfo() {
            this.usercenter = true;
            this.userupdate = JSON.parse(JSON.stringify(this.user))
        },
        submitForm() {
            this.$refs.userupdateRef.validate((valid) => {
                if (valid) {
                    userupdate(
                        this.userupdate.name,
                        this.userupdate.remark,
                        this.userupdate.nick,
                        this.userupdate.email,
                        this.userupdate.address,
                    ).then(res => {
                        if (res.status == true) {
                            ElMessage({
                                message: res.message,
                                type: 'success',
                                duration: 1500
                            })
                            this.usercenter = false
                        } else {
                            desolveresponse(res)
                        }

                    }).catch(e => {
                        desolvererror(e)
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
        resetForm() {
            this.$refs.userupdateRef.resetFields()
            this.userupdate = JSON.parse(JSON.stringify(this.user))
        },

        submitpwd() {
            this.$refs.pwdRef.validate((valid) => {
                if (valid) {
                    userupdatepwd( this.pwdform  ).then(res => {
                        if (res.status == true) {
                            ElMessage({
                                message: res.message,
                                type: 'success',
                                duration: 1500
                            })
                            this.pwdupdate = false
                        } else {
                            desolveresponse(res)
                        }

                    }).catch(e => {
                        desolvererror(e)
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
        resetpwd() {
            this.$refs.pwdRef.resetFields()
            this.pwdform ={
                OldPwd:"",
                NewPwd:"",
                NewConfirmPwd:""
            }
        },
        init() {
            GetOwnInfo().then(re => {
                if (re.status == true) {
                    this.user = re.data
                    log(this.user)
                    // if(this.user.role == 0)
                    // {
                    //     this.$router.push('/Company')
                    // }
                    // else if(this.user.role == 1)
                    // {
                    //     this.$router.push('/MyProject')
                    // }
                    // else
                    // {
                    //     this.$router.push('/404')
                    // }
                }
                else {
                    desolveresponse(re)
                }
            }).catch(e => {
                desolvererror(e)
            })
        },
        judragnullstring(e) {
            return judragnullstring(e)
        },
        handleClose(done) {
            ElMessageBox.confirm('确定关闭吗？', {
                distinguishCancelAndClose: true,
                confirmButtonText: '确定',
                cancelButtonText: '取消',
            })
                .then(() => {
                    done()
                })
                .catch(() => {
                    // catch error
                })
        },
        signout() {
            ElMessageBox.confirm('确定退出吗？', {
                distinguishCancelAndClose: true,
                confirmButtonText: '确定',
                cancelButtonText: '取消',
            })
                .then(() => {
                    Local.set("token", "  ")
            this.$router.push("/Login")
                })
                .catch(() => {
                    // catch error
                })
           
        }
    },
    mounted() {
        this.init()
        this.tabchange(location.href.split('/#/')[1])
    }
}
</script>
  
  <!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.common-layout .container {
    height: 100vh;
    width: 100vw;
    background-color: #efefef;
}

@keyframes sidebarchange_jian {
    0% {
        width: 200px;
    }

    100% {
        width: 64px;
    }
}

@keyframes sidebarchange_jia {
    0% {
        width: 64px;
    }

    100% {
        width: 200px;
    }
}

.jia {
    width: 200px;
    animation: sidebarchange_jia 0.6s;
}

.jian {
    width: 64px;
    animation: sidebarchange_jian 0.6s;
}

</style>
  