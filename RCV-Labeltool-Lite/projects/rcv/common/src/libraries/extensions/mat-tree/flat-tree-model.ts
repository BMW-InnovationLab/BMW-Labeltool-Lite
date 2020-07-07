import { FlatTreeControl } from '@angular/cdk/tree'
import { MatTreeFlatDataSource, MatTreeFlattener } from '@angular/material'
import { from, Observable } from 'rxjs'

import { HierarchyItemInterface } from '../../hierarchy'
import { FlatTreeNodeInterface } from './flat-tree-node.interface'

export class FlatTreeModel<
  TModel,
  TNestedNode extends HierarchyItemInterface<TModel>,
  TFlatNode extends FlatTreeNodeInterface<TModel>
> {
  private readonly _flattener: MatTreeFlattener<TNestedNode, TFlatNode>
  private readonly _control: FlatTreeControl<TFlatNode>
  private readonly _dataSource: MatTreeFlatDataSource<TNestedNode, TFlatNode>

  constructor() {
    this._flattener = new MatTreeFlattener<TNestedNode, TFlatNode>(
      this.transform,
      this.getLevel,
      this.isExpandable,
      this.getChildren,
    )
    this._control = new FlatTreeControl<TFlatNode>(this.getLevel, this.isExpandable)
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

  expand(path: string) {
    if (path == null || path === '') {
      return
    }
    const segments = path.split('/')
    const flatNodes = this._control.dataNodes.filter(value =>
      segments.find(segment => value.title === segment),
    )

    for (const node of flatNodes) {
      this._control.expand(node)
    }
  }

  private transform(nestedNode: TNestedNode, level: number): TFlatNode {
    return <TFlatNode>{
      model: nestedNode.item,
      isExpandable: nestedNode.children.length > 0,
      level: level,
      title: nestedNode.name,
    }
  }

  private isExpandable(node: TFlatNode): boolean {
    return node.isExpandable
  }

  private getLevel(node: TFlatNode): number {
    return node.level
  }

  private getChildren(node: TNestedNode): Observable<TNestedNode[]> {
    return from([node.children.map(value => <TNestedNode>value)])
  }
}
