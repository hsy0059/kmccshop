import { defineConfig } from '@playwright/test'

export default defineConfig({
  testDir: './e2e',
  timeout: 30000,
  expect: { timeout: 10000 },
  fullyParallel: false,
  retries: 0,
  use: {
    headless: true,
    viewport: { width: 1280, height: 720 },
    screenshot: 'only-on-failure',
    captureConsoleLog: true,
    launchOptions: {
      args: ['--use-gl=angle', '--use-angle=swiftshader', '--enable-unsafe-swiftshader'],
    },
  },
  webServer: [
    {
      command: 'npm run dev',
      port: 3000,
      timeout: 30000,
      reuseExistingServer: true,
      cwd: 'E:\\kmccXM\\02_admin',
    },
    {
      command: 'npm run dev:h5',
      port: 8080,
      timeout: 60000,
      reuseExistingServer: true,
      cwd: 'E:\\kmccXM\\01_uniapp\\uniapp',
    },
  ],
  projects: [
    { name: 'edge', use: { channel: 'msedge' } },
  ],
})
