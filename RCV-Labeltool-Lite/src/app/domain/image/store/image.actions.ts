import { RcvImageResponseInterface, RcvLabelMode, RcvTopicInterface } from '@rcv/domain'

export class ImageReady {
  static readonly type = '[Image] Image Ready'
  constructor(public ready: boolean) {}
}

export class LoadBlank {
  static readonly type = '[Image] Load Blank'
  constructor(public topic: RcvTopicInterface, public index: number) {}
}

export class LoadInitialImage {
  static readonly type = '[Image] Load Initial Image'
  constructor(public topic: RcvTopicInterface) {}
}

export class LoadImage {
  static readonly type = '[Image] Load Image'
  constructor(public topic: RcvTopicInterface, public index: number) {}
}

export class SelectMode {
  static readonly type = '[Image] Select Mode'
  constructor(public mode: RcvLabelMode) {}
}

export class SetImageResponse {
  static readonly type = '[Image] Set Image Response'
  constructor(public response?: RcvImageResponseInterface) {}
}

export class SetImageSize {
  static readonly type = '[Image] Set Image Size'
  constructor(public size: [number, number]) {}
}
