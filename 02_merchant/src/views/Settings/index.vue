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
        <el-upload
          class="logo-uploader"
          :auto-upload="true"
          :show-file-list="false"
          accept="image/jpeg,image/png,image/jpg,image/webp"
          :before-upload="beforeLogoUpload"
          :http-request="uploadLogo"
        >
          <img v-if="form.logo" :src="form.logo" class="preview-logo" />
          <el-icon v-else class="uploader-icon"><Plus /></el-icon>
        </el-upload>
        <div v-if="form.logo" class="upload-tip">已上传Logo，点击可重新上传</div>
        <div v-else class="upload-tip">支持 jpg、png、jpeg、webp，大小不超过 10MB</div>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="handleSave" :loading="saving">保存设置</el-button>
      </el-form-item>
    </el-form>
  </el-card>
</template>

<style scoped lang="scss">
.logo-uploader {
  :deep(.el-upload) {
    border: 1px dashed var(--el-border-color);
    border-radius: 6px;
    cursor: pointer;
    position: relative;
    overflow: hidden;
    transition: var(--el-transition-duration-fast);
    width: 120px;
    height: 120px;
    display: flex;
    justify-content: center;
    align-items: center;
    &:hover { border-color: var(--el-color-primary); }
  }
  .preview-logo { width: 120px; height: 120px; object-fit: cover; }
  .uploader-icon { font-size: 28px; color: #8c939d; width: 120px; height: 120px; text-align: center; line-height: 120px; }
}
.upload-tip { font-size: 12px; color: #909399; margin-top: 6px; }
</style>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { getMerchantDashboard, getMerchantInfo, updateMerchantInfo, uploadFile } from '@/api'
import { ElMessage } from 'element-plus'
import type { UploadProps } from 'element-plus'

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

const beforeLogoUpload: UploadProps['beforeUpload'] = (rawFile) => {
  const allowed = ['image/jpeg', 'image/png', 'image/jpg', 'image/webp']
  if (!allowed.includes(rawFile.type)) {
    ElMessage.error('仅支持 jpg、png、jpeg、webp 格式的图片')
    return false
  }
  if (rawFile.size / 1024 / 1024 > 10) {
    ElMessage.error('图片大小不能超过 10MB')
    return false
  }
  return true
}

const uploadLogo = async (options: any) => {
  try {
    const res = await uploadFile(options.file)
    form.logo = res.data?.url || ''
    ElMessage.success('Logo 上传成功')
  } catch (e) { /* 错误已由 request 拦截器提示 */ }
}

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