import { RcvObjectClassViewInterface, RcvTopicViewInterface } from '@rcv/domain/labeltool-client'

export class LoadTopics {
  static readonly type = '[TOPIC] Load Topics'
  constructor(public initialTopicId?: number) {}
}

export class SelectTopic {
  static readonly type = '[TOPIC] Select Topic'
  constructor(public topic: RcvTopicViewInterface) {}
}

export class LoadObjectclasses {
  static readonly type = '[TOPIC] Load Objectclasses'
  constructor(public topic: RcvTopicViewInterface) {}
}

export class SelectObjectclassFromLabelPage {
  static readonly type = '[TOPIC] Select Objectclass From Label Page'
  constructor(public objectclass: RcvObjectClassViewInterface) {}
}

export class SelectObjectclassFromObjectclassLoad {
  static readonly type = '[TOPIC] Select Objectclass From Objectclass Load'
  constructor(public objectclass: RcvObjectClassViewInterface) {}
}

export class SetChanges {
  static readonly type = '[TOPIC] HasChanges'
  constructor(public hasChanges = true) {}
}
