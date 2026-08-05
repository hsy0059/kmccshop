import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { mount } from '@vue/test-utils'

// mock three.js — 用 function 声明确保可被 new 调用
function mockObj3D(this: any) {
  this.position = { x: 0, y: 0, z: 0, set: vi.fn(), copy: vi.fn() }
  this.rotation = { x: 0, y: 0, z: 0 }
  this.scale = { x: 1, y: 1, z: 1, set: vi.fn() }
  this.userData = {}
  this.children = []
  this.add = vi.fn(function (this: any, c: any) { this.children.push(c) })
  this.remove = vi.fn()
  this.lookAt = vi.fn()
}

vi.mock('three', () => ({
  Scene: vi.fn(mockObj3D),
  PerspectiveCamera: vi.fn(function (this: any) {
    mockObj3D.call(this)
    this.aspect = 1
    this.updateProjectionMatrix = vi.fn()
  }),
  WebGLRenderer: vi.fn(function (this: any) {
    this.domElement = document.createElement('canvas')
    this.setSize = vi.fn()
    this.setPixelRatio = vi.fn()
    this.render = vi.fn()
    this.dispose = vi.fn()
  }),
  BufferGeometry: vi.fn(function (this: any) {
    const self = this
    this.attributes = {}
    this.setAttribute = function (name: string, attr: any) { self.attributes[name] = attr }
    this.dispose = vi.fn()
  }),
  BufferAttribute: vi.fn(function (this: any, arr: Float32Array) {
    this.array = arr
    this.needsUpdate = false
  }),
  PointsMaterial: vi.fn(function (this: any) { this.dispose = vi.fn() }),
  Points: vi.fn(function (this: any, geometry: any, material: any) {
    mockObj3D.call(this)
    this.geometry = geometry || { attributes: { position: { array: new Float32Array(9), needsUpdate: false } } }
    this.material = material || { dispose: vi.fn() }
  }),
  Color: vi.fn(function (this: any) { this.r = 0; this.g = 0; this.b = 0 }),
  AdditiveBlending: 2
}))

import * as THREE from 'three'
import ParticleBackground from '../ParticleBackground.vue'

describe('ParticleBackground', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    Object.defineProperty(HTMLElement.prototype, 'clientWidth', { configurable: true, value: 800 })
    Object.defineProperty(HTMLElement.prototype, 'clientHeight', { configurable: true, value: 600 })
  })

  it('组件能正常挂载', () => {
    const wrapper = mount(ParticleBackground)
    expect(wrapper.exists()).toBe(true)
    expect(wrapper.find('.particle-bg').exists()).toBe(true)
  })

  it('使用默认 props（color=#409EFF, count=3000）', () => {
    mount(ParticleBackground)
    expect(THREE.Color).toHaveBeenCalledWith('#409EFF')
    expect(THREE.BufferAttribute).toHaveBeenCalledWith(expect.any(Float32Array), 3)
    const attrCall = (THREE.BufferAttribute as any).mock.calls[0]
    expect(attrCall[0].length).toBe(9000) // 3000 * 3
  })

  it('接受自定义 color 和 count', () => {
    mount(ParticleBackground, {
      props: { color: '#FF0000', count: 500 }
    })
    expect(THREE.Color).toHaveBeenCalledWith('#FF0000')
    const attrCall = (THREE.BufferAttribute as any).mock.calls[0]
    expect(attrCall[0].length).toBe(1500) // 500 * 3
  })

  it('onMounted 时初始化 three.js 场景', async () => {
    mount(ParticleBackground)
    await vi.dynamicImportSettled()
    expect(THREE.Scene).toHaveBeenCalled()
    expect(THREE.PerspectiveCamera).toHaveBeenCalled()
    expect(THREE.WebGLRenderer).toHaveBeenCalled()
    expect(THREE.Points).toHaveBeenCalled()
  })

  it('onMounted 时启动 requestAnimationFrame 动画循环', async () => {
    mount(ParticleBackground)
    await vi.dynamicImportSettled()
    expect(globalThis.requestAnimationFrame).toHaveBeenCalled()
  })

  it('onUnmounted 时调用 cancelAnimationFrame 和 dispose', async () => {
    const wrapper = mount(ParticleBackground)
    await vi.dynamicImportSettled()
    const rendererInstance = (THREE.WebGLRenderer as any).mock.results[0].value
    wrapper.unmount()
    expect(globalThis.cancelAnimationFrame).toHaveBeenCalled()
    expect(rendererInstance.dispose).toHaveBeenCalled()
  })

  it('onUnmounted 时移除事件监听', async () => {
    const removeSpy = vi.spyOn(window, 'removeEventListener')
    const wrapper = mount(ParticleBackground)
    await vi.dynamicImportSettled()
    wrapper.unmount()
    expect(removeSpy).toHaveBeenCalledWith('mousemove', expect.any(Function))
    expect(removeSpy).toHaveBeenCalledWith('resize', expect.any(Function))
  })

  it('鼠标移动时触发事件处理', async () => {
    mount(ParticleBackground)
    await vi.dynamicImportSettled()
    // 触发鼠标移动
    window.dispatchEvent(new MouseEvent('mousemove', { clientX: 400, clientY: 300 }))
    // 验证不崩溃即可（实际 position 更新在 animate 循环中）
  })

  it('resize 事件触发 renderer.setSize', async () => {
    mount(ParticleBackground)
    await vi.dynamicImportSettled()
    const rendererInstance = (THREE.WebGLRenderer as any).mock.results[0].value
    const cameraInstance = (THREE.PerspectiveCamera as any).mock.results[0].value
    window.dispatchEvent(new Event('resize'))
    expect(rendererInstance.setSize).toHaveBeenCalled()
    expect(cameraInstance.updateProjectionMatrix).toHaveBeenCalled()
  })
})
