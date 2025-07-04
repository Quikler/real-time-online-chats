import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react({
      jsxImportSource: '@welldone-software/why-did-you-render', // <-----
    }),
  ],
  resolve: {
    alias: {
      '@src': path.resolve(__dirname, "./src"),
      '@components': path.resolve(__dirname, "./src/components"),
      '@services': path.resolve(__dirname, "./src/services"),
      '$fonts': path.resolve('/fonts')
    }
  },
})
