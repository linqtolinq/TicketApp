import axios from "axios";
// import { getCookie,setCookie } from './cookie.js';
import {Local} from "../jwt/jwt.js";
// const baseURL = "http://localhost:6425/api"
const baseURL = "http://27.129.129.247:6025/api"
// const baseURL = "http://localhost:6425/api"

const instance = axios.create({
  baseURL: baseURL, // 服务器地址
  timeout: 1000*60, // 响应时间
});

instance.defaults.withCredentials=true;

// 请求拦截 interceptors拦截  request请求
instance.interceptors.request.use(
  (config) => {
    // 请求后台的token数据
    let token = Local.get("token");
    if (token && token!= undefined) {
      config.headers.Authorization=token
    }
    return config;
  },
  (err) => {
    return Promise.reject(err);
  }
);

instance.interceptors.response.use(
 
  // 请求成功返回数据
  (res) => {
    // console.log(res.headers["jwt_token"])
    if(res.headers["jwt_token"]!= undefined && res.headers["jwt_token"]!="") {
      Local.set("token",res.headers["jwt_token"])//config.headers['Cookies'] 
    }
    return res.data;
  },
  (err) => {
    // console.log(err.headers)
    return Promise.reject(err);
  }
);
const Request={
  baseURL,
  instance
}
export default  Request;

