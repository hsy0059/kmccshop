<template>
  <div class="dashboard-container">
    <!-- 统计卡片 -->
    <div class="stat-row">
      <div class="stat-card primary">
        <div class="stat-glow"></div>
        <el-icon class="stat-icon" color="#409EFF"><User /></el-icon>
        <div class="stat-info">
          <span class="stat-value">{{ stats.userCount }}</span>
          <span class="stat-label">用户总数</span>
        </div>
      </div>
      <div class="stat-card success">
        <div class="stat-glow"></div>
        <el-icon class="stat-icon" color="#67C23A"><Shop /></el-icon>
        <div class="stat-info">
          <span class="stat-value">{{ stats.merchantCount }}</span>
          <span class="stat-label">入驻商家</span>
        </div>
      </div>
      <div class="stat-card warning">
        <div class="stat-glow"></div>
        <el-icon class="stat-icon" color="#E6A23C"><Document /></el-icon>
        <div class="stat-info">
          <span class="stat-value">{{ stats.todayOrders }}</span>
          <span class="stat-label">今日订单</span>
        </div>
      </div>
      <div class="stat-card danger">
        <div class="stat-glow"></div>
        <el-icon class="stat-icon" color="#F56C6C"><Money /></el-icon>
        <div class="stat-info">
          <span class="stat-value">{{ formatMoney(stats.todayRevenue) }}</span>
          <span class="stat-label">今日营收(元)</span>
        </div>
      </div>
    </div>

    <!-- 3D 数据可视化 -->
    <el-card class="chart-card">
      <template #header>
        <div class="card-header">
          <span>3D 数据概览</span>
          <span class="hint">拖拽旋转视角</span>
        </div>
      </template>
      <Bar3DChart :data="chartData" />
      <div class="chart-legend">
        <div v-for="d in chartData" :key="d.label" class="legend-item">
          <span class="dot" :style="{ background: d.color }"></span>
          <span class="label">{{ d.label }}</span>
          <span class="value">{{ d.value }}</span>
        </div>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { reactive, onMounted, computed } from 'vue'
import { getUserStats, getMerchantStats, getOrderStats } from '@/api'
import { User, Shop, Document, Money } from '@element-plus/icons-vue'
import Bar3DChart from '@/components/Bar3DChart.vue'

const stats = reactive({ userCount: 0, merchantCount: 0, todayOrders: 0, todayRevenue: 0 })

const formatMoney = (v: number) => v ? v.toFixed(2) : '0.00'

const chartData = computed(() => [
  { label: '用户总数', value: stats.userCount, color: '#409EFF' },
  { label: '入驻商家', value: stats.merchantCount, color: '#67C23A' },
  { label: '今日订单', value: stats.todayOrders, color: '#E6A23C' },
  { label: '今日营收', value: Math.round(stats.todayRevenue), color: '#F56C6C' }
])

const fetchStats = async () => {
  try {
    const [userRes, merchantRes, orderRes] = await Promise.all([
      getUserStats(), getMerchantStats(), getOrderStats()
    ])
    stats.userCount = userRes.data?.totalUsers ?? 0
    stats.merchantCount = merchantRes.data?.totalMerchants ?? 0
    stats.todayOrders = orderRes.data?.todayOrders ?? 0
    stats.todayRevenue = orderRes.data?.todayRevenue ?? 0
  } catch (e) { /* API not available yet */ }
}

onMounted(fetchStats)
</script>

<style scoped lang="scss">
.dashboard-container { padding: 4px; }
.stat-row {
  display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px; margin-bottom: 20px;
}
.stat-card {
  position: relative; overflow: hidden;
  background: #fff; border-radius: 12px; padding: 24px;
  display: flex; align-items: center; gap: 16px;
  box-shadow: 0 2px 12px rgba(0,0,0,0.08);
  transition: transform 0.3s, box-shadow 0.3s;
  &:hover { transform: translateY(-4px); box-shadow: 0 8px 24px rgba(0,0,0,0.12); }
  .stat-glow {
    position: absolute; top: -50%; right: -30%;
    width: 200px; height: 200px; border-radius: 50%;
    filter: blur(40px); opacity: 0.15;
  }
  &.primary .stat-glow { background: #409EFF; }
  &.success .stat-glow { background: #67C23A; }
  &.warning .stat-glow { background: #E6A23C; }
  &.danger .stat-glow { background: #F56C6C; }
  .stat-icon { font-size: 40px; z-index: 1; }
  .stat-info { z-index: 1;
    .stat-value { font-size: 28px; font-weight: 700; display: block; }
    .stat-label { font-size: 13px; color: #999; }
  }
}
.chart-card {
  .card-header { display: flex; justify-content: space-between; align-items: center;
    .hint { font-size: 12px; color: #999; }
  }
}
.chart-legend {
  display: flex; justify-content: center; gap: 32px; margin-top: 16px; flex-wrap: wrap;
  .legend-item { display: flex; align-items: center; gap: 8px; font-size: 14px;
    .dot { width: 12px; height: 12px; border-radius: 50%; }
    .label { color: #666; }
    .value { font-weight: 700; color: #333; }
  }
}
</style>
