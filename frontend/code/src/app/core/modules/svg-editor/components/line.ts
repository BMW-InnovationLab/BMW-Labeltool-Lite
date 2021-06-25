import { BoundingBoxEditorOptions } from '../models/bounding-box-editor-options.interface'
import { SvgDrawingMode } from '../models/svg-drawing-mode.enum'
import { AbstractTwoPointSvgElement } from './abstract-two-point-svg-element'

export class Line extends AbstractTwoPointSvgElement {
  applyOptions(_value: BoundingBoxEditorOptions): void {}

  get Area(): number {
    return 0
  }

  isValid(): boolean {
    return this._width > 0 || this._height > 0
  }

  destroy() {
    this.svg.selectize(false, { deepSelect: true })

    super.destroy()
  }

  draw(): void {
    if (!this.svg) {
      return
    }

    const strokeWidth = 5

    super.draw()

    this.svg
      .plot(this.Start.x, this.Start.y, this.End.x, this.End.y)
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
      .on('resizing', () => this.onMarkWorking.next(true))
      .on('resizedone', (e: Event) => this.moveOrResizeDone(e))
      .on('dragend', (e: Event) => this.moveOrResizeDone(e))
  }

  private moveOrResizeDone(event: any) {
    const el = event.target as SVGElement
    this.Start = { x: +el.getAttribute('x1'), y: +el.getAttribute('y1') }
    this.End = { x: +el.getAttribute('x2'), y: +el.getAttribute('y2') }

    this.onUpdate.next()
    this.onMarkWorking.next(false)
  }
}
