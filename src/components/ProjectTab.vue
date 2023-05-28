<template>
    <div style="margin-left: 0%;width: 100%;">
        <el-row>
            <el-col :span="2">
                <label style="line-height:30px;text-align: right;">检索公司:</label>
            </el-col>
            <el-col :span="10">
                <el-select style="width: 100%;" @change="comchange" v-model="companyselected" multiple filterable remote
                    reserve-keyword remote-show-suffix placeholder="请选择公司" :remote-method="comremoteMethod"
                    :loading="loading" :multiple-limit=10>
                    <el-option :key="0" :label="'全部'" :value="'0'"></el-option>
                    <el-option v-for="item in companyoptions" :key="item.id" :label="item.name" :value="item.id" />
                </el-select>
            </el-col>
            <el-col :span="2">
                <label style="line-height:30px;text-align: right;">检索业务:</label>
            </el-col>
            <el-col :span="10">
                <el-select @change="businesschange" style="width: 100%;" v-model="businessselected" multiple filterable
                    remote reserve-keyword remote-show-suffix placeholder="请选择业务" :remote-method="busremoteMethod"
                    :loading="loading" :multiple-limit=10>
                    <el-option :key="0" :label="'全部'" :value="'0'"></el-option>
                    <el-option v-for="item in businessoptions" :key="item.id" :label="item.name" :value="item.id" />
                </el-select>
            </el-col>
            <el-col :span="8" style="padding-left: 2%;">

                <el-input @keyup.enter="search()" clearable v-model="keywords" placeholder="请输入项目名称"
                    style="width:100%;margin-top: 2%;">
                    <template #append>
                        <el-button type="primary" id="projectsearch" icon="Search" @click="search()" />
                    </template>
                </el-input>
            </el-col>

        </el-row>
        <el-divider></el-divider>
        <el-table :data="tableData" style="width: 100%">
            <el-table-column fixed prop="name" label="项目名称" width="150" />
            <el-table-column prop="id" label="项目id" width="220" />

            <el-table-column prop="managerName" label="负责人" width="100" />
            <el-table-column prop="projectNumber" label="项目编号" width="200" />

            <!-- <el-table-column prop="businessDepartmentId" label="所属业务部ID" width="120" /> -->
            <el-table-column label="合同金额" prop="contractAmount" width="200">
                <template #default="scope">
                    {{ '￥' + judragnullstring(scope.row.contractAmount) }}
                </template>
            </el-table-column>
            <el-table-column label="到账金额" prop="receivedAmount" width="200">
                <template #default="scope">
                    {{ '￥' + judragnullstring(scope.row.receivedAmount) }}
                </template>
            </el-table-column>
            <el-table-column label="创建时间" prop="createTime" width="200">
                <template #default="scope">
                    {{ timeformate(scope.row.createTime) }}
                </template>
            </el-table-column>


            <el-table-column prop="remark" label="项目简介" width="600" />
            <el-table-column fixed="right" label="操作" width="180">
                <template #default="scope">
                    <el-button link type="primary" size="small" @click="updatecomfun(scope.row)">修改</el-button>
                    <el-button link type="danger" size="small" @click="deletecomfun(scope.row)">删除</el-button>
                    <el-button link type="primary" size="small" @click="detailcomfun(scope.row)">详情</el-button>
                </template>
            </el-table-column>
        </el-table>
        <el-pagination style="margin-top: 15px;" v-model:current-page="currentPage" v-model:page-size="pageSize"
            :page-sizes="[30, 50, 100, 200, 300, 400]" :small="false" :disabled="false" :hide-on-single-page="false"
            layout="total, sizes, prev, pager, next, jumper" :total="total" @size-change="handleSizeChange"
            @current-change="handleCurrentChange" />
        <el-drawer v-model="addliushui_vi" title="添加项目流水" size="50%">
            <!-- :before-close="handleClose" -->
            <el-form ref="addnewLiushuiRef" :model="newliushui" status-icon :rules="rules" label-width="120px"
                class="demo-ruleForm">
                <!-- <el-form-item label="流水编号" prop="serialNumber">
                    <el-input v-model="newliushui.serialNumber" clearable autocomplete="off" />
                </el-form-item> -->
                <el-form-item label="单据编号" prop="documentNumber">
                    <el-input v-model="newliushui.documentNumber" clearable autocomplete="off" />
                </el-form-item>
                <el-form-item label="流水类型" prop="flowTypeId">
                    <el-select v-model="newliushui.flowTypeId" filterable remote reserve-keyword placeholder="请输入流水类型"
                        remote-show-suffix :remote-method="flowtypessuggetion" :loading="flowloding">
                        <el-option v-for="item in flowoptions" :key="item.id" :label="item.name" :value="item.id" />
                    </el-select>
                </el-form-item>

                <el-form-item label="账目类型">
                    <el-switch  size="large" v-model="zhangmu_type" class="ml-2" inline-prompt
                    style="--el-switch-on-color: #13ce66; --el-switch-off-color: #409eff" active-text="出账"
                    inactive-text="入账" />
                </el-form-item>
                <!-- <el-form-item label="入账金额" prop="inAmount">
                    <el-input v-model="newliushui.inAmount" clearable autocomplete="off" />
                </el-form-item>
                <el-form-item label="出账金额" prop="outAmount">
                    <el-input v-model="newliushui.outAmount" clearable autocomplete="off" />
                </el-form-item> -->

                <el-form-item label="流动金额" prop="Amount">
                    <el-input v-model="newliushui.Amount" clearable autocomplete="off" />
                </el-form-item>

                <!-- <el-form-item label="财务负责人" prop="financeManager">
                    <el-input v-model="newliushui.financeManager" clearable autocomplete="off" />
                </el-form-item> -->

                <el-form-item>
                    <el-upload class="upload-demo" drag style="width: 100%;" :on-change="flowuploadsu" :auto-upload="false"
                        v-model:file-list="filess" multiple>
                        <el-icon class="el-icon--upload"><upload-filled /></el-icon>
                        <div class="el-upload__text">
                            拖拽或 <em>点击此处</em>
                        </div>
                        <template #tip>
                            <div class="el-upload__tip">
                                单个文件不要超过10M
                            </div>
                        </template>
                    </el-upload>
                </el-form-item>


                <el-form-item label="描述" prop="remark">
                    <el-input clearable v-model="newliushui.remark" :rows="2" type="textarea" />
                </el-form-item>

                <el-form-item>
                    <el-button type="primary" @click="addflow()">添加</el-button>
                    <el-button @click="resetflow()">重置</el-button>
                </el-form-item>

            </el-form>
        </el-drawer>
        <el-drawer v-model="updatepro" title="修改项目">
            <!-- :before-close="handleClose" -->
            <el-form ref="updateProjectRef" :model="updateproject" status-icon :rules="rules" label-width="120px"
                class="demo-ruleForm">
                <el-form-item label="项目ID" prop="id">
                    <el-input v-model="updateproject.id" clearable autocomplete="off" />
                </el-form-item>
                <el-form-item label="项目名称" prop="name">
                    <el-input v-model="updateproject.name" clearable autocomplete="off" />
                </el-form-item>
                <el-form-item label="负责人" prop="managerName">
                    <el-input v-model="updateproject.managerName" clearable autocomplete="off" />
                </el-form-item>
                <el-form-item label="项目编号" prop="projectNumber">
                    <el-input v-model="updateproject.projectNumber" clearable autocomplete="off" />
                </el-form-item>
                <!-- <el-form-item label="所属事业部ID" prop="businessDepartmentId">
                    <el-input v-model="updateproject.businessDepartmentId" clearable autocomplete="off" />
                </el-form-item> -->
                <el-form-item label="合同金额" prop="contractAmount">
                    <el-input v-model="updateproject.contractAmount" clearable autocomplete="off" />
                </el-form-item>
                <!-- <el-form-item label="到账金额" prop="receivedAmount">
                    <el-input v-model="updateproject.receivedAmount" clearable autocomplete="off" />
                </el-form-item> -->
                <el-form-item label="项目简介" prop="remark">
                    <el-input clearable v-model="updateproject.remark" :rows="4" type="textarea" />
                </el-form-item>

                <el-form-item>
                    <el-button type="primary" @click="submitupdateForm()">修改</el-button>
                    <el-button @click="resetupdateForm()">重置</el-button>
                </el-form-item>
            </el-form>
        </el-drawer>
        <el-drawer v-model="detailpro_v" title="项目详情" size="70%">

            <el-descriptions class="margin-top" title="公司信息" :column="2" border>
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

            </el-descriptions>
            <div style="height: 15px;"></div>

            <el-descriptions class="margin-top" title="业务信息" :column="2" border>
                <el-descriptions-item>
                    <template #label>
                        <div class="cell-item">
                            <el-icon>
                                <user />
                            </el-icon>
                            &nbsp;&nbsp;&nbsp;名称
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
            <div style="height: 15px;"></div>
            <el-descriptions class="margin-top" title="项目信息" :column="2" border>
                <el-descriptions-item>
                    <template #label>
                        <div class="cell-item">
                            <el-icon>
                                <user />
                            </el-icon>
                            &nbsp;&nbsp;&nbsp;名称
                        </div>
                    </template>
                    {{ judragnullstring(detailprojectmodel.name) }}
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
                    {{ judragnullstring(detailprojectmodel.managerName) }}
                </el-descriptions-item>

                <el-descriptions-item>
                    <template #label>
                        <div class="cell-item">
                            <el-icon>
                                <ToiletPaper />
                            </el-icon>
                            &nbsp;&nbsp;&nbsp;项目编号
                        </div>
                    </template>
                    {{ judragnullstring(detailprojectmodel.projectNumber) }}
                </el-descriptions-item>

                <el-descriptions-item>
                    <template #label>
                        <div class="cell-item">
                            <el-icon>
                                <Coin />
                            </el-icon>
                            &nbsp;&nbsp;&nbsp;合同金额
                        </div>
                    </template>
                    ￥ {{ judragnullstring(detailprojectmodel.contractAmount) }}
                </el-descriptions-item>
                <el-descriptions-item>
                    <template #label>
                        <div class="cell-item">
                            <el-icon>
                                <Coin />
                            </el-icon>
                            &nbsp;&nbsp;&nbsp;到账金额
                        </div>
                    </template>
                    ￥{{ judragnullstring(detailprojectmodel.receivedAmount) }}
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
                    {{ judragnullstring(detailprojectmodel.remark) }}
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
                    {{ timeformate(judragnullstring(detailprojectmodel.createTime)) }}
                </el-descriptions-item>

            </el-descriptions>
            <el-divider>
                <el-tag>归属流水</el-tag>
            </el-divider>

            <el-row>
                <el-col :span="16">
                    <div class="form">
                        <label for="search">
                            <input required="" @keyup.enter="searchflow" v-model="flow_keywords" autocomplete="off"
                                placeholder="输入检索单据编号（回车检索）" id="search" type="text">
                            <div class="icon">
                                <svg stroke-width="2" stroke="currentColor" viewBox="0 0 24 24" fill="none"
                                    xmlns="http://www.w3.org/2000/svg" class="swap-on">
                                    <path d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" stroke-linejoin="round"
                                        stroke-linecap="round"></path>
                                </svg>
                                <svg stroke-width="2" stroke="currentColor" viewBox="0 0 24 24" fill="none"
                                    xmlns="http://www.w3.org/2000/svg" class="swap-off">
                                    <path d="M10 19l-7-7m0 0l7-7m-7 7h18" stroke-linejoin="round" stroke-linecap="round">
                                    </path>
                                </svg>
                            </div>
                            <button @click="flow_keywords = ''" type="reset" class="close-btn">
                                <svg viewBox="0 0 20 20" class="h-5 w-5" xmlns="http://www.w3.org/2000/svg">
                                    <path clip-rule="evenodd"
                                        d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z"
                                        fill-rule="evenodd"></path>
                                </svg>
                            </button>
                        </label>
                    </div>
                </el-col>
                <el-col :span="8">
                    <button type="button" class="button" @click="addliushui_vi = true">
                        <span class="button__text">添加项目流水</span>
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
            <el-empty :image-size="200" style="width: 90%;height: 50%;" description="无数据"
                v-if="detailliushuimodel.length == 0" />
            <el-table :data="detailliushuimodel" border v-if="detailliushuimodel.length > 0">
                <el-table-column fixed label="流水编号" prop="serialNumber" width="200" />
                <el-table-column label="ID" prop="id" width="220" />
                <el-table-column label="财务负责人" prop="financeManager" width="100" />
                <el-table-column label="流水类别" prop="flowType" width="180" />
                <el-table-column label="单据编号" prop="documentNumber" width="200" />
                <el-table-column label="创建时间" prop="createTime" width="200">
                    <template #default="scope">
                        {{ timeformate(scope.row.createTime) }}
                    </template>
                </el-table-column>
                <el-table-column label="备注" prop="remark" width="400" />
            </el-table>

            <!-- <el-drawer v-model="addpro_vi" title="修改" :append-to-body="true">
                <el-form ref="addprojectref" :model="newpro" status-icon :rules="rules" label-width="120px"
                    class="demo-ruleForm">
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
                    <el-form-item label="项目到账金额" prop="ReceivedAmount">
                        <el-input v-model="newpro.ReceivedAmount" clearable autocomplete="off" />
                    </el-form-item>
                    <el-form-item label="简介" prop="Remark">
                        <el-input clearable v-model="newpro.Remark" :rows="4" type="textarea" />
                    </el-form-item>

                    <el-form-item>
                        <el-button type="primary" @click="addnewprofunc()">添加</el-button>
                        <el-button @click="resetyenepro()">重置</el-button>
                    </el-form-item>

                </el-form>
            </el-drawer> -->

        </el-drawer>

    </div>
</template>
          
<script>
import { log } from '../extension/log.js'
import { isInt, isFloat, judragnullstring } from '../extension/static.js'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getcompanys, getyewulist, GetAprojectFlow, deletepro, addflowapi, GetFlowTypesSuggestion, GetProjectList, updatepoject, GetOneProjectAllInfo } from '../request/api.js'
import { desolveresponse, desolvererror } from '../extension/response.js'
export default {
    name: "ProjectTab",
    data() {
        return {
            tableData: [
                // {
                //     id: 1,
                //     name: "",
                //     managerName: "",
                //     projectNumber: "",
                //     remark: "",
                //     businessDepartmentId: 23,
                //     contractAmount: 2000,
                //     receivedAmount: 500,
                //     createTime: ""
                // }

            ],
            companyselected: [],//第一个下拉框选中的
            businessselected: [],//第二个下拉框
            keywords: "",
            searchtype: "1",
            flowloding: false,
            rules: {
                name: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                managerName: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                projectNumber: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                documentNumber: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                serialNumber: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                remark: [{ required: false, message: "该字段必填", trigger: ['blur', 'change'] }],
                flowTypeId: [{ required: true, validator: this.flowtypeid, trigger: ['blur', 'change', 'click'] }],
                financeManager: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                businessDepartmentId: [{ required: true, validator: this.isint, trigger: ['blur', 'change'] }],
                contractAmount: [{ required: true, validator: this.isintorfloat, trigger: ['blur', 'change'] }],
                receivedAmount: [{ required: true, validator: this.isintorfloat, trigger: ['blur', 'change'] }],
                Amount: [{ required: true, validator: this.isintorfloat, trigger: ['blur', 'change'] }],
                // inAmount: [{ required: true, validator: this.isintorfloat, trigger: ['blur', 'change'] }],
                // outAmount: [{ required: true, validator: this.isintorfloat, trigger: ['blur', 'change'] }],


            },
            addpro: false,
            updatepro: false,
            detailpro_v: false,
            addliushui_vi: false,
            newliushui: {
                projectId: "",//项目ID
                // serialNumber: "",//流水编号
                documentNumber: "",// 单据编号
                flowTypeId: "",// 流水类型
                // financeManager: "",// 财务负责人
                remark: "",// 描述
                files: [],
                inAmount: 0,
                Amount: 0,
                outAmount: 0
            },
            updateproject: {
                id: 0,
                name: "",
                managerName: "",
                projectNumber: "",
                remark: "",
                businessDepartmentId: 0,
                contractAmount: 0,
                receivedAmount: 0,
            },
            currentPage: 1,
            pageSize: 30,
            total: 100,
            companyoptions: [],
            businessoptions: [],
            comloading: false,
            busloading: false,
            comlist: [],
            buslist: [],
            detailcommodel: {

            },
            detailyewumodel: {

            },
            detailprojectmodel: {

            },
            detailliushuimodel: [],
            flow_keywords: "",
            flowoptions: [],
            files: [],
            filess: [],
            zhangmu_type:true
        }
    },
    methods: {
        timeformate(time) {
            var oldTime = (new Date(time)).getTime();
            var curTime = new Date(oldTime).format("yyyy年MM月dd日");
            return (curTime);
        },
        judragnullstring(e) {
            return judragnullstring(e)
        },
        search() {
            GetProjectList(this.currentPage, this.pageSize, this.keywords != "" ? this.keywords : "#", JSON.stringify(this.businessselected)).then(res => {
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
        comchange() {
            if (this.companyselected.filter(s => s == "0").length > 0 && this.companyselected.length != 1) {
                this.companyselected = ["0"]
                ElMessage({
                    message: "选择全部择不能选择其他",
                    type: 'warning',
                    duration: 1500
                })
            }
        },
        businesschange() {
            if (this.businessselected.filter(s => s == "0").length > 0 && this.businessselected.length != 1) {
                this.businessselected = ["0"]
                ElMessage({
                    message: "选择全部择不能选择其他",
                    type: 'warning',
                    duration: 1500
                })
            }
        },
        comremoteMethod(query) {
            this.businessoptions = []
            this.businessselected = []
            this.comloading = true
            getcompanys(10, 1, query == "" ? "#" : query, "1").then(res => {
                if (res.status == true) {
                    log(res.data)
                    this.companyoptions = res.data.tables.filter(s => s.id != "0")
                    this.comloading = false
                } else {
                    this.companyoptions = []
                    this.comloading = false
                }

            }).catch(() => {
                this.companyoptions = []
                this.comloading = false
            })
        },
        busremoteMethod(query) {
            this.busloading = true
            getyewulist(JSON.stringify(this.companyselected), 1, 10, query != "" ? query : "#").then(res => {
                if (res.status == true) {
                    this.businessoptions = res.data.tables.filter(s => s.id != "0")
                } else {
                    this.businessoptions = []
                }
                this.busloading = false
            }).catch(() => {
                this.businessoptions = []
                this.busloading = false
            })
        },
        onAddItem() {
            this.addpro = true
        },
        updatecomfun(e) {
            log(e)
            this.updateproject = JSON.parse(JSON.stringify(e))
            this.updatepro = true
        },
        init() {
            this.search()
        },
        resetForm() {
            this.$refs.addnewprojectRef.resetFields()
            this.newproject = {
                name: "",
                managerName: "",
                projectNumber: "",
                remark: "",
                businessDepartmentId: 0,
                contractAmount: 0,
                receivedAmountc: 0
            }
        },
        resetupdateForm() {
            this.$refs.updateProjectRef.resetFields()
        },
        submitupdateForm() {
            this.$refs.updateProjectRef.validate((valid) => {
                if (valid) {
                    this.$loadingon()
                    updatepoject(this.updateproject).then(res => {
                        if (res.status == true) {
                            ElMessage({
                                message: res.message,
                                type: 'success',
                                duration: 1500
                            })
                            this.updatepro = false
                            this.updateproject = {
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
        isint(rule, val, callback) {
            if (!isInt(val)) {
                callback(new Error('请输入整数'))
                return
            }
            callback()
        },
        isintorfloat(rule, val, callback) {
            if (!isInt(val) && !isFloat(val)) {
                callback(new Error('请输入数字'))
                return
            }
            callback()
        },
        detailcomfun(e) {
            this.$loadingon()
            GetOneProjectAllInfo(e.id, "#").then(res => {
                if (res.status == true) {
                    this.detailprojectmodel = res.data.projects
                    this.detailpro_v = true
                    this.detailcommodel = res.data.company
                    this.detailyewumodel = res.data.business
                    this.detailliushuimodel = res.data.flows.filter(s => s.flow.id != "0")
                        .map(s => {
                            var i = {
                                "id": s.flow.id,
                                "projectId": s.flow.projectId,
                                "serialNumber": s.flow.serialNumber,
                                "documentNumber": s.flow.documentNumber,
                                "flowTypeId": s.flow.flowTypeId,
                                "financialStaffId": s.flow.financialStaffId,
                                "financeManager": s.flow.financeManager,
                                "createTime": s.flow.createTime,
                                "remark": s.flow.remark,
                                "flowType": s.flowType.name
                            }
                            return i
                        }

                        )


                    log(res.data)
                }
                else {
                    desolveresponse(res)
                }
                this.$loadingoff()
            }).catch((er) => {
                desolvererror(er)
                this.$loadingoff()
            })
        },
        searchflow() {
            GetAprojectFlow(this.detailprojectmodel.id, this.flow_keywords == "" ? "#" : this.flow_keywords).then(res => {
                if (res.status == true && res.data.length > 0) {
                    this.detailliushuimodel = res.data.filter(s => s.flow.id != "0").map(s => {
                        var i = {
                            "id": s.flow.id,
                            "projectId": s.flow.projectId,
                            "serialNumber": s.flow.serialNumber,
                            "documentNumber": s.flow.documentNumber,
                            "flowTypeId": s.flow.flowTypeId,
                            "financialStaffId": s.flow.financialStaffId,
                            "financeManager": s.flow.financeManager,
                            "createTime": s.flow.createTime,
                            "remark": s.flow.remark,
                            "flowType": s.flowType == null ? '该类别已删除' : s.flowType.name
                        }
                        return i
                    }
                    )

                } else {
                    this.detailliushuimodel = []
                }

            }).catch(er => {
                desolvererror(er)
            })
        },
        flowtypeid(rule, val, callback) {
            log(val)
            if (val == "" || val == "0" || val == undefined) {
                callback(new Error('请选择流水类型'))
                return
            }
            callback()
        },
        flowtypessuggetion(query) {
            this.flowloding = true
            GetFlowTypesSuggestion(10, query == "" ? "#" : query, "1").then(res => {
                if (res.status == true) {
                    this.flowoptions = res.data.filter(s => s.id != "0")
                    this.flowloding = false
                } else {
                    this.flowoptions = []
                    this.flowloding = false
                }

            }).catch(() => {
                this.flowoptions = []
                this.flowloding = false
            })
        },
        addflow() {
            this.$refs.addnewLiushuiRef.validate((valid) => {
                if (valid) {
                    this.$loadingon()
                    this.newliushui.projectId = this.detailprojectmodel.id
                    if(this.zhangmu_type == true) {
                        this.newliushui.outAmount = this.newliushui.Amount
                        this.newliushui.inAmount = 0
                    }
                    else
                    {
                        this.newliushui.outAmount  = 0
                        this.newliushui.inAmount = this.newliushui.Amount
                    }
                    addflowapi(this.newliushui, this.files).then(res => {
                        if (res.status == true) {
                            ElMessage({
                                message: res.message,
                                type: 'success',
                                duration: 1500
                            })
                            this.addliushui_vi = false
                            this.resetsflow()
                            this.searchflow()
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
        resetflow() {
            this.$refs.addnewLiushuiRef.resetFields()
        },
        resetsflow() {
            this.$refs.addnewLiushuiRef.resetFields()
            this.newliushui = {
                projectId: "",//项目ID
                serialNumber: "",//流水编号
                documentNumber: "",// 单据编号
                flowTypeId: "",// 流水类型
                financeManager: "",// 财务负责人
                remark: "",// 描述
            }
        },
        flowuploadsu(uploadFile, uploadFiles) {
            this.files = uploadFiles
            // log(this.files)
            // log("filess")
            // log(this.filess)
        },
        deletecomfun(e) {
            ElMessageBox.confirm('确定删除吗？', {
                distinguishCancelAndClose: true,
                confirmButtonText: '确定',
                type: 'warning',
                cancelButtonText: '取消',
            })
                .then(() => {
                    this.$loadingon()
                    deletepro(e.id).then(res => {
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
        }
    },
    mounted() {
        this.init();
    }
}
</script>
        
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
    transition: 0.3s cubic-bezier(.4, 0, .2, 1);
    position: absolute;
    height: 15px;
}

/* search icon */
.icon {
    position: absolute;
    left: var(--padding);
    transition: 0.3s cubic-bezier(.4, 0, .2, 1);
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

.form input:focus~.icon {
    transform: rotate(var(--rotate)) scale(1.3);
}

.form input:focus~.icon .swap-off {
    opacity: 1;
    transform: rotate(-80deg);
    visibility: visible;
    color: var(--icon-change-color);
}

.form input:focus~.icon .swap-on {
    opacity: 0;
    visibility: visible;
}

.form input:valid~.icon {
    transform: scale(1.3) rotate(var(--rotate))
}

.form input:valid~.icon .swap-off {
    opacity: 1;
    visibility: visible;
    color: var(--icon-change-color);
}

.form input:valid~.icon .swap-on {
    opacity: 0;
    visibility: visible;
}

.form input:valid~.close-btn {
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
 