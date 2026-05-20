<template>
  <el-card>
    <template #header><div class="card-header"><span>订单管理</span><el-select v-model="filterStatus" @change="fetchData" style="width:150px" clearable placeholder="订单状态"><el-option label="待支付" :value="1" /><el-option label="待接单" :value="2" /><el-option label="已接单" :value="3" /><el-option label="配送中" :value="4" /><el-option label="已完成" :value="7" /></el-select></div></template>
    <el-table :data="list" border>
      <el-table-column prop="orderNo" label="订单号" width="200" />
      <el-table-column prop="totalAmount" label="金额" width="100" />
      <el-table-column prop="actualAmount" label="实付" width="100" />
      <el-table-column label="状态" width="100"><template #default="{row}">{{ ['','待支付','待接单','已接单','配送中','已送达','已取消','已完成'][row.status] }}</template></el-table-column>
      <el-table-column prop="remark" label="备注" />
      <el-table-column prop="createdAt" label="下单时间" width="170" />
      <el-table-column label="操作" width="120">
        <template #default="{row}">
          <el-button size="small" v-if="row.status===2" type="success" @click="accept(row)">接单</el-button>
          <el-button size="small" type="primary" @click="viewDetail(row)">详情</el-button>
        </template>
      </el-table-column>
    </el-table>
    <div style="margin-top:16px;text-align:right"><el-pagination v-model:current-page="page" :total="total" :page-size="20" layout="total,prev,pager,next" @current-change="fetchData" /></div>
  </el-card>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { getMerchantOrders, acceptOrder, getMerchantInfo } from '@/api'
import { ElMessage } from 'element-plus'

const list = ref<any[]>([])
const page = ref(1)
const total = ref(0)
const merchantId = ref(0)
const filterStatus = ref()

const fetchData = async () => {
  try {
    const res = await getMerchantOrders({ page: page.value, pageSize: 20, status: filterStatus.value, merchantId: merchantId.value })
    list.value = res.data?.list || []
    total.value = res.data?.total || 0
  } catch (e) { list.value = []; total.value = 0 }
}

const init = async () => {
  const infoRes = await getMerchantInfo()
  merchantId.value = infoRes.data?.merchantId || 0
  fetchData()
}
init()

const accept = async (row: any) => {
  try { await acceptOrder(row.id); ElMessage.success('已接单'); fetchData() } catch (e) { }
}
const viewDetail = (row: any) => { ElMessage.info('订单号: ' + row.orderNo) }
</script>