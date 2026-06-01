import { defineConfig } from 'vitest/config'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [
    react(),
    tailwindcss(),
  ],
  server: {
    proxy: {
      '/api/auth': 'http://localhost:5002',
      '/api/recipes': 'http://localhost:5003',
      '/api/meal': 'http://localhost:5003',
      '/api/plans': 'http://localhost:5004',
      '/api/mealplan': 'http://localhost:5004',
    },
  },
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['./src/test/setup.ts'],
    css: false,
  },
})
