import { useEffect, useState } from 'react'
import {
  getChessBoardIndex,
  getCoordinates,
  isOutOfBounds,
} from '../services/Utils'

export function useHighlightMoves(game, playerColor, selectedPiece) {
  const [highlightedMoves, setHighlightedMoves] = useState([])
  useEffect(() => {
    const highlightMoves = () => {
      if (!selectedPiece || selectedPiece.piece === 'X') return
      const movesHighlight = Array.from({ length: 8 }, () =>
        Array.from({ length: 8 }, () => false),
      )
      if (isOutOfBounds(selectedPiece.i, selectedPiece.j)) return
      const fromIndex = getChessBoardIndex(selectedPiece.i, selectedPiece.j)
      const validMoves = game[`${playerColor}Moves`]
      validMoves.forEach((move) => {
        if (move.from === fromIndex) {
          const { i, j } = getCoordinates(move.to)
          movesHighlight[i][j] = true
        }
      })

      setHighlightedMoves(movesHighlight)
    }
    highlightMoves()
  }, [game, playerColor, selectedPiece])

  return { highlightedMoves }
}
