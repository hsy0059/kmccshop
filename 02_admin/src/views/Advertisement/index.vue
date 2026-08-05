<template>
  <el-card>
    <template #header><div class="card-header"><span>广告管理</span><el-button type="primary" size="small" @click="handleCreate">新增广告</el-button></div></template>
    <el-table :data="list" v-loading="loading" border>
      <el-table-column prop="id" label="ID" width="80" />
      <el-table-column prop="title" label="标题" />
      <el-table-column label="位置" width="100"><template #default="{row}">{{ row.position }}</template></el-table-column>
      <el-table-column prop="sortOrder" label="排序" width="80" />
      <el-table-column label="状态" width="100"><template #default="{row}"><el-tag :type="row.status===1?'success':'danger'">{{ row.status===1?'启用':'禁用' }}</el-tag></template></el-table-column>
      <el-table-column label="操作" width="200"><template #default="{row}"><el-button size="small" @click="handleEdit(row)">编辑</el-button><el-button size="small" type="danger" @click="handleDelete(row.id)">删除</el-button></template></el-table-column>
    </el-table>
    <div style="margin-top:16px;text-align:right"><el-pagination v-model:current-page="page" :total="total" :page-size="20" @current-change="fetchData" layout="total,prev,pager,next" /></div>

    <el-dialog v-model="dialogVisible" title="编辑广告" width="500px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="标题"><el-input v-model="form.title" /></el-form-item>
        <el-form-item label="广告图片">
          <el-upload
            class="ad-uploader"
            :auto-upload="true"
            :show-file-list="false"
            accept="image/jpeg,image/png,image/jpg,image/webp"
            :before-upload="beforeImageUpload"
            :http-request="uploadImage"
          >
            <img v-if="form.image" :src="form.image" class="preview-img" />
            <el-icon v-else class="uploader-icon"><Plus /></el-icon>
          </el-upload>
          <div v-if="form.image" class="upload-tip">已上传图片，点击可重新上传</div>
          <div v-else class="upload-tip">支持 jpg、png、jpeg、webp，大小不超过 10MB</div>
        </el-form-item>
        <el-form-item label="链接"><el-input v-model="form.linkUrl" /></el-form-item>
        <el-form-item label="位置"><el-input v-model="form.position" /></el-form-item>
        <el-form-item label="排序"><el-input-number v-model="form.sortOrder" /></el-form-item>
        <el-form-item label="状态"><el-switch v-model="form.status" :active-value="1" :inactive-value="0" /></el-form-item>
      </el-form>
      <template #footer><el-button @click="dialogVisible=false">取消</el-button><el-button type="primary" @click="handleSave">保存</el-button></template>
    </el-dialog>
  </el-card>
</template>

<style scoped lang="scss">
.ad-uploader {
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
  .preview-img { width: 120px; height: 120px; object-fit: cover; }
  .uploader-icon { font-size: 28px; color: #8c939d; width: 120px; height: 120px; text-align: center; line-height: 120px; }
}
.upload-tip { font-size: 12px; color: #909399; margin-top: 6px; }
</style>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { getAdList, createAd, updateAd, deleteAd, uploadFile } from '@/api'
import { ElMessage } from 'element-plus'
import type { UploadProps } from 'element-plus'

const list = ref([])
const loading = ref(false)
const page = ref(1)
const total = ref(0)
const dialogVisible = ref(false)
const form = reactive<any>({ title:'', image:'', linkUrl:'', position:'banner', sortOrder:0, status:1 })
let editId = 0

const fetchData = async () => {
  loading.value = true
  const res = await getAdList({ page: page.value, pageSize: 20 })
  list.value = res.data.list
  total.value = res.data.total
  loading.value = false
}
fetchData()

const handleCreate = () => {
  editId = 0
  Object.assign(form, { title:'', image:'', linkUrl:'', position:'banner', sortOrder:0, status:1 })
  dialogVisible.value = true
}

const handleEdit = (row: any) => {
  editId = row.id
  Object.assign(form, row)
  dialogVisible.value = true
}

const beforeImageUpload: UploadProps['beforeUpload'] = (rawFile) => {
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

const uploadImage = async (options: any) => {
  try {
    const res = await uploadFile(options.file)
    form.image = res.data?.url || ''
    ElMessage.success('图片上传成功')
  } catch (e) { /* 错误已由 request 拦截器提示 */ }
}

const handleSave = async () => {
  if (editId) { await updateAd(editId, form) }
  else { await createAd(form) }
  ElMessage.success('保存成功')
  dialogVisible.value = false
  fetchData()
}

const handleDelete = async (id: number) => {
  await deleteAd(id)
  ElMessage.success('删除成功')
  fetchData()
}
</script>