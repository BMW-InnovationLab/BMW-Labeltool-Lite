export interface HierarchyItem<TItem> {
  item: TItem
  readonly name: string
  children: HierarchyItem<TItem>[]
}
