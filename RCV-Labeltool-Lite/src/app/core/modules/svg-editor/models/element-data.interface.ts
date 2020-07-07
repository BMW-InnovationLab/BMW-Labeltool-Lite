import { SvgDrawingMode } from './svg-drawing-mode.enum'

export interface ElementData {
  Id: string
  mode: SvgDrawingMode
  Left: number
  Top: number
  Right: number
  Bottom: number
  isVisible: boolean
  isSelected: boolean
}
