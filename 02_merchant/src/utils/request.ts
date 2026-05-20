import axios from 'axios'
import { ElMessage } from 'element-plus'
const request = axios.create({ baseURL:'/api', timeout:15000 })
request.interceptors.request.use(config => { const t=localStorage.getItem('token'); if(t) config.headers.Authorization=`Bearer ${t}`; return config })
request.interceptors.response.use(r => { if(r.data.code!==0) { ElMessage.error(r.data.message||'失败'); return Promise.reject(new Error(r.data.message)) } return r.data }, () => { ElMessage.error('网络错误'); return Promise.reject() })
export default request