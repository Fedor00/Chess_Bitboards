import { MIDNIGHT_PATH } from '../config'
import { BLACK_WHITE_PATH } from '../config'

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
  return moves.map((move) => ({
    ...move,
    from: 63 - move.from,
    to: 63 - move.to,
  }))
}

export const rowNotation = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h']
export const colNotation = ['8', '7', '6', '5', '4', '3', '2', '1']
export const getPathForPiece = (piece) => {
  const pieceColor = piece === piece.toLowerCase() ? 'b' : 'w'
  return `${MIDNIGHT_PATH}/${pieceColor}${piece.toUpperCase()}.svg`
}
export const getNotationForTile = (i, j, playerColor) => {
  const rowNotation = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h']
  const colNotation = ['8', '7', '6', '5', '4', '3', '2', '1']
  const col = playerColor === 'white' ? colNotation[i] : colNotation[7 - i]
  const row = playerColor === 'white' ? rowNotation[j] : rowNotation[7 - j]
  return { col, row }
}
