import React from 'react'
import Tile from './Tile'
import { getCoordinates, getNotationForTile } from '@/services/Utils'

function Board({
  pieces,
  highlightedMoves,
  selectedPiece,
  handleDragStart,
  color,
  move,
}) {
  const isFromSquare = (i, j) => {
    const cooridnateFrom = getCoordinates(move?.from)
    return cooridnateFrom.i === i && cooridnateFrom.j === j
  }
  const isToSquare = (i, j) => {
    const cooridnateTo = getCoordinates(move?.to)
    return cooridnateTo.i === i && cooridnateTo.j === j
  }
  return (
    <>
      {pieces.map((row, rowIndex) => (
        <React.Fragment key={rowIndex}>
          {row.map((cell, colIndex) => (
            <div key={`${rowIndex}-${colIndex}`} className="aspect-square ">
              <Tile
                cell={cell}
                i={rowIndex}
                j={colIndex}
                onMouseDown={(e) =>
                  handleDragStart(e, cell, rowIndex, colIndex)
                }
                onTouchStart={(e) =>
                  handleDragStart(e, cell, rowIndex, colIndex)
                }
                isHighlighted={highlightedMoves[rowIndex]?.[colIndex]}
                notation={getNotationForTile(rowIndex, colIndex, color)}
                isPieceSelected={
                  selectedPiece?.i === rowIndex && selectedPiece?.j === colIndex
                }
                isFromSquare={isFromSquare(rowIndex, colIndex)}
                isToSquare={isToSquare(rowIndex, colIndex)}
              />
            </div>
          ))}
        </React.Fragment>
      ))}
    </>
  )
}

export default Board
