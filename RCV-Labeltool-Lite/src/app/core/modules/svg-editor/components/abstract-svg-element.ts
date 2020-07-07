import { BoundingBoxEditorOptions } from '../models/bounding-box-editor-options.interface'
import { SvgDrawingMode } from '../models/svg-drawing-mode.enum'
import { ElementService } from './../lib/element.service'

export abstract class AbstractSvgElement {
  protected currentX: number
  protected currentY: number
  protected svg: SVG.Element

  private _color: string
  private _isSelected = false
  private _isHovered = false
  private _zoomFactor = 1

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

  constructor(
    protected document: SVG.Doc,
    protected id: string,
    protected elementService: ElementService,
    protected drawingMode: SvgDrawingMode,
  ) {
    this.init()

    this.svg = this.create()
      .mouseover((evt: MouseEvent) => this.handleMouseOver(evt))
      .mousedown((evt: MouseEvent) => this.handleMouseDown(evt))
      .mouseup((evt: MouseEvent) => this.handleMouseUp(evt))
      .mouseout((evt: MouseEvent) => this.handleMouseOut(evt))
  }

  /**
   * draws the svg element
   *
   * @returns {svgjs.Element}
   */
  abstract draw(): SVG.Element

  abstract applyOptions(value: BoundingBoxEditorOptions): void

  destroy() {
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
  // Mouse Events
  // *******************************************************************

  protected handleMouseOver(event: MouseEvent) {
    if (!this.isHovered) {
      this.elementService.hover(this.Id)
    }
  }

  protected handleMouseOut(event: MouseEvent) {
    if (this.isHovered) {
      this.elementService.hover()
    }
  }

  protected handleMouseDown(event: MouseEvent) {
    this.elementService.work(true)
  }

  protected handleMouseUp(event: MouseEvent) {
    this.elementService.work(false)
    this.elementService.select(this.Id)
  }
}
