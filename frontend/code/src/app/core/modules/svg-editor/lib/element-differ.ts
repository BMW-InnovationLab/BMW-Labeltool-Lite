import { ILabel } from '@domain/object-detection'
import { RcvObjectClassViewInterface } from '@rcv/domain/labeltool-client'
import { AbstractTwoPointSvgElement } from '../components/abstract-two-point-svg-element'
import { Box } from '../components/box'
import { ElementBuilder } from './element-builder'

export class ElementDiffer {
  static diff(
    labels: ILabel[],
    existing: AbstractTwoPointSvgElement[],
    objectClasses: RcvObjectClassViewInterface[],
    createElement: (e: ILabel) => AbstractTwoPointSvgElement,
  ): AbstractTwoPointSvgElement[] {
    const result: AbstractTwoPointSvgElement[] = []
    // add elements
    labels.forEach(label => {
      if (label.isVisible && existing.findIndex(e => e.Id === label.Id) === -1) {
        result.push(createElement(label))
      }
    })
    existing.forEach(e => {
      const newElement = labels.find(label => label.Id === e.Id)
      if (newElement == null || !newElement.isVisible) {
        // remove element
        e.destroy()
      } else {
        // update existing element
        result.push(ElementDiffer.updateElement(objectClasses, e, newElement))
      }
    })

    return result
  }

  private static updateElement(
    objectClasses: RcvObjectClassViewInterface[],
    element: AbstractTwoPointSvgElement,
    label: ILabel,
  ) {
    const objectclass = objectClasses.find(o => o.Id === label.ObjectClassId)
    if (element instanceof Box) {
      const [startPoint, endPoint] = ElementBuilder.pointsFromLabel(label)
      ElementBuilder.setBoxGeometry(element, startPoint, endPoint)
      element.isSelected = label.isSelected

      if (objectclass) {
        element.ObjectClass = objectclass
      }

      element.draw()
    }

    return element
  }
}
