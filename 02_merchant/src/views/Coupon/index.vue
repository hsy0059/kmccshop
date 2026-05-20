<template>
  <el-card>
    <template #header><div class="card-header"><span>优惠券管理</span><el-button type="primary" size="small" @click="handleCreate">创建优惠券</el-button></div></template>
    <el-table :data="list" border v-loading="loading">
      <el-table-column prop="name" label="名称" />
      <el-table-column label="类型" width="100"><template #default="{row}">{{ ['','满减','折扣','无门槛'][row.type] }}</template></el-table-column>
      <el-table-column prop="discountValue" label="优惠值" width="100" />
      <el-table-column label="已领取/总量" width="120"><template #default="{row}">{{ row.receivedCount }}/{{ row.totalCount }}</template></el-table-column>
      <el-table-column prop="endTime" label="过期时间" width="170" />
      <el-table-column label="操作" width="150">
        <template #default="{row}">
          <el-button size="small" @click="handleEdit(row)">编辑</el-button>
          <el-button size="small" type="danger" @click="handleDelete(row)">停用</el-button>
        </template>
      </el-table-column>
    </el-table>
    <div style="margin-top:16px;text-align:right"><el-pagination v-model:current-page="page" :total="total" :page-size="20" layout="total,prev,pager,next" @current-change="fetchData" /></div>
    <el-empty v-if="!loading&&list.length===0" description="暂无优惠券" />
  </el-card>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { getMerchantCoupons, deleteCoupon, getMerchantInfo } from '@/api'
import { ElMessage } from 'element-plus'

const list = ref<any[]>([])
const page = ref(1)
const total = ref(0)
const merchantId = ref(0)
const loading = ref(false)

const fetchData = async () => {
  loading.value = true
  try {
    const res = await getMerchantCoupons({ page: page.value, pageSize: 20, merchantId: merchantId.value })
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

const handleCreate = () => { ElMessage.info('创建优惠券功能开发中') }
const handleEdit = (row: any) => { ElMessage.info('编辑优惠券功能开发中') }
const handleDelete = async (row: any) => {
  try { await deleteCoupon(row.id); ElMessage.success('已停用'); fetchData() } catch (e) { }
}
</script>