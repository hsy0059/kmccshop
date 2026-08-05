<template>
  <el-card>
    <template #header><div class="card-header"><span>商品管理</span><el-button type="primary" size="small" @click="handleCreate">新增商品</el-button></div></template>
    <el-table :data="list" v-loading="loading" border stripe>
      <el-table-column prop="id" label="ID" width="80" />
      <el-table-column prop="name" label="商品名称" min-width="160" show-overflow-tooltip />
      <el-table-column prop="price" label="价格" width="100" />
      <el-table-column prop="stock" label="库存" width="80" />
      <el-table-column label="状态" width="100">
        <template #default="{row}"><el-tag :type="row.status===1?'success':'danger'">{{ row.status===1?'上架':'下架' }}</el-tag></template>
      </el-table-column>
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{row}">
          <el-button size="small" @click="handleEdit(row)">编辑</el-button>
          <el-button size="small" :type="row.status===1?'warning':'success'" @click="toggleStatus(row)">{{ row.status===1?'下架':'上架' }}</el-button>
        </template>
      </el-table-column>
    </el-table>
    <div class="pagination-wrap">
      <el-pagination v-model:current-page="page" :total="total" :page-size="20" layout="total,prev,pager,next" @current-change="fetchData" background />
    </div>
    <el-dialog v-model="dialogVisible" :title="editId?'编辑商品':'新增商品'" width="600px">
      <el-form :model="form" :rules="rules" ref="formRef" label-width="100px">
        <el-form-item label="商品名称" prop="name"><el-input v-model="form.name" placeholder="请输入商品名称" /></el-form-item>
        <el-form-item label="价格" prop="price"><el-input-number v-model="form.price" :min="0" :precision="2" /></el-form-item>
        <el-form-item label="库存" prop="stock"><el-input-number v-model="form.stock" :min="0" /></el-form-item>
        <el-form-item label="描述"><el-input v-model="form.description" type="textarea" rows="3" /></el-form-item>
        <el-form-item label="商品图片">
          <el-upload
            class="image-uploader"
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
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible=false">取消</el-button>
        <el-button type="primary" @click="handleSave" :loading="saving">保存</el-button>
      </template>
    </el-dialog>
  </el-card>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { getProductList, createProduct, updateProduct, setProductStatus, getMerchantInfo, uploadFile } from '@/api'
import { ElMessage } from 'element-plus'
import type { UploadProps } from 'element-plus'

const list = ref<any[]>([])
const loading = ref(false)
const page = ref(1)
const total = ref(0)
const merchantId = ref(0)
const dialogVisible = ref(false)
const saving = ref(false)
const formRef = ref()
const form = reactive<any>({ name: '', price: 0, stock: 0, description: '', image: '' })
const editId = ref(0)

const rules = {
  name: [{ required: true, message: '请输入商品名称', trigger: 'blur' }],
  price: [{ required: true, message: '请输入价格', trigger: 'blur' }],
  stock: [{ required: true, message: '请输入库存', trigger: 'blur' }]
}

const fetchMerchantId = async () => {
  try {
    const res = await getMerchantInfo()
    merchantId.value = res.data?.merchantId || 0
  } catch { merchantId.value = 0 }
}

const fetchData = async () => {
  loading.value = true
  try {
    const res = await getProductList(merchantId.value, { page: page.value, pageSize: 20 })
    list.value = res.data?.list || []
    total.value = res.data?.total || 0
  } catch (e) { list.value = []; total.value = 0 }
  loading.value = false
}

const init = async () => {
  await fetchMerchantId()
  fetchData()
}
init()

const handleCreate = () => {
  editId.value = 0
  Object.assign(form, { name: '', price: 0, stock: 0, description: '', image: '' })
  dialogVisible.value = true
}
const handleEdit = (row: any) => {
  editId.value = row.id
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
  try { await formRef.value.validate() } catch { return }
  saving.value = true
  try {
    if (editId.value) await updateProduct(editId.value, form)
    else await createProduct(form)
    ElMessage.success('保存成功')
    dialogVisible.value = false
    fetchData()
  } catch (e) { }
  saving.value = false
}
const toggleStatus = async (row: any) => {
  try {
    const newStatus = row.status === 1 ? 0 : 1
    await setProductStatus(row.id, { status: newStatus })
    row.status = newStatus
    ElMessage.success('状态已更新')
  } catch (e) { }
}
</script>

<style scoped lang="scss">
.card-header { display: flex; justify-content: space-between; align-items: center; }
.pagination-wrap { margin-top: 16px; text-align: right; }
.image-uploader {
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
