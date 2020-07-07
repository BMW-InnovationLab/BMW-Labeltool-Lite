/**
 * Legacy handling for old image path urls
 *
 * @param obj object
 * @param newKey new key
 * @param oldKey old key
 */
export function fillPath<T extends object, K extends keyof T>(
  obj: T,
  newKey: K,
  oldKey: K,
  apiUrl: string,
): T {
  if (obj == null) {
    return obj
  }
  const newObj = { ...(obj as object) } as T
  const newValue = newObj[newKey]
  if (newValue == null) {
    newObj[newKey] = newObj[oldKey]
  } else if (typeof newValue === 'string') {
    newObj[newKey] = (apiUrl + stripApiPrefix(newValue)) as any
  }

  return newObj
}

function stripApiPrefix(url: string): string {
  return url.replace(/^\/api/, '')
}
