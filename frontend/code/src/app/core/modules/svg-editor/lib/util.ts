import { IPoint } from '../models/point.interface'

/**
 * gets the relative position to an svg
 *
 * @param {number} x
 * @param {number} y
 * @param {svgjs.Doc} svg
 * @returns {IPoint}
 */
export function getRelativePosition(x: number, y: number, svg: SVG.Doc): IPoint {
  return svg.point(x, y)
}

export function pointFromEvent(event: MouseEvent | TouchEvent, document: SVG.Doc) {
  if (event instanceof MouseEvent) {
    return getRelativePosition(event.clientX, event.clientY, document)
  } else if (event.touches.length === 1) {
    return getRelativePosition(event.touches[0].clientX, event.touches[0].clientY, document)
  }
}
