<template>
    <div style="margin-left: 0%;width: 100%;">
        <div style="justify-items: left;display: flex;">
            <el-button @click="onAddItem" type="primary">添加流水类别</el-button>
        </div>
        <el-table :data="tableData" style="width: 100%">
            <el-table-column fixde prop="id" label="序号" />
            <el-table-column prop="name" label="流水类别名称" />
            <el-table-column prop="remark" label="描述" />
            <el-table-column fixed="right" label="操作" width="150">
                <template #default="scope">
                    <el-button link type="primary" size="small" @click="updatecomfun(scope.row)">修改</el-button>
                    <el-button link type="danger" size="small" @click="deletecomfun(scope.row)">删除</el-button>
                </template>
            </el-table-column>
        </el-table>
        <el-drawer v-model="addcom" title="添加流水类别">
            <!-- :before-close="handleClose" -->
            <el-form ref="userupdateRef" :model="newfluvial" status-icon :rules="rules" label-width="120px"
                class="demo-ruleForm">
                <el-form-item label="流水类别名称" prop="name">
                    <el-input v-model="newfluvial.name" clearable autocomplete="off" />
                </el-form-item>
                <el-form-item label="描述" prop="remark">
                    <el-input clearable v-model="newfluvial.remark" :rows="2" type="textarea" />
                </el-form-item>

                <el-form-item>
                    <el-button type="primary" @click="updatepost()">添加</el-button>
                    <el-button @click="resetForm()">重置</el-button>
                </el-form-item>

            </el-form>
        </el-drawer>
        <el-drawer v-model="updatecom" title="修改流水类别">
            <!-- :before-close="handleClose" -->
            <el-form ref="comupdateRef" :model="updatefluvial" status-icon :rules="rules" label-width="120px"
                class="demo-ruleForm">
                <el-form-item label="序号" prop="id">
                    <label style="margin-left: 5%;">{{ updatefluvial.id }}</label>
                </el-form-item>
                <el-form-item label="流水类别名称" prop="name">
                    <el-input v-model="updatefluvial.name" clearable autocomplete="off" />
                </el-form-item>
                <el-form-item label="描述" prop="remark">
                    <el-input clearable v-model="updatefluvial.remark" :rows="2" type="textarea" />
                </el-form-item>

                <el-form-item>
                    <el-button type="primary" @click="submitupdateForm()">修改</el-button>
                    <el-button @click="resetupdateForm()">重置</el-button>
                </el-form-item>
            </el-form>
        </el-drawer>
    </div>
</template>
          
<script>
import { log } from '../extension/log.js'
import { getflowtypes, addflowtypes, updateflowtypes, deleteflowtypes } from '../request/api.js'
import { ElMessage, ElMessageBox } from 'element-plus'
import { desolveresponse, desolvererror } from '../extension/response.js'
export default {
    name: "FluvialCategory",
    mounted() {
        this.init()
    },
    data() {
        return {
            tableData: [
                // {
                //     id: 1,
                //     name: "第一个测试流水",
                //     remark: "这是第一个测试流水"
                // }

            ],
            rules: {
                name: [{ required: true, message: "该字段必填", trigger: ['blur', 'change'] }],
                remark: [{ required: false, trigger: ['blur', 'change'] }],
            },
            addcom: false,
            updatecom: false,
            newfluvial: {
                name: "",
                remark: ""
            },
            updatefluvial: {
                id: "",
                name: "",
                remark: ""
            },
        }
    },
    methods: {
        submitupdateForm() {
            this.$refs.comupdateRef.validate((valid) => {
                if (valid) {
                    this.$loadingon()
                    updateflowtypes(this.updatefluvial).then(re => {
                        if (re.status == true) {
                            ElMessage({
                                message: re.message,
                                type: 'success',
                                duration: 1500
                            })
                            this.search()
                            this.updatecom = false
                        }
                        else {
                            desolveresponse(re)
                        }
                        this.$loadingoff()
                    }).catch(e => {
                        desolvererror(e)
                        this.$loadingoff()
                    })
                }
                else {
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
            this.newfluvial = {
                name: "",
                remark: ""
            }
        },
        resetupdateForm() {
            this.$refs.comupdateRef.resetFields()
        },
        onAddItem() {
            this.addcom = true
        },
        updatecomfun(e) {
            log(e)
            this.updatefluvial = JSON.parse(JSON.stringify(e))
            this.updatecom = true
        },
        updatepost() {
            this.$refs.userupdateRef.validate((valid) => {
                if (valid) {
                    this.$loadingon()
                    addflowtypes(this.newfluvial).then(re => {
                        if (re.status == true) {
                            ElMessage({
                                message: re.message,
                                type: 'success',
                                duration: 1500
                            })
                            this.search()
                            this.addcom = false
                        }
                        else {
                            desolveresponse(re)
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
        search() {
            getflowtypes().then(re => {
                if (re.status == true) {
                    this.tableData = re.data.filter(s => s.id != "0")
                    log(this.tableData)
                }
                else {
                    desolveresponse(re)
                }
            }).catch(e => {
                desolvererror(e)
            })
        },
        deletecomfun(e) {
            ElMessageBox.confirm('确定删除吗？', {
                distinguishCancelAndClose: true,
                confirmButtonText: '确定',
                type: 'warning',
                cancelButtonText: '取消',
            }).catch().then(() => {
                this.$loadingon()
                deleteflowtypes(e.id).then(re => {
                    if (re.status == true) {
                        ElMessage({
                            message: re.message,
                            type: 'success',
                            duration: 1500
                        })
                        this.search()
                        this.$loadingoff()
                    }
                    else {
                        desolveresponse(re)
                        this.$loadingoff()
                    }
                }).catch(e => {
                    desolvererror(e)
                    this.$loadingoff()
                })
            })

        },
        init() {
            this.search()
        }
    }
}
</script>
        
<style scoped></style>