<template>
    <div>
        <p class="login-title title-gogo">发票管理系统</p>
    </div>
    <div id="login-box">
        <div class="login-image">
            <img src="@/assets/static/loginalign.png" alt="" srcset="">
        </div>
        <div class="login-inputs">
            <div class="login-input">
                <div style="padding-top: 35px;font-size: 20px;font-weight: 600;color:#1068be ;">登录</div>
                <div class="input-">
                    <input type="text" id="input-uid" class="login-item" @keyup.enter="login"  v-model="uid" @change="uidchange"
                        placeholder="请输入用户账号">
                    <label for="input-uid" class="login-item-label">账号：</label>
                    <label for="input-uid" id="uid-error" class="login-item-label-error">✘</label>
                    <label for="input-uid" id="uid-success" class="login-item-label-success">✔</label>
                    <label for="input-uid" id="uid-null" class="login-item-label-error">必填</label>
                </div>
                <div class="input-">
                    <input type="password" id="input-pwd" class="login-item" v-model="pwd" @change="pwdChange"
                        placeholder="请输入用户密码" @keyup.enter="login">
                    <label for="input-pwd" class="login-item-label">密码：</label>
                    <label for="input-uid" id="pwd-error" class="login-item-label-error">✘</label>
                    <label for="input-uid" id="pwd-null" class="login-item-label-error">必填</label>
                    <label for="input-uid" id="pwd-success" class="login-item-label-success">✔</label>
                </div>
                <button type="submit" @click="login" class="login-btn">登录</button>
                <!-- <el-button type="primary">Primary</el-button> -->
            </div>
        </div>
    </div>
</template>
  
<script>
import { ElMessage } from 'element-plus'
import { userlogin } from '../request/api.js'
import { log } from '../extension/log.js'
import { desolveresponse } from '../extension/response.js'
import {Local} from "../jwt/jwt.js";
export default {
    name: 'LoginIndex',
    data() {
        return {
            uid: "",
            pwd: "",
            uid_isvalid: false,
            pwd_isvalid: false
        }
    },
    mounted() {

    },
    methods: {
        login() {
            if (!this.uid_isvalid || !this.pwd_isvalid) {
                ElMessage({
                    message: '填写不完整或格式错误',
                    type: 'warning',
                    duration: 1500
                })
            }
            else {
                userlogin(this.uid, this.pwd).then(re => {
                    log(re)
                    if (re.status == false) {
                        desolveresponse(re)
                    }
                    else {
                        ElMessage({
                            message: re.message,
                            type: 'success',
                            duration: 1500
                        })
                        Local.set("token",re.data)
                        this.$router.push("/Init")
                    }
                })
            }
        },
        CheckUidInputIsValid(type) {
            var input = document.getElementById("input-uid").value;
            var input_valid = false;
            switch (type) {
                case "email":
                    var pattern = /^([a-zA-Z\d][\w-]{2,})@(\w{2,})\.([a-z]{2,})(\.[a-z]{2,})?$/;
                    if (pattern.test(input)) {
                        input_valid = true;
                    }
                    else {
                        input_valid = false;
                    }
                    break;
                case "phonenumber":
                    pattern = /^[1][0-9][0-9]{9}$/;
                    if (pattern.test(input)) {
                        input_valid = true;
                    }
                    else {
                        input_valid = false;
                    }
                    break;
                case "phone":
                    pattern = /^[1][0-9][0-9]{9}$/;
                    if (pattern.test(input)) {
                        input_valid = true;
                    }
                    else {
                        input_valid = false;
                    }
                    break
                default:
                    input_valid = false;
                    break;
            }
            this.uid_isvalid = input_valid
            if (input == null || input == "") {
                var testNode = document.getElementById("uid-error");
                if (testNode) {
                    testNode.setAttribute("style", "opacity:0;")
                }
                testNode = document.getElementById("uid-success");
                if (testNode) {
                    testNode.setAttribute("style", "opacity:0;")
                }
                testNode = document.getElementById("uid-null");
                if (testNode) {
                    testNode.setAttribute("style", "opacity:1;")
                }
                return
            }
            if (input_valid == false) {
                testNode = document.getElementById("uid-error");
                if (testNode) {
                    testNode.setAttribute("style", "opacity:1;")
                }
                testNode = document.getElementById("uid-success");
                if (testNode) {
                    testNode.setAttribute("style", "opacity:0;")
                }
                testNode = document.getElementById("uid-null");
                if (testNode) {
                    testNode.setAttribute("style", "opacity:0;")
                }
            }
            else {
                testNode = document.getElementById("uid-error");
                if (testNode) {
                    testNode.setAttribute("style", "opacity:0;")
                }
                testNode = document.getElementById("uid-success");
                if (testNode) {
                    testNode.setAttribute("style", "opacity:1;")
                }
                testNode = document.getElementById("uid-null");
                if (testNode) {
                    testNode.setAttribute("style", "opacity:0;")
                }
            }
        },
        CheckPwdInputIsValid() {

            var input = document.getElementById("input-pwd").value;
            if (input == null || input == "") {
                var testNode = document.getElementById("pwd-error");
                if (testNode) {
                    testNode.setAttribute("style", "opacity:0;")
                }
                testNode = document.getElementById("pwd-success");
                if (testNode) {
                    testNode.setAttribute("style", "opacity:0;")
                }
                testNode = document.getElementById("pwd-null");
                if (testNode) {
                    testNode.setAttribute("style", "opacity:1;")
                }
                this.pwd_isvalid = false;
                return
            }
            else {
                this.pwd_isvalid = true;
                testNode = document.getElementById("pwd-error");
                if (testNode) {
                    testNode.setAttribute("style", "opacity:0;")
                }
                testNode = document.getElementById("pwd-success");
                if (testNode) {
                    testNode.setAttribute("style", "opacity:1;")
                }
                testNode = document.getElementById("pwd-null");
                if (testNode) {
                    testNode.setAttribute("style", "opacity:0;")
                }

            }

        },
        uidchange() {
            this.CheckUidInputIsValid('phonenumber')
        },
        pwdChange() {
            this.CheckPwdInputIsValid()
        }
    }
}
</script>
  <!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
#login-box {
    /* min-width: 800px; */
    top: 100px;
    width: 50%;
    /* max-width: 900px; */
    background-color: rgba(255, 255, 255, 0.5);
    /* opacity: 0.7; */
    /* min-height: 500px; */
    height: 55vh;
    /* max-height: 500px; */
    align-self: center;
}
.login-title {
    font-size: 50px;
    color: white;
    font-family: 'Helvetica';
    text-shadow: 2px 2px 15px white;
    margin-top: -15vh;
    background-color: transparent;
}
.login-image {
    width: 40%;
    float: left;
    /* background-color: red; */
    height: 100%;
    overflow: none;
    /* min-width: 300px; */
}
@media screen and (max-width:1100px) {
    .login-image {
        width: 20%;
    }

    .login-image>img {
        display: none;
    }

    .login-inputs {
        width: 100%;
    }
}
.login-image>img {
    /* min-width: 300px; */
    margin-top: 30%;
    width: 100%;
    height: 60%;
    opacity: 1.5;
}
.login-inputs {
    /* background-color: red; */
    width: 60%;
    height: 100%;
    float: left;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;
}
.login-input {
    width: 65%;
    height: 80%;
    background-color: var(--backgroundcolor);
    border-radius: 5px;
    padding: 10px;
    box-shadow: 2px 2px 10px #bab8b8;
    display: flex;
    justify-content: start;
    align-items: center;
    flex-direction: column;
    position: relative;
    /* box-shadow: -5px -5px 5px #bab8b8; */
}
.input- {
    width: 100%;
    height: 100px;
    position: relative;
}
.login-item {
    display: block;
    margin: 0;
    width: 80%;
    margin-left: 8%;
    margin-top: 40px;
    height: 40px;
    padding: 0 10px 0 10px;
    /* color: #0f5ca8; */
    font-size: 15px;
    height: 40px;
    font-weight: 500;
    /* border-radius: 5px; */
    border-width: 0;
    border-bottom-width: 1px;
    outline: none;
}
.login-item-label {
    display: block;
    position: absolute;
    opacity: 0;
    margin: -30px 0 0 40px;
    font-size: 15px;
    color: #0f67bc;
}
.login-item-label-error {
    position: absolute;
    margin: -30px 0 0 35%;
    font-size: 9px;
    color: red;
    opacity: 0;
}
.login-item-label-success {
    position: absolute;
    margin: -30px 0 0 35%;
    font-size: 9px;
    color: green;
    opacity: 0;
}
.login-item:focus {
    animation-name: logininputborder;
    animation-duration: 0.5s;
    border-bottom-width: 2px;
    border-color: #0f67bc;
}
.login-item:placeholder-shown+.login-item-label {
    visibility: hidden;
    z-index: -1;
}
.login-item:not(:placeholder-shown)+.login-item-label,
.login-item:focus:not(:placeholder-shown)+.login-item-label {
    visibility: visible;
    z-index: 1;
    opacity: 1;
    animation-name: loginlabel;
    animation-duration: 0.4s;
    margin: -55px 0 0 40px;
}
.login-item:placeholder-shown+.login-item-label {
    visibility: visible;
    /* z-index: -1; */
    animation-name: loginlabel_back;
    animation-duration: 3s;
    margin: -30px 0 0 20px;
}
@keyframes loginlabel_back {
    from {
        margin-top: -55px;
        opacity: 1;
    }
    to {
        margin-top: -30px;
        opacity: 0;
    }
}
@keyframes loginlabel {
    from {
        margin-top: -30px;
    }
    to {
        margin-top: -55px;
    }
}
@keyframes logininputborder {
    from {
        border-color: #efefef;
    }

    to {
        border-color: #0f67bc;
        border-bottom-width: 2px;
    }
}
.login-btn {
    border: 0;
    font-size: 18px;
    background-color: #0c64ba;
    width: 100px;
    height: 40px;
    border-radius: 5px;
    color: white;
    margin-top: 40px;
}

.login-btn:hover {
    cursor: pointer !important;
    ;
}
.login-btn:focus {
    /* background-color: #0c64ba; */
    border: 1px solid #99d3f8;
    box-shadow: 0px 0px 15px 0px #007acc;
}
.title-gogo {
    /* color: #cca700; */
    font-size: 3em;
    width: 6.1em;
    height: 1.2em;
    padding-right: 0rem;
    border-right: 1px solid transparent;
    /* animation: typing 2s steps(42, end), blink-caret .75s step-end infinite; */
    font-family: Consolas, Monaco;
    word-break: break-all;
    overflow: hidden;
}
/* 打印效果 */
@keyframes typing {

    from {
        width: 0.5em;
    }

    to {
        width: 6.1em;
    }
}
/* 光标 */
@keyframes blink-caret {

    from,
    to {
        border-color: transparent;
    }

    50% {
        border-color: currentColor;
    }
}
</style>
  