export function cleanupValues<T>(object: T): T {
  if (object !== Object(object)) {
    return object
  }

  Object.keys(object).forEach(key => (object[key] === '' || object[key] === null) && delete object[key])
  return object
}
