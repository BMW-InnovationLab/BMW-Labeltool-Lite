import { ILabel } from '@domain/object-detection'
import { AbstractTwoPointSvgElement } from '../components/abstract-two-point-svg-element'

export class ElementDiffer {
  static diff(
    labels: ILabel[],
    existing: AbstractTwoPointSvgElement[],
    createElement: (e: ILabel) => AbstractTwoPointSvgElement,
    updateElement: (e: AbstractTwoPointSvgElement, l: ILabel) => AbstractTwoPointSvgElement,
  ): AbstractTwoPointSvgElement[] {
    const result: AbstractTwoPointSvgElement[] = []
    // add elements
    labels.forEach(label => {
      if (label.isVisible && existing.findIndex(e => e.Id === label.Id) === -1) {
        result.push(createElement(label))
      }
    })
    // remove elements
    existing.forEach(e => {
      const newElement = labels.find(label => label.Id === e.Id)
      if (newElement == null || !newElement.isVisible) {
        e.destroy()
      } else {
        result.push(updateElement(e, newElement))
      }
    })

    return result
  }
}
