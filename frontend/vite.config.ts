import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  server: {
    port: 3001,
    strictPort: false,
    open: true,
    proxy: {
      '/api': {
        target: 'http://localhost:5147',
        changeOrigin: true,
        rejectUnauthorized: false,
        secure: false,
      }
    }
  }
})
