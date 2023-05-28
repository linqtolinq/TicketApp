import Request from "./request";
let request = Request.instance;
export function userlogin(UserName, PassWord) {
    return request({
        url: "/UserEntity/Login",
        method: "post",
        data: {
            UserName: UserName,
            PassWord: PassWord
        }
    })
}
export function useradd(data) {
    return request({
        url: "/UserEntity/AddUser",
        method: "post",
        data: data
    })
}
export function GetOwnInfo() {
    return request({
        url: "/UserEntity/GetOwnInfo",
        method: "get",
    })
}


export function deleteuser(id) {
    return request({
        url: "/UserEntity/AdminSignOutOwnAccount",
        method: "delete",
        params: {
            id: id
        }
    })
}
///注销
export function signoutuser() {
    return request({
        url: "/UserEntity/SignOutOwnAccount",
        method: "delete",
    })
}

export function GetUserLists(pagenumber, pagesize, keywords) {
    return request({
        url: "/UserEntity/GetUserLists",
        method: "get",
        params: {
            pagenumber: pagenumber,
            pagesize: pagesize,
            keywords: keywords
        }
    })
}

export function userupdate(Name, Remark, Nick, Email, Address) {
    return request({
        url: "/UserEntity/UpdateOwnAccountInfo",
        method: "post",
        data: {
            Name: Name,
            Remark: Remark,
            Nick: Nick,
            Email: Email,
            Address: Address,
        }
    })
}
export function userupdatepwd(data) {
    return request({
        url: "/UserEntity/UpdateOwnAccountPwd",
        method: "post",
        data: data
    })
}

export function addcompany(data) {
    return request({
        url: "/Company/AddCompany",
        method: "post",
        data: data
    })
}

export function aupdatecompany(data) {
    return request({
        url: "/Company/UpdateCompany",
        method: "post",
        data: data
    })
}

export function getcompanys(pagesize, pagenumber, keywords, type) {
    switch (type) {
        case "1":
            return request({
                url: "/Company/GetCompanyListByName",
                method: "get",
                params: {
                    pagenumber: pagenumber,
                    pagesize: pagesize,
                    keywords: keywords
                }
            })
        case "2":
            return request({
                url: "/Company/GetCompanyListByManagerName",
                method: "get",
                params: {
                    pagenumber: pagenumber,
                    pagesize: pagesize,
                    keywords: keywords
                }
            })
        default:
            break
    }

}

export function GetOneCompanyAllInfo(id) {
    return request({
        url: "/Company/GetOneCompanyAllInfo",
        method: "get",
        params: {
            companyid: id
        }
    })
}
export function GetCompanyBusinessDepartmentInfo(id, keywords) {
    return request({
        url: "/BusinessDepartment/GetCompanyBusinessDepartmentInfo",
        method: "get",
        params: {
            companyid: id,
            keywords: keywords
        }
    })
}
export function GetOneCompanyInfo(id) {
    return request({
        url: "/Company/GetOneCompanyInfo",
        method: "get",
        params: {
            companyid: id
        }
    })
}

export function DeleteOneCompany(id) {
    return request({
        url: "/Company/DeleteCompany",
        method: "delete",
        params: {
            companyid: id
        }
    })
}

export function addyewu(data) {
    return request({
        url: "/BusinessDepartment/AddBusinessDepartment",
        method: "post",
        data: data
    })
}
export function updateyewu(data) {
    return request({
        url: "/BusinessDepartment/UpdateBusinessDepartment",
        method: "post",
        data: data
    })
}
export function deleteyewu(id) {
    return request({
        url: "/BusinessDepartment/DeleteBusinessDepartment",
        method: "delete",
        params:
        {
            id: id
        }
    })
}

export function getyewulist(comids, pagenumber, pagesize, keywords) {
    return request({
        url: "/BusinessDepartment/GetBusinessDepartmentList",
        method: "get",
        params: {
            comids: comids,
            pagenumber: pagenumber,
            pagesize: pagesize,
            keywords: keywords,

        }
    })
}

export function GetOneBusinessDepartmentAllInfo(id) {
    return request({
        url: "/BusinessDepartment/GetOneBusinessDepartmentAllInfo",
        method: "get",
        params: {
            id: id
        }
    })
}
export function getflowtypes() {
    return request({
        url: "/Flow/GetFlowTypes",
        method: "get"
    })
}
export function addflowtypes(data) {
    return request({
        url: "/Flow/AddFlowtype",
        method: "post",
        data: data
    })
}
export function updateflowtypes(data) {
    return request({
        url: "/Flow/UpdateFlowtype",
        method: "post",
        data: data
    })
}

export function deleteflowtypes(id) {
    return request({
        url: "/Flow/DeleteFlowType",
        method: "delete",
        params: {
            id: id
        }
    })
}
export function addnewproject(data) {
    return request({
        url: "/Project/AddProject",
        method: "post",
        data: data
    })
}
export function jiansuo_yewu_project(id, keywords) {
    return request({
        url: "/Project/GetOneyewuAllInfo",
        method: "get",
        params: {
            id: id,
            keywords: keywords
        }
    })
}


export function GetProjectList(pagenumber, pagesize, keywords, businessids) {
    return request({
        url: "/Project/GetProjectList",
        method: "get",
        params: {
            pagenumber: pagenumber,
            pagesize: pagesize,
            keywords: keywords,
            businessids: businessids
        }
    })
}


export function updatepoject(data) {
    return request({
        url: "/Project/UpdateProject",
        method: "post",
        data: data
    })
}
export function GetOneProjectAllInfo(id,word) {

    return request({
        url: "/Project/GetOneProjectAllInfo",
        method: "get",
        params: {
            id: id,
            keywords:word
        }
    })
}
// 检索一个项目的流水
export function GetAprojectFlow(projectid,keywords)
{
    return request({
        url: "/Flow/GetAprojectFlow",
        method: "get",
        params: {
            projectid: projectid,
            keywords: keywords
        }
    })
}
export function GetFlowTypesSuggestion(counts,keywords)
{
    return request({
        url: "/Flow/GetFlowTypesSuggestion",
        method: "get",
        params: {
            counts: counts,
            keywords: keywords
        }
    })
}
import fileToBase64 from '../extension/static.js'
// 添加流水
export async function addflowapi(flow,files)
{
    // var formData = new FormData();
    // files.forEach(e => {
    //     formData.append('file', e.raw)
    // });
    // console.log(files)
    // formData.append('ProjectId',flow.projectId)
    // formData.append('SerialNumber',flow.serialNumber)
    // formData.append('DocumentNumber',flow.documentNumber)
    // formData.append('FlowTypeId',flow.flowTypeId)
    // formData.append('FinanceManager',flow.financeManager)
    // formData.append('Remark',flow.remark)
    var po =(await fileToBase64(files.map(s=>s.raw))).map(s=>{
        var i = {
            FileName :s.name,
            FileType:s.type,
            FileStream:s.base64,
        }
        return i
    })

   return Promise.all(po)
    .then((results) => {
        flow.files = results
        return request({
            url: '/flow/addflow',
            method: 'post',
            data: flow,
        })
    })
    .catch((error) => {
      console.error(error);
    });
}

export async function UpdateFlow(flow,files)
{
    // var formData = new FormData();
    // files.forEach(e => {
    //     formData.append('file', e.raw)
    // });
    
    // formData.append('ProjectId',flow.projectId)
    // formData.append('Id',flow.id)
    // formData.append('SerialNumber',flow.serialNumber)
    // formData.append('DocumentNumber',flow.documentNumber)
    // formData.append('FlowTypeId',flow.flowTypeId)
    // formData.append('FinanceManager',flow.financeManager)
    // formData.append('Remark',flow.remark)

    // return request({
    //     url: '/flow/updateflow',
    //     method: 'post',
    //     data: formData,
    //     headers: {
    //       'Content-Type': 'multipart/form-data',
    //     }
    // })

    var po =(await fileToBase64(files.map(s=>s.raw))).map(s=>{
        var i = {
            FileName :s.name,
            FileType:s.type,
            FileStream:s.base64,
        }
        return i
    })

   return Promise.all(po)
    .then((results) => {
        flow.files = results
        return request({
            url: '/flow/updateflow',
            method: 'post',
            data: flow,
        })
    })
    .catch((error) => {
      console.error(error);
    });

}

export function deletepro(id) {
    return request({
        url: "/Project/DeleteProject",
        method: "delete",
        params: {
            id: id
        }
    })
}



export function getprojectsuggestionlist(buids, pagenumber, pagesize, keywords) {
    return request({
        url: "/Project/getprojectsuggestionlist",
        method: "get",
        params: {
            buids: buids,
            pagenumber: pagenumber,
            pagesize: pagesize,
            keywords: keywords,

        }
    })
}
//根据流水编号检索
export function getflowlist(projectids, pagenumber, pagesize, keywords) {
    return request({
        url: "/Flow/GetFlowList",
        method: "get",
        params: {
            projectids: projectids,
            pagenumber: pagenumber,
            pagesize: pagesize,
            keywords: keywords,

        }
    })
}

export function DeleteFlow(id)
{
    return request({
        url: "/Flow/DeleteFlow",
        method: "delete",
        params: {
            id: id
        }
    })
}

export function DeleteSupportMaterial(ids)
{
    return request({
        url: "/Flow/DeleteSupportMaterial",
        method: "delete",
        params: {
            id: ids
        }
    })
}

export function GetAFlowAllInfo(id)
{
    return request({
        url: "/Flow/GetAFlowAllInfo",
        method: "get",
        params: {
            id: id
        }
    })
}
export function UpdateAuserAllBusinessDepartment(uid,newids)
{
    return request({
        url: "/Relation/UpdateAuserAllBusinessDepartment",
        method: "post",
       params:{
        uid:uid,
        newids:newids
       }
    })
}

export function getAuserAllBusinessDepartment(id)
{
    return request({
        url: "/Relation/GetAuserAllBusinessDepartment",
        method: "get",
       params:{
        id:id,
       }
    })
}

export function getAuserOwnAllBusinessDepartment(keywords,pagenumber,pagesize)
{
    return request({
        url: "/Relation/getAuserOwnAllBusinessDepartment",
        method: "get",
       params:{
        keywords:keywords,
        pagepagenumber:pagenumber,
        pagesize:pagesize
       }
    })
}

export function DeletAuserAllBusinessDepartment(uid,businessids)
{
    return request({
        url: "/Relation/DeletAuserAllBusinessDepartment",
        method: "delete",
        params:{
            uid:uid,
            businessids:businessids
           }
    })
}

export function OutputFlow(data)
{
    return request({
        url: "/Flow/OutputFlow",
        method: "post",
        responseType: "blob",
        data:data
    })
}