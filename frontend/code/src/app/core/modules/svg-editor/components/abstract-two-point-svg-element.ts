import { AbstractSvgElement } from './abstract-svg-element'

import { ElementData } from '../models/element-data.interface'
import { IPoint } from '../models/point.interface'

export abstract class AbstractTwoPointSvgElement extends AbstractSvgElement {
  protected svgBack?: SVG.Element

  private _start: IPoint = { x: 0, y: 0 }
  private _startBack: Partial<IPoint> = {}
  private _end: IPoint = { x: 0, y: 0 }
  private _endBack: Partial<IPoint> = {}
  protected _width = 0
  protected _height = 0

  set Start(point: IPoint) {
    this._start = { x: Math.round(point.x), y: Math.round(point.y) }
    this.updateWidth()
    this.updateHeight()
  }

  get Start() {
    return this._start
  }

  set End(point: IPoint) {
    this._end = { x: Math.round(point.x), y: Math.round(point.y) }
    this.updateWidth()
    this.updateHeight()
  }

  get End() {
    return this._end
  }

  set StartBack(point: Partial<IPoint>) {
    this._startBack = { x: Math.round(point.x), y: Math.round(point.y) }
  }

  get StartBack() {
    return this._startBack
  }

  set EndBack(point: Partial<IPoint>) {
    this._endBack = { x: Math.round(point.x), y: Math.round(point.y) }
  }

  get EndBack() {
    return this._endBack
  }

  get hasBackPoints(): boolean {
    return this._startBack != null && this._endBack != null
  }

  get BackWidth(): number {
    return Math.abs(this.StartBack.x - this.EndBack.x)
  }

  get BackHeight(): number {
    return Math.abs(this.StartBack.y - this.EndBack.y)
  }

  get Area(): number {
    return this._width * this._height
  }

  protected get resizeOptions() {
    const viewBox = this.document.viewbox()
    return {
      constraint: {
        minX: 0,
        minY: 0,
        maxX: viewBox.width,
        maxY: viewBox.height,
      },
    }
  }

  abstract isValid(): boolean

  destroy() {
    this.svg.selectize(false)
    if (this.hasBackPoints && this.svgBack != null) {
      this.svgBack.selectize(false)
    }

    super.destroy()
  }

  getData(): ElementData {
    return {
      Id: this.Id,
      mode: this.drawingMode,
      Left: this.Start.x,
      Top: this.Start.y,
      Right: this.End.x,
      Bottom: this.End.y,
      LeftBack: this.StartBack.x,
      TopBack: this.StartBack.y,
      RightBack: this.EndBack.x,
      BottomBack: this.EndBack.y,
      isVisible: true,
      isSelected: this.isSelected,
    }
  }

  /**
   * draws the element by resizing from start to end and moves to its top left position
   *
   * @returns {svgjs.Element}
   */
  draw(): void {
    const resizeOptions = this.resizeOptions

    this.svg = this.svg
      .size(this._width, this._height)
      .move(Math.min(this.Start.x, this.End.x), Math.min(this.Start.y, this.End.y))
      .draggable(resizeOptions.constraint)
      .resize(resizeOptions)

    if (this.hasBackPoints && this.svgBack != null) {
      this.svgBack
        .size(this.BackWidth, this.BackHeight)
        .move(Math.min(this.StartBack.x, this.EndBack.x), Math.min(this.StartBack.y, this.EndBack.y))
        .resize(resizeOptions)
    }
  }

  moveDown(amount: number = 1) {
    if (this.End.y >= this.document.viewbox().height) {
      return
    }
    this._start.y += amount
    this._end.y += amount
    this.draw()
    this.onChange.next(this)
  }

  moveLeft(amount: number = 1) {
    if (this.Start.x <= 0) {
      return
    }
    this._start.x -= amount
    this._end.x -= amount
    this.draw()
    this.onChange.next(this)
  }

  moveRight(amount: number = 1) {
    if (this.End.x >= this.document.viewbox().width) {
      return
    }
    this._start.x += amount
    this._end.x += amount
    this.draw()
    this.onChange.next(this)
  }

  moveUp(amount: number = 1) {
    if (this.Start.y <= 0) {
      return
    }
    this._start.y -= amount
    this._end.y -= amount
    this.draw()
    this.onChange.next(this)
  }

  after(element: AbstractTwoPointSvgElement) {
    this.svg.after(element.svg)
  }

  private updateWidth() {
    this._width = Math.abs(this.Start.x - this.End.x)
  }

  private updateHeight() {
    this._height = Math.abs(this.Start.y - this.End.y)
  }
}
