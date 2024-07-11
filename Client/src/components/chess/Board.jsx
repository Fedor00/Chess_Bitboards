import React from 'react'
import Tile from './Tile'
import { getNotationForTile } from '@/services/Utils'

function Board({
  pieces,
  highlightedMoves,
  selectedPiece,
  handleDragStart,
  color,
}) {
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
                isHighlighted={highlightedMoves[rowIndex]?.[colIndex]}
                notation={getNotationForTile(rowIndex, colIndex, color)}
                isPieceSelected={
                  selectedPiece?.i === rowIndex && selectedPiece?.j === colIndex
                }
              />
            </div>
          ))}
        </React.Fragment>
      ))}
    </>
  )
}

export default Board
