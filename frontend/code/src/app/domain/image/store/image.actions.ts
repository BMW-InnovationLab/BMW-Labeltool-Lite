import { RcvLabelMode } from '@rcv/domain'
import {
  RcvImageLabelInterface,
  RcvImageNavigationResultViewInterface,
  RcvTopicViewInterface,
} from '@rcv/domain/labeltool-client'

export class ImageReady {
  static readonly type = '[Image] Image Ready'
  constructor(public ready: boolean) {}
}

export class LoadBlank {
  static readonly type = '[Image] Load Blank'
  constructor(public topic: RcvTopicViewInterface, public index: number) {}
}

export class LoadInitialImage {
  static readonly type = '[Image] Load Initial Image'
  constructor(public topic: RcvTopicViewInterface) {}
}

export class LoadImage {
  static readonly type = '[Image] Load Image'
  constructor(public topic: RcvTopicViewInterface, public index: number) {}
}

export class LoadImageAfterDelete {
  static readonly type = '[Image] Load Image After Delete'
  constructor(public topic: RcvTopicViewInterface, public index: number) {}
}

export class LoadImageFromThumbnail {
  static readonly type = '[Image] Load Image From Thumbnail'
  constructor(public topic: RcvTopicViewInterface, public index: number) {}
}

export class RemoveImage {
  static readonly type = '[Image] Remove Image'
  constructor(public topic: RcvTopicViewInterface, public image: RcvImageLabelInterface) {}
}

export class SelectMode {
  static readonly type = '[Image] Select Mode'
  constructor(public mode: RcvLabelMode) {}
}

export class SetImageResponse {
  static readonly type = '[Image] Set Image Response'
  constructor(
    public topic?: RcvTopicViewInterface,
    public response?: RcvImageNavigationResultViewInterface,
    public loadThumbnails = true,
  ) {}
}

export class SetImageSize {
  static readonly type = '[Image] Set Image Size'
  constructor(public size: [number, number]) {}
}

export class SetDefaultImageSize {
  static readonly type = '[Image] Set Default Image Size (100% Zoom)'
  constructor(public size: [number, number]) {}
}

export class SetZoom {
  static readonly type = '[Image] Set Zoom'
  constructor(public zoom: number) {}
}

export class SetBrightness {
  static readonly type = '[Image] Set Brightness'
  constructor(public brightness: number) {}
}
