import { HttpClient } from '@angular/common/http'
import { Injectable } from '@angular/core'
import { environment } from '@environment'
import { LabeltoolConfigurationService } from '@rcv/domain'
import { ImagesService, RcvImageLabelInterface } from '@rcv/domain/labeltool-client'
import { Observable } from 'rxjs'
import { first, map } from 'rxjs/operators'

class HttpClientUrlExtractor extends HttpClient {
  url: string

  constructor() {
    super(null)
  }

  get(url: string, options: any): Observable<any> {
    this.url = url
    return null
  }
}

@Injectable({
  providedIn: 'root',
})
export class LabelImageService {
  constructor(private config: LabeltoolConfigurationService) {}

  getImageUrl(topicId: number, image: RcvImageLabelInterface): Observable<string> {
    // get the url from generated swagger
    return this.config.CurrentApiUrl$.pipe(
      first(),
      map(apiUrl => {
        const htttpClientUrlExtractor = new HttpClientUrlExtractor()
        const imagesService = new ImagesService(htttpClientUrlExtractor, this.stripApiPrefix(apiUrl), null)
        imagesService.getImage(topicId, image.Id)
        return environment.services.mock ? (<any>image).Path : htttpClientUrlExtractor.url
      }),
    )
  }

  private stripApiPrefix(url: string): string {
    return url.replace(/\/api/, '')
  }
}
