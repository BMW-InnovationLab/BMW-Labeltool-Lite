import { HierarchyItem } from '@common'
import { RcvTopicInterface } from '@rcv/domain'

export class TopicHierarchyItem implements HierarchyItem<RcvTopicInterface> {
  private _name: string

  item: RcvTopicInterface
  children: TopicHierarchyItem[] = []

  constructor(item?: RcvTopicInterface) {
    if (item) {
      this.item = item
    }
  }

  get name(): string {
    return this.item == null ? this._name : this.item.Name
  }

  set name(value: string) {
    this._name = value
  }
}
