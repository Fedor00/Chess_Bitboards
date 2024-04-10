import { useEffect, useRef } from 'react'
import { useAuth } from '../../contexts/AuthContext'
import { getPathForPiece } from '../../services/Utils'
import PlayerName from './PlayerName'

import Loading from '../Loading'
import { useDragPiece } from '../../hooks/useDragPiece'
import { useHighlightMoves } from '../../hooks/useHighlightMoves'
import useChessBoardUtils from '../../hooks/useChessBoardUtils'
import { Button } from '../ui/button'
import { FaComments, FaFlag } from 'react-icons/fa'
import Board from './Board'
function ChessBoard({ game, makeMove, resign }) {
  const chessBoardRef = useRef(null)
  const { user } = useAuth()
  const myDivRef = useRef(null)
  const { playerColor, opponentUsername } = useChessBoardUtils(user, game)
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
          <div className="space-x-1">
            <Button onClick={resign}>
              <FaFlag color="red" />
            </Button>
            <Button onClick={() => {}}>
              <FaComments color="#add8e6" />
            </Button>
          </div>
          {opponentUsername ? (
            <PlayerName name={opponentUsername} />
          ) : (
            <Loading />
          )}
        </div>

        <div
          className="grid cursor-grab grid-cols-8 active:cursor-grabbing"
          ref={chessBoardRef}
        >
          <Board
            pieces={game?.pieces}
            highlightedMoves={highlightedMoves}
            selectedPiece={selectedPiece}
            handleDragStart={handleDragStart}
            color={playerColor}
          />
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
