import { useReducer, useCallback, useEffect } from 'react'
import { getChessBoardIndex, isOutOfBounds } from '../services/Utils'

const initialState = {
  isDragging: false,
  mousePosition: { x: 0, y: 0 },
  imgSize: { width: 0, height: 0 },
  selectedPiece: { i: null, j: null, piece: null },
}

function dragReducer(state, action) {
  switch (action.type) {
    case 'START_DRAG':
      return {
        ...state,
        isDragging: true,
        mousePosition: action.mousePosition,
        imgSize: action.imgSize,
        selectedPiece: action.selectedPiece,
      }
    case 'MOVE_DRAG':
      return {
        ...state,
        mousePosition: action.mousePosition,
      }
    case 'END_DRAG':
      return {
        ...state,
        isDragging: false,
      }
    case 'RESET':
      return initialState
    default:
      throw new Error(`Unhandled action type: ${action.type}`)
  }
}

export function useDragPiece(game, chessBoardRef, makeMove, playerColor) {
  const [state, dispatch] = useReducer(dragReducer, initialState)

  const handleDragStart = useCallback(
    (e, cell, rowIndex, colIndex) => {
      if (isOutOfBounds(rowIndex, colIndex) || cell === 'X') return

      const piece = game.pieces[rowIndex][colIndex]
      const selectedPiece = { i: rowIndex, j: colIndex, piece }
      const mousePosition = { x: e.clientX, y: e.clientY }

      if (chessBoardRef.current) {
        const { width } = chessBoardRef.current.getBoundingClientRect()
        const tileWidth = width / game.pieces[0].length
        const imgSize = { width: tileWidth, height: tileWidth }

        dispatch({
          type: 'START_DRAG',
          selectedPiece,
          mousePosition,
          imgSize,
        })
      }
    },
    [game.pieces, chessBoardRef],
  )
  const handleDragMove = useCallback(
    (x, y) => {
      if (!state.isDragging) return
      const mousePosition = { x: x, y: y }
      dispatch({ type: 'MOVE_DRAG', mousePosition })
    },
    [state.isDragging],
  )
  const handleDragEnd = useCallback(async () => {
    if (!state.isDragging || !chessBoardRef.current) return
    const { left, top, width } = chessBoardRef.current.getBoundingClientRect()
    const tileWidth = width / game.pieces[0].length
    const newRowIndex = Math.floor((state.mousePosition.y - top) / tileWidth)
    const newColIndex = Math.floor((state.mousePosition.x - left) / tileWidth)
    const fromIndex = getChessBoardIndex(
      state.selectedPiece.i,
      state.selectedPiece.j,
    )
    const toIndex = getChessBoardIndex(newRowIndex, newColIndex)
    if (!isOutOfBounds(newRowIndex, newColIndex)) {
      try {
        if (
          fromIndex !== toIndex &&
          ((playerColor === 'white' && game.isWhiteTurn) ||
            (playerColor === 'black' && !game.isWhiteTurn))
        )
          await makeMove(fromIndex, toIndex)
      } catch (error) {
        console.log('error')
      } finally {
        dispatch({ type: 'END_DRAG' })
        dispatch({ type: 'RESET' })
      }
    } else dispatch({ type: 'END_DRAG' })
  }, [
    state.isDragging,
    state.mousePosition.y,
    state.mousePosition.x,
    state.selectedPiece.i,
    state.selectedPiece.j,
    chessBoardRef,
    game.pieces,
    game.isWhiteTurn,
    playerColor,
    makeMove,
  ])

  useEffect(() => {
    const mouseUpHandler = (e) => handleDragEnd(e)
    const mouseMoveHandler = (e) => {
      const chessBoard = chessBoardRef.current
      const rect = chessBoard.getBoundingClientRect()
      const clamp = (num, min, max) => Math.min(Math.max(num, min), max)
      const clampedX = clamp(e.clientX, rect.left, rect.right)
      const clampedY = clamp(e.clientY, rect.top, rect.bottom)
      handleDragMove(clampedX, clampedY)
    }
    document.addEventListener('mouseup', mouseUpHandler)
    document.addEventListener('mousemove', mouseMoveHandler)

    return () => {
      document.removeEventListener('mouseup', mouseUpHandler)
      document.removeEventListener('mousemove', mouseMoveHandler)
    }
  }, [chessBoardRef, handleDragEnd, handleDragMove])

  return {
    ...state,
    handleDragStart,
  }
}
