<template>
  <div class="login-container">
    <ParticleBackground color="#67C23A" :count="2500" />
    <el-card class="login-card">
      <div class="logo-area">
        <div class="logo-ring"></div>
        <h2>商家管理平台</h2>
        <p class="subtitle">Merchant Console</p>
      </div>
      <el-form ref="formRef" :model="form" :rules="rules">
        <el-form-item prop="phone">
          <el-input v-model="form.phone" placeholder="手机号" prefix-icon="Phone" size="large" />
        </el-form-item>
        <el-form-item prop="password">
          <el-input v-model="form.password" type="password" placeholder="密码" prefix-icon="Lock" size="large" @keyup.enter="handleLogin" />
        </el-form-item>
        <el-form-item>
          <el-button type="success" :loading="loading" size="large" style="width:100%" @click="handleLogin">登 录</el-button>
        </el-form-item>
      </el-form>
      <p class="tips">需管理员审核开通商家账号</p>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useMerchantStore } from '@/stores/merchant'
import { ElMessage } from 'element-plus'
import ParticleBackground from '@/components/ParticleBackground.vue'

const router = useRouter()
const store = useMerchantStore()
const loading = ref(false)
const formRef = ref()
const form = reactive({ phone: '', password: '' })
const rules = {
  phone: [{ required: true, message: '请输入手机号', trigger: 'blur' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }]
}

const handleLogin = async () => {
  try { await formRef.value?.validate() } catch { return }
  loading.value = true
  try {
    await store.login(form.phone, form.password)
    ElMessage.success('登录成功')
    router.push('/dashboard')
  } catch (e) {
  } finally {
    loading.value = false
  }
}
</script>

<style scoped lang="scss">
.login-container {
  position: relative;
  display: flex; align-items: center; justify-content: center; height: 100vh;
  background: linear-gradient(135deg, #0d1f0d 0%, #1a3a1a 50%, #0f3460 100%);
  overflow: hidden;
}
.login-card {
  position: relative; z-index: 1;
  width: 420px; padding: 40px 36px; border-radius: 20px;
  background: rgba(255,255,255,0.08);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(255,255,255,0.15);
  box-shadow: 0 8px 32px rgba(0,0,0,0.3);
  :deep(.el-input__wrapper) {
    background: rgba(255,255,255,0.1);
    border: 1px solid rgba(255,255,255,0.2);
    box-shadow: none;
    .el-input__inner { color: #fff; &::placeholder { color: rgba(255,255,255,0.5); } }
    .el-input__prefix { color: rgba(255,255,255,0.6); }
  }
}
.logo-area { text-align: center; margin-bottom: 32px;
  .logo-ring {
    width: 64px; height: 64px; margin: 0 auto 16px;
    border-radius: 50%;
    background: linear-gradient(135deg, #67C23A, #95d475);
    box-shadow: 0 0 40px rgba(103,194,58,0.6);
    animation: pulse 2s ease-in-out infinite;
  }
  h2 { color: #fff; font-size: 22px; margin-bottom: 4px; }
  .subtitle { color: rgba(255,255,255,0.6); font-size: 13px; }
}
.tips { text-align: center; color: rgba(255,255,255,0.5); font-size: 12px; margin-top: 8px; }
@keyframes pulse {
  0%, 100% { transform: scale(1); box-shadow: 0 0 40px rgba(103,194,58,0.6); }
  50% { transform: scale(1.08); box-shadow: 0 0 60px rgba(103,194,58,0.9); }
}
</style>
