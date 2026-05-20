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
        <el-form-item label="图片URL"><el-input v-model="form.image" /></el-form-item>
        <el-form-item label="链接"><el-input v-model="form.linkUrl" /></el-form-item>
        <el-form-item label="位置"><el-input v-model="form.position" /></el-form-item>
        <el-form-item label="排序"><el-input-number v-model="form.sortOrder" /></el-form-item>
        <el-form-item label="状态"><el-switch v-model="form.status" :active-value="1" :inactive-value="0" /></el-form-item>
      </el-form>
      <template #footer><el-button @click="dialogVisible=false">取消</el-button><el-button type="primary" @click="handleSave">保存</el-button></template>
    </el-dialog>
  </el-card>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { getAdList, createAd, updateAd, deleteAd } from '@/api'
import { ElMessage } from 'element-plus'

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