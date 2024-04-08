import { useCallback, useEffect } from 'react'
import { useAuth } from '../contexts/AuthContext'
import {
  createPrivateGame,
  getCurrentGame,
  joinPrivateGame,
  makeMoveApi,
  matchGameApi,
  resignGameApi,
} from '../services/GameApi'
import { flipMoves, flipPieces } from '../services/Utils'
import { getChessAudio, playAudio } from '../services/AudioEffects'
import { CHESS_SOUNDS, DEFAULT_PIECES } from '../config'
import ChessPlayOptions from './ChessPlayOptions'
import ShowTextModal from './ShowTextModal'
import { useChessSignal } from '../contexts/SignalRContext'
import GameOverModal from './GameOverModal'
import ChessBoard from './ChessBoard'
import useGameReducer, { GameActionTypes } from '../reducers/useGameReducer'
import useChessBoardUtils from '../hooks/useChessBoardUtils'

function Play() {
  const { user } = useAuth()
  const { state: gameState, dispatch } = useGameReducer()
  const { playerColor } = useChessBoardUtils(user, gameState.game)
  const { connection } = useChessSignal()
  const processGameUpdate = useCallback(
    (gameData) => {
      let updatedGame = { ...gameData }
      console.log(playerColor)
      if (playerColor === 'black') {
        updatedGame = {
          ...updatedGame,
          pieces: flipPieces(updatedGame.pieces),
          blackMoves: flipMoves(updatedGame.blackMoves),
        }
      }
      return updatedGame
    },
    [playerColor],
  )
  const handleMoveMade = useCallback(
    (moveData) => {
      if (!moveData) return
      playAudio(getChessAudio(moveData))
      dispatch({
        type: GameActionTypes.UPDATE_GAME,
        payload: processGameUpdate(moveData?.gameDto),
      })

      if (
        moveData?.gameDto?.status !== 'waiting' &&
        moveData?.gameDto?.status !== 'playing'
      ) {
        dispatch({
          type: GameActionTypes.GAME_OVER,
          payload: moveData?.gameDto?.status,
        })
      }
    },
    [dispatch, processGameUpdate],
  )
  useEffect(() => {
    const setupSignalRListeners = () => {
      connection.on('MoveMade', (moveData) => {
        handleMoveMade(moveData)
      })
      connection.on('GameStarted', (gameData) => {
        dispatch({
          type: GameActionTypes.UPDATE_GAME,
          payload: processGameUpdate(gameData),
        })
        playAudio(new Audio(`${CHESS_SOUNDS}/notify.mp3`))
      })
      connection.on('GameResigned', (gameStatus) => {
        dispatch({ type: GameActionTypes.GAME_OVER, payload: gameStatus })
        playAudio(new Audio(`${CHESS_SOUNDS}/checkmate.mp3`))
      })
    }
    if (connection) {
      setupSignalRListeners()
      return () => {
        connection.off('MoveMade')
        connection.off('GameStarted')
        connection.off('GameResigned')
      }
    }
  }, [connection, dispatch, handleMoveMade, processGameUpdate, user])

  useEffect(() => {
    const fetchGame = async () => {
      const gameData = await getCurrentGame(user)
      dispatch({
        type: GameActionTypes.UPDATE_GAME,
        payload: gameData ? processGameUpdate(gameData) : DEFAULT_PIECES,
      })
    }

    if (user) fetchGame()
  }, [user, dispatch, processGameUpdate])

  const makeMove = async (from, to) => {
    await makeMoveApi(user, from, to)
    // Consider dispatching an update action after making a move if necessary.
  }

  const handleCreatePrivate = async () => {
    const data = await createPrivateGame(user, true)
    if (data) {
      dispatch({ type: GameActionTypes.SHOW_MODAL })
      dispatch({
        type: GameActionTypes.UPDATE_GAME,
        payload: processGameUpdate(data),
      })
    }
  }

  const handleJoinPrivate = async (gameId) => {
    const data = await joinPrivateGame(user, gameId)
    if (data)
      dispatch({
        type: GameActionTypes.UPDATE_GAME,
        payload: processGameUpdate(data),
      })
  }

  const handlePlayRandom = async () => {
    const data = await matchGameApi(user, false)
    if (data)
      dispatch({
        type: GameActionTypes.UPDATE_GAME,
        payload: processGameUpdate(data),
      })
  }

  const resign = async () => {
    await resignGameApi(user)
    //dispatch({ type: 'GAME_OVER' })
  }

  return (
    <>
      {gameState.game && (
        <ChessBoard
          game={gameState.game}
          updateGamePieces={(newPieces) =>
            dispatch({
              type: GameActionTypes.UPDATE_PIECES,
              payload: newPieces,
            })
          }
          makeMove={makeMove}
          resign={resign}
        />
      )}
      {!gameState.game && (
        <ChessPlayOptions
          handlePlayAi={() => {}}
          handleJoinPrivate={handleJoinPrivate}
          handleCreatePrivate={handleCreatePrivate}
          handlePlayRandom={handlePlayRandom}
        />
      )}
      <ShowTextModal
        showModal={gameState.showModal}
        setShowModal={(show) =>
          dispatch({ type: GameActionTypes.TOGGLE_MODAL, payload: show })
        }
        text={gameState.game?.id}
      />
      <GameOverModal
        showModal={gameState.showGameOver}
        setShowModal={(show) =>
          dispatch({
            type: GameActionTypes.TOGGLE_GAME_OVER_MODAL,
            payload: show,
          })
        }
        onClose={() => dispatch({ type: GameActionTypes.RESET_GAME })}
        gameStatus={gameState.gameStatus}
      />
    </>
  )
}

export default Play
