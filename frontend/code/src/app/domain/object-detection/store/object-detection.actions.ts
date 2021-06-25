import {
  RcvImageLabelInterface,
  RcvLabelInterface,
  RcvTopicViewInterface,
} from '@rcv/domain/labeltool-client'
import { RcvLabelViewInterface } from '@rcv/domain/labeltool-client/model/label-view.interface'
import { ElementData } from '@svg-editor'
import { ILabel } from '../models'

export class AddLabel {
  static readonly type = '[OBJECT-DETECTION] Add Label'
  constructor(public label: ILabel) {}
}

export class AddLabelFromSuggestion {
  static readonly type = '[OBJECT-DETECTION] AddLabelFromSuggestion'
  constructor(public label: RcvLabelViewInterface) {}
}

export class RemoveLabel {
  static readonly type = '[OBJECT-DETECTION] Remove Label'
  constructor(public element: ElementData | ILabel) {}
}

export class ChangeLabel {
  static readonly type = '[OBJECT-DETECTION] Change Label'
  constructor(public element: ElementData | ILabel) {}
}

export class ChangeLabels {
  static readonly type = '[OBJECT-DETECTION] Change Labels'
  constructor(public labels: RcvLabelInterface[]) {}
}

export class ChangeLabelsFromSave {
  static readonly type = '[OBJECT-DETECTION] ChangeLabelsFromSave'
  constructor(public labels: ILabel[]) {}
}

export class CopyLabel {
  static readonly type = '[OBJECT-DETECTION] Copy Label'
  constructor(public label: ILabel) {}
}

export class HoverLabel {
  static readonly type = '[OBJECT-DETECTION] Hover Label'
  constructor(public label: ILabel) {}
}

export class RestoreLabels {
  static readonly type = '[OBJECT-DETECTION] Restore Labels'
  constructor(public topic: RcvTopicViewInterface, public image: RcvImageLabelInterface) {}
}

export class SelectLabel {
  static readonly type = '[OBJECT-DETECTION] Select Label'
  constructor(public label?: ILabel) {}
}

export class SetFill {
  static readonly type = '[OBJECT-DETECTION] Set Fill'
  constructor(public fill: boolean) {}
}

export class SetStrokeWidth {
  static readonly type = '[OBJECT-DETECTION] Set Stroke Width'
  constructor(public width: number) {}
}

export class ShowAllLabels {
  static readonly type = '[LABEL SIDENAV] ShowAllLabels'
}

export class ToggleFill {
  static readonly type = '[OBJECT-DETECTION] Toggle Fill'
  constructor() {}
}

export class ToggleLabelVisibility {
  static readonly type = '[OBJECT-DETECTION] Toggle Label Visibility'
  constructor(public label: ILabel) {}
}

export class ToggleVisibility {
  static readonly type = '[OBJECT-DETECTION] Toggle Visibility'
}
