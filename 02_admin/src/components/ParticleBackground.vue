<template>
  <div ref="container" class="particle-bg"></div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import * as THREE from 'three'

const props = withDefaults(defineProps<{
  color?: string
  count?: number
}>(), {
  color: '#409EFF',
  count: 3000
})

const container = ref<HTMLDivElement>()
let scene: THREE.Scene
let camera: THREE.PerspectiveCamera
let renderer: THREE.WebGLRenderer
let points: THREE.Points
let animationId = 0
let mouseX = 0
let mouseY = 0

const init = () => {
  const el = container.value!
  const w = el.clientWidth
  const h = el.clientHeight

  scene = new THREE.Scene()
  camera = new THREE.PerspectiveCamera(75, w / h, 0.1, 1000)
  camera.position.z = 400

  renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true })
  renderer.setSize(w, h)
  renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2))
  el.appendChild(renderer.domElement)

  // 粒子几何
  const geometry = new THREE.BufferGeometry()
  const positions = new Float32Array(props.count * 3)
  const velocities = new Float32Array(props.count * 3)
  for (let i = 0; i < props.count; i++) {
    positions[i * 3] = (Math.random() - 0.5) * 800
    positions[i * 3 + 1] = (Math.random() - 0.5) * 800
    positions[i * 3 + 2] = (Math.random() - 0.5) * 800
    velocities[i * 3] = (Math.random() - 0.5) * 0.3
    velocities[i * 3 + 1] = (Math.random() - 0.5) * 0.3
    velocities[i * 3 + 2] = (Math.random() - 0.5) * 0.3
  }
  geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3))
  geometry.setAttribute('velocity', new THREE.BufferAttribute(velocities, 3))

  const material = new THREE.PointsMaterial({
    color: new THREE.Color(props.color),
    size: 2.5,
    transparent: true,
    opacity: 0.8,
    blending: THREE.AdditiveBlending,
    depthWrite: false
  })
  points = new THREE.Points(geometry, material)
  scene.add(points)
}

const animate = () => {
  animationId = requestAnimationFrame(animate)
  const positions = points.geometry.attributes.position.array as Float32Array
  const velocities = points.geometry.attributes.velocity.array as Float32Array
  for (let i = 0; i < positions.length; i += 3) {
    positions[i] += velocities[i]
    positions[i + 1] += velocities[i + 1]
    positions[i + 2] += velocities[i + 2]
    // 边界反弹
    if (Math.abs(positions[i]) > 400) velocities[i] *= -1
    if (Math.abs(positions[i + 1]) > 400) velocities[i + 1] *= -1
    if (Math.abs(positions[i + 2]) > 400) velocities[i + 2] *= -1
  }
  points.geometry.attributes.position.needsUpdate = true
  points.rotation.y += 0.0005
  points.rotation.x += 0.0003
  // 鼠标视差
  camera.position.x += (mouseX * 50 - camera.position.x) * 0.03
  camera.position.y += (mouseY * 50 - camera.position.y) * 0.03
  camera.lookAt(scene.position)
  renderer.render(scene, camera)
}

const onMouseMove = (e: MouseEvent) => {
  mouseX = (e.clientX / window.innerWidth) * 2 - 1
  mouseY = -(e.clientY / window.innerHeight) * 2 + 1
}

const onResize = () => {
  if (!container.value) return
  const w = container.value.clientWidth
  const h = container.value.clientHeight
  camera.aspect = w / h
  camera.updateProjectionMatrix()
  renderer.setSize(w, h)
}

onMounted(() => {
  init()
  animate()
  window.addEventListener('mousemove', onMouseMove)
  window.addEventListener('resize', onResize)
})

onUnmounted(() => {
  cancelAnimationFrame(animationId)
  window.removeEventListener('mousemove', onMouseMove)
  window.removeEventListener('resize', onResize)
  renderer?.dispose()
  points?.geometry?.dispose()
  ;(points?.material as THREE.Material)?.dispose()
})
</script>

<style scoped>
.particle-bg {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 0;
}
</style>
