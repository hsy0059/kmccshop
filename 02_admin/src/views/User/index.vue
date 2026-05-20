<template>
  <el-card>
    <template #header><div class="card-header"><span>用户列表</span></div></template>
    <el-table :data="list" v-loading="loading" border>
      <el-table-column prop="id" label="ID" width="80" />
      <el-table-column prop="nickname" label="昵称" />
      <el-table-column prop="phone" label="手机号" width="130" />
      <el-table-column prop="realName" label="真实姓名" />
      <el-table-column label="类型" width="100"><template #default="{row}">{{ ['','学生','商家','骑手','管理员'][row.userType] }}</template></el-table-column>
      <el-table-column label="状态" width="100"><template #default="{row}"><el-tag :type="row.status===1?'success':'danger'">{{ row.status===1?'正常':'禁用' }}</el-tag></template></el-table-column>
      <el-table-column prop="createdAt" label="注册时间" width="170" />
      <el-table-column label="操作" width="200"><template #default="{row}">
        <el-button size="small" @click="handleEdit(row)">编辑</el-button>
        <el-button size="small" type="danger" @click="handleDelete(row.id)">禁用</el-button>
      </template></el-table-column>
    </el-table>
    <div style="margin-top:16px;text-align:right"><el-pagination v-model:current-page="page" :total="total" :page-size="20" @current-change="fetchData" layout="total,prev,pager,next" /></div>

    <el-dialog v-model="dialogVisible" title="编辑用户" width="500px">
      <el-form :model="editForm" label-width="100px">
        <el-form-item label="昵称"><el-input v-model="editForm.nickname" /></el-form-item>
        <el-form-item label="真实姓名"><el-input v-model="editForm.realName" /></el-form-item>
        <el-form-item label="用户类型"><el-select v-model="editForm.userType"><el-option label="学生" :value="1" /><el-option label="商家" :value="2" /><el-option label="骑手" :value="3" /><el-option label="管理员" :value="4" /></el-select></el-form-item>
        <el-form-item label="状态"><el-select v-model="editForm.status"><el-option label="正常" :value="1" /><el-option label="禁用" :value="0" /></el-select></el-form-item>
      </el-form>
      <template #footer><el-button @click="dialogVisible=false">取消</el-button><el-button type="primary" @click="handleSave">保存</el-button></template>
    </el-dialog>
  </el-card>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { getUserList, updateUser, deleteUser } from '@/api'
import { ElMessage, ElMessageBox } from 'element-plus'

const list = ref([])
const loading = ref(false)
const page = ref(1)
const total = ref(0)
const dialogVisible = ref(false)
const editForm = reactive<any>({ nickname: '', realName: '', userType: 1, status: 1 })
let editId = 0

const fetchData = async () => {
  loading.value = true
  const res = await getUserList({ page: page.value, pageSize: 20 })
  list.value = res.data.list
  total.value = res.data.total
  loading.value = false
}
fetchData()

const handleEdit = (row: any) => {
  editId = row.id
  editForm.nickname = row.nickname
  editForm.realName = row.realName
  editForm.userType = row.userType
  editForm.status = row.status
  dialogVisible.value = true
}

const handleSave = async () => {
  await updateUser(editId, editForm)
  ElMessage.success('更新成功')
  dialogVisible.value = false
  fetchData()
}

const handleDelete = async (id: number) => {
  await ElMessageBox.confirm('确定禁用该用户?', '提示', { type: 'warning' })
  await deleteUser(id)
  ElMessage.success('操作成功')
  fetchData()
}
</script>