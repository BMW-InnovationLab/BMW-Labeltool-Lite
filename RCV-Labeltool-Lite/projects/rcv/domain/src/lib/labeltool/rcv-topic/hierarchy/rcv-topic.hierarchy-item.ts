import { HierarchyItemInterface } from '@rcv/common'
import { RcvTopicInterface } from '../rcv-topic.interface'

export class RcvTopicHierarchyItem implements HierarchyItemInterface<RcvTopicInterface> {
  private _name: string
  item?: RcvTopicInterface
  children: RcvTopicHierarchyItem[] = []

  constructor(item?: RcvTopicInterface) {
    this.item = item
  }

  get name(): string {
    return this.item == null ? this._name : this.item.Name
  }

  set name(value: string) {
    this._name = value
  }
}
