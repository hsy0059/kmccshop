<template>
  <el-card>
    <template #header><div class="card-header"><span>订餐订单</span><el-select v-model="filterStatus" @change="fetchData" style="width:150px" clearable placeholder="订单状态"><el-option label="待支付" :value="1" /><el-option label="待接单" :value="2" /><el-option label="已接单" :value="3" /><el-option label="配送中" :value="4" /><el-option label="已送达" :value="5" /><el-option label="已取消" :value="6" /><el-option label="已完成" :value="7" /></el-select></div></template>
    <el-table :data="list" v-loading="loading" border>
      <el-table-column prop="orderNo" label="订单号" width="200" />
      <el-table-column prop="totalAmount" label="金额" width="100" />
      <el-table-column prop="actualAmount" label="实付" width="100" />
      <el-table-column label="状态" width="100"><template #default="{row}">{{ ['','待支付','待接单','已接单','配送中','已送达','已取消','已完成'][row.status] }}</template></el-table-column>
      <el-table-column prop="remark" label="备注" />
      <el-table-column prop="createdAt" label="下单时间" width="170" />
      <el-table-column label="操作" width="120"><template #default>
        <el-button size="small" type="primary">详情</el-button>
      </template></el-table-column>
    </el-table>
    <div style="margin-top:16px;text-align:right"><el-pagination v-model:current-page="page" :total="total" :page-size="20" @current-change="fetchData" layout="total,prev,pager,next" /></div>
  </el-card>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { getOrderList } from '@/api'

const list = ref([])
const loading = ref(false)
const page = ref(1)
const total = ref(0)
const filterStatus = ref()

const fetchData = async () => {
  loading.value = true
  const res = await getOrderList({ page: page.value, pageSize: 20, status: filterStatus.value })
  list.value = res.data.list
  total.value = res.data.total
  loading.value = false
}
fetchData()
</script>