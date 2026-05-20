<template>
  <el-card>
    <template #header><div class="card-header"><span>骑手列表</span></div></template>
    <el-table :data="list" v-loading="loading" border>
      <el-table-column prop="id" label="ID" width="80" />
      <el-table-column prop="realName" label="姓名" />
      <el-table-column prop="phone" label="电话" width="130" />
      <el-table-column label="审核状态" width="100"><template #default="{row}">{{ ['待审核','通过','拒绝'][row.auditStatus] }}</template></el-table-column>
      <el-table-column label="工作状态" width="100"><template #default="{row}">{{ ['休息','接单中','配送中','禁用'][row.status] }}</template></el-table-column>
      <el-table-column prop="orderCount" label="完成订单" width="90" />
      <el-table-column prop="balance" label="余额" width="100" />
      <el-table-column label="操作" width="220"><template #default="{row}">
        <el-button size="small" type="success" v-if="row.auditStatus===0" @click="approve(row.id,1)">通过</el-button>
        <el-button size="small" type="danger" v-if="row.auditStatus===0" @click="approve(row.id,2)">拒绝</el-button>
      </template></el-table-column>
    </el-table>
    <div style="margin-top:16px;text-align:right"><el-pagination v-model:current-page="page" :total="total" :page-size="20" @current-change="fetchData" layout="total,prev,pager,next" /></div>
  </el-card>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { getRiderList, approveRider } from '@/api'
import { ElMessage } from 'element-plus'

const list = ref([])
const loading = ref(false)
const page = ref(1)
const total = ref(0)
const fetchData = async () => {
  loading.value = true
  const res = await getRiderList({ page: page.value, pageSize: 20 })
  list.value = res.data.list
  total.value = res.data.total
  loading.value = false
}
fetchData()

const approve = async (id: number, status: number) => {
  await approveRider(id, { auditStatus: status })
  ElMessage.success('操作成功')
  fetchData()
}
</script>