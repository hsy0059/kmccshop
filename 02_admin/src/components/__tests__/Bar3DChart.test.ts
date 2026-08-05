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
    this.attributes = {}
    this.setAttribute = vi.fn()
    this.dispose = vi.fn()
    this.setFromPoints = vi.fn().mockReturnThis()
  }),
  BoxGeometry: vi.fn(function (this: any) { this.dispose = vi.fn() }),
  MeshPhongMaterial: vi.fn(function (this: any) { this.dispose = vi.fn() }),
  MeshBasicMaterial: vi.fn(function (this: any) { this.dispose = vi.fn() }),
  Mesh: vi.fn(function (this: any, geometry?: any, material?: any) {
    mockObj3D.call(this)
    this.geometry = geometry || { dispose: vi.fn() }
    this.material = material || { dispose: vi.fn() }
    this.scale = { x: 1, y: 1, z: 1, set: vi.fn() }
  }),
  Group: vi.fn(mockObj3D),
  GridHelper: vi.fn(mockObj3D),
  AmbientLight: vi.fn(mockObj3D),
  DirectionalLight: vi.fn(mockObj3D),
  Color: vi.fn(function (this: any) { this.r = 0; this.g = 0; this.b = 0 }),
  LineBasicMaterial: vi.fn(function (this: any) { this.dispose = vi.fn() }),
  Line: vi.fn(mockObj3D),
  Vector3: vi.fn(function (this: any, x = 0, y = 0, z = 0) { this.x = x; this.y = y; this.z = z })
}))

import * as THREE from 'three'
import Bar3DChart from '../Bar3DChart.vue'

const mockData = [
  { label: '用户', value: 100, color: '#409EFF' },
  { label: '商家', value: 50, color: '#67C23A' },
  { label: '订单', value: 200, color: '#E6A23C' },
  { label: '营收', value: 300, color: '#F56C6C' }
]

describe('Bar3DChart', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    Object.defineProperty(HTMLElement.prototype, 'clientWidth', { configurable: true, value: 800 })
    Object.defineProperty(HTMLElement.prototype, 'clientHeight', { configurable: true, value: 380 })
  })

  afterEach(() => {
    vi.restoreAllMocks()
  })

  it('组件能正常挂载', () => {
    const wrapper = mount(Bar3DChart, { props: { data: mockData } })
    expect(wrapper.exists()).toBe(true)
    expect(wrapper.find('.bar3d-container').exists()).toBe(true)
  })

  it('根据 data 长度创建对应数量的柱子（Mesh）', async () => {
    mount(Bar3DChart, { props: { data: mockData } })
    await vi.dynamicImportSettled()
    // 4 个数据项 + 1 个底盘 = 5 个 Mesh
    expect(THREE.Mesh).toHaveBeenCalledTimes(mockData.length + 1)
  })

  it('使用 data 中的 color 创建 MeshPhongMaterial', async () => {
    mount(Bar3DChart, { props: { data: mockData } })
    await vi.dynamicImportSettled()
    expect(THREE.Color).toHaveBeenCalledWith('#409EFF')
    expect(THREE.Color).toHaveBeenCalledWith('#67C23A')
    expect(THREE.Color).toHaveBeenCalledWith('#E6A23C')
    expect(THREE.Color).toHaveBeenCalledWith('#F56C6C')
  })

  it('创建了网格辅助线 GridHelper', async () => {
    mount(Bar3DChart, { props: { data: mockData } })
    await vi.dynamicImportSettled()
    expect(THREE.GridHelper).toHaveBeenCalled()
  })

  it('创建了环境光和平行光', async () => {
    mount(Bar3DChart, { props: { data: mockData } })
    await vi.dynamicImportSettled()
    expect(THREE.AmbientLight).toHaveBeenCalled()
    expect(THREE.DirectionalLight).toHaveBeenCalled()
  })

  it('onMounted 时启动 requestAnimationFrame 动画循环', async () => {
    mount(Bar3DChart, { props: { data: mockData } })
    await vi.dynamicImportSettled()
    expect(globalThis.requestAnimationFrame).toHaveBeenCalled()
  })

  it('data 变化时重建柱子', async () => {
    const wrapper = mount(Bar3DChart, { props: { data: mockData } })
    await vi.dynamicImportSettled()
    const initialMeshCallCount = (THREE.Mesh as any).mock.calls.length
    await wrapper.setProps({
      data: [
        { label: 'A', value: 10, color: '#000' },
        { label: 'B', value: 20, color: '#fff' }
      ]
    })
    expect((THREE.Mesh as any).mock.calls.length).toBeGreaterThan(initialMeshCallCount)
  })

  it('onUnmounted 时调用 cancelAnimationFrame 和 dispose', async () => {
    const wrapper = mount(Bar3DChart, { props: { data: mockData } })
    await vi.dynamicImportSettled()
    const rendererInstance = (THREE.WebGLRenderer as any).mock.results[0].value
    wrapper.unmount()
    expect(globalThis.cancelAnimationFrame).toHaveBeenCalled()
    expect(rendererInstance.dispose).toHaveBeenCalled()
  })

  it('onUnmounted 时移除事件监听', async () => {
    const removeSpy = vi.spyOn(window, 'removeEventListener')
    const wrapper = mount(Bar3DChart, { props: { data: mockData } })
    await vi.dynamicImportSettled()
    wrapper.unmount()
    expect(removeSpy).toHaveBeenCalledWith('mousemove', expect.any(Function))
    expect(removeSpy).toHaveBeenCalledWith('mouseup', expect.any(Function))
    expect(removeSpy).toHaveBeenCalledWith('resize', expect.any(Function))
  })

  it('resize 事件触发 renderer.setSize', async () => {
    mount(Bar3DChart, { props: { data: mockData } })
    await vi.dynamicImportSettled()
    const rendererInstance = (THREE.WebGLRenderer as any).mock.results[0].value
    const cameraInstance = (THREE.PerspectiveCamera as any).mock.results[0].value
    window.dispatchEvent(new Event('resize'))
    expect(rendererInstance.setSize).toHaveBeenCalled()
    expect(cameraInstance.updateProjectionMatrix).toHaveBeenCalled()
  })

  it('空数据时 maxVal 默认为 1，不报错', async () => {
    mount(Bar3DChart, { props: { data: [] } })
    await vi.dynamicImportSettled()
    // 只创建底盘，无柱子
    expect(THREE.Mesh).toHaveBeenCalledTimes(1)
  })
})
