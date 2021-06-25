import { SvgDrawingMode } from './svg-drawing-mode.enum'

export interface ElementData {
  Id: number
  mode: SvgDrawingMode
  Left: number
  Top: number
  Right: number
  Bottom: number
  LeftBack?: number
  TopBack?: number
  RightBack?: number
  BottomBack?: number
  isVisible: boolean
  isSelected: boolean
}
