import { IPoint } from '../models/point.interface'

const cursorSize = 150
const cursorOffset = cursorSize / 2

export class Cursor {
  private cursor: SVG.Doc

  constructor(document: any) {
    this.cursor = document.group()
    this.cross(cursorOffset, cursorSize, '#000', [2, 2])
    this.cross(cursorOffset, 22, '#fff', [0, 2, 0])

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

  private cross(offset: number, size: number, color: string, dash: number[]) {
    const stroke = { color: color, dasharray: dash, width: 2 }

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
