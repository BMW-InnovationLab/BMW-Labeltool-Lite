import { RcvObjectclassInterface, RcvTopicInterface } from '@rcv/domain'

export interface TopicStateModel {
  objectclasses: RcvObjectclassInterface[]
  selectedTopic?: RcvTopicInterface
  selectedObjectclass?: RcvObjectclassInterface
  topics: RcvTopicInterface[]
}
