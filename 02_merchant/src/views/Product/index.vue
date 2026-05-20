<template>
  <el-card>
    <template #header><div class="card-header"><span>商品管理</span><el-button type="primary" size="small" @click="handleCreate">新增商品</el-button></div></template>
    <el-table :data="list" border>
      <el-table-column prop="id" label="ID" width="80" />
      <el-table-column prop="name" label="商品名称" />
      <el-table-column prop="price" label="价格" width="100" />
      <el-table-column prop="stock" label="库存" width="80" />
      <el-table-column label="状态" width="100">
        <template #default="{row}"><el-tag :type="row.status===1?'success':'danger'">{{ row.status===1?'上架':'下架' }}</el-tag></template>
      </el-table-column>
      <el-table-column label="操作" width="200">
        <template #default="{row}">
          <el-button size="small" @click="handleEdit(row)">编辑</el-button>
          <el-button size="small" :type="row.status===1?'warning':'success'" @click="toggleStatus(row)">{{ row.status===1?'下架':'上架' }}</el-button>
        </template>
      </el-table-column>
    </el-table>
    <div style="margin-top:16px;text-align:right"><el-pagination v-model:current-page="page" :total="total" :page-size="20" layout="total,prev,pager,next" @current-change="fetchData" /></div>
    <el-dialog v-model="dialogVisible" :title="editId?'编辑商品':'新增商品'" width="600px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="商品名称"><el-input v-model="form.name" /></el-form-item>
        <el-form-item label="价格"><el-input-number v-model="form.price" :precision="2" /></el-form-item>
        <el-form-item label="库存"><el-input-number v-model="form.stock" /></el-form-item>
        <el-form-item label="描述"><el-input v-model="form.description" type="textarea" rows="3" /></el-form-item>
        <el-form-item label="图片URL"><el-input v-model="form.image" /></el-form-item>
      </el-form>
      <template #footer><el-button @click="dialogVisible=false">取消</el-button><el-button type="primary" @click="handleSave">保存</el-button></template>
    </el-dialog>
  </el-card>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { getProductList, createProduct, updateProduct, setProductStatus, getMerchantInfo } from '@/api'
import { ElMessage } from 'element-plus'

const list = ref<any[]>([])
const page = ref(1)
const total = ref(0)
const merchantId = ref(0)
const dialogVisible = ref(false)
const form = reactive<any>({ name: '', price: 0, stock: 0, description: '', image: '' })
let editId = 0

const fetchMerchantId = async () => {
  const res = await getMerchantInfo()
  merchantId.value = res.data?.merchantId || 0
}

const fetchData = async () => {
  try {
    const res = await getProductList(merchantId.value, { page: page.value, pageSize: 20 })
    list.value = res.data?.list || []
    total.value = res.data?.total || 0
  } catch (e) { list.value = []; total.value = 0 }
}

const init = async () => {
  await fetchMerchantId()
  fetchData()
}
init()

const handleCreate = () => {
  editId = 0; Object.assign(form, { name: '', price: 0, stock: 0, description: '', image: '' }); dialogVisible.value = true
}
const handleEdit = (row: any) => {
  editId = row.id; Object.assign(form, row); dialogVisible.value = true
}
const handleSave = async () => {
  try {
    if (editId) await updateProduct(editId, form)
    else await createProduct(form)
    ElMessage.success('保存成功'); dialogVisible.value = false; fetchData()
  } catch (e) { }
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