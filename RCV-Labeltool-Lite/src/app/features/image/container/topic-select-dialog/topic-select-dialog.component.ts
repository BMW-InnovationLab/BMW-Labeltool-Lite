import { Component, Inject, OnInit } from '@angular/core'
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material'

import { FlatTreeModel } from '@common'
import { TopicHierarchyItem, TopicHierarchyService, TopicTreeNode } from '@domain/topic'
import { RcvTopicInterface } from '@rcv/domain'

export interface DialogParameter {
  selectedTopic: RcvTopicInterface
  topics: RcvTopicInterface[]
}

@Component({
  selector: 'rcv-topic-select-dialog',
  templateUrl: './topic-select-dialog.component.html',
  styleUrls: ['./topic-select-dialog.component.scss'],
})
export class TopicSelectDialogComponent implements OnInit {
  topicTree = new FlatTreeModel<RcvTopicInterface, TopicHierarchyItem, TopicTreeNode>()

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: DialogParameter,
    private dialogRef: MatDialogRef<TopicSelectDialogComponent>,
    private topicHierarchyService: TopicHierarchyService,
  ) {}

  ngOnInit() {
    this.handleTopics(this.data.topics)
  }

  hasNestedChild = (_: number, nodeData: TopicTreeNode) => nodeData.isExpandable

  handleTopics(topics: RcvTopicInterface[]) {
    const flatTree = this.topicHierarchyService.buildFlatTree(topics)
    this.topicTree.updateData(flatTree)

    const parent = this.findParent(this.data.selectedTopic, this.topicTree.Control.dataNodes)
    if (parent) {
      this.topicTree.Control.expand(parent)
    }
  }

  findParent(topic: RcvTopicInterface, flatTree: TopicTreeNode[]): TopicTreeNode | null {
    const topicIndex = flatTree.findIndex(node => {
      if (!node.model) {
        return false
      }

      return node.model.Id === topic.Id
    })
    const topicNode = flatTree[topicIndex]

    for (let i = topicIndex; i >= 0; --i) {
      const node = flatTree[i]
      if (node.level === topicNode.level - 1) {
        return node
      }
    }

    return null
  }

  topicActive(node: TopicTreeNode, selectedTopic: RcvTopicInterface): boolean {
    if (!selectedTopic || !node.model) {
      return false
    }

    return node.model.Id === selectedTopic.Id
  }

  selectTopic(node: TopicTreeNode) {
    this.dialogRef.close(node.model)
  }
}
