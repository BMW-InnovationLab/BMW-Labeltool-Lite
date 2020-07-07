import { ApiConnectionFactory } from './api.connection.factory'

export abstract class AbstractApiRepository {
  constructor(protected connector: ApiConnectionFactory) {}
}
