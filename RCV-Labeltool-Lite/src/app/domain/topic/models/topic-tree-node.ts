import { FlatTreeNodeInterface } from '@common'
import { RcvTopicInterface } from '@rcv/domain'

export class TopicTreeNode implements FlatTreeNodeInterface<RcvTopicInterface> {
  private _title: string
  isExpandable: boolean
  level: number
  model: RcvTopicInterface

  get title(): string {
    return this.model != null ? this.model.Name : this._title
  }

  set title(value: string) {
    this._title = value
  }
}
