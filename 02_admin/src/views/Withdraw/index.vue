<template>
  <el-card>
    <template #header><div class="card-header"><span>提现审核</span></div></template>
    <el-table :data="list" v-loading="loading" border>
      <el-table-column prop="id" label="ID" width="80" />
      <el-table-column prop="amount" label="提现金额" width="120" />
      <el-table-column prop="actualAmount" label="实际金额" width="120" />
      <el-table-column prop="accountType" label="方式" width="100" />
      <el-table-column label="状态" width="100"><template #default="{row}">{{ ['','待审核','通过','打款中','已完成','已拒绝'][row.status] }}</template></el-table-column>
      <el-table-column prop="createdAt" label="申请时间" width="170" />
      <el-table-column label="操作" width="200"><template #default="{row}">
        <el-button size="small" type="success" v-if="row.status===1" @click="handleAudit(row.id,2)">通过</el-button>
        <el-button size="small" type="danger" v-if="row.status===1" @click="handleAudit(row.id,5)">拒绝</el-button>
      </template></el-table-column>
    </el-table>
    <div style="margin-top:16px;text-align:right"><el-pagination v-model:current-page="page" :total="total" :page-size="20" @current-change="fetchData" layout="total,prev,pager,next" /></div>
  </el-card>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { getWithdrawList, auditWithdraw } from '@/api'
import { ElMessage } from 'element-plus'

const list = ref([])
const loading = ref(false)
const page = ref(1)
const total = ref(0)

const fetchData = async () => {
  loading.value = true
  const res = await getWithdrawList({ page: page.value, pageSize: 20 })
  list.value = res.data.list
  total.value = res.data.total
  loading.value = false
}
fetchData()

const handleAudit = async (id: number, status: number) => {
  await auditWithdraw(id, { status })
  ElMessage.success('审核完成')
  fetchData()
}
</script>