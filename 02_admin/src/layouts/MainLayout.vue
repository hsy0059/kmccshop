<template>
  <el-container>
    <el-aside width="200px">
      <div class="logo"><img src="@/assets/logo.png" alt="logo" class="logo-img" /><span class="logo-text">校园生活平台</span></div>
      <el-menu :default-active="route.path" router :collapse="false" background-color="#304156" text-color="#bfcbd9" active-text-color="#409EFF">
        <el-menu-item index="/dashboard"><el-icon><DataLine /></el-icon><span>工作台</span></el-menu-item>
        <el-sub-menu index="users"><template #title><el-icon><User /></el-icon><span>用户管理</span></template>
          <el-menu-item index="/users">用户列表</el-menu-item>
          <el-menu-item index="/feedbacks">反馈管理</el-menu-item>
        </el-sub-menu>
        <el-sub-menu index="biz"><template #title><el-icon><Shop /></el-icon><span>业务管理</span></template>
          <el-menu-item index="/merchants">商家管理</el-menu-item>
          <el-menu-item index="/riders">骑手管理</el-menu-item>
          <el-menu-item index="/orders">订餐订单</el-menu-item>
          <el-menu-item index="/errands">跑腿订单</el-menu-item>
          <el-menu-item index="/withdraws">提现审核</el-menu-item>
        </el-sub-menu>
        <el-sub-menu index="community"><template #title><el-icon><ChatRound /></el-icon><span>社区管理</span></template>
          <el-menu-item index="/posts">帖子管理</el-menu-item>
          <el-menu-item index="/secondgoods">二手管理</el-menu-item>
          <el-menu-item index="/lostfound">失物招领</el-menu-item>
          <el-menu-item index="/advertisements">广告管理</el-menu-item>
        </el-sub-menu>
        <el-sub-menu index="system"><template #title><el-icon><Setting /></el-icon><span>系统管理</span></template>
          <el-menu-item index="/campus">校区管理</el-menu-item>
          <el-menu-item index="/scripts">数据脚本</el-menu-item>
        </el-sub-menu>
      </el-menu>
    </el-aside>
    <el-container>
      <el-header>
        <el-icon @click="toggleCollapse" class="collapse-btn"><Fold /></el-icon>
        <div class="header-right">
          <span>{{ store.userInfo?.nickname || '管理员' }}</span>
          <el-button type="primary" link @click="handleLogout">退出</el-button>
        </div>
      </el-header>
      <el-main>
        <router-view />
      </el-main>
    </el-container>
  </el-container>
</template>

<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router'
import { useUserStore } from '@/stores/user'
import { ref } from 'vue'

const route = useRoute()
const router = useRouter()
const store = useUserStore()
const isCollapse = ref(false)
const toggleCollapse = () => { isCollapse.value = !isCollapse.value }

const handleLogout = () => {
  store.logout()
  router.push('/login')
}
</script>

<style scoped lang="scss">
.logo {
  height: 60px; display: flex; align-items: center; justify-content: center; gap: 8px;
  color: #fff; font-size: 16px; font-weight: 700; background: #263445;
  .logo-img { height: 36px; width: 36px; object-fit: contain; border-radius: 6px; }
  .logo-text { white-space: nowrap; }
}
.el-aside { background: #304156; overflow: hidden; }
.el-header {
  display: flex; align-items: center; justify-content: space-between;
  background: #fff; border-bottom: 1px solid #e6e6e6; padding: 0 20px;
  .collapse-btn { font-size: 20px; cursor: pointer; }
  .header-right { display: flex; align-items: center; gap: 12px; }
}
.el-main { background: #f5f7fa; min-height: calc(100vh - 60px); }
</style>