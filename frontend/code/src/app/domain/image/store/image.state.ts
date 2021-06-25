import { HttpErrorResponse } from '@angular/common/http'

import { TranslateService } from '@ngx-translate/core'
import { Action, Selector, State, StateContext } from '@ngxs/store'
import { AbstractRcvImageRepository, RcvImageResponseInterface, RcvLabelMode } from '@rcv/domain'
import { RcvLabelInterface } from '@rcv/domain/labeltool-client'
import { NotificationService } from '@rcv/ui'
import { throwError } from 'rxjs'
import { catchError, map } from 'rxjs/operators'
import { ChangeLabels } from '../../object-detection'
import {
  ImageReady,
  LoadBlank,
  LoadImage,
  LoadImageAfterDelete,
  LoadImageFromThumbnail,
  LoadInitialImage,
  RemoveImage,
  SelectMode,
  SetBrightness,
  SetDefaultImageSize,
  SetImageResponse,
  SetImageSize,
  SetZoom,
} from './image.actions'
import { ImageStateModel } from './image.model'

type Context = StateContext<ImageStateModel>

@State<ImageStateModel>({
  name: 'image',
  defaults: {
    imageReady: false,
    imageSize: [0, 0],
    defaultImageSize: [0, 0],
    mode: RcvLabelMode.ObjectDetection,
    zoom: 1,
    brightness: 1,
  },
})
export class ImageState {
  constructor(
    private imageRepository: AbstractRcvImageRepository,
    private notificationService: NotificationService,
    private translate: TranslateService,
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
  static defaultImageSize(state: ImageStateModel) {
    return state.defaultImageSize
  }

  @Selector()
  static polygonNodeScale(state: ImageStateModel) {
    const imageWidth =
      state.imageResponse && state.imageResponse.ImageLabel ? state.imageResponse.ImageLabel.Width : 0
    const defaultWidth = state.defaultImageSize ? state.defaultImageSize[0] : 0
    if (defaultWidth === 0 || imageWidth === 0) {
      return 1
    }
    return imageWidth / defaultWidth
  }

  @Selector()
  static imageReady(state: ImageStateModel) {
    return state.imageReady
  }

  @Selector()
  static mode(state: ImageStateModel) {
    return state.mode
  }

  @Selector()
  static zoom(state: ImageStateModel) {
    return state.zoom
  }

  @Selector()
  static brightness(state: ImageStateModel) {
    return state.brightness
  }

  @Action(ImageReady)
  imageReady(ctx: Context, action: ImageReady) {
    ctx.patchState({ imageReady: action.ready })
  }

  @Action(LoadBlank)
  loadBlank(ctx: Context, action: LoadBlank) {
    const state = ctx.getState()
    return this.imageRepository.blank(state.mode, action.topic, action.index, 'Standard').pipe(
      map(r => ctx.dispatch(new SetImageResponse(action.topic, r))),
      catchError(err => {
        this.notificationService.error(this.translate.instant('Could not load image'), err)
        return throwError(err)
      }),
    )
  }

  @Action(LoadInitialImage)
  loadInitialImage(ctx: Context, action: LoadInitialImage) {
    const state = ctx.getState()
    return this.imageRepository.lastBlank(state.mode, action.topic, 'Standard').pipe(
      map(r => ctx.dispatch(new SetImageResponse(action.topic, r))),
      catchError((err: HttpErrorResponse) => this.handle404(ctx, err)),
    )
  }

  @Action([LoadImage, LoadImageAfterDelete])
  loadImage(ctx: Context, action: LoadImage) {
    const state = ctx.getState()
    return this.imageRepository.image(state.mode, action.topic, action.index, 'Standard').pipe(
      map(r => ctx.dispatch(new SetImageResponse(action.topic, r))),
      catchError((err: HttpErrorResponse) => this.handle404(ctx, err)),
    )
  }

  @Action(LoadImageFromThumbnail)
  loadImageFromThumbnail(ctx: Context, action: LoadImage) {
    const state = ctx.getState()
    return this.imageRepository.image(state.mode, action.topic, action.index, 'Standard').pipe(
      map(r => ctx.dispatch(new SetImageResponse(action.topic, r, false))),
      catchError((err: HttpErrorResponse) => this.handle404(ctx, err)),
    )
  }

  @Action(RemoveImage)
  removeImage(ctx: Context, action: RemoveImage) {
    return this.imageRepository.delete(action.topic, action.image).pipe(
      map(index => {
        this.notificationService.success(this.translate.instant('Removed image'))
        ctx.dispatch(new LoadImageAfterDelete(action.topic, index))
      }),
      catchError(err => {
        this.notificationService.error(this.translate.instant('Could not remove image'), err)
        return throwError(err)
      }),
    )
  }

  @Action(SelectMode)
  selectMode(ctx: Context, action: SelectMode) {
    ctx.patchState({ mode: action.mode })
  }

  @Action(SetImageResponse)
  setImageResponse(ctx: Context, { response, topic, loadThumbnails }: SetImageResponse) {
    ctx.patchState({ imageResponse: <RcvImageResponseInterface>response, imageReady: false })

    const image = response ? response.ImageLabel : undefined

    ctx.dispatch([new ChangeLabels(<RcvLabelInterface[]>(image ? image.Labels : []))])
  }

  @Action(SetImageSize)
  setImageSize(ctx: Context, action: SetImageSize) {
    ctx.patchState({ imageSize: action.size })
  }

  @Action(SetDefaultImageSize)
  setDefaultImageSize(ctx: Context, action: SetImageSize) {
    ctx.patchState({ defaultImageSize: action.size })
  }

  @Action(SetZoom)
  setZoom(ctx: Context, action: SetZoom) {
    ctx.patchState({ zoom: Math.max(1, action.zoom) })
  }

  @Action(SetBrightness)
  setBrightness(ctx: Context, action: SetBrightness) {
    ctx.patchState({ brightness: Math.max(0.1, action.brightness) })
  }

  private handle404(ctx: Context, err: HttpErrorResponse) {
    // 404 clears image navigation
    if (err.status === 404) {
      ctx.dispatch([new SetImageResponse(), new ImageReady(true)])
    } else {
      this.notificationService.error(this.translate.instant('Could not load image'), err)
    }
    return throwError(err)
  }
}
