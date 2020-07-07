import { AbstractEntityCollectionRepository } from '../../libraries/extensions'
import { ErrorCollector } from '../error-collector/error-collector'
import { HttpApiConnection } from './http.api.connection'

export class HttpApiCollectionConnection<TEntity> extends AbstractEntityCollectionRepository<TEntity> {
  constructor(logger: ErrorCollector, private httpConnection: HttpApiConnection) {
    super(logger)
  }

  protected load(): Promise<TEntity[]> {
    return this.httpConnection.httpGet()
  }

  protected add(entry: TEntity): Promise<TEntity> {
    return this.httpConnection.httpPost(entry)
  }

  protected update(id: string, entry: TEntity): Promise<boolean> {
    return this.httpConnection.httpPut(entry, id)
  }

  protected remove(id: string): Promise<boolean> {
    return this.httpConnection.httpDelete(id)
  }

  get Api(): HttpApiConnection {
    return this.httpConnection
  }
}
