<template>
  <div>
    <div class="dashboard-container">
      <div class="stat-card primary">
        <el-icon class="stat-icon" color="#409EFF"><User /></el-icon>
        <div class="stat-info">
          <span class="stat-value">{{ stats.userCount }}</span>
          <span class="stat-label">用户总数</span>
        </div>
      </div>
      <div class="stat-card success">
        <el-icon class="stat-icon" color="#67C23A"><Shop /></el-icon>
        <div class="stat-info">
          <span class="stat-value">{{ stats.merchantCount }}</span>
          <span class="stat-label">入驻商家</span>
        </div>
      </div>
      <div class="stat-card warning">
        <el-icon class="stat-icon" color="#E6A23C"><Document /></el-icon>
        <div class="stat-info">
          <span class="stat-value">{{ stats.todayOrders }}</span>
          <span class="stat-label">今日订单</span>
        </div>
      </div>
      <div class="stat-card danger">
        <el-icon class="stat-icon" color="#F56C6C"><Money /></el-icon>
        <div class="stat-info">
          <span class="stat-value">{{ formatMoney(stats.todayRevenue) }}</span>
          <span class="stat-label">今日营收(元)</span>
        </div>
      </div>
    </div>
    <el-card><template #header>系统概览</template><p style="color:#999;padding:40px 0;text-align:center">欢迎使用校园生活服务平台管理系统</p></el-card>
  </div>
</template>

<script setup lang="ts">
import { reactive, onMounted } from 'vue'
import { getUserStats, getMerchantStats, getOrderStats } from '@/api'
import { User, Shop, Document, Money } from '@element-plus/icons-vue'

const stats = reactive({ userCount: 0, merchantCount: 0, todayOrders: 0, todayRevenue: 0 })

const formatMoney = (v: number) => v ? v.toFixed(2) : '0.00'

const fetchStats = async () => {
  try {
    const [userRes, merchantRes, orderRes] = await Promise.all([
      getUserStats(), getMerchantStats(), getOrderStats()
    ])
    stats.userCount = userRes.data?.data?.totalUsers ?? 0
    stats.merchantCount = merchantRes.data?.data?.totalMerchants ?? 0
    stats.todayOrders = orderRes.data?.data?.todayOrders ?? 0
    stats.todayRevenue = orderRes.data?.data?.todayRevenue ?? 0
  } catch (e) { /* API not available yet */ }
}

onMounted(fetchStats)
</script>

<style scoped lang="scss">
.dashboard-container {
  display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px; margin-bottom: 20px;
}
.stat-card {
  background: #fff; border-radius: 12px; padding: 24px; display: flex; align-items: center; gap: 16px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.06);
  .stat-icon { font-size: 40px; }
  .stat-info { .stat-value { font-size: 28px; font-weight: 700; display: block; } .stat-label { font-size: 13px; color: #999; } }
}
</style>