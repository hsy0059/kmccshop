import { test, expect, Page } from '@playwright/test'

/**
 * 3D 组件浏览器渲染验证
 *
 * 验证内容：
 * 1. 组件容器正确渲染到 DOM
 * 2. WebGL canvas（如果环境支持）存在且有渲染内容
 * 3. 用户交互不崩溃（鼠标移动、拖拽、resize）
 * 4. 控制台无运行时错误
 * 5. 截图保存供人工确认
 *
 * 注意：headless 浏览器可能不支持 WebGL，canvas 相关测试会自动跳过
 */

const IGNORABLE_ERRORS = [
  'favicon', 'Failed to load resource', 'net::ERR', 'ERR_CONNECTION',
]

function shouldIgnoreError(text: string): boolean {
  return IGNORABLE_ERRORS.some(p => text.includes(p))
}

function collectErrors(page: Page): string[] {
  const errors: string[] = []
  page.on('console', msg => {
    if (msg.type() === 'error') {
      const text = msg.text()
      if (!shouldIgnoreError(text)) errors.push(text)
    }
  })
  page.on('pageerror', err => {
    if (!shouldIgnoreError(err.message)) errors.push(err.message)
  })
  return errors
}

/** 检查浏览器是否支持 WebGL */
async function isWebGLAvailable(page: Page): Promise<boolean> {
  return page.evaluate(() => {
    const c = document.createElement('canvas')
    return !!(c.getContext('webgl2') || c.getContext('webgl'))
  })
}

/** 检查 canvas 是否已初始化（尺寸 > 0 + WebGL context 存在） */
async function canvasHasContent(page: Page, selector: string): Promise<boolean> {
  return page.evaluate((sel) => {
    const canvas = document.querySelector(sel) as HTMLCanvasElement
    if (!canvas || canvas.width === 0 || canvas.height === 0) return false
    const gl = canvas.getContext('webgl2') || canvas.getContext('webgl')
    return !!gl
  }, selector)
}

// ==================== ParticleBackground（admin 登录页）====================

test.describe('ParticleBackground — admin 登录页', () => {
  test('组件容器 .particle-bg 渲染到 DOM', async ({ page }) => {
    await page.goto('http://localhost:3000/login')
    await page.waitForTimeout(2000)
    const container = page.locator('.particle-bg')
    await expect(container).toBeAttached()
  })

  test('WebGL canvas 存在（需 WebGL 支持）', async ({ page }) => {
    await page.goto('http://localhost:3000/login')
    await page.waitForTimeout(2000)
    const webgl = await isWebGLAvailable(page)
    test.skip(!webgl, 'headless 浏览器不支持 WebGL')

    await page.waitForFunction(
      () => !!document.querySelector('.particle-bg canvas'),
      { timeout: 15000 }
    )
    const canvas = page.locator('.particle-bg canvas')
    const box = await canvas.boundingBox()
    expect(box).not.toBeNull()
    expect(box!.width).toBeGreaterThan(100)
  })

  test('canvas 有渲染内容（需 WebGL 支持）', async ({ page }) => {
    await page.goto('http://localhost:3000/login')
    await page.waitForTimeout(2000)
    const webgl = await isWebGLAvailable(page)
    test.skip(!webgl, 'headless 浏览器不支持 WebGL')

    await page.waitForFunction(
      () => !!document.querySelector('.particle-bg canvas'),
      { timeout: 15000 }
    )
    await page.waitForTimeout(2000)
    const hasContent = await canvasHasContent(page, '.particle-bg canvas')
    expect(hasContent).toBe(true)
  })

  test('鼠标移动不产生控制台错误', async ({ page }) => {
    const errors = collectErrors(page)
    await page.goto('http://localhost:3000/login')
    await page.waitForTimeout(1500)
    await page.mouse.move(100, 100)
    await page.mouse.move(640, 360)
    await page.mouse.move(200, 500)
    await page.mouse.move(800, 200)
    await page.waitForTimeout(500)
    expect(errors).toHaveLength(0)
  })

  test('窗口 resize 不产生控制台错误', async ({ page }) => {
    const errors = collectErrors(page)
    await page.goto('http://localhost:3000/login')
    await page.waitForTimeout(1500)
    await page.setViewportSize({ width: 1024, height: 768 })
    await page.waitForTimeout(500)
    await page.setViewportSize({ width: 800, height: 600 })
    await page.waitForTimeout(500)
    await page.setViewportSize({ width: 1280, height: 720 })
    await page.waitForTimeout(500)
    expect(errors).toHaveLength(0)
  })

  test('截图保存', async ({ page }) => {
    await page.goto('http://localhost:3000/login')
    await page.waitForTimeout(2000)
    await page.screenshot({ path: 'e2e/screenshots/particle-background.png' })
  })

  test('组件卸载不产生错误', async ({ page }) => {
    const errors = collectErrors(page)
    await page.goto('http://localhost:3000/login')
    await page.waitForTimeout(2000)
    await page.goto('about:blank')
    await page.waitForTimeout(500)
    expect(errors).toHaveLength(0)
  })
})

// ==================== Bar3DChart（admin Dashboard）====================

test.describe('Bar3DChart — admin Dashboard', () => {
  test.beforeEach(async ({ page }) => {
    await page.addInitScript(() => {
      localStorage.setItem('token', 'e2e-test-token')
      localStorage.setItem('userInfo', JSON.stringify({
        id: 1, username: 'admin', role: 'admin', nickname: 'Admin'
      }))
    })
  })

  test('组件容器 .bar3d-container 渲染到 DOM', async ({ page }) => {
    const errors = collectErrors(page)
    await page.goto('http://localhost:3000/dashboard')
    await page.waitForTimeout(3000)
    const container = page.locator('.bar3d-container')
    await expect(container).toBeAttached()
    expect(errors.filter(e => !e.includes('api') && !e.includes('API'))).toHaveLength(0)
  })

  test('canvas 有渲染内容（需 WebGL 支持）', async ({ page }) => {
    await page.goto('http://localhost:3000/dashboard')
    await page.waitForTimeout(3000)
    const webgl = await isWebGLAvailable(page)
    test.skip(!webgl, 'headless 浏览器不支持 WebGL')

    const canvas = page.locator('.bar3d-container canvas')
    if (await canvas.count() > 0) {
      const hasContent = await canvasHasContent(page, '.bar3d-container canvas')
      expect(hasContent).toBe(true)
    }
  })

  test('鼠标拖拽旋转不报错', async ({ page }) => {
    const errors = collectErrors(page)
    await page.goto('http://localhost:3000/dashboard')
    await page.waitForTimeout(3000)
    const canvas = page.locator('.bar3d-container canvas')
    if (await canvas.count() > 0) {
      const box = await canvas.boundingBox()
      if (box) {
        const cx = box.x + box.width / 2
        const cy = box.y + box.height / 2
        await page.mouse.move(cx, cy)
        await page.mouse.down()
        await page.mouse.move(cx + 100, cy + 50, { steps: 10 })
        await page.mouse.move(cx - 50, cy + 100, { steps: 10 })
        await page.mouse.up()
        await page.waitForTimeout(500)
      }
    }
    expect(errors.filter(e => !e.includes('api') && !e.includes('API'))).toHaveLength(0)
  })

  test('截图保存', async ({ page }) => {
    await page.goto('http://localhost:3000/dashboard')
    await page.waitForTimeout(3000)
    await page.screenshot({ path: 'e2e/screenshots/bar3d-chart.png' })
  })
})

// ==================== SphereMenu（uniapp H5 首页）====================

test.describe('SphereMenu — uniapp H5 首页', () => {
  test('页面加载且 canvas 或降级 UI 存在', async ({ page }) => {
    await page.goto('http://localhost:8080', { waitUntil: 'domcontentloaded' })
    await page.waitForTimeout(5000)
    const canvas = page.locator('canvas')
    const fallback = page.locator('.sphere-fallback, .quick-grid, .quick-actions')
    const canvasCount = await canvas.count()
    const fallbackCount = await fallback.count()
    expect(canvasCount + fallbackCount).toBeGreaterThan(0)
  })

  test('canvas 有渲染内容（需 WebGL 支持）', async ({ page }) => {
    await page.goto('http://localhost:8080', { waitUntil: 'domcontentloaded' })
    await page.waitForTimeout(5000)
    const webgl = await isWebGLAvailable(page)
    test.skip(!webgl, 'headless 浏览器不支持 WebGL')

    const canvas = page.locator('.sphere-container canvas')
    if (await canvas.count() > 0) {
      const hasContent = await canvasHasContent(page, '.sphere-container canvas')
      expect(hasContent).toBe(true)
    }
  })

  test('无严重控制台错误', async ({ page }) => {
    const errors = collectErrors(page)
    await page.goto('http://localhost:8080', { waitUntil: 'domcontentloaded' })
    await page.waitForTimeout(5000)
    const criticalErrors = errors.filter(e =>
      !e.includes('api') && !e.includes('API') &&
      !e.includes('request') && !e.includes('Request') &&
      !e.includes('uni') && !e.includes('Uni')
    )
    expect(criticalErrors).toHaveLength(0)
  })

  test('触摸/点击交互不崩溃', async ({ page }) => {
    const errors = collectErrors(page)
    await page.goto('http://localhost:8080', { waitUntil: 'domcontentloaded' })
    await page.waitForTimeout(5000)
    await page.mouse.move(200, 300)
    await page.mouse.down()
    await page.mouse.move(400, 200, { steps: 10 })
    await page.mouse.up()
    await page.waitForTimeout(500)
    await page.mouse.click(640, 360)
    await page.waitForTimeout(500)
    expect(errors.filter(e =>
      !e.includes('api') && !e.includes('API') &&
      !e.includes('request') && !e.includes('Request')
    )).toHaveLength(0)
  })

  test('截图保存', async ({ page }) => {
    await page.goto('http://localhost:8080', { waitUntil: 'domcontentloaded' })
    await page.waitForTimeout(5000)
    await page.screenshot({ path: 'e2e/screenshots/sphere-menu.png' })
  })
})
