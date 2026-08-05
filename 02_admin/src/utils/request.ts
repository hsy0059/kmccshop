import axios from 'axios'
import { ElMessage } from 'element-plus'
import router from '@/router'

const request = axios.create({
  baseURL: '/api',
  timeout: 15000
})

request.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

request.interceptors.response.use(
  response => {
    const res = response.data
    if (res.code !== 0) {
      ElMessage.error(res.message || '请求失败')
      return Promise.reject(new Error(res.message))
    }
    return res
  },
  error => {
    const status = error.response?.status
    if (status === 401) {
      // 登录已过期，清除 token 并跳转登录页
      localStorage.removeItem('token')
      ElMessage.error('登录已过期，请重新登录')
      if (router.currentRoute.value.path !== '/login') {
        router.replace('/login')
      }
    } else if (status >= 500) {
      ElMessage.error(`服务器错误 ${status}`)
    } else if (status) {
      ElMessage.error(error.response?.data?.message || `请求失败 ${status}`)
    } else {
      ElMessage.error('网络异常，请检查网络连接')
    }
    return Promise.reject(error)
  }
)

export default request