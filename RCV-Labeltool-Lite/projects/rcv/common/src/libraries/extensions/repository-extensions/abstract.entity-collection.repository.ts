import { BehaviorSubject, Observable, ReplaySubject } from 'rxjs'
import { map } from 'rxjs/operators'

import { ErrorCollector } from '../../../modules/error-collector'
import { RepositoryErrorTypes } from './repository.error-type.enum'

export abstract class AbstractEntityCollectionRepository<TModel> {
  private subject = new ReplaySubject<TModel[]>(1)
  private isLoadingSubject = new BehaviorSubject<boolean>(false)
  private isLoaded = false

  // private timeStamp: Date = new Date(0) // todo enable auto reload

  constructor(private error: ErrorCollector) {}

  get Entries$(): Observable<TModel[]> {
    if (!this.isLoaded) {
      this.reload()
      // this.timeStamp = new Date()
      this.isLoaded = true
    }
    return this.subject.asObservable()
  }

  Entry$(modelFilter: (model: TModel) => boolean): Observable<TModel> {
    return this.Entries$.pipe(map(value => value.find(modelFilter) as TModel))
  }

  get IsLoading$(): Observable<boolean> {
    return this.isLoadingSubject.asObservable()
  }

  async reload() {
    this.isLoadingSubject.next(true)
    try {
      const value = await this.load()
      this.isLoadingSubject.next(false)
      this.subject.next(value)
    } catch (reason) {
      // TODO: in case of error push an empty list, this is no proper error handling but a start
      this.subject.next([])
      this.isLoadingSubject.next(false)
      this.error.emit(RepositoryErrorTypes.LoadFailed, 'Error while loading Data', reason)
    }
  }

  async addEntry(entity: TModel): Promise<TModel> {
    const result = await this.add(entity)
    if (result) {
      await this.reload()
    }
    return result
  }

  async updateEntry(entityId: string, value: TModel): Promise<boolean> {
    const result = await this.update(entityId, value)
    if (result) {
      await this.reload()
    }
    return result
  }

  async removeEntry(entityId: string): Promise<boolean> {
    const result = await this.remove(entityId)
    if (result) {
      await this.reload()
    }
    return result
  }

  protected abstract load(): Promise<TModel[]>

  protected abstract add(entry: TModel): Promise<TModel>

  protected abstract update(id: string, entry: TModel): Promise<boolean>

  protected abstract remove(id: string): Promise<boolean>
}
