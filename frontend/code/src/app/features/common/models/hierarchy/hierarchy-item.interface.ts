export interface HierarchyItem<TItem> {
  item: TItem
  isHandleAsItem: boolean
  readonly name: string
  children: HierarchyItem<TItem>[]
}
