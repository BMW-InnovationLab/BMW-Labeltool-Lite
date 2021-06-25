import { RcvObjectClassViewInterface } from '@rcv/domain/labeltool-client'
import { pointFromEvent } from '../lib/util'
import { BoundingBoxEditorOptions } from '../models/bounding-box-editor-options.interface'
import { AbstractTwoPointSvgElement } from './abstract-two-point-svg-element'

export class Box extends AbstractTwoPointSvgElement {
  catchClicks = false
  fillBox = true
  strokeWidth = 3

  private cancelMove = false
  private _objectClass: RcvObjectClassViewInterface
  private _edges: SVG.Line[]
  private _mask: SVG.Element
  private _maskBackground: SVG.Element
  private _maskRect: SVG.Element

  isValid(): boolean {
    return this._width > 0 && this._height > 0
  }

  get ObjectClass(): RcvObjectClassViewInterface {
    return this._objectClass
  }

  set ObjectClass(value: RcvObjectClassViewInterface) {
    this._objectClass = value
    this.Color = value ? `#${value.ColorCode}` : '#fff'
  }

  applyOptions(value: BoundingBoxEditorOptions): void {
    this.catchClicks = value.catchBoxClicks
    this.fillBox = value.fillBox
    this.strokeWidth = value.strokeWidth
    this.draw()
  }

  destroy() {
    super.destroy()

    if (this.svgBack != null) {
      this.svgBack.remove()
      this._edges.forEach(e => e.remove())
      this._mask.remove()
    }
  }

  draw(): void {
    super.draw()

    this.addResizableHandles(this.svg)
    this.styleRect(this.svg, false)

    if (this.hasBackPoints && this.svgBack != null) {
      this.addResizableHandles(this.svgBack)
      this.styleRect(this.svgBack, true)
      this.connectEdges()
      this.updateMask()
    }
  }

  createBackElement(setDefaultPosition = true) {
    this._mask = this.document.mask()
    this.svgBack = this.document
      .rect()
      .draggable(this.resizeOptions.constraint)
      .on('beforedrag', (e: Event) => this.beforeDrag(e))
      .on('resizedone', (e: Event) => this.backMoveOrResizeDone(e))
      .on('dragend', (e: Event) => this.backMoveOrResizeDone(e))
    // readjust order
    this.svg.before(this.svgBack)
    this._edges = [this.document.line(), this.document.line(), this.document.line(), this.document.line()]

    if (!setDefaultPosition) {
      return
    }

    const offset = (this._height + this._width) / 6
    const documentWidth = this.document.viewbox().width - offset
    const documentHeight = this.document.viewbox().height - offset

    const xOffset = this.End.x >= documentWidth ? offset * -1 : offset
    const yOffset = this.End.y >= documentHeight ? offset * -1 : offset
    this.StartBack = { x: this.Start.x + xOffset, y: this.Start.y + yOffset }
    this.EndBack = { x: this.End.x + xOffset, y: this.End.y + yOffset }
  }

  protected create(): SVG.Element {
    return this.document
      .rect()
      .on('resizing', () => this.onMarkWorking.next(true))
      .on('resizedone', (e: Event) => this.moveOrResizeDone(e))
      .on('beforedrag', (e: Event) => this.beforeDrag(e))
      .on('dragend', (e: Event) => this.moveOrResizeDone(e))
  }

  protected markWorking(event: MouseEvent | TouchEvent) {
    if (!this.catchClicks) {
      const point = pointFromEvent(event, this.document)
      if (this.insideRect(point.x, point.y)) {
        // don't catch clicks inside the rect
        this.cancelMove = true
        event.preventDefault()
        return
      }
    }

    this.cancelMove = false
    super.markWorking(event)
  }

  private addResizableHandles(svg: SVG.Element) {
    svg.addClass(this.catchClicks ? 'bounding-box' : 'selector-box').selectize(this.isSelected, {
      pointSize: 7 / this.zoomFactor,
      pointStroke: { width: 1 / this.zoomFactor, color: '#000' },
      pointType: 'rect',
      rotationPoint: false,
      classRect: this.catchClicks ? 'svg_select_boundingRect' : 'svg_select_selectBox',
    })
  }

  private insideRect(x: number, y: number) {
    const margin = 5
    return (
      x > this.Start.x + margin &&
      y > this.Start.y + margin &&
      x < this.End.x - margin &&
      y < this.End.y - margin
    )
  }

  private beforeDrag(event: any) {
    if (this.cancelMove) {
      event.preventDefault()
    }
  }

  private moveOrResizeDone(event: any) {
    const e = event.target as SVGElement
    this.Start = { x: +e.getAttribute('x'), y: +e.getAttribute('y') }
    this.End = {
      x: this.Start.x + +e.getAttribute('width'),
      y: this.Start.y + +e.getAttribute('height'),
    }

    this.updateData()
  }

  private backMoveOrResizeDone(event: any) {
    const e = event.target as SVGElement
    this.StartBack = { x: +e.getAttribute('x'), y: +e.getAttribute('y') }
    this.EndBack = {
      x: this.StartBack.x + +e.getAttribute('width'),
      y: this.StartBack.y + +e.getAttribute('height'),
    }

    this.updateData()
  }

  private updateData() {
    this.onUpdate.next()
    this.onChange.next(this)
    this.onMarkWorking.next(false)
  }

  private styleRect(element: SVG.Element, backElement: boolean) {
    const opacity = this.fillBox ? (this.isHovered || this.isSelected ? 0.4 : 0.2) : 0
    const color = this.catchClicks ? this.Color : '#fff'

    element
      .fill({
        opacity: backElement ? 0 : opacity,
        color: color,
      })
      .stroke({
        width: this.isSelected && !backElement ? 0 : this.strokeWidth / this.zoomFactor,
        color: color,
        opacity: backElement ? 0.5 : 1,
      })
  }

  private connectEdges() {
    if (this._edges == null || !this._edges.length) {
      return
    }

    this._edges[0].plot(this.Start.x, this.Start.y, this.StartBack.x, this.StartBack.y)
    this._edges[1].plot(this.End.x, this.Start.y, this.EndBack.x, this.StartBack.y)
    this._edges[2].plot(this.Start.x, this.End.y, this.StartBack.x, this.EndBack.y)
    this._edges[3].plot(this.End.x, this.End.y, this.EndBack.x, this.EndBack.y)

    this._edges.forEach(e =>
      e.stroke({
        width: this.strokeWidth / this.zoomFactor,
        color: this.Color,
        opacity: 0.5,
      }),
    )
  }

  private updateMask() {
    if (this._mask) {
      this._mask.unmask()
    }
    if (!this._maskRect) {
      this._maskRect = this.document.rect()
    }
    if (!this._maskBackground) {
      this._maskBackground = this.document.rect().fill({ color: '#fff' })
    }

    this._maskRect.size(this.svg.width(), this.svg.height()).move(this.svg.x(), this.svg.y())
    const viewbox = this.document.viewbox()
    this._maskBackground.size(viewbox.width, viewbox.height)

    this._mask.add(this._maskBackground)
    this._mask.add(this._maskRect)
    this.svgBack.maskWith(this._mask)
    this._edges.forEach(e => {
      // if back element is horizontally or vertically aligned we need to unmask to make the edges visible (not sure why)
      if (this.Start.x === this.StartBack.x || this.Start.y === this.StartBack.y) {
        e.unmask()
      } else {
        e.maskWith(this._mask)
      }
    })
  }
}
