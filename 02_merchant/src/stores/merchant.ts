import { defineStore } from 'pinia'
import { ref } from 'vue'
import { loginByPassword } from '@/api'
export const useMerchantStore = defineStore('merchant', () => {
  const token = ref(localStorage.getItem('m_token') || '')
  const info = ref<any>(null)
  const login = async (phone: string, pw: string) => {
    const res = await loginByPassword({ phone, password: pw })
    token.value = res.data.token
    info.value = res.data.userInfo
    localStorage.setItem('m_token', res.data.token)
  }
  const logout = () => { token.value = ''; info.value = null; localStorage.removeItem('m_token') }
  return { token, info, login, logout }
})