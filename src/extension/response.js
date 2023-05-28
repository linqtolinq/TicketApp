import { ElMessage } from 'element-plus'
import { log } from './log.js'
import router from '../router/router.js'
export function desolveresponse(res)
{
    if (res.code && res.code == 401 )
    {
        ElMessage({
            message: res.message,
            type: 'error',
            duration:1500
        })
        router.push("/Login")
    }
  else if (res.code && res.code == 200 )
    {
        ElMessage({
            message: res.message,
            type: 'warning',
            duration:1500
        })
    }
}

export function desolvererror(err)
{
    log(err)
    if (err.response && err.response.data!= undefined && err.response.data.Code && err.response.data.Code == 401 )
    {
        ElMessage({
            message: err.response.data.Message,
            type: 'error',
            duration:1500
        })
        router.push("/Login")
    }
  else if (err.response && err.response.data!= undefined && err.response.data.code && err.response.data.code == 200 )
    {
        ElMessage({
            message: err.response.data.Message,
            type: 'warning',
            duration:1500
        })
    }
}