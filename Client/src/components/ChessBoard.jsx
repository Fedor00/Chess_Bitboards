import React, { useEffect, useRef } from 'react'
import { useAuth } from '../contexts/AuthContext'
import { getPathForPiece } from '../services/Utils'
import PlayerName from './PlayerName'

import Loading from './Loading'
import Tile from './Tile'
import { useDragPiece } from '../hooks/useDragPiece'
import { useHighlightMoves } from '../hooks/useHighlightMoves'
import useChessBoardUtils from '../hooks/useChessBoardUtils'
import { Button } from './ui/button'
function ChessBoard({ game, makeMove, resign }) {
  const chessBoardRef = useRef(null)
  const { user } = useAuth()
  const myDivRef = useRef(null)
  const { playerColor, opponentUsername, getNotationForTile } =
    useChessBoardUtils(user, game)
  const { isDragging, imgSize, mousePosition, selectedPiece, handleDragStart } =
    useDragPiece(game, chessBoardRef, makeMove, playerColor)
  const { highlightedMoves } = useHighlightMoves(
    game,
    playerColor,
    selectedPiece,
  )
  useEffect(() => {
    if (myDivRef.current) {
      myDivRef.current.scrollIntoView({ behavior: 'smooth' })
    }
  }, [myDivRef])
  return (
    <div
      className="mb-2 flex max-h-full flex-col items-center justify-center"
      ref={myDivRef}
    >
      <div className="grid w-[calc(100vmin-3rem)] grid-rows-1">
        <div className="flex w-full justify-between">
          <PlayerName name={user?.username} />
          <Button className="" onClick={resign}>
            resign
          </Button>
          {game?.secondPlayer?.id ? (
            <PlayerName name={opponentUsername} />
          ) : (
            <Loading />
          )}
        </div>

        <div
          className="grid cursor-grab grid-cols-8 active:cursor-grabbing"
          ref={chessBoardRef}
        >
          {game.pieces.map((row, rowIndex) => (
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
                    notation={getNotationForTile(rowIndex, colIndex)}
                    isPieceSelected={
                      selectedPiece?.i === rowIndex &&
                      selectedPiece?.j === colIndex
                    }
                  />
                </div>
              ))}
            </React.Fragment>
          ))}
        </div>
      </div>
      {isDragging && selectedPiece && (
        <img
          src={getPathForPiece(selectedPiece.piece)}
          alt="Dragging piece"
          className="object-contain"
          style={{
            width: imgSize.width,
            height: imgSize.height,
            position: 'fixed',
            left: mousePosition.x - imgSize.width / 2,
            top: mousePosition.y - imgSize.height / 2,
            pointerEvents: 'none',
          }}
        />
      )}
    </div>
  )
}

export default ChessBoard
