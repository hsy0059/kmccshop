import { createApp } from 'vue'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import * as Icons from '@element-plus/icons-vue'
import zhCn from 'element-plus/dist/locale/zh-cn.mjs'
import { createPinia } from 'pinia'
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'
import App from './App.vue'
import router from './router'
import './styles/common.scss'

const app = createApp(App)
for (const [key, comp] of Object.entries(Icons)) { app.component(key, comp) }
const pinia = createPinia()
pinia.use(piniaPluginPersistedstate)
app.use(ElementPlus, { locale: zhCn }).use(pinia).use(router).mount('#app')