import { HttpClient, HttpResponseBase } from '@angular/common/http'
import { Observable, of } from 'rxjs'
import { catchError, map } from 'rxjs/operators'

export class HttpApiConnection {
  private options: any = { observe: 'response', responseType: 'text' }
  private _baseUrl: string

  get baseUrl() {
    return this._baseUrl
  }
  set baseUrl(value: string) {
    this._baseUrl = value.trim()
  }

  constructor(protected http: HttpClient, baseUrl: string, private entitySegments: string[]) {
    this.baseUrl = baseUrl
  }

  apiUrl(...pathSegments: string[]): string {
    const urlSegments = [this.baseUrl, ...this.entitySegments, ...pathSegments]
    return urlSegments.join('/')
  }

  httpGet<TModel>(...pathSegments: string[]): Promise<TModel> {
    return this.http.get<TModel>(this.apiUrl(...pathSegments)).toPromise()
  }

  httpPost<TModel>(data: TModel, ...pathSegments: string[]): Promise<any> {
    return this.http.post<TModel>(this.apiUrl(...pathSegments), data).toPromise()
  }

  async httpPut<TModel>(data: TModel, ...pathSegments: string[]): Promise<boolean> {
    const result = <HttpResponseBase>(
      (<any>await this.http.put(this.apiUrl(...pathSegments), data, this.options).toPromise())
    )
    return result.ok
  }

  async httpDelete(...pathSegments: string[]): Promise<boolean> {
    const result = <HttpResponseBase>(
      (<any>await this.http.delete(this.apiUrl(...pathSegments), this.options).toPromise())
    )
    return result.ok
  }

  httpResourceExists(path: string): Observable<boolean> {
    return this.http.get(path, this.options).pipe(
      map(r => true),
      catchError(e => of(false)),
    )
  }

  get httpClient() {
    return this.http
  }
}
