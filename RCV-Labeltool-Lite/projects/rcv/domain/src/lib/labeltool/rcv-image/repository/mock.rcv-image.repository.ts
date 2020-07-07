import { Injectable } from '@angular/core'
import { Observable, of } from 'rxjs'

import { RcvTopicInterface } from '../../rcv-topic'
import { RcvImageResponseInterface } from '../model/rcv-image-response.interface'
import { RcvImageInterface } from '../model/rcv-image.interface'
import { RcvLabelMode } from '../model/rcv-label-mode.enum'
import { AbstractRcvImageRepository } from './abstract.rcv-image.repository'

export const images: RcvImageInterface[] = [
  {
    Id: '01.png',
    Url: 'https://dummyimage.com/2000x2000&text=1',
    Path: 'https://dummyimage.com/2000x2000&text=1',
    Index: 0,
    Width: 2000,
    Height: 2000,
    Labels: [
      {
        Id: 'LABEL_1',
        ObjectClassId: '1',
        ObjectClassName: 'Auto',
        SourceModel: 'source model name 1',
        Left: 300,
        Right: 1000,
        Top: 100,
        Bottom: 250,
      },
      {
        Id: 'LABEL_2',
        ObjectClassId: '1',
        ObjectClassName: 'Auto',
        SourceModel: 'source model name 2',
        Left: 700,
        Right: 600,
        Top: 300,
        Bottom: 400,
        Confidence: 0.5,
      },
      {
        Id: 'LABEL_3',
        ObjectClassId: '2',
        ObjectClassName: 'Bagger',
        SourceModel: 'source model name 2',
        Left: 1500,
        Right: 1900,
        Top: 1500,
        Bottom: 1800,
        Confidence: 0.666,
        Prediction: 0.55555555,
        PredictionClass: 'Prediction Class',
      },
    ],
    Segments: [
      {
        Id: 'SEGMENT_1',
        Name: 'Segment 1',
        ObjectClassId: '1',
        ObjectClassName: 'Auto',
        SourceModel: 'source model 1',
        DataImage: '',
      },
      {
        Id: 'SEGMENT_2',
        Name: 'Segment 2',
        ObjectClassId: '1',
        ObjectClassName: 'Auto',
        SourceModel: 'source model 2',
        DataImage: '',
      },
    ],
  },
  {
    Id: '02.png',
    Url: 'https://dummyimage.com/750x1000&text=2',
    Path: 'https://dummyimage.com/750x1000&text=2',
    Index: 1,
    Width: 750,
    Height: 1000,
    Labels: [
      {
        Id: 'LABEL_1',
        ObjectClassId: '1',
        ObjectClassName: 'Auto',
        SourceModel: 'source model name 1',
        Left: 300,
        Right: 500,
        Top: 100,
        Bottom: 250,
        Confidence: 66.666666,
      },
    ],
    Segments: [
      {
        Id: 'SEGMENT_1',
        Name: 'Segment 1',
        ObjectClassId: '1',
        ObjectClassName: 'Auto',
        SourceModel: 'source model 1',
        DataImage: '',
      },
    ],
  },
  {
    Id: '03.png',
    Url: 'https://dummyimage.com/1000x500&text=3',
    Path: 'https://dummyimage.com/1000x500&text=3',
    Index: 2,
    Width: 1000,
    Height: 500,
    Labels: [
      {
        Id: 'LABEL_1',
        ObjectClassId: '1',
        ObjectClassName: 'Auto',
        SourceModel: 'source model name 1',
        Left: 100,
        Right: 200,
        Top: 100,
        Bottom: 200,
      },
    ],
    Segments: [],
  },
  {
    Id: '04.png',
    Url: 'https://dummyimage.com/1000x750&text=4',
    Path: 'https://dummyimage.com/1000x750&text=4',
    Width: 1000,
    Height: 750,
    Index: 3,
    Labels: [],
    Segments: [],
  },
  {
    Id: '05.png',
    Url: 'https://dummyimage.com/1000x750&text=5',
    Path: 'https://dummyimage.com/1000x750&text=5',
    Width: 1000,
    Height: 750,
    Index: 4,
    Labels: [],
    Segments: [],
  },
  {
    Id: '06.png',
    Url: 'https://dummyimage.com/1000x750&text=6',
    Path: 'https://dummyimage.com/1000x750&text=6',
    Width: 1000,
    Height: 750,
    Index: 5,
    Labels: [],
    Segments: [],
  },
  {
    Id: '07.png',
    Url: 'https://dummyimage.com/1000x750&text=7',
    Path: 'https://dummyimage.com/1000x750&text=7',
    Width: 1000,
    Height: 750,
    Index: 6,
    Labels: [],
    Segments: [],
  },
  {
    Id: '08.png',
    Url: 'https://dummyimage.com/1000x750&text=8',
    Path: 'https://dummyimage.com/1000x750&text=8',
    Width: 1000,
    Height: 750,
    Index: 7,
    Labels: [],
    Segments: [],
  },
  {
    Id: '09.png',
    Url: 'https://dummyimage.com/1000x750&text=9',
    Path: 'https://dummyimage.com/1000x750&text=9',
    Width: 1000,
    Height: 750,
    Index: 8,
    Labels: [],
    Segments: [],
  },
]

@Injectable({
  providedIn: 'root',
})
export class MockRcvImageRepository extends AbstractRcvImageRepository {
  image(mode: RcvLabelMode, topic: RcvTopicInterface, index: number): Observable<RcvImageResponseInterface> {
    return of(this.dummy(images[index]))
  }

  blank(
    mode: RcvLabelMode,
    topic: RcvTopicInterface,
    index: number,
    shuffle: boolean,
  ): Observable<RcvImageResponseInterface> {
    return of(this.dummy(images[index + 1]))
  }

  lastBlank(
    mode: RcvLabelMode,
    topic: RcvTopicInterface,
    shuffle: boolean,
  ): Observable<RcvImageResponseInterface> {
    return of(this.dummy(images[images.length - 1]))
  }

  next(
    mode: RcvLabelMode,
    topic: RcvTopicInterface,
    index: number,
    shuffle: boolean,
  ): Observable<RcvImageResponseInterface> {
    return of(this.dummy(images[index + 1]))
  }

  nextBlank(
    mode: RcvLabelMode,
    topic: RcvTopicInterface,
    index: number,
    shuffle: boolean,
  ): Observable<RcvImageResponseInterface> {
    return of(this.dummy(images[index + 1]))
  }

  previous(
    mode: RcvLabelMode,
    topic: RcvTopicInterface,
    index: number,
    shuffle: boolean,
  ): Observable<RcvImageResponseInterface> {
    return of(this.dummy(images[index - 1]))
  }

  previousBlank(
    mode: RcvLabelMode,
    topic: RcvTopicInterface,
    index: number,
    shuffle: boolean,
  ): Observable<RcvImageResponseInterface> {
    return of(this.dummy(images[index - 1]))
  }

  private dummy(image: RcvImageInterface): RcvImageResponseInterface {
    return {
      HasNext: true,
      HasNextBlank: true,
      HasPrevious: true,
      HasPreviousBlank: true,
      ImageCount: images.length,
      ImageLabel: image,
    }
  }
}
