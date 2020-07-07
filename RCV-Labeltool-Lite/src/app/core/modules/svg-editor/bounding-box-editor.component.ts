import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core'
import { BehaviorSubject, Subscription } from 'rxjs'

declare const SVG: any

import { ILabel } from '@domain/object-detection'
import { RcvObjectclassInterface } from '@rcv/domain'
import { AbstractTwoPointSvgElement } from './components/abstract-two-point-svg-element'
import { Box } from './components/box'
import { Cursor } from './components/cursor'
import { ElementService } from './lib/element.service'
import { getRelativePosition } from './lib/util'
import { BoundingBoxEditorOptions } from './models/bounding-box-editor-options.interface'
import { ElementData } from './models/element-data.interface'
import { IPoint } from './models/point.interface'
import { SvgDrawingMode } from './models/svg-drawing-mode.enum'

const optionsHeight = 67
const optionsWidth = 230

@Component({
  selector: 'rcv-bounding-box-editor',
  templateUrl: './bounding-box-editor.component.html',
  styleUrls: ['./bounding-box-editor.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class BoundingBoxEditorComponent implements OnInit, OnDestroy {
  @ViewChild('pane') PaneElementReference: ElementRef

  private objectClasses$ = new BehaviorSubject<RcvObjectclassInterface[]>([])
  private selectedLabel$ = new BehaviorSubject<ILabel | undefined>(undefined)

  private $s = new Subscription()

  private _selectedObjectClass: RcvObjectclassInterface
  private newElement: AbstractTwoPointSvgElement | undefined
  private get PaneElement(): HTMLDivElement {
    return this.PaneElementReference.nativeElement
  }
  private cursor: Cursor

  document: any // FIXME SVG.Doc
  working = false

  @Input() drawingMode: SvgDrawingMode = SvgDrawingMode.Box
  @Input() selectEditor = false
  @Input()
  set options(value: BoundingBoxEditorOptions) {
    this.elementService.options = value
  }
  @Input()
  set size(value: any) {
    if (
      value.height <= 0 ||
      value.width <= 0 ||
      value.naturalHeight <= 0 ||
      value.naturalWidth <= 0 ||
      !this.document
    ) {
      return
    }
    this.elementService.zoomFactor = value.width / value.naturalWidth
    this.document.viewbox(0, 0, value.naturalWidth, value.naturalHeight)
  }

  @Input()
  set selectedLabel(value: ILabel | undefined) {
    this.selectedLabel$.next(value)
    if (value) {
      this.elementService.select(value.Id, false)
    }
  }
  get selectedLabel(): ILabel | undefined {
    return this.selectedLabel$.getValue()
  }

  @Input()
  set hoveredLabel(value: ILabel) {
    this.elementService.hover(!!value && value.Id, false)
  }

  @Input()
  set labels(value: ILabel[]) {
    this.elementService.setupLabels(value, this.document, this.drawingMode, this.objectClasses)
  }

  @Input()
  set objectClasses(value: RcvObjectclassInterface[]) {
    this.objectClasses$.next(value)
  }
  get objectClasses(): RcvObjectclassInterface[] {
    return this.objectClasses$.getValue()
  }

  @Input()
  set selectedObjectClass(value: RcvObjectclassInterface) {
    this._selectedObjectClass = value
  }
  get selectedObjectClass(): RcvObjectclassInterface {
    return this._selectedObjectClass
  }

  @Output() elementAdded: EventEmitter<ElementData> = new EventEmitter()
  @Output() elementChanged: EventEmitter<ElementData> = new EventEmitter()
  @Output() elementRemoved: EventEmitter<ElementData> = new EventEmitter()
  @Output() elementsChanged: EventEmitter<any[]> = new EventEmitter()
  @Output() hovered: EventEmitter<ILabel> = new EventEmitter()
  @Output() selected: EventEmitter<ILabel> = new EventEmitter()

  constructor(private elementService: ElementService) {}

  ngOnInit() {
    // create document
    this.document = SVG(this.PaneElement).addClass('editor-pane')
    this.cursor = new Cursor(this.document)
    this.document.mousedown((evt: MouseEvent) => this.handleMouseDown(evt))
    this.document.mousemove((evt: MouseEvent) => this.moveCursor(evt))
    this.document.mouseout(() => this.cursor.hide())
    this.document.mouseover(() => this.cursor.show())
    this.document.mouseup((evt: MouseEvent) => this.handleDocumentMouseUp(evt))

    window.addEventListener('mouseup', (evt: MouseEvent) => this.handleMouseUp(evt))
    window.addEventListener('mousemove', (evt: MouseEvent) => this.handleMouseMove(evt))
    this.PaneElement.addEventListener('mouseover', (evt: MouseEvent) => this.handleMouseOver(evt), true)

    this.$s.add(this.elementService.changed.subscribe(e => this.elementChanged.emit(e ? e.getData() : null)))
    this.$s.add(this.elementService.hovered.subscribe(e => this.hovered.emit(e ? e.getData() : null)))
    this.$s.add(this.elementService.selected.subscribe(e => this.selected.emit(e ? e.getData() : null)))
    this.$s.add(this.elementService.working.subscribe(working => (this.working = working)))
    this.$s.add(
      this.elementService.elementsChanged.subscribe(elements => this.elementsChanged.emit(elements)),
    )
  }

  ngOnDestroy() {
    this.$s.unsubscribe()
  }

  clear() {
    this.elementService.clear()
  }

  moveCursor(event: MouseEvent) {
    const p = getRelativePosition(event.clientX, event.clientY, this.document)
    this.cursor.move(p)
  }

  labelOptionsPosition() {
    if (!this.selectedLabel || !this.selectedLabel.isVisible || this.working) {
      return {}
    }

    const documentWidth = this.document.viewbox().width
    const documentHeight = this.document.viewbox().height
    const top = this.selectedLabel.Bottom * this.document.viewbox().zoom + 5
    const threshold = 70
    const overflowBottom = documentHeight * this.document.viewbox().zoom < top + threshold
    const overflowTop = this.selectedLabel.Top <= threshold

    if (overflowBottom) {
      if (overflowTop) {
        return this.labelMiddle(this.selectedLabel, documentWidth, documentHeight)
      }
      return this.labelTop(this.selectedLabel, documentWidth, documentHeight)
    }

    // default
    return this.labelBottom(this.selectedLabel, documentWidth, documentHeight)
  }

  /**
   * starts creating a new element
   */
  protected handleMouseDown(event: MouseEvent): void {
    const p = getRelativePosition(event.clientX, event.clientY, this.document)

    if (this.drawingMode === SvgDrawingMode.Box) {
      const box = this.elementService.createNewBox(this.document, this.drawingMode, p, p)
      box.ObjectClass = this.selectedObjectClass
      box.isSelected = true
      this.newElement = box
    } else {
      this.newElement = this.elementService.createNewLine(this.document, this.drawingMode, p)
    }

    this.working = true
    event.stopPropagation()
    event.preventDefault()
  }

  /**
   * stops creating a new bounding box
   */
  protected handleMouseUp(event: MouseEvent): void {
    if (!!this.newElement) {
      this.endTwoPointElement()
      event.stopPropagation()
    }
  }

  protected handleDocumentMouseUp(event: MouseEvent): void {
    this.selected.emit(undefined)
    this.hovered.emit(undefined)
  }

  protected handleMouseMove(event: MouseEvent): void {
    if (!!this.newElement) {
      const p = getRelativePosition(event.clientX, event.clientY, this.document)
      this.resizeTwoPointElement(p)
      event.stopPropagation()
    }
  }

  protected handleMouseOver(event: MouseEvent) {
    // just catch event
    if (this.working) {
      event.stopPropagation()
    }
  }

  /**
   * resizes new element while creating
   */
  private resizeTwoPointElement(point: IPoint) {
    if (!this.newElement) {
      return
    }
    this.newElement.EndX = Math.min(Math.max(0, point.x), this.document.viewbox().width)
    this.newElement.EndY = Math.min(Math.max(0, point.y), this.document.viewbox().height)
    this.newElement.draw()
  }

  private endTwoPointElement() {
    const element = this.newElement
    this.newElement = undefined
    if (!element) {
      return
    }

    if (element.Width > 0 && element.Height > 0) {
      if (element instanceof Box) {
        if (element.StartX > element.EndX) {
          const x = element.StartX
          element.StartX = element.EndX
          element.EndX = x
        }
        if (element.StartY > element.EndY) {
          const y = element.StartY
          element.StartY = element.EndY
          element.EndY = y
        }
      }
      element.draw()
      // emit the change
      this.elementService.elements.push(element)
      this.elementService.update()
      this.elementAdded.emit(element.getData())
      this.selected.emit(element.getData() as any)
    } else {
      element.destroy()
    }
    this.working = false
  }

  private labelBottom(selected: any, documentWidth: number, documentHeight: number) {
    const zoom = this.document.viewbox().zoom
    const x = this.horizontalAlign(selected, documentWidth, documentHeight)
    const y = (selected.Bottom + 5) * zoom
    return { transform: `translate(${x}px, ${y}px)` }
  }

  private labelMiddle(selected: any, documentWidth: number, documentHeight: number) {
    const zoom = this.document.viewbox().zoom
    const x = this.horizontalAlign(selected, documentWidth, documentHeight)
    const y = selected.Top * zoom - ((selected.Top - selected.Bottom) * zoom + optionsHeight) / 2
    return { transform: `translate(${x}px, ${y}px)` }
  }

  private labelTop(selected: any, documentWidth: number, documentHeight: number) {
    const zoom = this.document.viewbox().zoom
    const x = this.horizontalAlign(selected, documentWidth, documentHeight)
    const y = (selected.Top - 5) * zoom - optionsHeight
    return { transform: `translate(${x}px, ${y}px)` }
  }

  private horizontalAlign(selected: any, documentWidth: number, documentHeight: number): number {
    const zoom = this.document.viewbox().zoom
    if (selected.Right < optionsWidth / zoom) {
      // align left
      return selected.Left * zoom
    }

    return selected.Right * zoom - optionsWidth
  }
}
