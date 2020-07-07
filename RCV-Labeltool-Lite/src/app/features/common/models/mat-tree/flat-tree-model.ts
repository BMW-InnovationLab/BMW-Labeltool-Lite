import { FlatTreeControl } from '@angular/cdk/tree'
import { MatTreeFlatDataSource, MatTreeFlattener } from '@angular/material'
import { Observable, of } from 'rxjs'

import { HierarchyItem } from '@common'
import { FlatTreeNodeInterface } from './flat-tree-node.interface'

export class FlatTreeModel<
  TModel,
  TNestedNode extends HierarchyItem<TModel>,
  TFlatNode extends FlatTreeNodeInterface<TModel>
> {
  private readonly _flattener: MatTreeFlattener<TNestedNode, TFlatNode>
  private readonly _control: FlatTreeControl<TFlatNode>
  private readonly _dataSource: MatTreeFlatDataSource<TNestedNode, TFlatNode>

  constructor() {
    this._flattener = new MatTreeFlattener<TNestedNode, TFlatNode>(
      this.transform,
      this.getLevel,
      this.isExpanded,
      this.getChildren,
    )
    this._control = new FlatTreeControl<TFlatNode>(this.getLevel, this.isExpanded)
    this._dataSource = new MatTreeFlatDataSource<TNestedNode, TFlatNode>(this._control, this._flattener)
  }

  updateData(newData: TNestedNode[]) {
    this._dataSource.data = newData
  }

  get Control(): FlatTreeControl<TFlatNode> {
    return this._control
  }

  get DataSource(): MatTreeFlatDataSource<TNestedNode, TFlatNode> {
    return this._dataSource
  }

  private transform(nestedNode: TNestedNode, level: number): TFlatNode {
    return <TFlatNode>{
      model: nestedNode.item,
      isExpandable: nestedNode.children.length > 0,
      level: level,
      title: nestedNode.name,
    }
  }

  private isExpanded(node: TFlatNode): boolean {
    return node.isExpandable
  }

  private getLevel(node: TFlatNode): number {
    return node.level
  }

  private getChildren(node: TNestedNode): Observable<TNestedNode[]> {
    return of(node.children as TNestedNode[])
  }
}
