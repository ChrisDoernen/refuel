import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react({ babel: { plugins: ["relay"] } })],
  server: {
    watch: {
      usePolling: true
    },
    proxy: {
      "/graphql": {
        target: "http://localhost:5150",
      }
    }
  }
})
