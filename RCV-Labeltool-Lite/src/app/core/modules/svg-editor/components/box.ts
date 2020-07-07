import { RcvObjectclassInterface } from '@rcv/domain'
import { getRelativePosition } from '../lib/util'
import { BoundingBoxEditorOptions } from '../models/bounding-box-editor-options.interface'
import { AbstractTwoPointSvgElement } from './abstract-two-point-svg-element'

export class Box extends AbstractTwoPointSvgElement {
  catchClicks = false
  strokeWidth = 3

  private cancelMove = false
  private _objectClass: RcvObjectclassInterface

  get ObjectClass(): RcvObjectclassInterface {
    return this._objectClass
  }
  set ObjectClass(value: RcvObjectclassInterface) {
    this._objectClass = value
    this.Color = value ? `#${value.ColorCode}` : '#fff'
  }

  applyOptions(value: BoundingBoxEditorOptions): void {
    this.catchClicks = value.catchBoxClicks
    this.draw()
  }

  /**
   * draws a box by filling it with the actual color
   *
   * @returns {svgjs.Element}
   */
  draw(): SVG.Element {
    const opacity: number = this.isHovered || this.isSelected ? 0.5 : 0.3

    const draw = super
      .draw()
      .addClass(this.catchClicks ? 'bounding-box' : 'selector-box')
      .fill({
        opacity: opacity,
        color: this.Color,
      })
      .stroke({
        width: this.isSelected || !this.catchClicks ? 0 : this.strokeWidth / this.zoomFactor,
        color: this.Color,
      })
      .selectize(this.catchClicks ? this.isSelected : true, {
        pointSize: 7 / this.zoomFactor,
        pointStroke: { width: 1 / this.zoomFactor, color: '#000' },
        pointType: 'rect',
        rotationPoint: false,
        classRect: this.catchClicks ? 'svg_select_boundingRect' : 'svg_select_selectBox',
      })

    return draw
  }

  /**
   * creates a svg box at the root document
   */
  protected create(): SVG.Element {
    return this.document
      .rect()

      .on('resizedone', (e: Event) => {
        this.moveOrResizeDone(e)
      })
      .on('beforedrag', (e: Event) => {
        this.beforeDrag(e)
      })
      .on('dragend', (e: Event) => {
        this.moveOrResizeDone(e)
      })
  }

  protected handleMouseDown(event: MouseEvent) {
    if (!this.catchClicks) {
      const point = getRelativePosition(event.clientX, event.clientY, this.document)
      if (this.insideRect(point.x, point.y)) {
        // don't catch clicks inside the rect
        this.cancelMove = true
        event.preventDefault()
        return
      }
    }

    this.cancelMove = false
    super.handleMouseDown(event)
  }

  private insideRect(x: number, y: number) {
    const margin = 5
    return (
      x > this.StartX + margin && y > this.StartY + margin && x < this.EndX - margin && y < this.EndY - margin
    )
  }

  private beforeDrag(event: Event) {
    if (this.cancelMove) {
      event.preventDefault()
    }
  }

  private moveOrResizeDone(event: any) {
    const el = event.target as SVGElement
    this.StartX = Number(el.getAttribute('x'))
    this.StartY = Number(el.getAttribute('y'))
    this.EndX = this.StartX + Number(el.getAttribute('width'))
    this.EndY = this.StartY + Number(el.getAttribute('height'))

    this.elementService.update()
    this.elementService.change(this)
  }
}
