import { vi } from 'vitest'

// mock requestAnimationFrame / cancelAnimationFrame
let rafId = 0
const rafCallbacks = new Map<number, FrameRequestCallback>()
globalThis.requestAnimationFrame = vi.fn((cb: FrameRequestCallback) => {
  const id = ++rafId
  rafCallbacks.set(id, cb)
  return id
})
globalThis.cancelAnimationFrame = vi.fn((id: number) => {
  rafCallbacks.delete(id)
})

// mock matchMedia（Element Plus 可能用到）
if (!window.matchMedia) {
  window.matchMedia = vi.fn().mockReturnValue({
    matches: false,
    media: '',
    onchange: null,
    addListener: vi.fn(),
    removeListener: vi.fn(),
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    dispatchEvent: vi.fn()
  })
}

// mock canvas getContext（WebGL 不可用）
HTMLCanvasElement.prototype.getContext = vi.fn().mockReturnValue({
  getExtension: vi.fn(),
  getParameter: vi.fn(),
  createShader: vi.fn(),
  shaderSource: vi.fn(),
  compileShader: vi.fn(),
  createProgram: vi.fn(),
  attachShader: vi.fn(),
  linkProgram: vi.fn(),
  useProgram: vi.fn(),
  createBuffer: vi.fn(),
  bindBuffer: vi.fn(),
  bufferData: vi.fn(),
  enableVertexAttribArray: vi.fn(),
  vertexAttribPointer: vi.fn(),
  uniformMatrix4fv: vi.fn(),
  drawArrays: vi.fn(),
  viewport: vi.fn(),
  clearColor: vi.fn(),
  clear: vi.fn(),
  enable: vi.fn(),
  disable: vi.fn(),
  depthFunc: vi.fn()
})
