// Convert a single number from 0-63 to i and j coordinates
export function getCoordinates(number) {
  const i = Math.floor(number / 8)
  const j = number % 8
  return { i, j }
}

// Convert i and j coordinates to a single number from 0-63
export function getChessBoardIndex(i, j) {
  const number = i * 8 + j
  return number
}
