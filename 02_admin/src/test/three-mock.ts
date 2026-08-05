import { vi } from 'vitest'

/**
 * three.js 完整 mock
 * jsdom 不支持 WebGL，所有 three.js 类需要 mock 为简单的 JS 对象
 */

class MockObject3D {
  position = { x: 0, y: 0, z: 0, set: vi.fn(), copy: vi.fn() }
  rotation = { x: 0, y: 0, z: 0 }
  scale = { x: 1, y: 1, z: 1, set: vi.fn() }
  userData: Record<string, any> = {}
  children: any[] = []
  add = vi.fn((child: any) => { this.children.push(child) })
  remove = vi.fn((child: any) => {
    const idx = this.children.indexOf(child)
    if (idx >= 0) this.children.splice(idx, 1)
  })
  lookAt = vi.fn()
}

class MockScene extends MockObject3D {}
class MockGroup extends MockObject3D {}

class MockCamera extends MockObject3D {
  aspect = 1
  updateProjectionMatrix = vi.fn()
}

class MockMesh extends MockObject3D {
  geometry: any
  material: any
  constructor(geometry?: any, material?: any) {
    super()
    this.geometry = geometry || { dispose: vi.fn() }
    this.material = material || { dispose: vi.fn() }
  }
}

class MockSprite extends MockObject3D {
  material: any
  constructor(material?: any) {
    super()
    this.material = material || {}
  }
}

class MockPoints extends MockObject3D {
  geometry: any
  material: any
  constructor(geometry?: any, material?: any) {
    super()
    this.geometry = geometry || {}
    this.material = material || {}
  }
}

class MockLine extends MockObject3D {
  geometry: any
  material: any
  constructor(geometry?: any, material?: any) {
    super()
    this.geometry = geometry || {}
    this.material = material || {}
  }
}

class MockLight extends MockObject3D {}

class MockGeometry {
  attributes: Record<string, any> = {}
  setAttribute = vi.fn((name: string, attr: any) => { this.attributes[name] = attr })
  dispose = vi.fn()
  setFromPoints = vi.fn().mockReturnThis()
}

class MockBufferAttribute {
  array: Float32Array
  needsUpdate = false
  constructor(array: Float32Array) { this.array = array }
}

class MockMaterial {
  dispose = vi.fn()
}

class MockColor {
  r = 0; g = 0; b = 0
  constructor(color?: any) {
    if (typeof color === 'string') {
      this.r = 0.5; this.g = 0.5; this.b = 0.5
    }
  }
}

class MockRaycaster {
  setFromCamera = vi.fn()
  intersectObjects = vi.fn().mockReturnValue([])
}

class MockVector2 { x = 0; y = 0 }
class MockVector3 {
  x = 0; y = 0; z = 0
  constructor(x = 0, y = 0, z = 0) { this.x = x; this.y = y; this.z = z }
}

class MockTextureLoader {
  load = vi.fn().mockReturnValue({})
}

class MockGridHelper extends MockObject3D {}

class MockWebGLRenderer {
  domElement: HTMLCanvasElement
  setSize = vi.fn()
  setPixelRatio = vi.fn()
  render = vi.fn()
  dispose = vi.fn()
  constructor() {
    this.domElement = document.createElement('canvas')
  }
}

export const ThreeMock = {
  Scene: MockScene,
  PerspectiveCamera: MockCamera,
  WebGLRenderer: MockWebGLRenderer,
  BufferGeometry: MockGeometry,
  BufferAttribute: MockBufferAttribute,
  PointsMaterial: MockMaterial,
  MeshPhongMaterial: MockMaterial,
  MeshBasicMaterial: MockMaterial,
  LineBasicMaterial: MockMaterial,
  SpriteMaterial: MockMaterial,
  Points: MockPoints,
  Mesh: MockMesh,
  Sprite: MockSprite,
  Line: MockLine,
  Group: MockGroup,
  Color: MockColor,
  Raycaster: MockRaycaster,
  Vector2: MockVector2,
  Vector3: MockVector3,
  TextureLoader: MockTextureLoader,
  GridHelper: MockGridHelper,
  AmbientLight: MockLight,
  DirectionalLight: MockLight,
  BoxGeometry: MockGeometry,
  IcosahedronGeometry: MockGeometry,
  SphereGeometry: MockGeometry,
  AdditiveBlending: 2,
  PCFSoftShadowMap: 2,
  DoubleSide: 2
}

export default ThreeMock
