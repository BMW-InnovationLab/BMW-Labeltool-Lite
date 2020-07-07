import { Injectable } from '@angular/core'

import { RcvTopicInterface } from '@rcv/domain'
import { TopicHierarchyItem } from '../models/topic-hierarchy-item'
import { TopicDomainModule } from '../topic-domain.module'

@Injectable({
  providedIn: TopicDomainModule,
})
export class TopicHierarchyService {
  constructor() {}

  buildFlatTree(topics: RcvTopicInterface[]): TopicHierarchyItem[] {
    const items: TopicHierarchyItem[] = []
    topics = [...topics].sort((a, b) => {
      const aPath = a.Path || ''
      const bPath = b.Path || ''

      return aPath.localeCompare(bPath) * -1
    })

    for (const topic of topics) {
      const segments = topic.Path ? topic.Path.split('/') : []
      const item = new TopicHierarchyItem(topic)
      let targetList = items
      for (const segment of segments) {
        let layer = targetList.find(i => i.name === segment)
        if (layer == null) {
          const newLayer = new TopicHierarchyItem()
          newLayer.name = segment
          layer = newLayer
          targetList.push(layer)
        }
        targetList = layer.children
      }
      targetList.push(item)
      targetList.sort((a, b) => this.compare(a, b))
    }
    items.sort((a, b) => this.compare(a, b))

    return items
  }

  private compare(a: TopicHierarchyItem, b: TopicHierarchyItem): number {
    if (a.children.length > 0 && b.children.length === 0) {
      return -1
    } else if (a.children.length === 0 && b.children.length > 0) {
      return 1
    } else {
      return a.name.localeCompare(b.name)
    }
  }
}
