import { createRouter, createWebHashHistory } from 'vue-router'
import LoginIndex from '../components/LoginIndex.vue'
import NotFound404 from '../components/NotFound.vue'
import HomeIndex from '../components/HomeIndex.vue'
import CompanyTab from '../components/CompanyTab.vue'
import BusinessTab from '../components/BusinessTab.vue'
import PeopleTab from '../components/PeopleTab.vue'
import FluvialCategory from '../components/FluvialCategory.vue'
import ProjectTab from '../components/ProjectTab.vue'
import MyBusiness from '../components/MyBusiness.vue'
import MyFlow from '../components/MyFlow.vue'
import MyProject from '../components/MyProject.vue'
import SystemMessage from '../components/SystemMessage.vue'
import EmptyPage from '../components/EmptyPage.vue'

import FlowList from '../components/FlowList.vue'
const routes =
    [
        {
            path: "/",
            name:"default",
            redirect: '/Login',//页面默认加载的路由
        },
        {
            path: "/Login",
            name:"Login",
            component: LoginIndex,//
        },
        {
            path: "/Init",
            name:"EmptyPage",
            component: EmptyPage,//
        },
        {
            path: "/404",
            name:"404",
            component: NotFound404,//
        },
        {
            // path: '*',
            redirect:"/404",
            path: "/:pathMatch(.*)",
        },
        {
            path: "/Home",
            name:"Home",
            component:HomeIndex, 
            children: [
                        {
                            path: "/Company",
                            name:"Company",
                            component: CompanyTab,//页面默认加载的路由
                        },
                        {
                            path:"/Business",
                            name:"Business",
                            component: BusinessTab,
                        },
                        {
                            path:"/FinancialStaff",
                            name:"FinancialStaff",
                            component: PeopleTab,
                        },
                        {
                            path:"/FluvialCategory",
                            name:"FluvialCategory",
                            component: FluvialCategory,
                        },
                        {
                            path:"/Project",
                            name:"Project",
                            component:ProjectTab
                        },
                        {
                            path:"/MyBusiness",
                            name:"MyBusiness",
                            component:MyBusiness
                        },
                        {
                            path:"/Flow",
                            name:"Flow",
                            component:FlowList
                        }, 
                        {
                            path:"/MyFlow",
                            name:"MyFlow",
                            component:MyFlow
                        },
                        {
                            path:"/MyProject",
                            name:"MyProject",
                            component:MyProject
                        },
                        {
                            path:"/SystemMessage",
                            name:"SystemMessage",
                            component:SystemMessage
                        },
                        
                        
                    ]
        },
        // {
        //     path: "/LoginPage",
        //     name: "LoginPage",
        //     component: LoginPage
        // },
        // {
        //     path: "/StartPage",
        //     name: "StartPage",
        //     component: StartPage//MobelPhone
        // },
        // {
        //     path: "/MobelLogin",
        //     name: "MobelLogin",
        //     component: MobelLogin//MobelPhone
        // },
        // {
        //     path: "/MobelPhone",
        //     name: "MobelPhone",
        //     component: MobelPhone,
        //     children: [
        //         {
        //             path: "/MobelPhone",
        //             name:"start",
        //             redirect: '/MobelPhone/qrcode',//页面默认加载的路由
        //         },
        //         {
        //             path: '/MobelPhone/qrcode',
        //             name: 'qrcode',
        //             component: QRcode 
        //         },
        //         {
        //             path: '/MobelPhone/SchoolForum',//UserCenter
        //             name: 'schoolforum',
        //             component: SchoolForum
        //         },
        //         {
        //             path: '/MobelPhone/UserCenter',//UserCenter
        //             name: 'UserCenter',
        //             component: UserCenter
        //         }
        //     ]
        
        // }
    ]

const router = createRouter({
    history: createWebHashHistory(),
    routes: routes
})

export default router