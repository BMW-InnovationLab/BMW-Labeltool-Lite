import { IPoint } from '../models/point.interface'

const cursorSize = 150
const cursorOffset = cursorSize / 2

export class Cursor {
  private cursor: SVG.Doc

  constructor(document: any) {
    this.cursor = document.group()
    this.cursor.addClass('svg-cursor')
    this.cross(cursorOffset, cursorSize, '#000', 1)
    this.cross(cursorOffset, 30, '#fff', 1, true)

    this.hide()
  }

  hide() {
    this.cursor.hide()
  }

  show() {
    this.cursor.show()
  }

  move(point: IPoint) {
    this.cursor.move(point.x - cursorOffset, point.y - cursorOffset)
  }

  scale(zoom: number) {
    const zoomInverse = 1 / zoom

    this.cursor.clear()
    this.cross(cursorOffset, cursorSize, '#000', zoomInverse)
    this.cross(cursorOffset, 30, '#fff', zoomInverse, true)
  }

  private cross(offset: number, size: number, color: string, zoom: number, off = false) {
    size = size * zoom
    const dashWidth = 2 * zoom
    const dash = off ? [0, dashWidth, 0] : [dashWidth, dashWidth]
    const stroke = { color: color, dasharray: dash, width: dashWidth }

    this.cursor
      .line(`${offset - size / 2},${offset} ${offset + size / 2},${offset}`)
      .fill('none')
      .stroke(stroke)
    this.cursor
      .line(`${offset},${offset - size / 2} ${offset},${offset + size / 2}`)
      .fill('none')
      .stroke(stroke)
  }
}
