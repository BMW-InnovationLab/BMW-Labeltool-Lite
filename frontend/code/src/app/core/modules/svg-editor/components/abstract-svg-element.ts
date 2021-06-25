import { Subject } from 'rxjs'

import { BoundingBoxEditorOptions } from '../models/bounding-box-editor-options.interface'
import { SvgDrawingMode } from '../models/svg-drawing-mode.enum'

export abstract class AbstractSvgElement {
  protected svg: SVG.Element

  private _color: string
  private _isSelected = false
  private _isHovered = false
  private _zoomFactor = 1

  onChange = new Subject<AbstractSvgElement>()
  onHover = new Subject<number | undefined>()
  onMarkWorking = new Subject<boolean>()
  onSelect = new Subject<number>()
  onUpdate = new Subject()

  get Id() {
    return this.id
  }

  get Color(): string {
    return this._color
  }
  set Color(value: string) {
    if (this.Color === value) {
      return
    }
    this._color = value
    this.draw()
  }

  get isSelected(): boolean {
    return this._isSelected
  }
  set isSelected(value: boolean) {
    if (this.isSelected === value) {
      return
    }
    this._isSelected = value
    this.draw()
  }

  get isHovered(): boolean {
    return this._isHovered
  }
  set isHovered(value: boolean) {
    if (this.isHovered === value) {
      return
    }
    this._isHovered = value
    this.draw()
  }

  get zoomFactor() {
    return this._zoomFactor
  }
  set zoomFactor(value: number) {
    if (this.zoomFactor === value) {
      return
    }
    this._zoomFactor = value
    this.draw()
  }

  constructor(protected document: SVG.Doc, protected id: number, protected drawingMode: SvgDrawingMode) {
    this.init()

    this.svg = this.create()
      .mousedown((evt: MouseEvent) => this.markWorking(evt))
      .mouseout((evt: MouseEvent) => this.handleMouseOut(evt))
      .mouseover(() => this.handleMouseOver())
      .mouseup(() => this.selectElement())
      .touchend(() => this.selectElement())
      .touchstart((evt: MouseEvent) => this.markWorking(evt))
  }

  abstract draw(): void

  abstract applyOptions(value: BoundingBoxEditorOptions): void

  destroy() {
    // complete all subjects
    this.onChange.complete()
    this.onHover.complete()
    this.onMarkWorking.complete()
    this.onSelect.complete()
    this.onUpdate.complete()

    // finally remove element
    this.svg.remove()
  }

  show() {
    this.svg.show()
  }

  hide() {
    this.svg.hide()
  }

  abstract getData(): any

  protected init() {
    this._color = '#000'
  }

  /**
   * creates the svg element
   *
   * @returns {svgjs.Element}
   */
  protected abstract create(): SVG.Element

  // *******************************************************************
  // Input handler
  // *******************************************************************

  protected handleMouseOver() {
    if (!this.isHovered) {
      this.onHover.next(this.Id)
    }
  }

  protected handleMouseOut(event: MouseEvent) {
    if (this.isHovered) {
      this.onHover.next()
    }
  }

  protected markWorking(event: MouseEvent | TouchEvent) {
    this.onMarkWorking.next(true)
  }

  protected selectElement() {
    this.onMarkWorking.next(false)
    this.onSelect.next(this.Id)
  }
}
