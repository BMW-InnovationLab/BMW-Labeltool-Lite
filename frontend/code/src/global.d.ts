declare namespace SVG {
  export type Element = any
  export type Doc = any
  export type Line = any
  export type Point = any
}

declare module 'normalize-wheel' {
  export default function normalizeWheel(
    event: Event,
  ): {
    spinX: number
    spinY: number
    pixelX: number
    pixelY: number
  }
}
