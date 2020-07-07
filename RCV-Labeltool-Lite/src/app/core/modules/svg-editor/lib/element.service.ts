import { Injectable } from '@angular/core'
import { Subject } from 'rxjs'

import { ILabel } from '@domain/object-detection'
import { RcvObjectclassInterface } from '@rcv/domain'
import { generateGuid } from 'src/app/core/libraries/guid'
import { AbstractSvgElement } from '../components/abstract-svg-element'
import { AbstractTwoPointSvgElement } from '../components/abstract-two-point-svg-element'
import { Box } from '../components/box'
import { Line } from '../components/line'
import { BoundingBoxEditorOptions } from '../models/bounding-box-editor-options.interface'
import { IPoint } from '../models/point.interface'
import { SvgDrawingMode } from '../models/svg-drawing-mode.enum'
import { ElementDiffer } from './element-differ'

@Injectable()
export class ElementService {
  elements: AbstractTwoPointSvgElement[] = []

  changed = new Subject<AbstractSvgElement>()
  selected = new Subject<AbstractSvgElement>()
  hovered = new Subject<AbstractSvgElement>()
  working = new Subject<boolean>()
  elementsChanged = new Subject<ILabel[]>()

  set options(value: BoundingBoxEditorOptions) {
    this._options = value
    this.elements.forEach(e => e.applyOptions(value))
  }
  get options() {
    return this._options
  }

  set zoomFactor(value: number) {
    this._zoomFactor = value
    this.elements.forEach(e => (e.zoomFactor = value))
  }
  get zoomFactor() {
    return this._zoomFactor
  }

  private _options: BoundingBoxEditorOptions
  private _zoomFactor = 1

  constructor() {}

  clear() {
    this.elements.forEach(e => e.destroy())
    this.elements.length = 0
    this.update()
  }

  createNewBox(
    document: SVG.Doc,
    drawingMode: SvgDrawingMode,
    startPoint: IPoint,
    endPoint: IPoint,
    id = generateGuid(),
  ): Box {
    const box = new Box(document, id, this, drawingMode)
    box.applyOptions(this.options)
    box.zoomFactor = this.zoomFactor
    box.StartX = startPoint.x
    box.StartY = startPoint.y
    box.EndX = endPoint.x
    box.EndY = endPoint.y

    return box
  }

  createNewLine(document: SVG.Doc, drawingMode: SvgDrawingMode, point: IPoint): Line {
    const line = new Line(document, generateGuid(), this, drawingMode)
    line.zoomFactor = this.zoomFactor
    line.StartX = point.x
    line.StartY = point.y
    line.EndX = point.x
    line.EndY = point.y

    return line
  }

  findElement(elements: AbstractTwoPointSvgElement[], id: string): AbstractTwoPointSvgElement | undefined {
    return elements.find(b => b.Id === id) || undefined
  }

  reorderElements(elements: AbstractTwoPointSvgElement[]) {
    const elementsCopy = [...elements]
    // sort from large to small
    elementsCopy.sort((a, b) => b.area() - a.area())
    // move small elements last to front
    elementsCopy.forEach((e, index) => {
      const next = index + 1
      if (next < elementsCopy.length) {
        e.after(elementsCopy[next])
      }
    })

    return elementsCopy
  }

  setupLabels(
    labels: ILabel[],
    document: SVG.Doc,
    drawingMode: SvgDrawingMode,
    objectClasses: RcvObjectclassInterface[],
  ) {
    this.elements = ElementDiffer.diff(
      labels,
      this.elements,
      this.createElement(document, drawingMode, objectClasses),
      this.updateElement(objectClasses),
    )
  }

  change(element: AbstractSvgElement) {
    this.changed.next(element)
  }

  select(elementId: string, emit = true) {
    this.elements.forEach(element => (element.isSelected = element.Id === elementId))
    this.update()
  }

  hover(elementId?: string, emit = true) {
    this.elements.forEach(element => (element.isHovered = element.Id === elementId))
    if (emit) {
      this.hovered.next(elementId ? this.findElement(this.elements, elementId) : undefined)
    }
  }

  work(value: boolean) {
    this.working.next(value)
  }

  update() {
    this.elements = this.reorderElements(this.elements)
    this.elementsChanged.next(this.elements.map(e => e.getData() as any))
  }

  private createElement(
    document: SVG.Doc,
    drawingMode: SvgDrawingMode,
    objectClasses: RcvObjectclassInterface[],
  ) {
    return (label: ILabel) => {
      const startPoint: IPoint = { x: label.Left, y: label.Top }
      const endPoint: IPoint = { x: label.Right, y: label.Bottom }
      const box = this.createNewBox(document, drawingMode, startPoint, endPoint, label.Id)
      const objectclass = objectClasses.find(o => o.Id === label.ObjectClassId)
      if (objectclass) {
        box.ObjectClass = objectclass
      }
      box.isSelected = label.isSelected
      box.draw()

      return box
    }
  }

  private updateElement(objectClasses: RcvObjectclassInterface[]) {
    return (element: AbstractTwoPointSvgElement, label: ILabel) => {
      const objectclass = objectClasses.find(o => o.Id === label.ObjectClassId)
      if (objectclass && element instanceof Box) {
        element.ObjectClass = objectclass
      }

      return element
    }
  }
}
