export function compareName(a: string | undefined, b: string | undefined): boolean {
  if (typeof a !== 'string') {
    a = ''
  }
  if (typeof b !== 'string') {
    b = ''
  }

  return cleanString(a) === cleanString(b)
}

function cleanString(value: string): string {
  return value.trim().toLocaleLowerCase()
}
