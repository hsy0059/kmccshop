import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'

export default defineConfig(({ mode }) => {
  const isH5 = mode === 'h5'
  const isAndroid = mode === 'android'

  return {
    plugins: [vue()],
    resolve: {
      alias: {
        '@': path.resolve(import.meta.dirname, 'src')
      }
    },
    base: isAndroid ? './' : '/',
    build: {
      outDir: isAndroid ? 'www' : (isH5 ? 'dist-h5' : 'dist'),
      assetsInlineLimit: 4096,
      chunkSizeWarningLimit: 1000,
      rollupOptions: {
        output: {
          manualChunks: {
            'element-plus': ['element-plus'],
            'vue-vendor': ['vue', 'vue-router', 'pinia']
          }
        }
      }
    },
    server: {
      port: 3001,
      proxy: {
        '/api': {
          target: 'http://localhost:53517',
          changeOrigin: true
        }
      }
    },
    css: {
      preprocessorOptions: {
        scss: { api: 'modern-compiler', silenceDeprecations: ['legacy-js-api', 'import', 'global-builtin', 'color-functions'] }
      }
    }
  }
})