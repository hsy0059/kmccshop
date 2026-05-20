<template>
  <el-card>
    <template #header><div class="card-header"><span>跑腿订单</span></div></template>
    <el-table :data="list" v-loading="loading" border>
      <el-table-column prop="orderNo" label="订单号" width="200" />
      <el-table-column prop="title" label="标题" />
      <el-table-column prop="tipAmount" label="小费" width="100" />
      <el-table-column label="状态" width="100"><template #default="{row}">{{ ['','待接单','已接单','配送中','已完成','已取消'][row.status] }}</template></el-table-column>
      <el-table-column prop="pickupAddress" label="取件地址" />
      <el-table-column prop="createdAt" label="创建时间" width="170" />
    </el-table>
    <div style="margin-top:16px;text-align:right"><el-pagination v-model:current-page="page" :total="total" :page-size="20" @current-change="fetchData" layout="total,prev,pager,next" /></div>
  </el-card>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { getErrandList } from '@/api'

const list = ref([])
const loading = ref(false)
const page = ref(1)
const total = ref(0)
const fetchData = async () => {
  loading.value = true
  const res = await getErrandList({ page: page.value, pageSize: 20 })
  list.value = res.data.list
  total.value = res.data.total
  loading.value = false
}
fetchData()
</script>