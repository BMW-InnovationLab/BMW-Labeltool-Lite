import { Injectable } from '@angular/core'
import { Observable } from 'rxjs'
import { map } from 'rxjs/operators'

import { HierarchyItemInterface } from '@rcv/common'
import { RcvTopicModule } from '../rcv-topic.module'
import { AbstractRcvTopicRepository } from '../repository/abstract.rcv-topic.repository'
import { RcvTopicHierarchyItem } from './rcv-topic.hierarchy-item'

@Injectable({ providedIn: RcvTopicModule })
export class RcvTopicHierarchyService {
  constructor(private repo: AbstractRcvTopicRepository) {}

  get TopicHierarchy$(): Observable<RcvTopicHierarchyItem[]> {
    return this.repo.Topics$.pipe(
      map(topics => {
        const items: RcvTopicHierarchyItem[] = []
        topics = topics.sort((a, b) => {
          a.Path = a.Path || ''
          b.Path = b.Path || ''

          return a.Path.localeCompare(b.Path) * -1
        })

        for (const topic of topics) {
          const segments = extractPathSegments(topic.Path || '')
          const item = new RcvTopicHierarchyItem(topic)
          let targetList = items
          let targetParent = targetList
          for (const segment of segments) {
            targetParent = targetList
            let layer = targetList.find(i => i.name === segment)
            if (layer == null) {
              const newLayer = new RcvTopicHierarchyItem()
              newLayer.name = segment
              layer = newLayer
              targetList.push(layer)
              targetList.sort((a, b) => this.compare(a, b))
            }
            targetList = layer.children
          }
          targetList.push(item)
          targetList.sort((a, b) => this.compare(a, b))
          targetParent.sort((a, b) => this.compare(a, b))
        }

        return items
      }),
    )
  }

  get PathHierarchy$(): Observable<RcvTopicHierarchyItem[]> {
    return this.repo.Topics$.pipe(
      map(topics => {
        const items: RcvTopicHierarchyItem[] = []

        topics.sort((a, b) => {
          const pathA = a.Path || ''
          const pathB = b.Path || ''

          return pathA.localeCompare(pathB)
        })

        for (const topic of topics) {
          const segments = extractPathSegments(topic.Path || '')
          let targetList = items
          const path = []
          for (const segment of segments) {
            path.push(segment)
            let layer = targetList.find(i => i.name === segment)
            if (layer == null) {
              layer = new RcvTopicHierarchyItem({ Path: path.join('/'), Name: segment } as any)
              layer.name = segment
              targetList.push(layer)
              targetList.sort((a, b) => this.compare(a, b))
            }
            targetList = layer.children
          }
          targetList.sort((a, b) => this.compare(a, b))
        }

        return items
      }),
    )
  }

  private compare<TItem>(a: HierarchyItemInterface<TItem>, b: HierarchyItemInterface<TItem>): number {
    if (a.children.length > 0 && b.children.length === 0) {
      return -1
    } else if (a.children.length === 0 && b.children.length > 0) {
      return 1
    } else {
      return a.name.localeCompare(b.name)
    }
  }
}

export function extractPathSegments(path: string) {
  return path != null && path !== '' ? path.split('/') : []
}
