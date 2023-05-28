export function judragnullstring(str)
{
    if (str == null || str.length == 0 || str==undefined)
    {
        return "无内容"
    }
    return str
}

export function isFloat  (val) {
    var re = /^\d+$/.test(val) == false && /^\d+\.\d+$/.test(val) == false
    if (re) {
        return false
    }
    else {
        return true
    }
}
export function isInt ( val) {
    var re = /^\d+$/.test(val) == false 
    if (re) {
        return false
    }
    else {
        return true
    }
}
export function isip (val)  {
    var regip = /^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$/
    var domin = /^(([-\u4E00-\u9FA5a-z0-9]{1,63})\.)+([\u4E00-\u9FA5a-z]{2,63})\.?$/

    if (regip.test(val) || domin.test(val)) {
       return true
    }
    else {
        return false
    }

}
 
export function istelephonenumber (phonenumber) {
    const parmTel =  /^1(0|1|2|3|4|5|6|7|8|9)\d{9}$/
    // 验证手机号
    if(!parmTel.test(phonenumber)){
       return false
    }
    return true
}
export function isImage(type) {
    return type.includes("image");
  }

///判断是否为移动端
export function IsAppUser ()
{
    if(navigator.userAgent.match(/(phone|pad|pod|iPhone|iPod|ios|iPad|Android|Mobile|BlackBerry|IEMobile|MQQBrowser|JUC|Fennec|wOSBrowser|BrowserNG|WebOS|Symbian|Windows Phone)/i))
   {
    return true
   }
   else
   {
    return false
   }
}

export function getFileExtensionFromMimeType(mimeType) {
    const map = {
      "image/jpeg": "jpg",
      "image/png": "png",
      "image/gif": "gif",
      "image/webp": "webp",
      "image/svg+xml": "svg",
      "text/plain": "txt",
      "text/csv": "csv",
      "text/html": "html",
      "text/css": "css",
      "text/javascript": "js",
      "application/json": "json",
      "application/pdf": "pdf",
      "application/msword": "doc",
      "application/vnd.openxmlformats-officedocument.wordprocessingml.document": "docx",
      "application/vnd.ms-excel": "xls",
      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet": "xlsx",
      "application/vnd.ms-powerpoint": "ppt",
      "application/vnd.openxmlformats-officedocument.presentationml.presentation": "pptx",
      // Add more mappings as needed
    };
  //  const  map1 = {
  //     "jpg" : "image/jpeg",
  //     "png" :"image/png",
  //    "gif" :"image/gif",
  //    "webp" :"image/webp",
  //    "svg" :"image/svg+xml",
  //    "txt" : "text/plain",
  //    "csv" : "text/csv",
  //    "html" :"text/html",
  //    "css" : "text/css",
  //    "js" :"text/javascript",
  //    "json" : "application/json",
  //    "pdf" : "application/pdf",
  //    "doc" : "application/msword",
  //    "docx" :"application/vnd.openxmlformats-officedocument.wordprocessingml.document",
  //    "xls":  "application/vnd.ms-excel",
  //    "xlsx" :"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
  //    "ppt" :"application/vnd.ms-powerpoint",
  //    "pptx" :"application/vnd.openxmlformats-officedocument.presentationml.presentation",
  //     // Add more mappings as needed
  //   };
    return map[mimeType] || "";
  }

  export function readFileAsBase64(file) {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => {
        resolve(reader.result.split(',')[1]);
      };
      reader.onerror = reject;
      reader.readAsDataURL(file);
    });
  }


  // 选择文件并将其转换为 base64 格式
const getBase64 = (file) => {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => {
        const base64 = reader.result
        const fileInfo = {
          name: file.name,
          type: file.type,
          size: file.size,
          base64: base64,
        };
        resolve(fileInfo);
      };
      reader.onerror = (error) => reject(error);
    });
  };
  
  // 处理多个文件并返回一个包含所有文件信息的 Promise
  const fileToBase64 = (files) => {
    const promises = Array.from(files).map((file) => getBase64(file));
    return Promise.all(promises);
  };
  
  
  export  default fileToBase64;

export function  b64toBlob(base64Str,contentType) {
    const byteChars = atob(base64Str);
    const byteNumbers = new Array(byteChars.length);
  
    for (let i = 0; i < byteChars.length; i++) {
      byteNumbers[i] = byteChars.charCodeAt(i);
    }
  
    const byteArray = new Uint8Array(byteNumbers);
    return new Blob([byteArray], { type: contentType });
  }
  
  