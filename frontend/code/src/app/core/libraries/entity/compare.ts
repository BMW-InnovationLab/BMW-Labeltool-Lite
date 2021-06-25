import { IEntity } from '../../models/entity.interface'

/**
 * Compares objects with id properties. Useful in forms
 * @param o1
 * @param o2
 */
export function selectCompare(o1: IEntity, o2: IEntity): boolean {
  return o1 && o2 && o1.Id === o2.Id
}
