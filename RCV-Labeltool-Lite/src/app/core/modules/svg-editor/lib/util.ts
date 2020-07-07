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
