import { RcvImageInterface, RcvTopicInterface } from '@rcv/domain'
import { ElementData } from '@svg-editor'
import { ILabel } from '../models'

export class AddLabel {
  static readonly type = '[OBJECT-DETECTION] Add Label'
  constructor(public label: ILabel) {}
}

export class RemoveLabel {
  static readonly type = '[OBJECT-DETECTION] Remove Label'
  constructor(public element: ElementData | ILabel) {}
}

export class ChangeLabel {
  static readonly type = '[OBJECT-DETECTION] Change Label'
  constructor(public element: ElementData) {}
}

export class ChangeLabels {
  static readonly type = '[OBJECT-DETECTION] Change Labels'
  constructor(public labels: ILabel[]) {}
}

export class HoverLabel {
  static readonly type = '[OBJECT-DETECTION] Hover Label'
  constructor(public label: ILabel) {}
}

export class RestoreLabels {
  static readonly type = '[OBJECT-DETECTION] Restore Labels'
  constructor(public topic: RcvTopicInterface, public image: RcvImageInterface) {}
}

export class SelectLabel {
  static readonly type = '[OBJECT-DETECTION] Select Label'
  constructor(public label?: ILabel) {}
}

export class ShowAllLabels {
  static readonly type = '[LABEL SIDENAV] ShowAllLabels'
}

export class ToggleLabelVisibility {
  static readonly type = '[OBJECT-DETECTION] Toggle Label Visibility'
  constructor(public label: ILabel) {}
}

export class ToggleVisibility {
  static readonly type = '[OBJECT-DETECTION] Toggle Visibility'
}
