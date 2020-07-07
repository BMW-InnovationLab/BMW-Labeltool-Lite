export interface RcvSegmentInterface {
  Id?: string
  ObjectClassId: string
  ObjectClassName: string

  SourceModel: string
  DataImage: string

  Name: string

  recolorize?: boolean

  isActive?: boolean
  isNew?: boolean
  isSelected?: boolean
  isVisible?: boolean
}
