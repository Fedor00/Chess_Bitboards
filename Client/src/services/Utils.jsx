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
export const isOutOfBounds = (i, j) => {
  return i < 0 || i > 7 || j < 0 || j > 7
}
export const flipPieces = (pieces) => {
  return pieces.map((row, rowIndex) => {
    return row.map((_, cellIndex) => {
      return pieces[7 - rowIndex][7 - cellIndex]
    })
  })
}
export const flipMoves = (moves) => {
  return moves.map((move) => {
    move.from = 63 - move.from
    move.to = 63 - move.to
    return move
  })
}
export const rowNotation = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h']
export const colNotation = ['8', '7', '6', '5', '4', '3', '2', '1']
