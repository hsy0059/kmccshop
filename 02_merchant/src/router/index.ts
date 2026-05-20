import { createRouter, createWebHistory, createWebHashHistory } from 'vue-router'

const isNative = (window as any).Capacitor?.isNativePlatform?.() || false
export default createRouter({ history: isNative ? createWebHashHistory() : createWebHistory(), routes:[
  { path:'/login', name:'Login', component:() => import('@/views/Login.vue'), meta:{ title:'登录' } },
  { path:'/', component:() => import('@/layouts/MerchantLayout.vue'), redirect:'/dashboard', children:[
    { path:'dashboard', name:'Dashboard', component:() => import('@/views/Dashboard/index.vue'), meta:{ title:'工作台' } },
    { path:'products', name:'Products', component:() => import('@/views/Product/index.vue'), meta:{ title:'商品管理' } },
    { path:'orders', name:'Orders', component:() => import('@/views/Order/index.vue'), meta:{ title:'订单管理' } },
    { path:'comments', name:'Comments', component:() => import('@/views/OrderComment/index.vue'), meta:{ title:'评价管理' } },
    { path:'coupons', name:'Coupons', component:() => import('@/views/Coupon/index.vue'), meta:{ title:'优惠券' } },
    { path:'settings', name:'Settings', component:() => import('@/views/Settings/index.vue'), meta:{ title:'店铺设置' } }
  ] }
]})