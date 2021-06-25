import { FlatTreeNodeInterface } from '@common'
import { RcvTopicViewInterface } from '@rcv/domain/labeltool-client'

export class TopicTreeNode implements FlatTreeNodeInterface<RcvTopicViewInterface> {
  private _title: string
  isHandleAsItem: boolean
  isExpandable: boolean
  level: number
  model: RcvTopicViewInterface

  get title(): string {
    return this.model != null ? this.model.Name : this._title
  }

  set title(value: string) {
    this._title = value
  }
}
