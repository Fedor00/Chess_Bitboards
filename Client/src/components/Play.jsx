import { useEffect, useState } from 'react'
import Board from './Board'
import { useAuth } from '../contexts/AuthContext'
import { getCurrentGame, makeMoveApi } from '../services/GameApi'
import { DEFAULT_PIECES } from '../config'
import Loading from './Loading'

function Play() {
  const { user } = useAuth()
  const [game, setGame] = useState()
  const updatGameState = (newGame) => {
    // If the user is not the white player, we need to flip the board
    if (user?.id !== newGame.bottomPlayerId) {
      newGame.pieces = newGame.pieces.map((row, rowIndex) => {
        return row.map((_, cellIndex) => {
          // we use 9 because the board is 10x10 in order to include  border annotations
          return newGame.pieces[9 - rowIndex][9 - cellIndex]
        })
      })
      //also need to flip the moves
      newGame.blackMoves = newGame.blackMoves.map((move) => {
        move.from = 63 - move.from
        move.to = 63 - move.to
        return move
      })
    }
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
    const gameData = await makeMoveApi(user, from, to)
    if (gameData) updatGameState(gameData)
    else setGame(DEFAULT_PIECES)
  }

  const updateGamePieces = (newPieces) => {
    setGame((current) => ({ ...current, pieces: newPieces }))
  }

  return (
    <div className="h-full">
      {game ? (
        <Board
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
