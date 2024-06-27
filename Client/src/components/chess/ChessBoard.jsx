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
function ChessBoard({ game, makeMove, resign, cancelGame }) {
  const chessBoardRef = useRef(null)
  const { user } = useAuth()
  const [showCode, setShowCode] = useState(false)
  const myDivRef = useRef(null)
  console.log(game)
  const { playerColor, opponentUsername } = useChessBoardUtils(user, game)
  const { isDragging, imgSize, mousePosition, selectedPiece, handleDragStart } =
    useDragPiece(game, chessBoardRef, makeMove, playerColor)
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
  return (
    <>
      <div
        className="mb-2 flex max-h-full flex-col items-center justify-center"
        ref={myDivRef}
      >
        <div className="grid w-[calc(100vmin-3rem)] grid-rows-1">
          <div className="flex w-full justify-between">
            <PlayerName name={user?.username} />
            <div className="flex space-x-1">
              <Button onClick={resign}>
                <FaFlag color="red" />
              </Button>
              <Button onClick={() => setShowCode(true)}>Game Id</Button>
              <div className="relative">
                {unseenMessages > 0 && (
                  <span className="absolute -right-1 -top-1 flex h-3 w-3">
                    <span className="absolute inline-flex h-full w-full animate-ping rounded-full bg-sky-400 opacity-75"></span>
                    <span className="relative inline-flex h-3 w-3 rounded-full bg-sky-500"></span>
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
      {showChat && chat && (
        <Chat
          chat={chat}
          onSendMessage={addMessage}
          showChatSheet={showChat}
          setShowChatSheet={setShowChat}
        />
      )}
      <ShowTextModal
        showModal={showCode}
        setShowModal={setShowCode}
        text={game?.id}
      />
    </>
  )
}

export default ChessBoard
