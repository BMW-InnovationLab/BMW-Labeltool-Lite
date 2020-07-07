export interface FlatTreeNodeInterface<TModel> {
  level: number
  isExpandable: boolean
  model: TModel
  title: string
}
