import { createRouter, createWebHistory, createWebHashHistory } from 'vue-router'

const isNative = (window as any).Capacitor?.isNativePlatform?.() || false
const router = createRouter({
  history: isNative ? createWebHashHistory() : createWebHistory(),
  routes: [
    {
      path: '/login',
      name: 'Login',
      component: () => import('@/views/Login.vue'),
      meta: { title: '登录' }
    },
    {
      path: '/',
      component: () => import('@/layouts/MainLayout.vue'),
      redirect: '/dashboard',
      children: [
        { path: 'dashboard', name: 'Dashboard', component: () => import('@/views/Dashboard/index.vue'), meta: { title: '工作台' } },
        { path: 'users', name: 'Users', component: () => import('@/views/User/index.vue'), meta: { title: '用户管理' } },
        { path: 'merchants', name: 'Merchants', component: () => import('@/views/Merchant/index.vue'), meta: { title: '商家管理' } },
        { path: 'orders', name: 'Orders', component: () => import('@/views/Order/index.vue'), meta: { title: '订餐订单' } },
        { path: 'errands', name: 'Errands', component: () => import('@/views/Errand/index.vue'), meta: { title: '跑腿订单' } },
        { path: 'riders', name: 'Riders', component: () => import('@/views/Rider/index.vue'), meta: { title: '骑手管理' } },
        { path: 'posts', name: 'Posts', component: () => import('@/views/Post/index.vue'), meta: { title: '帖子管理' } },
        { path: 'secondgoods', name: 'SecondGoods', component: () => import('@/views/SecondGoods/index.vue'), meta: { title: '二手管理' } },
        { path: 'lostfound', name: 'LostFound', component: () => import('@/views/LostFound/index.vue'), meta: { title: '失物招领' } },
        { path: 'campus', name: 'Campus', component: () => import('@/views/Campus/index.vue'), meta: { title: '校区管理' } },
        { path: 'advertisements', name: 'Advertisements', component: () => import('@/views/Advertisement/index.vue'), meta: { title: '广告管理' } },
        { path: 'feedbacks', name: 'Feedbacks', component: () => import('@/views/Feedback/index.vue'), meta: { title: '反馈管理' } },
        { path: 'withdraws', name: 'Withdraws', component: () => import('@/views/Withdraw/index.vue'), meta: { title: '提现审核' } },
        { path: 'scripts', name: 'Scripts', component: () => import('@/views/Script/index.vue'), meta: { title: '数据脚本' } }
      ]
    }
  ]
})

// 登录守卫：未登录访问受保护页面时跳转登录页
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token')
  if (!token && to.path !== '/login') {
    next('/login')
  } else if (token && to.path === '/login') {
    next('/dashboard')
  } else {
    next()
  }
})

export default router