import React, { useRef, useState } from 'react'
import Tile from './Tile'
import { useAuth } from '../contexts/AuthContext'
import {
  getChessBoardIndex,
  getCoordinates,
  isOutOfBounds,
} from '../services/Utils'

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
    if (isOutOfBounds(rowIndex, colIndex)) return
    const fromIndex = getChessBoardIndex(rowIndex, colIndex)
    const validMoves = game[`${isUserTurn}Moves`]

    validMoves.forEach((move) => {
      if (move.from === fromIndex) {
        const { i, j } = getCoordinates(move.to)
        movesHighlight[i][j] = true
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
    const fromIndex = getChessBoardIndex(selectedPiece.i, selectedPiece.j)
    const toIndex = getChessBoardIndex(newRowIndex, newColIndex)
    if (
      !isOutOfBounds(newRowIndex, newColIndex) &&
      highlightedMoves[newRowIndex]?.[newColIndex]
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
  const getNotationForTile = (i, j) => {
    const rowNotation = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h']
    const colNotation = ['8', '7', '6', '5', '4', '3', '2', '1']
    const col = isUserTurn === 'white' ? colNotation[i] : colNotation[7 - i]
    const row = isUserTurn === 'white' ? rowNotation[j] : rowNotation[7 - j]
    return { col, row }
  }
  return (
    <div className="max-w-screen flex max-h-full min-h-[92vh] w-full items-center justify-center ">
      <div
        className="grid  w-[calc(100vmin-3rem)]  grid-cols-8"
        ref={chessBoardRef}
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
                  isHighlighted={highlightedMoves[rowIndex]?.[colIndex]}
                  notation={getNotationForTile(rowIndex, colIndex)}
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
