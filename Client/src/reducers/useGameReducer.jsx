import { useReducer } from 'react'
export const GameActionTypes = {
  UPDATE_GAME: 'UPDATE_GAME',
  GAME_OVER: 'GAME_OVER',
  SHOW_MODAL: 'SHOW_MODAL',
  TOGGLE_MODAL: 'TOGGLE_MODAL',
  TOGGLE_GAME_OVER_MODAL: 'TOGGLE_GAME_OVER_MODAL',
  UPDATE_PIECES: 'UPDATE_PIECES',
  RESET_GAME: 'RESET_GAME',
}

const initialState = {
  game: null,
  showModal: false,
  showGameOver: false,
  gameStatus: '',
}
function gameReducer(state, action) {
  switch (action.type) {
    case GameActionTypes.UPDATE_GAME:
      return { ...state, game: action.payload }
    case GameActionTypes.GAME_OVER:
      return { ...state, showGameOver: true, gameStatus: action.payload }
    case GameActionTypes.SHOW_MODAL:
      return { ...state, showModal: true }
    case GameActionTypes.TOGGLE_MODAL:
      return { ...state, showModal: !state.showModal }
    case GameActionTypes.TOGGLE_GAME_OVER_MODAL:
      return { ...state, showGameOver: !state.showGameOver }
    case GameActionTypes.UPDATE_PIECES:
      return { ...state, game: { ...state.game, pieces: action.payload } }
    case GameActionTypes.RESET_GAME:
      return initialState
    default:
      throw new Error(`Unhandled action type: ${action.type}`)
  }
}

function useGameReducer(initialGameState) {
  const initialState = {
    game: initialGameState,
    showGameOver: false,
    gameStatus: '',
  }
  const [state, dispatch] = useReducer(gameReducer, initialState)
  return { state, dispatch }
}

export default useGameReducer
