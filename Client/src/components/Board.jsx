import React, { useRef, useState } from 'react'
import Tile from './Tile'
import { useAuth } from '../contexts/AuthContext'
import { getChessBoardIndex } from '../services/Utils'

function ChessBoard({ game, updateGamePieces, makeMove }) {
  const chessBoardRef = useRef(null)
  const { user } = useAuth()
  const isUserTurn = user?.id === game?.bottomPlayerId ? 'white' : 'black'

  const [highlightedMoves, setHighlightedMoves] = useState([])
  const [selectedPiece, setSelectedPiece] = useState({
    i: null,
    j: null,
    piece: null,
  })

  const highlightMoves = (rowIndex, colIndex) => {
    const movesHighlight = Array.from({ length: 8 }, () => Array(8).fill(false))
    if (rowIndex < 1 || rowIndex > 8 || colIndex < 1 || colIndex > 8) return
    const fromIndex = getChessBoardIndex(rowIndex - 1, colIndex - 1)
    const validMoves = game[`${isUserTurn}Moves`]

    validMoves.forEach((move) => {
      if (move.from === fromIndex) {
        const row = Math.floor(move.to / 8)
        const col = move.to % 8
        movesHighlight[row][col] = true
      }
    })

    setHighlightedMoves(movesHighlight)
  }

  const onPieceSelected = (rowIndex, colIndex) => {
    if (
      selectedPiece?.i === rowIndex &&
      selectedPiece?.j === colIndex &&
      highlightedMoves.length !== 0
    )
      setHighlightedMoves([])
    else highlightMoves(rowIndex, colIndex)

    setSelectedPiece({
      i: rowIndex,
      j: colIndex,
      piece: game.pieces[rowIndex][colIndex],
    })
    const newBoard = game.pieces.map((row) => [...row])
    newBoard[rowIndex][colIndex] = 'X'
    updateGamePieces(newBoard)
  }

  const restoreSelectedPiece = () => {
    const newBoard = game.pieces.map((row) => [...row])
    newBoard[selectedPiece.i][selectedPiece.j] = selectedPiece.piece
    updateGamePieces(newBoard)
  }

  const onDragEnd = async (mouseX, mouseY) => {
    if (!chessBoardRef.current) return
    const { left, top, width } = chessBoardRef.current.getBoundingClientRect()
    const tileWidth = width / game.pieces[0].length
    const newRowIndex = Math.floor((mouseY - top) / tileWidth)
    const newColIndex = Math.floor((mouseX - left) / tileWidth)
    const fromIndex = getChessBoardIndex(
      selectedPiece.i - 1,
      selectedPiece.j - 1,
    )
    const toIndex = getChessBoardIndex(newRowIndex - 1, newColIndex - 1)
    if (
      newRowIndex >= 1 &&
      newRowIndex < game.pieces.length - 1 &&
      newColIndex >= 1 &&
      newColIndex < game.pieces[0].length - 1
    ) {
      try {
        if (fromIndex !== toIndex) {
          setHighlightedMoves([])
          await makeMove(fromIndex, toIndex)
        } else restoreSelectedPiece()
      } catch (error) {
        restoreSelectedPiece()
      }
    } else {
      restoreSelectedPiece()
    }
  }

  return (
    <div className="max-w-screen flex h-screen max-h-[93vh] w-full items-center justify-center">
      <div
        className="grid grid-cols-10"
        ref={chessBoardRef}
        style={{ width: 'calc(100vmin )', height: 'calc(100vmin - 6rem)' }}
      >
        {game.pieces.map((row, rowIndex) => (
          <React.Fragment key={rowIndex}>
            {row.map((cell, colIndex) => (
              <div
                key={`${rowIndex}-${colIndex}`}
                className="aspect-square w-full"
              >
                <Tile
                  cell={cell}
                  i={rowIndex}
                  j={colIndex}
                  onPieceSelected={() => onPieceSelected(rowIndex, colIndex)}
                  onDragEnd={onDragEnd}
                  selectedPiece={selectedPiece.piece}
                  isHighlighted={highlightedMoves[rowIndex - 1]?.[colIndex - 1]}
                />
              </div>
            ))}
          </React.Fragment>
        ))}
      </div>
    </div>
  )
}

export default ChessBoard
