import { ErrorCollector } from '../../../modules/error-collector'
import { AbstractEntityCollectionRepository } from './abstract.entity-collection.repository'

export class MockEntityCollectionRepository<TModel> extends AbstractEntityCollectionRepository<TModel> {
  private storage: {
    [id: string]: TModel
  } = {}

  constructor(errorCollector: ErrorCollector, private idExtractor: (model: TModel) => string) {
    super(errorCollector)
  }

  initialize(entities: TModel[] = []) {
    entities.forEach(async value => {
      await this.add(value)
    })
  }

  protected async load(): Promise<TModel[]> {
    return this.Storage
  }

  protected async add(entry: TModel): Promise<TModel> {
    this.storage[this.idExtractor(entry)] = entry
    return entry
  }

  protected async update(id: string, entry: TModel): Promise<boolean> {
    this.storage[id] = entry
    return true
  }

  protected async remove(id: string): Promise<boolean> {
    delete this.storage[id]
    return true
  }

  get Storage(): TModel[] {
    const data: TModel[] = []
    Object.keys(this.storage).forEach(key => data.push(this.storage[key]))
    return data
  }
}
