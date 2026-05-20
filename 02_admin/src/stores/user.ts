import { defineStore } from 'pinia'
import { ref } from 'vue'
import { loginByPassword } from '@/api'

export const useUserStore = defineStore('user', () => {
  const token = ref(localStorage.getItem('token') || '')
  const userInfo = ref<any>(null)

  const setToken = (val: string) => {
    token.value = val
    localStorage.setItem('token', val)
  }

  const logout = () => {
    token.value = ''
    userInfo.value = null
    localStorage.removeItem('token')
  }

  const login = async (phone: string, password: string) => {
    const res = await loginByPassword({ phone, password })
    setToken(res.data.token)
    userInfo.value = res.data.userInfo
    return res
  }

  return { token, userInfo, setToken, logout, login }
})