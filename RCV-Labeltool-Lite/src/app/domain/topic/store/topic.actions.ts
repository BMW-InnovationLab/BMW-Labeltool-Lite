import { RcvObjectclassInterface, RcvTopicInterface } from '@rcv/domain'

export class LoadTopics {
  static readonly type = '[TOPIC] Load Topics'
  constructor(public initialTopicId?: number) {}
}

export class SelectTopic {
  static readonly type = '[TOPIC] Select Topic'
  constructor(public topic: RcvTopicInterface) {}
}

export class LoadObjectclasses {
  static readonly type = '[TOPIC] Load Objectclasses'
  constructor(public topic: RcvTopicInterface) {}
}

export class SelectObjectclass {
  static readonly type = '[TOPIC] Select Objectclass'
  constructor(public objectclass: RcvObjectclassInterface) {}
}
