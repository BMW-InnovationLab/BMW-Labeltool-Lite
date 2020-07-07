export interface HierarchyItemInterface<TItem> {
  item?: TItem
  readonly name: string
  children: HierarchyItemInterface<TItem>[]
}
