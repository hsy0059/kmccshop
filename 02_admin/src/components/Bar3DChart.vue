<template>
  <div ref="container" class="bar3d-container"></div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue'
import * as THREE from 'three'

interface BarData {
  label: string
  value: number
  color: string
}

const props = defineProps<{ data: BarData[] }>()

const container = ref<HTMLDivElement>()
let scene: THREE.Scene
let camera: THREE.PerspectiveCamera
let renderer: THREE.WebGLRenderer
let bars: THREE.Mesh[] = []
let animationId = 0
let isDragging = false
let prevX = 0
let rotationY = 0
let targetRotationY = 0

const init = () => {
  const el = container.value!
  const w = el.clientWidth
  const h = el.clientHeight

  scene = new THREE.Scene()

  camera = new THREE.PerspectiveCamera(50, w / h, 0.1, 1000)
  camera.position.set(0, 8, 16)
  camera.lookAt(0, 3, 0)

  renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true })
  renderer.setSize(w, h)
  renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2))
  el.appendChild(renderer.domElement)

  // 灯光
  const ambient = new THREE.AmbientLight(0xffffff, 0.6)
  scene.add(ambient)
  const dirLight = new THREE.DirectionalLight(0xffffff, 0.8)
  dirLight.position.set(10, 20, 10)
  scene.add(dirLight)

  // 底盘
  const baseGeo = new THREE.BoxGeometry(12, 0.2, 8)
  const baseMat = new THREE.MeshPhongMaterial({
    color: 0x1a2a4a,
    transparent: true,
    opacity: 0.6
  })
  const base = new THREE.Mesh(baseGeo, baseMat)
  base.position.y = -0.1
  scene.add(base)

  // 网格线
  const grid = new THREE.GridHelper(12, 12, 0x409EFF, 0x2a3a5a)
  grid.position.y = 0
  scene.add(grid)

  buildBars()
}

const buildBars = () => {
  // 清除旧柱子
  bars.forEach(b => {
    scene.remove(b)
    b.geometry.dispose()
    ;(b.material as THREE.Material).dispose()
  })
  bars = []

  const maxVal = Math.max(...props.data.map(d => d.value), 1)
  const barWidth = 1.6
  const spacing = 2.6
  const totalWidth = (props.data.length - 1) * spacing
  const startX = -totalWidth / 2

  props.data.forEach((item, i) => {
    const targetHeight = Math.max(0.1, (item.value / maxVal) * 8)
    const geo = new THREE.BoxGeometry(barWidth, 0.1, barWidth)
    const mat = new THREE.MeshPhongMaterial({
      color: new THREE.Color(item.color),
      transparent: true,
      opacity: 0.9,
      emissive: new THREE.Color(item.color),
      emissiveIntensity: 0.2
    })
    const bar = new THREE.Mesh(geo, mat)
    bar.position.x = startX + i * spacing
    bar.position.y = 0.05
    bar.userData = { targetHeight, currentHeight: 0.1 }
    scene.add(bar)
    bars.push(bar)
  })
}

const animate = () => {
  animationId = requestAnimationFrame(animate)
  // 柱子生长动画
  bars.forEach(bar => {
    const ud = bar.userData
    if (ud.currentHeight < ud.targetHeight) {
      ud.currentHeight += (ud.targetHeight - ud.currentHeight) * 0.05
      bar.scale.y = ud.currentHeight / 0.1
      bar.position.y = ud.currentHeight / 2
    }
    // 轻微浮动
    bar.position.y += Math.sin(Date.now() * 0.001 + bar.position.x) * 0.002
  })
  // 旋转
  rotationY += (targetRotationY - rotationY) * 0.05
  scene.rotation.y = rotationY
  renderer.render(scene, camera)
}

const onMouseDown = (e: MouseEvent) => { isDragging = true; prevX = e.clientX }
const onMouseMove = (e: MouseEvent) => {
  if (isDragging) {
    targetRotationY += (e.clientX - prevX) * 0.01
    prevX = e.clientX
  }
}
const onMouseUp = () => { isDragging = false }

const onResize = () => {
  if (!container.value) return
  const w = container.value.clientWidth
  const h = container.value.clientHeight
  camera.aspect = w / h
  camera.updateProjectionMatrix()
  renderer.setSize(w, h)
}

watch(() => props.data, () => buildBars(), { deep: true })

onMounted(() => {
  init()
  animate()
  const el = renderer.domElement
  el.addEventListener('mousedown', onMouseDown)
  window.addEventListener('mousemove', onMouseMove)
  window.addEventListener('mouseup', onMouseUp)
  window.addEventListener('resize', onResize)
})

onUnmounted(() => {
  cancelAnimationFrame(animationId)
  const el = renderer?.domElement
  el?.removeEventListener('mousedown', onMouseDown)
  window.removeEventListener('mousemove', onMouseMove)
  window.removeEventListener('mouseup', onMouseUp)
  window.removeEventListener('resize', onResize)
  renderer?.dispose()
  bars.forEach(b => { b.geometry.dispose(); (b.material as THREE.Material).dispose() })
})
</script>

<style scoped>
.bar3d-container {
  width: 100%;
  height: 380px;
  cursor: grab;
}
.bar3d-container:active { cursor: grabbing; }
</style>
