import { Component, Inject, OnDestroy, OnInit } from '@angular/core'
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog'
import { FlatTreeNodeInterface } from '@rcv/common'
import { Subscription } from 'rxjs'

import { FlatTreeModel } from '@common'
import { TopicTreeNode } from '@domain/topic'
import { RcvTopicHierarchyItem, RcvTopicHierarchyService } from '@rcv/domain'
import { RcvTopicViewInterface } from '@rcv/domain/labeltool-client'

export interface DialogParameter {
  selectedTopic: RcvTopicViewInterface
  topics: RcvTopicViewInterface[]
}

@Component({
  selector: 'rcv-topic-select-dialog',
  templateUrl: './topic-select-dialog.component.html',
  styleUrls: ['./topic-select-dialog.component.scss'],
})
export class TopicSelectDialogComponent implements OnInit, OnDestroy {
  topicTree = new FlatTreeModel<RcvTopicViewInterface, any, TopicTreeNode>()

  private $s = new Subscription()

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: DialogParameter,
    private dialogRef: MatDialogRef<TopicSelectDialogComponent>,
    private topicHierarchyService: RcvTopicHierarchyService,
  ) {}

  ngOnInit() {
    this.$s.add(this.topicHierarchyService.TopicHierarchy$.subscribe(topics => this.handleTopics(topics)))
  }

  ngOnDestroy(): void {
    this.$s.unsubscribe()
  }

  isContainer = (_: number, nodeData: FlatTreeNodeInterface<any>) =>
    nodeData.isExpandable && !nodeData.isHandleAsItem

  isItemContainer = (_: number, nodeData: FlatTreeNodeInterface<any>) =>
    nodeData.isExpandable && nodeData.isHandleAsItem

  handleTopics(topics: RcvTopicHierarchyItem[]) {
    this.topicTree.updateData(topics)

    const nodes = this.topicTree.Control.dataNodes
    let topicIndex = this.getTopicIndex(this.data.selectedTopic.Id, nodes)

    do {
      const [parent, index] = this.findParent(topicIndex, nodes)
      topicIndex = index
      if (parent) {
        this.topicTree.Control.expand(parent)
      }
    } while (topicIndex)

    this.expandAllVirtualTopicNodes()
  }

  topicActive(node: TopicTreeNode, selectedTopic: RcvTopicViewInterface): boolean {
    if (!selectedTopic || !node.model) {
      return false
    }

    return node.model.Id === selectedTopic.Id
  }

  selectTopic(node: TopicTreeNode) {
    this.dialogRef.close(node.model)
  }

  selectContainer() {
    this.expandAllVirtualTopicNodes()
  }

  private getTopicIndex(topicId: number | null, flatTree: TopicTreeNode[]) {
    if (topicId == null) {
      return null
    }

    return flatTree.findIndex(node => {
      if (!node.model) {
        return false
      }

      return node.model.Id === topicId
    })
  }

  private findParent(
    topicIndex: number | null,
    flatTree: TopicTreeNode[],
  ): [TopicTreeNode | null, number | null] {
    const topicNode = flatTree[topicIndex]

    for (let i = topicIndex; i >= 0; --i) {
      const node = flatTree[i]
      if (node.level === topicNode.level - 1) {
        return [node, i]
      }
    }

    return [null, null]
  }

  private expandAllVirtualTopicNodes() {
    const control = this.topicTree.Control
    const virtualTopicNodes = control.dataNodes.filter(value => value.isExpandable && value.isHandleAsItem)
    for (const node of virtualTopicNodes) {
      control.expand(node)
    }
  }
}
