<template>
  <el-card>
    <template #header><div class="card-header"><span>反馈管理</span></div></template>
    <el-table :data="list" v-loading="loading" border>
      <el-table-column prop="id" label="ID" width="80" />
      <el-table-column prop="title" label="标题" />
      <el-table-column label="类型" width="100"><template #default="{row}">{{ ['','问题反馈','功能建议','其他'][row.type] }}</template></el-table-column>
      <el-table-column label="状态" width="100"><template #default="{row}">{{ ['','待处理','处理中','已回复','已关闭'][row.status] }}</template></el-table-column>
      <el-table-column prop="createdAt" label="提交时间" width="170" />
      <el-table-column label="操作" width="120"><template #default="{row}">
        <el-button size="small" @click="openReply(row)">回复</el-button>
      </template></el-table-column>
    </el-table>
    <div style="margin-top:16px;text-align:right"><el-pagination v-model:current-page="page" :total="total" :page-size="20" @current-change="fetchData" layout="total,prev,pager,next" /></div>

    <el-dialog v-model="replyVisible" title="回复反馈" width="500px">
      <el-input v-model="replyContent" type="textarea" rows="4" placeholder="请输入回复内容" />
      <template #footer><el-button @click="replyVisible=false">取消</el-button><el-button type="primary" @click="handleReply">提交回复</el-button></template>
    </el-dialog>
  </el-card>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { getFeedbackList, replyFeedback } from '@/api'
import { ElMessage } from 'element-plus'

const list = ref([])
const loading = ref(false)
const page = ref(1)
const total = ref(0)
const replyVisible = ref(false)
const replyContent = ref('')
let replyId = 0

const fetchData = async () => {
  loading.value = true
  const res = await getFeedbackList({ page: page.value, pageSize: 20 })
  list.value = res.data.list
  total.value = res.data.total
  loading.value = false
}
fetchData()

const openReply = (row: any) => {
  replyId = row.id
  replyContent.value = ''
  replyVisible.value = true
}

const handleReply = async () => {
  await replyFeedback(replyId, { replyContent: replyContent.value })
  ElMessage.success('回复成功')
  replyVisible.value = false
  fetchData()
}
</script>