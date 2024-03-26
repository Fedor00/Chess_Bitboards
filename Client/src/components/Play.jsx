import { useEffect, useState } from 'react'

import { useAuth } from '../contexts/AuthContext'
import { getCurrentGame, makeMoveApi } from '../services/GameApi'
import { CHESS_SOUNDS, DEFAULT_PIECES } from '../config'
import Loading from './Loading'
import ChessBoard from './Chessboard'

function Play() {
  const { user } = useAuth()
  const [game, setGame] = useState()
  const updatGameState = (newGame) => {
    // If the user is not the white player, we need to flip the board
    if (user?.id !== newGame.bottomPlayerId) {
      newGame.pieces = newGame.pieces.map((row, rowIndex) => {
        return row.map((_, cellIndex) => {
          return newGame.pieces[7 - rowIndex][7 - cellIndex]
        })
      })
      //also need to flip the moves
      newGame.moves = newGame.blackMoves.map((move) => {
        move.from = 63 - move.from
        move.to = 63 - move.to
        return move
      })
    }
    console.log(newGame)
    setGame(newGame)
  }
  useEffect(() => {
    const fetchGame = async () => {
      const gameData = await getCurrentGame(user)
      if (gameData) updatGameState(gameData)
      else setGame(DEFAULT_PIECES)
    }
    if (user) fetchGame()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [user])

  const makeMove = async (from, to) => {
    const moveData = await makeMoveApi(user, from, to)
    console.log(moveData)
    if (moveData) updatGameState(moveData.gameDto)
    else setGame(DEFAULT_PIECES)
    playSound(moveData)
  }
  const playSound = (moveData) => {
    let audio
    if (moveData.isCheckmate) audio = new Audio(`${CHESS_SOUNDS}/checkmate.mp3`)
    else if (moveData.isCheck) audio = new Audio(`${CHESS_SOUNDS}/check.mp3`)
    else if (moveData.isDraw || moveData.isStalemate)
      audio = new Audio(`${CHESS_SOUNDS}/draw.mp3`)
    else if (moveData.isCapture)
      audio = new Audio(`${CHESS_SOUNDS}/capture.mp3`)
    else if (moveData.isCastle) audio = new Audio(`${CHESS_SOUNDS}/castle.mp3`)
    else if (moveData.isPromotion)
      audio = new Audio(`${CHESS_SOUNDS}/promotion.mp3`)
    else audio = new Audio(`${CHESS_SOUNDS}/move.mp3`)

    audio.play()
  }
  const updateGamePieces = (newPieces) => {
    setGame((current) => ({ ...current, pieces: newPieces }))
  }

  return (
    <div>
      {game ? (
        <ChessBoard
          game={game}
          updateGamePieces={updateGamePieces}
          makeMove={makeMove}
        />
      ) : (
        <div className="flex h-screen content-center justify-center">
          <Loading />
        </div>
      )}
    </div>
  )
}

export default Play
