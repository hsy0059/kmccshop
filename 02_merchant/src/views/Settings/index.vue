<template>
  <el-card>
    <template #header><span>店铺设置</span></template>
    <el-form :model="form" label-width="120px" style="max-width:600px" v-loading="loading">
      <el-form-item label="店铺名称">
        <el-input v-model="form.name" placeholder="请输入店铺名称" />
      </el-form-item>
      <el-form-item label="店铺电话">
        <el-input v-model="form.phone" placeholder="请输入电话" />
      </el-form-item>
      <el-form-item label="店铺地址">
        <el-input v-model="form.address" placeholder="请输入地址" />
      </el-form-item>
      <el-form-item label="营业时间">
        <el-input v-model="form.businessHours" placeholder="如: 08:00-22:00" />
      </el-form-item>
      <el-form-item label="起送金额">
        <el-input-number v-model="form.minDeliveryAmount" :min="0" :precision="2" /> 元
      </el-form-item>
      <el-form-item label="配送费">
        <el-input-number v-model="form.deliveryFee" :min="0" :precision="2" /> 元
      </el-form-item>
      <el-form-item label="店铺描述">
        <el-input v-model="form.description" type="textarea" rows="3" placeholder="请输入店铺描述" />
      </el-form-item>
      <el-form-item label="店铺Logo">
        <el-input v-model="form.logo" placeholder="请输入Logo图片URL" />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="handleSave" :loading="saving">保存设置</el-button>
      </el-form-item>
    </el-form>
  </el-card>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { getMerchantDashboard, getMerchantInfo, updateMerchantInfo } from '@/api'
import { ElMessage } from 'element-plus'

const loading = ref(false)
const saving = ref(false)
const merchantId = ref(0)

const form = reactive<any>({
  name: '', phone: '', address: '', businessHours: '',
  minDeliveryAmount: 0, deliveryFee: 0, description: '', logo: ''
})

const fetchSettings = async () => {
  loading.value = true
  try {
    const dashRes = await getMerchantDashboard()
    const merchant = dashRes.data?.merchant || dashRes.data || {}
    merchantId.value = merchant.id || dashRes.data?.merchantId || 0
    Object.assign(form, {
      name: merchant.name || '',
      phone: merchant.phone || '',
      address: merchant.address || '',
      businessHours: merchant.businessHours || '',
      minDeliveryAmount: merchant.minDeliveryAmount ?? 0,
      deliveryFee: merchant.deliveryFee ?? 0,
      description: merchant.description || '',
      logo: merchant.logo || ''
    })
  } catch (e) {
    try {
      const res = await getMerchantInfo()
      merchantId.value = res.data?.merchantId || 0
    } catch (e2) { /* ignore */ }
  }
  loading.value = false
}
fetchSettings()

const handleSave = async () => {
  if (!merchantId.value) { ElMessage.error('无法获取商家信息'); return }
  saving.value = true
  try {
    await updateMerchantInfo(merchantId.value, form)
    ElMessage.success('保存成功')
  } catch (e) { }
  saving.value = false
}
</script>