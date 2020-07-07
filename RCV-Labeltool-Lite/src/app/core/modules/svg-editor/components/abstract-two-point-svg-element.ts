import { AbstractSvgElement } from './abstract-svg-element'

import { ElementData } from '../models/element-data.interface'
import { IPoint } from '../models/point.interface'

export abstract class AbstractTwoPointSvgElement extends AbstractSvgElement {
  private _start: IPoint = { x: 0, y: 0 }
  private _end: IPoint = { x: 0, y: 0 }
  private _width = 0
  private _height = 0

  get StartX(): number {
    return this._start.x
  }

  set StartX(value: number) {
    this._start.x = Math.round(value)
    this.updateWidth()
  }

  get StartY(): number {
    return this._start.y
  }

  set StartY(value: number) {
    this._start.y = Math.round(value)
    this.updateHeight()
  }

  get EndX(): number {
    return this._end.x
  }

  set EndX(value: number) {
    this._end.x = Math.round(value)
    this.updateWidth()
  }

  get EndY(): number {
    return this._end.y
  }

  set EndY(value: number) {
    this._end.y = Math.round(value)
    this.updateHeight()
  }

  get Width(): number {
    return this._width
  }

  get Height(): number {
    return this._height
  }

  area(): number {
    return this.Width * this.Height
  }

  destroy() {
    this.svg.selectize(false)

    super.destroy()
  }

  getData(): ElementData {
    return {
      Id: this.Id,
      mode: this.drawingMode,
      Left: this.StartX,
      Top: this.StartY,
      Right: this.EndX,
      Bottom: this.EndY,
      isVisible: true,
      isSelected: this.isSelected,
    }
  }

  /**
   * draws the element by resizing from start to end and moves to its top left position
   *
   * @returns {svgjs.Element}
   */
  draw(): SVG.Element {
    const viewBox = this.document.viewbox()
    const resizeOptions = {
      constraint: {
        minX: 0,
        minY: 0,
        maxX: viewBox.width,
        maxY: viewBox.height,
      },
    }

    return this.svg
      .size(this.Width, this.Height)
      .move(Math.min(this.StartX, this.EndX), Math.min(this.StartY, this.EndY))
      .draggable(resizeOptions.constraint)
      .resize(resizeOptions)
  }

  moveDown(amount: number = 1) {
    if (this.EndY >= this.document.viewbox().height) {
      return
    }
    this.StartY += amount
    this.EndY += amount
    this.draw()
    this.elementService.change(this)
  }

  moveLeft(amount: number = 1) {
    if (this.StartX <= 0) {
      return
    }
    this.StartX -= amount
    this.EndX -= amount
    this.draw()
    this.elementService.change(this)
  }

  moveRight(amount: number = 1) {
    if (this.EndX >= this.document.viewbox().width) {
      return
    }
    this.StartX += amount
    this.EndX += amount
    this.draw()
    this.elementService.change(this)
  }

  moveUp(amount: number = 1) {
    if (this.StartY <= 0) {
      return
    }
    this.StartY -= amount
    this.EndY -= amount
    this.draw()
    this.elementService.change(this)
  }

  after(element: AbstractTwoPointSvgElement) {
    this.svg.after(element.svg)
  }

  private updateWidth() {
    this._width = Math.abs(this.StartX - this.EndX)
  }

  private updateHeight() {
    this._height = Math.abs(this.StartY - this.EndY)
  }
}
