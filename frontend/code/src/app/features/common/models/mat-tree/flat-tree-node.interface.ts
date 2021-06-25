export interface FlatTreeNodeInterface<TModel> {
  level: number
  isHandleAsItem: boolean
  isExpandable: boolean
  model: TModel
  title: string
}
