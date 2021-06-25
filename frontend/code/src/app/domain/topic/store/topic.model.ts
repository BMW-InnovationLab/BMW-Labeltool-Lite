import { RcvObjectClassViewInterface, RcvTopicViewInterface } from '@rcv/domain/labeltool-client'

export interface TopicStateModel {
  objectclasses: RcvObjectClassViewInterface[]
  selectedTopic?: RcvTopicViewInterface
  selectedObjectclass?: RcvObjectClassViewInterface
  topics: RcvTopicViewInterface[]
  hasChanges: Boolean
}
