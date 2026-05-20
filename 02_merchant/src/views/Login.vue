<template><div class="login-container"><el-card class="login-card"><h2>商家登录</h2><el-form :model="form" :rules="rules"><el-form-item prop="phone"><el-input v-model="form.phone" placeholder="手机号" /></el-form-item><el-form-item prop="password"><el-input v-model="form.password" type="password" placeholder="密码" @keyup.enter="handleLogin" /></el-form-item><el-form-item><el-button type="primary" :loading="loading" style="width:100%" @click="handleLogin">登 录</el-button></el-form-item></el-form><p style="text-align:center;color:#999;font-size:12px">需管理员审核开通商家账号</p></el-card></div></template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useMerchantStore } from '@/stores/merchant'
import { ElMessage } from 'element-plus'
const router = useRouter(); const store = useMerchantStore()
const loading = ref(false)
const form = reactive({ phone:'', password:'' })
const rules = { phone:[{required:true,message:'请输入手机号'}], password:[{required:true,message:'请输入密码'}] }
const handleLogin = async () => { loading.value=true; try{ await store.login(form.phone, form.password); ElMessage.success('登录成功'); router.push('/dashboard') }catch(e){}finally{ loading.value=false } }
</script>

<style scoped lang="scss">
.login-container { display:flex; align-items:center; justify-content:center; height:100vh; background:linear-gradient(135deg,#667eea,#764ba2); }
.login-card { width:400px; padding:20px 30px; border-radius:12px; h2 { text-align:center; margin-bottom:24px; } }
</style>