<template><el-container><el-aside width="200px"><div class="logo">商家管理</div><el-menu default-active="/dashboard" router background-color="#304156" text-color="#bfcbd9" active-text-color="#409EFF"><el-menu-item index="/dashboard"><el-icon><DataLine /></el-icon><span>工作台</span></el-menu-item><el-menu-item index="/products"><el-icon><Goods /></el-icon><span>商品管理</span></el-menu-item><el-menu-item index="/orders"><el-icon><Document /></el-icon><span>订单管理</span></el-menu-item><el-menu-item index="/comments"><el-icon><ChatRound /></el-icon><span>评价管理</span></el-menu-item><el-menu-item index="/coupons"><el-icon><Ticket /></el-icon><span>优惠券</span></el-menu-item><el-menu-item index="/settings"><el-icon><Setting /></el-icon><span>店铺设置</span></el-menu-item></el-menu></el-aside><el-container><el-header><span>{{ store.info?.nickname||'商家' }}</span><el-button type="primary" link @click="handleLogout">退出</el-button></el-header><el-main><router-view /></el-main></el-container></el-container></template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useMerchantStore } from '@/stores/merchant'
const router = useRouter(); const store = useMerchantStore()
const handleLogout = () => { store.logout(); router.push('/login') }
</script>

<style scoped lang="scss">
.logo { height:60px; display:flex; align-items:center; justify-content:center; color:#fff; font-size:16px; font-weight:700; background:#263445; }
.el-aside { background:#304156; overflow:hidden; }
.el-header { display:flex; align-items:center; justify-content:space-between; background:#fff; border-bottom:1px solid #e6e6e6; padding:0 20px; }
.el-main { background:#f5f7fa; min-height:calc(100vh - 60px); }
</style>