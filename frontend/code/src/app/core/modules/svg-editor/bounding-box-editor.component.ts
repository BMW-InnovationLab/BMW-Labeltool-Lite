import {
  ChangeDetectionStrategy,
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

import { ISize } from '@core/models/ISize'
import { ILabel } from '@domain/object-detection'
import { RcvObjectClassViewInterface } from '@rcv/domain/labeltool-client'
import { BehaviorSubject, combineLatest, EMPTY, Observable, Subscription } from 'rxjs'
import { delay, map } from 'rxjs/operators'
import { AbstractTwoPointSvgElement } from './components/abstract-two-point-svg-element'
import { Box } from './components/box'
import { Cursor } from './components/cursor'
import { ElementService } from './lib/element.service'
import {
  LabelOptionsPosition,
  labelOptionsPosition,
  LabelOptionsSettings,
} from './lib/label-options-position'
import { getRelativePosition } from './lib/util'
import { BoundingBoxEditorOptions } from './models/bounding-box-editor-options.interface'
import { ElementData } from './models/element-data.interface'
import { IPoint } from './models/point.interface'
import { SvgDrawingMode } from './models/svg-drawing-mode.enum'

declare const SVG: any

const optionsOffset = 10

@Component({
  selector: 'rcv-bounding-box-editor',
  templateUrl: './bounding-box-editor.component.html',
  styleUrls: ['./bounding-box-editor.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  encapsulation: ViewEncapsulation.None,
})
export class BoundingBoxEditorComponent implements OnInit, OnDestroy {
  @ViewChild('pane', { static: true }) PaneElementReference: ElementRef
  @ViewChild('labelOptions', { static: false, read: ElementRef }) labelOptions!: ElementRef

  private objectClasses$ = new BehaviorSubject<RcvObjectClassViewInterface[]>([])

  private $s = new Subscription()

  private _selectedObjectClass: RcvObjectClassViewInterface
  private newElement: AbstractTwoPointSvgElement | undefined
  private get PaneElement(): HTMLDivElement {
    return this.PaneElementReference.nativeElement
  }
  private selectedLabel$ = new BehaviorSubject<ILabel | undefined>(undefined)
  private cursor: Cursor
  private currentLabels: ILabel[] = []

  document: any // FIXME SVG.Doc
  hideLabelOptions$: Observable<boolean> = EMPTY
  labelOptionsPosition$: Observable<LabelOptionsPosition> = EMPTY
  working$ = new BehaviorSubject<boolean>(false)

  @Input() drawingMode: SvgDrawingMode = SvgDrawingMode.Box
  @Input() selectEditor = true
  @Input()
  set options(value: BoundingBoxEditorOptions) {
    this.elementService.options = value
  }
  @Input()
  set size(value: ISize) {
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
    // on size change reload labels
    this.elementService.clear()
    this.elementService.setupLabels(this.currentLabels, this.document, this.drawingMode, this.objectClasses)
    if (this.cursor) {
      this.cursor.scale(this.document.viewbox().zoom)
    }
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
    this.currentLabels = [...value]
    this.elementService.setupLabels(value, this.document, this.drawingMode, this.objectClasses)
  }

  @Input()
  set objectClasses(value: RcvObjectClassViewInterface[]) {
    this.objectClasses$.next(value)
  }
  get objectClasses(): RcvObjectClassViewInterface[] {
    return this.objectClasses$.getValue()
  }

  @Input()
  set selectedObjectClass(value: RcvObjectClassViewInterface) {
    this._selectedObjectClass = value
  }
  get selectedObjectClass(): RcvObjectClassViewInterface {
    return this._selectedObjectClass
  }

  @Input() spatialDrawing = false

  @Output() elementAdded: EventEmitter<ElementData> = new EventEmitter()
  @Output() elementChanged: EventEmitter<ElementData> = new EventEmitter()
  @Output() elementRemoved: EventEmitter<ElementData> = new EventEmitter()
  @Output() elementsChanged: EventEmitter<any[]> = new EventEmitter()
  @Output() hovered: EventEmitter<ILabel> = new EventEmitter()
  @Output() selected: EventEmitter<ILabel> = new EventEmitter()

  get optionsWidth(): number {
    if (this.labelOptions && this.labelOptions.nativeElement) {
      return this.labelOptions.nativeElement.offsetWidth
    }
  }

  get optionsHeight(): number {
    if (this.labelOptions && this.labelOptions.nativeElement) {
      return this.labelOptions.nativeElement.offsetHeight
    }
  }

  constructor(private elementService: ElementService) {}

  handleKeydown = (event: KeyboardEvent) => {
    const selected = this.elementService.selectedElements

    if (!selected.length) {
      return
    }

    event.stopPropagation()

    switch (event.key) {
      case 'Delete': {
        // delete selected element
        selected.forEach(s => {
          this.elementRemoved.emit(s.getData())
          s.destroy()
        })

        this.elementService.elements = this.elementService.elements.filter(e => !e.isSelected)
        this.selectedLabel = undefined
        this.elementService.update()
        break
      }
      case 'ArrowDown': {
        selected.forEach(s => s.moveDown())
        break
      }
      case 'ArrowLeft': {
        selected.forEach(s => s.moveLeft())
        break
      }
      case 'ArrowRight': {
        selected.forEach(s => s.moveRight())
        break
      }
      case 'ArrowUp': {
        selected.forEach(s => s.moveUp())
        break
      }
    }
  }

  ngOnInit() {
    // create document
    this.document = SVG(this.PaneElement)
    this.cursor = new Cursor(this.document)

    window.addEventListener('mousemove', this)
    window.addEventListener('mouseup', this)
    window.addEventListener('touchend', this)
    window.addEventListener('touchmove', this)

    this.document.mousedown((evt: MouseEvent) => this.createElement(evt))
    this.document.mousemove((evt: MouseEvent) => this.moveCursor(evt))
    this.document.mouseout(() => this.cursor.hide())
    this.document.mouseover(() => this.cursor.show())
    this.document.touchstart((evt: TouchEvent) => this.createElement(evt))

    document.addEventListener('keydown', this.handleKeydown)
    this.PaneElement.addEventListener('mouseover', this.handleMouseOver, true)

    this.$s.add(this.elementService.changed.subscribe(e => this.elementChanged.emit(e ? e.getData() : null)))
    this.$s.add(this.elementService.hovered.subscribe(e => this.hovered.emit(e ? e.getData() : null)))
    this.$s.add(this.elementService.selected.subscribe(e => this.selected.emit(e ? e.getData() : null)))
    this.$s.add(this.elementService.working.subscribe(working => this.working$.next(working)))
    this.$s.add(
      this.elementService.elementsChanged.subscribe(elements => this.elementsChanged.emit(elements)),
    )

    this.hideLabelOptions$ = combineLatest([this.selectedLabel$, this.working$]).pipe(
      map(([selectedLabel, working]) => !selectedLabel || !selectedLabel.isVisible || working),
    )

    this.labelOptionsPosition$ = this.hideLabelOptions$.pipe(
      delay(0), // add a little delay to get the size of the LabelOptions menu inside labelOptionsPosition()
      map(isHidden => this.labelOptionsPosition(isHidden)),
    )
  }

  ngOnDestroy() {
    window.removeEventListener('mousemove', this)
    window.removeEventListener('mouseup', this)
    window.removeEventListener('touchend', this)
    window.removeEventListener('touchmove', this)

    if (this.document != null) {
      this.document.mousedown(null)
      this.document.mousemove(null)
      this.document.mouseout(null)
      this.document.mouseover(null)
      this.document.touchstart(null)
    }

    document.removeEventListener('keydown', this.handleKeydown)
    this.PaneElement.removeEventListener('mouseover', this.handleMouseOver, true)
    this.$s.unsubscribe()
  }

  /**
   * Handles events registered by addEventListener
   *
   * @param event
   */
  handleEvent(event: MouseEvent | TouchEvent) {
    switch (event.type) {
      case 'mousemove':
      case 'touchmove':
        return this.resizeElement(event)
      case 'mouseup':
      case 'touchend':
        return this.completeElement()
    }
  }

  clear() {
    this.elementService.clear()
  }

  moveCursor(event: MouseEvent) {
    const p = getRelativePosition(event.clientX, event.clientY, this.document)
    this.cursor.move(p)
  }

  labelOptionsPosition(isHidden: boolean) {
    if (isHidden) {
      return { transform: `translate(-10000px, -10000px)` }
    }

    const settings: LabelOptionsSettings = {
      documentWidth: this.document.viewbox().width,
      documentHeight: this.document.viewbox().height,
      documentZoom: this.document.viewbox().zoom,
      optionsOffset: optionsOffset,
      optionsHeight: this.optionsHeight,
      optionsWidth: this.optionsWidth,
      label: this.selectedLabel,
    }
    return labelOptionsPosition(settings)
  }

  createElement(event: MouseEvent | TouchEvent): boolean {
    const p = this.contactPoint(event)
    // deselect all elements on click
    this.elementService.select(-1)

    if (
      this.document == null ||
      this.working$.getValue() ||
      !p ||
      (event as any).target.classList.contains('svg_select_points')
    ) {
      return false
    }

    if (this.drawingMode === SvgDrawingMode.Box) {
      const box = this.elementService.createNewBox(this.document, this.drawingMode, p, p)
      box.ObjectClass = this.selectedObjectClass
      box.isSelected = true
      this.newElement = box
    } else {
      this.newElement = this.elementService.createNewLine(this.document, this.drawingMode, p)
    }

    this.working$.next(true)
    event.preventDefault()

    return true
  }

  resizeElement(event: MouseEvent | TouchEvent) {
    const p = this.contactPoint(event)

    if (!!this.newElement && !!p) {
      this.resizeTwoPointElement(p)
    }
  }

  completeElement() {
    if (!!this.newElement) {
      this.endTwoPointElement()
    }
  }

  private handleMouseOver = (event: MouseEvent) => {
    // just catch event
    if (this.working$.getValue()) {
      event.stopPropagation()
    }
  }

  private contactPoint(event: MouseEvent | TouchEvent): IPoint | false {
    if (this.document == null) {
      return false
    }

    if ('touches' in event) {
      if (event.touches.length !== 1) {
        return false
      }

      const touch = event.touches[0]
      // special handling for apple pencil
      if (touch.touchType !== 'stylus') {
        return false
      }

      return getRelativePosition(touch.clientX, touch.clientY, this.document)
    } else {
      return getRelativePosition(event.clientX, event.clientY, this.document)
    }
  }

  /**
   * resizes new element while creating
   */
  private resizeTwoPointElement(point: IPoint) {
    if (!this.newElement) {
      return
    }
    this.newElement.End = {
      x: Math.min(Math.max(0, point.x), this.document.viewbox().width),
      y: Math.min(Math.max(0, point.y), this.document.viewbox().height),
    }
    this.newElement.draw()
  }

  private endTwoPointElement() {
    const element = this.newElement
    this.newElement = undefined
    if (!element) {
      return
    }

    if (element.isValid()) {
      if (element instanceof Box) {
        if (element.Start.x > element.End.x) {
          const x = element.Start.x
          element.Start = { x: element.End.x, y: element.Start.y }
          element.End = { x: x, y: element.End.y }
        }
        if (element.Start.y > element.End.y) {
          const y = element.Start.y
          element.Start = { x: element.Start.x, y: element.End.y }
          element.End = { x: element.End.x, y: y }
        }
        if (this.spatialDrawing) {
          element.createBackElement()
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
    this.working$.next(false)
  }
}
