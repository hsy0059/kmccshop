<template>
  <el-card>
    <template #header><div class="card-header"><span>失物招领</span></div></template>
    <el-table :data="list" v-loading="loading" border>
      <el-table-column prop="id" label="ID" width="80" />
      <el-table-column prop="title" label="标题" />
      <el-table-column label="类型" width="100"><template #default="{row}">{{ row.type===1?'寻物':'招领' }}</template></el-table-column>
      <el-table-column prop="location" label="地点" width="120" />
      <el-table-column prop="viewCount" label="浏览" width="80" />
      <el-table-column label="状态" width="100"><template #default="{row}">{{ ['','寻找中','已归还','已过期'][row.status] }}</template></el-table-column>
      <el-table-column prop="createdAt" label="发布时间" width="170" />
    </el-table>
    <div style="margin-top:16px;text-align:right"><el-pagination v-model:current-page="page" :total="total" :page-size="20" @current-change="fetchData" layout="total,prev,pager,next" /></div>
  </el-card>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { getLostFoundList } from '@/api'
const list = ref([])
const loading = ref(false)
const page = ref(1)
const total = ref(0)
const fetchData = async () => {
  loading.value = true
  const res = await getLostFoundList({ page: page.value, pageSize: 20 })
  list.value = res.data.list
  total.value = res.data.total
  loading.value = false
}
fetchData()
</script>