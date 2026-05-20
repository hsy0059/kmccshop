<template>
  <div>
    <div class="dashboard-container">
      <div class="stat-card primary">
        <el-icon class="stat-icon" color="#409EFF"><Document /></el-icon>
        <div class="stat-info">
          <span class="stat-value">{{ stats.todayOrders }}</span>
          <span class="stat-label">今日订单</span>
        </div>
      </div>
      <div class="stat-card success">
        <el-icon class="stat-icon" color="#67C23A"><Money /></el-icon>
        <div class="stat-info">
          <span class="stat-value">{{ formatMoney(stats.todayRevenue) }}</span>
          <span class="stat-label">今日营收</span>
        </div>
      </div>
      <div class="stat-card warning">
        <el-icon class="stat-icon" color="#E6A23C"><Goods /></el-icon>
        <div class="stat-info">
          <span class="stat-value">{{ stats.productCount }}</span>
          <span class="stat-label">商品总数</span>
        </div>
      </div>
      <div class="stat-card primary">
        <el-icon class="stat-icon" color="#409EFF"><Star /></el-icon>
        <div class="stat-info">
          <span class="stat-value">{{ stats.rating }}</span>
          <span class="stat-label">店铺评分</span>
        </div>
      </div>
    </div>
    <el-card><template #header>店铺信息 — {{ stats.merchantName }}</template><p style="color:#999;padding:40px 0;text-align:center">欢迎使用商家管理系统</p></el-card>
  </div>
</template>

<script setup lang="ts">
import { reactive, onMounted } from 'vue'
import { getMerchantInfo, getMerchantOrderStats } from '@/api'
import { Document, Money, Goods, Star } from '@element-plus/icons-vue'

const stats = reactive({ todayOrders: 0, todayRevenue: 0, productCount: 0, rating: 0, merchantName: '' })

const formatMoney = (v: number) => v ? v.toFixed(2) : '0.00'

const fetchStats = async () => {
  try {
    const infoRes = await getMerchantInfo()
    const { productCount, rating, merchantId, merchantName } = infoRes.data || {}
    stats.productCount = productCount ?? 0
    stats.rating = rating ?? 0
    stats.merchantName = merchantName ?? ''
    if (merchantId) {
      const orderRes = await getMerchantOrderStats(merchantId)
      stats.todayOrders = orderRes.data?.todayOrders ?? 0
      stats.todayRevenue = orderRes.data?.todayRevenue ?? 0
    }
  } catch (e) { /* API not available yet */ }
}

onMounted(fetchStats)
</script>

<style scoped lang="scss">
.dashboard-container { display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px; margin-bottom: 20px; }
.stat-card { background: #fff; border-radius: 12px; padding: 24px; display: flex; align-items: center; gap: 16px; box-shadow:0 2px 8px rgba(0,0,0,0.06); .stat-icon { font-size: 40px; } .stat-info { .stat-value { font-size:28px;font-weight:700;display:block; } .stat-label { font-size:13px;color:#999; } } }
</style>