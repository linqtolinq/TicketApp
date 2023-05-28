<template>
    <el-row>
        <el-col :span="2">
            <label style="line-height:30px;text-align: right;">检索业务:</label>
        </el-col>
        <el-col :span="10">
            <el-select @change="businesschange" style="width: 100%;" v-model="businessselected" multiple filterable remote
                reserve-keyword remote-show-suffix placeholder="请选择业务" :remote-method="busremoteMethod"
                :loading="busloading" :multiple-limit=10>
                <el-option :key="0" :label="'全部'" :value="'0'"></el-option>
                <el-option v-for="item in businessoptions" :key="item.id" :label="item.name" :value="item.id" />
            </el-select>
        </el-col>

        <el-col :span="2">
            <label style="line-height:30px;text-align: right;">检索项目:</label>
        </el-col>
        <el-col :span="10">
            <el-select @change="prochange" style="width: 100%;" v-model="projectselected" multiple filterable remote
                reserve-keyword remote-show-suffix placeholder="请选择项目" :remote-method="proremoteMethod"
                :loading="proloading" :multiple-limit=10>
                <el-option :key="0" :label="'全部'" :value="'0'"></el-option>
                <el-option v-for="item in projectionoptions" :key="item.id" :label="item.name" :value="item.id" />
            </el-select>
        </el-col>

        <el-col :span="8" style="padding-left: 2%;">
            <el-input @keyup.enter="search()" clearable v-model="keywords" placeholder="请输入流水编号"
                style="width:100%;margin-top: 2%;">
                <template #append>
                    <el-button type="primary" id="projectsearch" icon="Search" @click="search()" />
                </template>
            </el-input>
        </el-col>
        <el-col :span="2" style="padding-top: 10px;;">
            <el-button type="primary" :disabled="selectedtables.length == 0" @click="out_file">导出</el-button>
        </el-col>
        <el-divider></el-divider>
        <el-table :data="tableData" @selection-change="handleSelectionChange">
            <el-table-column type="selection" width="55" />
            <el-table-column fixed label="流水编号" prop="serialNumber" width="200" />
            <el-table-column label="ID" prop="id" width="220" />
            <el-table-column label="财务负责人" prop="financeManager" width="100" />
            <el-table-column label="流水类别" prop="flowType" width="180" />
            <el-table-column label="单据编号" prop="documentNumber" width="200" />
            <el-table-column label="入账额" prop="inAmount" width="100" />
            <el-table-column label="出账额" prop="outAmount" width="100" />
            <el-table-column label="创建时间" prop="createTime" width="200">
                <template #default="scope">
                    {{ timeformate(scope.row.createTime) }}
                </template>
            </el-table-column>
            <el-table-column label="备注" prop="remark" width="400" />

            <el-table-column fixed="right" label="操作" width="180">
                <template #default="scope">
                    <el-button link type="primary" size="small" @click="updateflowfun(scope.row)">修改</el-button>
                    <el-button link type="danger" size="small" @click="deleteflowfun(scope.row)">删除</el-button>
                    <el-button link type="primary" size="small" @click="detailflowfun(scope.row)">详情</el-button>
                </template>
            </el-table-column>
        </el-table>

        <el-pagination style="margin-top: 15px;" v-model:current-page="currentPage" v-model:page-size="pageSize"
            :page-sizes="[30, 50, 100, 200, 300, 400]" :small="false" :disabled="false" :hide-on-single-page="false"
            layout="total, sizes, prev, pager, next, jumper" :total="total" @size-change="handleSizeChange"
            @current-change="handleCurrentChange" />


    </el-row>

    <el-drawer v-model="addliushui_vi" title="修改项目流水" size="50%">
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
            <!-- <el-form-item label="财务负责人" prop="financeManager">
                <el-input v-model="newliushui.financeManager" clearable autocomplete="off" />
            </el-form-item> -->


            <el-form-item label="账目类型">
                <el-switch size="large" v-model="zhangmu_type" class="ml-2" inline-prompt
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
                <el-button type="primary" @click="updateflow()">修改</el-button>
                <el-button @click="resetflow()">重置</el-button>
            </el-form-item>

        </el-form>
    </el-drawer>

    <el-drawer v-model="detailliushui_vi" title="修改项目流水" size="50%">


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
        <div style="height: 15px;"></div>

        <el-descriptions class="margin-top" title="流水信息" :column="2" border>
            <el-descriptions-item>
                <template #label>
                    <div class="cell-item">
                        <el-icon>
                            <tickets />
                        </el-icon>
                        &nbsp;&nbsp;&nbsp;单据编号
                    </div>
                </template>
                {{ judragnullstring(detailflowmodel.documentNumber) }}
            </el-descriptions-item>
            <el-descriptions-item>
                <template #label>
                    <div class="cell-item">
                        <el-icon>
                            <tickets />
                        </el-icon>
                        &nbsp;&nbsp;&nbsp;流水编号
                    </div>
                </template>
                {{ judragnullstring(detailflowmodel.serialNumber) }}
            </el-descriptions-item>

            <el-descriptions-item>
                <template #label>
                    <div class="cell-item">
                        <el-icon>
                            <UserFilled />
                        </el-icon>
                        &nbsp;&nbsp;&nbsp;管理员
                    </div>
                </template>
                {{ judragnullstring(detailflowmodel.financeManager) }}
            </el-descriptions-item>



            <el-descriptions-item>
                <template #label>
                    <div class="cell-item">
                        <el-icon>
                            <UserFilled />
                        </el-icon>
                        &nbsp;&nbsp;&nbsp;经办人
                    </div>
                </template>
                {{ judragnullstring(userdetail.name) }}
            </el-descriptions-item>

            <el-descriptions-item>
                <template #label>
                    <div class="cell-item">
                        <el-icon>
                            <Iphone />
                        </el-icon>
                        &nbsp;&nbsp;&nbsp;经办人手机
                    </div>
                </template>
                {{ judragnullstring(userdetail.userName) }}
            </el-descriptions-item>
            <el-descriptions-item>
                <template #label>
                    <div class="cell-item">
                        <el-icon>
                            <Message />
                        </el-icon>
                        &nbsp;&nbsp;&nbsp;经办人邮箱
                    </div>
                </template>
                {{ judragnullstring(userdetail.email) }}
            </el-descriptions-item>
            <el-descriptions-item>
                <template #label>
                    <div class="cell-item">
                        <el-icon>
                            <tickets />
                        </el-icon>
                        &nbsp;&nbsp;&nbsp;流水类型
                    </div>
                </template>
                {{ judragnullstring(detailflowmodel.flowType) }}
            </el-descriptions-item>
            <el-descriptions-item>
                <template #label>
                    <div class="cell-item">
                        <el-icon>
                            <Coin />
                        </el-icon>
                        &nbsp;&nbsp;&nbsp;入账
                    </div>
                </template>
                ￥ {{ (detailflowmodel.inAmount) }}
            </el-descriptions-item>
            <el-descriptions-item>
                <template #label>
                    <div class="cell-item">
                        <el-icon>
                            <Coin />
                        </el-icon>
                        &nbsp;&nbsp;&nbsp;出账
                    </div>
                </template>
                ￥{{ judragnullstring(detailflowmodel.outAmount) }}
            </el-descriptions-item>
           

            <el-descriptions-item>
                <template #label>
                    <div class="cell-item">
                        <el-icon>
                            <Aim />
                        </el-icon>
                        &nbsp;&nbsp;创建时间
                    </div>
                </template>
                {{ timeformate(judragnullstring(detailflowmodel.createTime)) }}
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
                {{ judragnullstring(detailflowmodel.remark) }}
            </el-descriptions-item>
        </el-descriptions>


        <div style="height: 15px;"></div>
        <el-divider content-position="left">
            <el-tag>支撑材料</el-tag>
        </el-divider>
        <!-- <el-card shadow="hover" style="display:flex;width: 100%;"> -->
        <el-row style="width:100%">
            <div style="width:100%; display: inline-block; ">
                <el-image v-for="(item, index) in file" :key="index"
                    style="width: 160px; height: 200px;border: dashed 2px #efefef;border-radius: 10px;margin: 10px;padding: 10px;"
                    :src="item" :zoom-rate="1.2" :preview-src-list="file" :initial-index="4" fit="cover">
                    <template #error>
                        <div class="image-slot">
                            <el-icon><icon-picture /></el-icon>
                        </div>
                    </template></el-image>
                <el-result style="margin: 10px; border: dashed 2px #efefef;border-radius: 10px;"
                    v-for="(item, index) in flownotimages" :key="index" :sub-title="item.fileName">
                    <template #icon>
                        <el-icon style="font-size: 70px;">
                            <Document />
                        </el-icon>
                    </template>
                    <template #extra>
                        <center style="font-size: 15px;">该文件不支持预览</center>
                    </template>
                </el-result>
            </div>
        </el-row>
        <!-- </el-card> -->

    </el-drawer>
</template>
 
<script>
import { log } from '../extension/log.js'
import { judragnullstring,isInt, isFloat,  isImage, getFileExtensionFromMimeType } from '../extension/static.js'
import { desolvererror, desolveresponse } from '../extension/response.js'
import {
    OutputFlow,
    getcompanys, getAuserOwnAllBusinessDepartment, getflowlist, GetAFlowAllInfo,
    UpdateFlow, getprojectsuggestionlist, DeleteFlow, GetFlowTypesSuggestion
} from '../request/api.js'
import { ElMessage, ElMessageBox } from 'element-plus'
export default {
    name: 'MyFlow',
    data() {
        return {
            comloading: false,
            businessloading: false,
            proloading: false,

            companyselected: [],
            companyoptions: [],
            flowloding: false,

            businessselected: [],
            businessoptions: [],

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
            },
            flowoptions: [],
            projectselected: [],
            projectionoptions: [],
            tableData: [],
            total: 0,
            currentPage: 1,
            pageSize: 30,
            keywords: "",
            // 流水详情
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
            zhangmu_type: true,
            addliushui_vi: false,
            detailliushui_vi: false,

            detailflowmodel: {},
            detailprojectmodel: {},
            detailcommodel: {},
            detailyewumodel: {},
            userdetail: {},
            flowimages: [
                {
                    "base64": '',
                    "fileName": "file",
                    "fileType": "text/javascript"
                }
            ],
            file: [],
            flownotimages: [],
            selectedtables: [],
            files:[]
        }
    },
    methods: {
        isintorfloat(rule, val, callback) {
            if (!isInt(val) && !isFloat(val)) {
                callback(new Error('请输入数字'))
                return
            }
            callback()
        },
        timeformate(time) {
            var oldTime = (new Date(time)).getTime();
            var curTime = new Date(oldTime).format("yyyy年MM月dd日");
            return (curTime);
        },
        judragnullstring(e) {
            return judragnullstring(e)
        },
        handleSelectionChange(e) {
            this.selectedtables = e
            log(e)
        },
        businesschange() {
            this.projectselected = []
            if (this.businessselected.filter(s => s == "0").length > 0 && this.companyselected.length != 1) {
                this.businessselected = ["0"]
                ElMessage({
                    message: "选择全部择不能选择其他",
                    type: 'warning',
                    duration: 1500
                })
            }
        },
        prochange() {
            if (this.projectselected.filter(s => s == "0").length > 0 && this.companyselected.length != 1) {
                this.projectselected = ["0"]
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
            getAuserOwnAllBusinessDepartment(query != "" ? query : "#", 1, 10).then(res => {
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
        proremoteMethod(query) {
            this.proloading = true
            getprojectsuggestionlist(JSON.stringify(this.businessselected), 1, 10, query != "" ? query : "#").then(res => {
                if (res.status == true) {
                    this.projectionoptions = res.data.filter(s => s.id != "0")
                } else {
                    this.projectionoptions = []
                }
                this.proloading = false
            }).catch(() => {
                this.projectionoptions = []
                this.proloading = false
            })
        },
        // 导出文件
        out_file() {
            ElMessageBox.confirm('这需要花费一定的时间，确定导出吗？', {
                distinguishCancelAndClose: true,
                confirmButtonText: '确定',
                type: 'warning',
                cancelButtonText: '取消',
            })
                .then(() => {
                    this.$loadingon()
                    OutputFlow(this.selectedtables).then(res => {
                        var content = res;
                        var blob = new Blob([content]);
                        var fileName = "流水.xlsx"; //要保存的文件名称
                        if ("download" in document.createElement("a")) {
                            // 非IE下载
                            var elink = document.createElement("a");
                            elink.download = fileName;
                            elink.style.display = "none";
                            elink.href = URL.createObjectURL(blob);
                            document.body.appendChild(elink);
                            elink.click();
                            URL.revokeObjectURL(elink.href); // 释放URL 对象
                            document.body.removeChild(elink);
                        } else {
                            // IE10+下载
                            navigator.msSaveBlob(blob, fileName);
                        }
                        this.$loadingoff()
                    }).catch(er => {
                        desolvererror(er)
                        this.$loadingoff()
                    })
                })
        },
        search() {
            getflowlist(JSON.stringify(this.projectselected), this.currentPage, this.pageSize, this.keywords != "" ? this.keywords : "#").then(res => {
                if (res.status == true) {
                    this.tableData = res.data.tables.filter(s => s.flow.id != "0").map(s => {
                        var i = {
                            "id": s.flow.id,
                            "projectId": s.flow.projectId,
                            "serialNumber": s.flow.serialNumber,
                            "documentNumber": s.flow.documentNumber,
                            "flowTypeId": s.flow.flowTypeId,
                            "financialStaffId": s.flow.financialStaffId,
                            "financeManager": s.flow.financeManager,
                            "createTime": s.flow.createTime,
                            "inAmount": s.flow.inAmount,
                            "outAmount": s.flow.outAmount,
                            "remark": s.flow.remark,
                            "flowType": s.flowType == null ? '该类别已删除' : s.flowType.name
                        }
                        return i
                    })
                    this.total = Number(res.data.counts) - 1
                    log(this.tableData)
                }
                else {
                    this.tableData = []
                }
            })
        },
        handleSizeChange() {
            this.search()
        },
        handleCurrentChange() {
            this.search()
        },
        updateflowfun(e) {
            this.newliushui = JSON.parse(JSON.stringify(e))
            this.addliushui_vi = true
        },
        deleteflowfun(e) {
            ElMessageBox.confirm('确定删除吗？', {
                distinguishCancelAndClose: true,
                confirmButtonText: '确定',
                type: 'warning',
                cancelButtonText: '取消',
            })
                .then(() => {
                    this.$loadingon()
                    DeleteFlow(e.id).then(res => {
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
        detailflowfun(e) {
            this.flowimages = []
            this.flownotimages = []
            this.file = []
            this.$loadingon()
            GetAFlowAllInfo(e.id).then(res => {
                if (res.status == true) {
                    this.detailprojectmodel = res.data.projects
                    this.detailliushui_vi = true
                    // this.detailflowmodel = JSON.parse(JSON.stringify(e))
                    this.detailcommodel = res.data.company
                    this.detailyewumodel = res.data.businessDepartment
                    this.detailprojectmodel = res.data.project
                    this.userdetail = res.data.flow.user

                    this.detailflowmodel = {
                        "id": res.data.flow.flow.id,
                        "projectId": res.data.flow.flow.projectId,
                        "serialNumber": res.data.flow.flow.serialNumber,
                        "documentNumber": res.data.flow.flow.documentNumber,
                        "flowTypeId": res.data.flow.flow.flowTypeId,
                        "financialStaffId": res.data.flow.flow.financialStaffId,
                        "financeManager": res.data.flow.flow.financeManager,
                        "inAmount": res.data.flow.flow.inAmount,
                        "outAmount": res.data.flow.flow.outAmount,
                        "createTime": res.data.flow.flow.createTime,
                        "remark": res.data.flow.flow.remark,
                        "flowType": res.data.flow.flowType == null ? '此类型已删除' : res.data.flow.flowType.name,
                        "flowFiles": res.data.flow.flowFiles.map(s => {
                            if (isImage(s.fileType)) {
                                // const blob = b64toBlob(s.base64,s.fileType);
                                // const urlObj = URL.createObjectURL(blob);
                                var j = {
                                    "base64": s.base64,
                                    "fileName": s.fileName + '.' + getFileExtensionFromMimeType(s.fileType),
                                    "fileType": s.fileType,
                                    "isimage": true
                                }
                                this.flowimages.push(j)
                                this.file.push(s.base64)
                                return j
                            }
                            else {
                                var jj = {
                                    "base64": s.base64,
                                    "fileName": s.fileName + '.' + getFileExtensionFromMimeType(s.fileType),
                                    "fileType": s.fileType,
                                    "isimage": false
                                }
                                this.flownotimages.push(jj)
                                return jj
                            }

                        })
                    }
                    log(this.file)
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
        flowuploadsu(uploadFile, uploadFiles) {
            this.files = uploadFiles
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
        resetflow() {
            this.$refs.addnewLiushuiRef.resetFields()
        },
        updateflow() {
            ElMessageBox.confirm('确定修改吗？此操作将删除之前提交的历史文件，请务必在此时提交完整文件材料！', {
                distinguishCancelAndClose: true,
                confirmButtonText: '确定',
                type: 'warning',
                cancelButtonText: '取消',
            })
                .then(() => {

                    this.$refs.addnewLiushuiRef.validate((valid) => {
                        if (valid) {
                            this.$loadingon()
                            if (this.zhangmu_type == true) {
                                this.newliushui.outAmount = this.newliushui.Amount
                                this.newliushui.inAmount = 0
                            }
                            else {
                                this.newliushui.outAmount = 0
                                this.newliushui.inAmount = this.newliushui.Amount
                            }
                            UpdateFlow(this.newliushui, this.files).then(res => {
                                if (res.status == true) {
                                    ElMessage({
                                        message: res.message,
                                        type: 'success',
                                        duration: 1500
                                    })
                                    this.addliushui_vi = false
                                    this.resetflow()
                                    // this.tableData = []
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
                })
        }
    },
    mounted() {
        this.search()
    }
}
</script>
 
 <!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
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

.demo-image__error .block {
    padding: 30px 0;
    text-align: center;
    border-right: solid 1px var(--el-border-color);
    display: inline-block;
    width: 49%;
    box-sizing: border-box;
    vertical-align: top;
}

.demo-image__error .demonstration {
    display: block;
    color: var(--el-text-color-secondary);
    font-size: 14px;
    margin-bottom: 20px;
}

.demo-image__error .el-image {
    padding: 0 5px;
    max-width: 300px;
    max-height: 200px;
    width: 100%;
    height: 200px;
}

.demo-image__error .image-slot {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 100%;
    height: 100%;
    background: var(--el-fill-color-light);
    color: var(--el-text-color-secondary);
    font-size: 30px;
}

.demo-image__error .image-slot .el-icon {
    font-size: 30px;
}

.button .svg {
    width: 30px;
    stroke: #fff;
}

.demo-image__error .image-slot {
    font-size: 30px;
}

.demo-image__error .image-slot .el-icon {
    font-size: 30px;
}

.demo-image__error .el-image {
    width: 100%;
    height: 200px;
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
}</style>
 