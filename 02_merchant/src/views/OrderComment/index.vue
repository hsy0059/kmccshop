<template>
  <el-card>
    <template #header><span>评价管理</span></template>
    <el-table :data="list" border v-loading="loading">
      <el-table-column prop="id" label="ID" width="80" />
      <el-table-column prop="rating" label="评分" width="80">
        <template #default="{row}">
          <el-rate v-model="row.rating" disabled show-score />
        </template>
      </el-table-column>
      <el-table-column prop="content" label="评价内容" />
      <el-table-column label="回复内容" width="200">
        <template #default="{row}">
          <span v-if="row.replyContent" style="color:#67C23A">{{ row.replyContent }}</span>
          <span v-else style="color:#999">未回复</span>
        </template>
      </el-table-column>
      <el-table-column prop="createdAt" label="评价时间" width="170" />
      <el-table-column label="操作" width="120">
        <template #default="{row}">
          <el-button v-if="!row.replyContent" size="small" type="primary" @click="handleReply(row)">回复</el-button>
        </template>
      </el-table-column>
    </el-table>
    <div style="margin-top:16px;text-align:right"><el-pagination v-model:current-page="page" :total="total" :page-size="20" layout="total,prev,pager,next" @current-change="fetchData" /></div>
    <el-empty v-if="!loading&&list.length===0" description="暂无评价数据" />
  </el-card>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { getOrderComments, replyComment, getMerchantInfo } from '@/api'
import { ElMessage, ElMessageBox } from 'element-plus'

const list = ref<any[]>([])
const page = ref(1)
const total = ref(0)
const merchantId = ref(0)
const loading = ref(false)

const fetchData = async () => {
  loading.value = true
  try {
    const res = await getOrderComments({ page: page.value, pageSize: 20, merchantId: merchantId.value })
    list.value = res.data?.list || []
    total.value = res.data?.total || 0
  } catch (e) { list.value = [] }
  loading.value = false
}

const init = async () => {
  const infoRes = await getMerchantInfo()
  merchantId.value = infoRes.data?.merchantId || 0
  fetchData()
}
init()

const handleReply = async (row: any) => {
  try {
    const { value } = await ElMessageBox.prompt('回复内容', '回复评价', { confirmButtonText: '回复', inputType: 'textarea' })
    if (value) {
      await replyComment(row.id, { replyContent: value })
      ElMessage.success('回复成功')
      fetchData()
    }
  } catch (e) { /* cancelled */ }
}
</script>