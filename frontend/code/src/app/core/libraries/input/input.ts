export function digitFromInput(event: KeyboardEvent): { valid: boolean; digit: number } {
  if (event.code == null) {
    // fallback for IE
    if (event.keyCode >= 49 && event.keyCode <= 58) {
      return { valid: true, digit: event.keyCode - 48 }
    }

    return { valid: false, digit: -1 }
  }

  const validKeys = ['1', '2', '3', '4', '5', '6', '7', '8', '9']
  const index = validKeys.indexOf(event.code.substr(-1, 1))
  if (index !== -1) {
    return { valid: true, digit: +validKeys[index] }
  }

  return { valid: false, digit: -1 }
}
