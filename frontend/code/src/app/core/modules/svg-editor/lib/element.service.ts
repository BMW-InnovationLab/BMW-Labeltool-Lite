import { Injectable } from '@angular/core'
import { Subject } from 'rxjs'

import { ILabel } from '@domain/object-detection'
import { RcvObjectClassViewInterface } from '@rcv/domain/labeltool-client'
import { generateGuid } from 'src/app/core/libraries/guid'
import { AbstractSvgElement } from '../components/abstract-svg-element'
import { AbstractTwoPointSvgElement } from '../components/abstract-two-point-svg-element'
import { Box } from '../components/box'
import { Line } from '../components/line'
import { BoundingBoxEditorOptions } from '../models/bounding-box-editor-options.interface'
import { IPoint } from '../models/point.interface'
import { SvgDrawingMode } from '../models/svg-drawing-mode.enum'
import { ElementBuilder } from './element-builder'
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

  get selectedElements(): AbstractTwoPointSvgElement[] {
    return this.elements.filter(e => e.isSelected)
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
    if (this.elements.length === 0) {
      return
    }
    this.elements.forEach(e => e.destroy())
    this.elements.length = 0
    this.update()
  }

  createNewBox(
    document: SVG.Doc,
    drawingMode: SvgDrawingMode,
    startPoint: IPoint,
    endPoint: IPoint,
    startBackPoint?: IPoint,
    endBackPoint?: IPoint,
    id = generateGuid() as any,
  ): Box {
    const box = new Box(document, id, drawingMode)
    ElementBuilder.setBoxGeometry(box, startPoint, endPoint, startBackPoint, endBackPoint)
    this.connectEvents(box)

    box.applyOptions(this.options)
    box.zoomFactor = this.zoomFactor

    return box
  }

  createNewLine(document: SVG.Doc, drawingMode: SvgDrawingMode, point: IPoint): Line {
    const line = new Line(document, generateGuid() as any, drawingMode)
    this.connectEvents(line)
    line.zoomFactor = this.zoomFactor
    line.Start = point
    line.End = point

    return line
  }

  findElement(
    elements: AbstractTwoPointSvgElement[],
    id: string | number,
  ): AbstractTwoPointSvgElement | undefined {
    return elements.find(b => b.Id === id) || undefined
  }

  reorderElements(elements: AbstractTwoPointSvgElement[]) {
    const elementsCopy = [...elements]
    // sort from large to small
    elementsCopy.sort((a, b) => b.Area - a.Area)
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
    objectClasses: RcvObjectClassViewInterface[],
  ) {
    if (document == null) {
      this.elements.forEach(e => e.destroy())
      this.elements.length = 0
      return
    }

    this.elements = ElementDiffer.diff(
      labels,
      this.elements,
      objectClasses,
      this.createElement(document, drawingMode, objectClasses),
    )
  }

  change(element: AbstractSvgElement) {
    this.changed.next(element)
  }

  select(elementId: number, emit = true) {
    this.elements.forEach(element => (element.isSelected = element.Id === elementId))
    if (emit) {
      this.selected.next(this.findElement(this.elements, elementId))
    }
  }

  hover(elementId?: number, emit = true) {
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
    objectClasses: RcvObjectClassViewInterface[],
  ) {
    return (label: ILabel) => {
      const [startPoint, endPoint] = ElementBuilder.pointsFromLabel(label)
      const [startBackPoint, endBackPoint] = ElementBuilder.backPointsFromLabel(label)
      const box = this.createNewBox(
        document,
        drawingMode,
        startPoint,
        endPoint,
        startBackPoint,
        endBackPoint,
        label.Id,
      )
      const objectclass = objectClasses.find(o => o.Id === label.ObjectClassId)
      if (objectclass) {
        box.ObjectClass = objectclass
      }
      box.isSelected = label.isSelected
      box.draw()

      return box
    }
  }

  private connectEvents(element: AbstractSvgElement) {
    element.onChange.subscribe(e => this.change(e))
    element.onHover.subscribe(e => this.hover(e))
    element.onMarkWorking.subscribe(w => this.work(w))
    element.onSelect.subscribe(e => this.select(e))
    element.onUpdate.subscribe(() => this.update())
  }
}
