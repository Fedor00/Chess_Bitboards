import { useEffect, useRef, useState } from 'react'
import { useAuth } from '../../contexts/AuthContext'
import { getPathForPiece } from '../../services/Utils'
import PlayerName from './PlayerName'

import Loading from '../Loading'
import { useDragPiece } from '../../hooks/useDragPiece'
import { useHighlightMoves } from '../../hooks/useHighlightMoves'
import useChessBoardUtils from '../../hooks/useChessBoardUtils'
import { Button } from '../ui/button'
import { FaComments, FaFlag, FaTimes } from 'react-icons/fa'
import Board from './Board'
import useChat from '@/hooks/useChat'
import Chat from '../chat/Chat'
import ShowTextModal from '../ShowTextModal'
import PromotionPiece from './PromotionPiece'
function ChessBoard({ game, makeMove, resign, cancelGame }) {
  const chessBoardRef = useRef(null)
  const { user } = useAuth()

  const [showCode, setShowCode] = useState(false)
  const [showChoosePiece, setShowChoosePiece] = useState(false)
  const [move, setMove] = useState(null)
  const myDivRef = useRef(null)

  useEffect(() => {
    setMove(game.move)
  }, [game])
  const { playerColor, opponentUsername, isPromotionMove } = useChessBoardUtils(
    user,
    game,
  )
  const makeMoveWhenPromotion = async (piece) => {
    console.log(move.from, move.to, piece)
    await makeMove(move.from, move.to, piece)

    setShowChoosePiece(false)
  }

  const doMove = async (from, to) => {
    const isPromotion = isPromotionMove(from, to, selectedPiece.piece, game)

    if (!isPromotion) {
      await makeMove(from, to, 'X')

      return
    }
    setMove({ from, to })
    setShowChoosePiece(true)
  }

  const { isDragging, imgSize, mousePosition, selectedPiece, handleDragStart } =
    useDragPiece(game, chessBoardRef, doMove, playerColor)

  const { highlightedMoves } = useHighlightMoves(
    game,
    playerColor,
    selectedPiece,
  )
  const { chat, addMessage, showChat, setShowChat, unseenMessages } = useChat(
    game?.id,
  )

  useEffect(() => {
    if (myDivRef.current) {
      myDivRef.current.scrollIntoView({ behavior: 'smooth' })
    }
  }, [myDivRef])
  useEffect(() => {
    const preventDefault = (e) => e.preventDefault()

    const chessBoardElement = chessBoardRef.current
    if (chessBoardElement) {
      chessBoardElement.addEventListener('touchstart', preventDefault, {
        passive: false,
      })
      chessBoardElement.addEventListener('touchmove', preventDefault, {
        passive: false,
      })
    }

    return () => {
      if (chessBoardElement) {
        chessBoardElement.removeEventListener('touchstart', preventDefault)
        chessBoardElement.removeEventListener('touchmove', preventDefault)
      }
    }
  }, [])
  return (
    <>
      <div
        className="flex flex-col items-center justify-center max-h-full mb-2"
        ref={myDivRef}
      >
        <div className="grid w-full grid-rows-1 items-center justify-center sm:w-[calc(100vmin-7rem)]">
          <div className="flex justify-between w-full">
            <PlayerName name={user?.username} />
            <div className="flex space-x-1">
              <Button onClick={resign}>
                <FaFlag color="red" />
              </Button>
              <Button onClick={() => setShowCode(true)}>Game Id</Button>
              <div className="relative">
                {unseenMessages > 0 && (
                  <span className="absolute flex w-3 h-3 -right-1 -top-1">
                    <span className="absolute inline-flex w-full h-full rounded-full opacity-75 animate-ping bg-sky-400"></span>
                    <span className="relative inline-flex w-3 h-3 rounded-full bg-sky-500"></span>
                  </span>
                )}
                <Button onClick={() => setShowChat((current) => !current)}>
                  <FaComments color="#add8e6" />
                </Button>
              </div>
              {!opponentUsername && (
                <Button onClick={cancelGame}>
                  <FaTimes color="#add8e6" />
                </Button>
              )}
            </div>
            {opponentUsername ? (
              <PlayerName name={opponentUsername} />
            ) : (
              <Loading />
            )}
          </div>

          <div
            className="grid grid-cols-8 cursor-grab active:cursor-grabbing"
            ref={chessBoardRef}
          >
            <Board
              pieces={game?.pieces}
              highlightedMoves={highlightedMoves}
              selectedPiece={selectedPiece}
              handleDragStart={handleDragStart}
              color={playerColor}
              move={move}
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
      {showChat && chat && opponentUsername && (
        <Chat
          chat={chat}
          onSendMessage={addMessage}
          showChatSheet={showChat}
          setShowChatSheet={setShowChat}
          opponentUsername={opponentUsername}
        />
      )}
      <ShowTextModal
        showModal={showCode}
        setShowModal={setShowCode}
        text={game?.id}
      />
      <PromotionPiece
        showModal={showChoosePiece}
        setShowModal={setShowChoosePiece}
        onChoosePiece={makeMoveWhenPromotion}
        color={playerColor}
      />
    </>
  )
}

export default ChessBoard
