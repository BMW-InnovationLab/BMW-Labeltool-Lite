/* SystemJS module definition */

declare var module: NodeModule

interface NodeModule {
  id: string
}

import { Doc } from 'svg.js'

// Create an augmentation for "./observable"
declare module 'svg.js' {
  // Augment the 'Observable' class definition with interface merging
  interface Doc {
    panZoom(option?: any): void

    panStart(ev: any): void
  }

  interface Element {
    draggable(opts?: any): Element

    resize(val?: any): Element

    selectize(val: boolean, opts?: any): Element
  }
}
