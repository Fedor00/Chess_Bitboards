import { useEffect, useState } from 'react'

import { useAuth } from '../contexts/AuthContext'
import {
  createPrivateGame,
  getCurrentGame,
  joinPrivateGame,
  makeMoveApi,
  matchGameApi,
} from '../services/GameApi'

import ChessBoard from './Chessboard'
import { flipMoves, flipPieces } from '../services/Utils'
import { getChessAudio, playAudio } from '../services/AudioEffects'
import { CHESS_SOUNDS, DEFAULT_PIECES } from '../config'
import ChessPlayOptions from './ChessPlayOptions'
import PlayerName from './PlayerName'
import ShowTextModal from './ShowTextModal'
import Loading from './Loading'
import { useChessSignal } from '../contexts/SignalRContext'

function Play() {
  const { user } = useAuth()
  const [game, setGame] = useState()
  const [showModal, setShowModal] = useState(false)
  const { connection } = useChessSignal()
  useEffect(() => {
    if (connection) {
      connection.on('MoveMade', (moveData) => {
        updatGameState(moveData.gameDto)
      })
      connection.on('GameStarted', (gameData) => {
        updatGameState(gameData)
        playAudio(new Audio(`${CHESS_SOUNDS}/notify.mp3`))
      })
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [connection])

  const updatGameState = (newGame) => {
    if (user?.id !== newGame.bottomPlayerId) {
      newGame.pieces = flipPieces(newGame.pieces)
      newGame.moves = flipMoves(newGame.blackMoves)
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
    const moveData = await makeMoveApi(user, from, to)
    playSound(moveData)
  }
  const playSound = (moveData) => {
    if (moveData) {
      const audio = getChessAudio(moveData)
      playAudio(audio)
    }
  }
  const updateGamePieces = (newPieces) => {
    setGame((current) => ({ ...current, pieces: newPieces }))
  }
  const handlePlayAi = async () => {
    //TODO: Implement playing against AI
  }
  const handleCreatePrivate = async () => {
    const data = await createPrivateGame(user, true)
    if (data) {
      setShowModal(true)
      updatGameState(data)
    }
  }
  const handleJoinPrivate = async (gameId) => {
    const data = await joinPrivateGame(user, gameId)
    if (data) updatGameState(data)
  }
  const handlePlayRandom = async () => {
    const data = await matchGameApi(user, false)
    if (data) updatGameState(data)
  }
  return (
    <>
      {game ? (
        <div>
          <div className="flex flex-row content-center justify-center px-3">
            <div className="mr-5">
              <PlayerName name={user?.username} />
            </div>

            <div className="ml-5">
              {game?.topPlayerId ? <PlayerName name="opponent" /> : <Loading />}
            </div>
          </div>
          <div>
            <ChessBoard
              game={game}
              updateGamePieces={updateGamePieces}
              makeMove={makeMove}
            />
          </div>
        </div>
      ) : (
        <ChessPlayOptions
          handlePlayAi={handlePlayAi}
          handleJoinPrivate={handleJoinPrivate}
          handleCreatePrivate={handleCreatePrivate}
          handlePlayRandom={handlePlayRandom}
        />
      )}
      <ShowTextModal
        showModal={showModal}
        setShowModal={setShowModal}
        text={game?.id}
      />
    </>
  )
}

export default Play
