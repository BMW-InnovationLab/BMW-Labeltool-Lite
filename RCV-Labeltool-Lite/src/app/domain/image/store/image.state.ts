import { HttpErrorResponse } from '@angular/common/http'
import { Action, Selector, State, StateContext } from '@ngxs/store'
import { throwError } from 'rxjs'
import { catchError, map } from 'rxjs/operators'

import { AbstractRcvImageRepository, RcvLabelMode } from '@rcv/domain'
import { NotificationService } from '@rcv/ui'
import { ChangeLabels, ILabel } from '../../object-detection'
import {
  ImageReady,
  LoadBlank,
  LoadImage,
  LoadInitialImage,
  SelectMode,
  SetImageResponse,
  SetImageSize,
} from './image.actions'
import { ImageStateModel } from './image.model'

@State<ImageStateModel>({
  name: 'image',
  defaults: {
    imageReady: false,
    imageSize: [0, 0],
    mode: RcvLabelMode.ObjectDetection,
  },
})
export class ImageState {
  constructor(
    private imageRepository: AbstractRcvImageRepository,
    private notificationService: NotificationService,
  ) {}

  @Selector()
  static count(state: ImageStateModel) {
    return state.imageResponse ? state.imageResponse.ImageCount : 0
  }

  @Selector()
  static image(state: ImageStateModel) {
    return state.imageResponse ? state.imageResponse.ImageLabel : undefined
  }

  @Selector()
  static imageResponse(state: ImageStateModel) {
    return state.imageResponse
  }

  @Selector()
  static imageSize(state: ImageStateModel) {
    return state.imageSize
  }

  @Selector()
  static imageReady(state: ImageStateModel) {
    return state.imageReady
  }

  @Selector()
  static mode(state: ImageStateModel) {
    return state.mode
  }

  @Action(ImageReady)
  imageReady(ctx: StateContext<ImageStateModel>, action: ImageReady) {
    ctx.patchState({ imageReady: action.ready })
  }

  @Action(LoadBlank)
  loadBlank(ctx: StateContext<ImageStateModel>, action: LoadBlank) {
    const state = ctx.getState()
    return this.imageRepository.blank(state.mode, action.topic, action.index, false).pipe(
      map(r => ctx.dispatch(new SetImageResponse(r))),
      catchError(err => {
        this.notificationService.error('Cannot load Image', err)
        return throwError(err)
      }),
    )
  }

  @Action(LoadInitialImage)
  loadInitialImage(ctx: StateContext<ImageStateModel>, action: LoadInitialImage) {
    const state = ctx.getState()
    return this.imageRepository.lastBlank(state.mode, action.topic, false).pipe(
      map(r => ctx.dispatch(new SetImageResponse(r))),
      catchError((err: HttpErrorResponse) => this.handle404(ctx, err)),
    )
  }

  @Action([LoadImage])
  loadImage(ctx: StateContext<ImageStateModel>, action: LoadImage) {
    const state = ctx.getState()
    return this.imageRepository.image(state.mode, action.topic, action.index).pipe(
      map(r => ctx.dispatch(new SetImageResponse(r))),
      catchError((err: HttpErrorResponse) => this.handle404(ctx, err)),
    )
  }

  @Action(SelectMode)
  selectMode(ctx: StateContext<ImageStateModel>, action: SelectMode) {
    ctx.patchState({ mode: action.mode })
  }

  @Action(SetImageResponse)
  setImageResponse(ctx: StateContext<ImageStateModel>, action: SetImageResponse) {
    ctx.patchState({ imageResponse: action.response, imageReady: false })

    let labels: ILabel[] = []
    if (action.response) {
      const image = action.response.ImageLabel

      labels = image.Labels.map(l => ({ ...l, isVisible: true, isSelected: false, isActive: false }))
    }

    ctx.dispatch([new ChangeLabels(labels)])
  }

  @Action(SetImageSize)
  setImageSize(ctx: StateContext<ImageStateModel>, action: SetImageSize) {
    ctx.patchState({ imageSize: action.size })
  }

  private handle404(ctx: StateContext<ImageStateModel>, err: HttpErrorResponse) {
    // 404 clears image navigation
    if (err.status === 404) {
      ctx.dispatch([new SetImageResponse(), new ImageReady(true)])
    } else {
      this.notificationService.error('Cannot load image', err)
    }
    return throwError(err)
  }
}
