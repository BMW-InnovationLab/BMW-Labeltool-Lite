import { BoundingBoxEditorOptions } from '../models/bounding-box-editor-options.interface'
import { SvgDrawingMode } from '../models/svg-drawing-mode.enum'
import { AbstractTwoPointSvgElement } from './abstract-two-point-svg-element'

export class Line extends AbstractTwoPointSvgElement {
  applyOptions(_value: BoundingBoxEditorOptions): void {}

  area(): number {
    return 0
  }

  destroy() {
    this.svg.selectize(false, { deepSelect: true })

    super.destroy()
  }

  draw(): SVG.Element {
    if (!this.svg) {
      return
    }

    const strokeWidth = 5

    const svg = super.draw() as SVG.Line
    return svg
      .plot(this.StartX, this.StartY, this.EndX, this.EndY)
      .stroke({
        width: strokeWidth / this.zoomFactor,
        color: this.Color,
      })
      .selectize(this.isSelected, {
        pointSize: 7 / this.zoomFactor,
        pointType: 'rect',
        deepSelect: true,
        rotationPoint: false,
      })
  }

  protected init() {
    this.Color = this.drawingMode === SvgDrawingMode.ForegroundLine ? '#33cc33' : '#ff0000'
  }

  protected create(): SVG.Element {
    return this.document
      .line(0, 0, 0, 0)
      .addClass('move')
      .on('resizedone', (e: Event) => {
        this.moveOrResizeDone(e)
      })
      .on('dragend', (e: Event) => {
        this.moveOrResizeDone(e)
      })
  }

  private moveOrResizeDone(event: any) {
    const el = event.target as SVGElement
    this.StartX = Number(el.getAttribute('x1'))
    this.StartY = Number(el.getAttribute('y1'))
    this.EndX = Number(el.getAttribute('x2'))
    this.EndY = Number(el.getAttribute('y2'))

    this.elementService.update()
  }
}
