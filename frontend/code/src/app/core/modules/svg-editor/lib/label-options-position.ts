import { ILabel } from '@domain/object-detection'

export interface LabelOptionsSettings {
  documentWidth: number
  documentHeight: number
  documentZoom: number
  optionsOffset: number
  optionsHeight: number
  optionsWidth: number
  label: ILabel
}

export interface LabelOptionsPosition {
  transform?: string
}

export function labelBottom(settings: LabelOptionsSettings): LabelOptionsPosition {
  const x = horizontalAlign(settings)
  const y = (settings.label.Bottom + settings.optionsOffset) * settings.documentZoom
  return { transform: `translate(${x}px, ${y}px)` }
}

export function labelMiddle(settings: LabelOptionsSettings): LabelOptionsPosition {
  const x = horizontalAlign(settings) - settings.optionsOffset
  const y =
    settings.label.Top * settings.documentZoom -
    ((settings.label.Top - settings.label.Bottom) * settings.documentZoom + settings.optionsHeight) / 2
  return { transform: `translate(${x}px, ${y}px)` }
}

export function labelTop(settings: LabelOptionsSettings): LabelOptionsPosition {
  const x = horizontalAlign(settings)
  const y = (settings.label.Top - settings.optionsOffset) * settings.documentZoom - settings.optionsHeight
  return { transform: `translate(${x}px, ${y}px)` }
}

export function horizontalAlign(settings: LabelOptionsSettings): number {
  if (settings.label.Right < settings.optionsWidth / settings.documentZoom) {
    // align left
    return settings.label.Left * settings.documentZoom
  }

  return settings.label.Right * settings.documentZoom - settings.optionsWidth
}

export function labelOptionsPosition(settings: LabelOptionsSettings): LabelOptionsPosition {
  const top = settings.label.Bottom * settings.documentZoom + 5
  const overflowBottom = settings.documentHeight * settings.documentZoom < top + settings.optionsHeight
  const overflowTop = settings.label.Top * settings.documentZoom <= settings.optionsHeight

  if (overflowBottom) {
    if (overflowTop) {
      return labelMiddle(settings)
    }
    return labelTop(settings)
  }

  // default
  return labelBottom(settings)
}
