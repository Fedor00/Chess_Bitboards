import { useEffect, useState } from 'react'
import Board from './Board'
import { useAuth } from '../contexts/AuthContext'
import { getCurrentGame, makeMoveApi } from '../services/GameApi'
import { DEFAULT_PIECES } from '../config'

function Play() {
  const { user } = useAuth()
  const [game, setGame] = useState({ pieces: DEFAULT_PIECES })

  useEffect(() => {
    const fetchGame = async () => {
      const data = await getCurrentGame(user)
      console.log(data)
      setGame((current) => ({
        ...current,
        ...data,
      }))
    }

    if (user) fetchGame()
  }, [user])

  const makeMove = async (from, to) => {
    const gameData = await makeMoveApi(user, from, to)
    if (gameData) setGame(gameData)
  }

  const updateGamePieces = (newPieces) => {
    setGame((current) => ({ ...current, pieces: newPieces }))
  }

  return (
    <div className="h-full">
      <Board
        game={game}
        updateGamePieces={updateGamePieces}
        makeMove={makeMove}
      />
      ;
    </div>
  )
}

export default Play
