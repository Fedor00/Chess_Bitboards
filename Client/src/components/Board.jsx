import React, { useRef, useState } from 'react'

import Tile from './Tile'

function Board({ board, setBoard }) {
  const chessBoardRef = useRef(null)
  const [draggedCell, setDraggedCell] = useState({
    i: null,
    j: null,
    piece: null,
  })

  const whenDragged = (i, j) => {
    setDraggedCell({ i, j, piece: board[i][j] })
    const newBoard = board.map((row) => [...row])
    newBoard[i][j] = 'X'
    setBoard(newBoard)
  }

  const whenDragEnd = (mouseX, mouseY) => {
    if (!chessBoardRef.current) return
    const { left, top, width } = chessBoardRef.current.getBoundingClientRect()
    const tileWidth = width / board[0].length

    const newI = Math.floor((mouseY - top) / tileWidth)
    const newJ = Math.floor((mouseX - left) / tileWidth)
    console.log(newI, newJ)
    if (
      newI >= 1 &&
      newI < board.length - 1 &&
      newJ >= 1 &&
      newJ < board[0].length - 1
    ) {
      const newBoard = board.map((row) => [...row])
      newBoard[newI][newJ] = draggedCell.piece
      setBoard(newBoard)
    } else {
      const newBoard = board.map((row) => [...row])
      newBoard[draggedCell.i][draggedCell.j] = draggedCell.piece
      setBoard(newBoard)
    }
  }
  return (
    <div className=" max-w-screen flex h-screen max-h-[93vh] w-full items-center justify-center">
      <div className="flex">
        <div>
          <div
            ref={chessBoardRef}
            className="grid grid-cols-10"
            style={{
              width: 'calc(100vmin )',
              height: 'calc(100vmin - 6rem)',
            }}
          >
            {board.map((row, i) => (
              <React.Fragment key={i}>
                {row.map((cell, j) => (
                  <div key={`${i}-${j}`} className="w-full aspect-square">
                    <Tile
                      cell={cell}
                      i={i}
                      j={j}
                      whenDragged={() => whenDragged(i, j)}
                      whenDragEnd={whenDragEnd}
                      draggedCell={draggedCell.piece}
                    />
                  </div>
                ))}
              </React.Fragment>
            ))}
          </div>
        </div>
      </div>
    </div>
  )
}

export default Board
