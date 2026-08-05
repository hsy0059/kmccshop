<template>
  <el-card>
    <template #header>
      <div class="card-header">
        <span>订单管理</span>
        <el-select v-model="filterStatus" @change="fetchData" style="width:150px" clearable placeholder="订单状态">
          <el-option label="待支付" :value="1" />
          <el-option label="待接单" :value="2" />
          <el-option label="已接单" :value="3" />
          <el-option label="配送中" :value="4" />
          <el-option label="已完成" :value="7" />
        </el-select>
      </div>
    </template>
    <el-table :data="list" v-loading="loading" border stripe>
      <el-table-column prop="orderNo" label="订单号" width="200" />
      <el-table-column prop="totalAmount" label="金额" width="100" />
      <el-table-column prop="actualAmount" label="实付" width="100" />
      <el-table-column label="状态" width="100">
        <template #default="{row}">
          <el-tag :type="statusType(row.status)" size="small">{{ statusText(row.status) }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="remark" label="备注" show-overflow-tooltip />
      <el-table-column prop="createdAt" label="下单时间" width="170" />
      <el-table-column label="操作" width="180" fixed="right">
        <template #default="{row}">
          <el-button size="small" v-if="row.status===2" type="success" @click="accept(row)">接单</el-button>
          <el-button size="small" type="primary" @click="viewDetail(row)">详情</el-button>
        </template>
      </el-table-column>
    </el-table>
    <div class="pagination-wrap">
      <el-pagination v-model:current-page="page" :total="total" :page-size="20" layout="total,prev,pager,next" @current-change="fetchData" background />
    </div>

    <el-dialog v-model="detailVisible" title="订单详情" width="600px">
      <el-descriptions :column="2" border v-if="detailRow">
        <el-descriptions-item label="订单号">{{ detailRow.orderNo }}</el-descriptions-item>
        <el-descriptions-item label="状态">{{ statusText(detailRow.status) }}</el-descriptions-item>
        <el-descriptions-item label="订单金额">¥{{ detailRow.totalAmount }}</el-descriptions-item>
        <el-descriptions-item label="实付金额">¥{{ detailRow.actualAmount }}</el-descriptions-item>
        <el-descriptions-item label="下单时间">{{ detailRow.createdAt }}</el-descriptions-item>
        <el-descriptions-item label="备注">{{ detailRow.remark || '-' }}</el-descriptions-item>
      </el-descriptions>
    </el-dialog>
  </el-card>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { getMerchantOrders, acceptOrder, getMerchantInfo } from '@/api'
import { ElMessage } from 'element-plus'

const list = ref<any[]>([])
const loading = ref(false)
const page = ref(1)
const total = ref(0)
const merchantId = ref(0)
const filterStatus = ref()
const detailVisible = ref(false)
const detailRow = ref<any>(null)

const statusText = (s: number) => ['','待支付','待接单','已接单','配送中','已送达','已取消','已完成'][s] || '未知'
const statusType = (s: number) => ['','danger','warning','primary','success','success','info','success'][s] || 'info'

const fetchData = async () => {
  loading.value = true
  try {
    const res = await getMerchantOrders({ page: page.value, pageSize: 20, status: filterStatus.value, merchantId: merchantId.value })
    list.value = res.data?.list || []
    total.value = res.data?.total || 0
  } catch (e) {
    list.value = []
    total.value = 0
  }
  loading.value = false
}

const init = async () => {
  try {
    const infoRes = await getMerchantInfo()
    merchantId.value = infoRes.data?.merchantId || 0
  } catch { merchantId.value = 0 }
  fetchData()
}
init()

const accept = async (row: any) => {
  try { await acceptOrder(row.id); ElMessage.success('已接单'); fetchData() } catch (e) { }
}
const viewDetail = (row: any) => { detailRow.value = row; detailVisible.value = true }
</script>

<style scoped lang="scss">
.card-header { display: flex; justify-content: space-between; align-items: center; }
.pagination-wrap { margin-top: 16px; text-align: right; }
</style>
