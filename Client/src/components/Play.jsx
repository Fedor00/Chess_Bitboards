import { useState } from 'react'
import Board from './Board'

function Play() {
  const [board, setBoard] = useState([
    [' ', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', ' '],
    ['8', 'r', 'n', 'b', 'q', 'k', 'b', 'n', 'r', '8'],
    ['7', 'p', 'p', 'p', 'p', 'p', 'p', 'p', 'p', '7'],
    ['6', 'X', 'X', 'X', 'X', 'X', 'X', 'X', 'X', '6'],
    ['5', 'X', 'X', 'X', 'X', 'X', 'X', 'X', 'X', '5'],
    ['4', 'X', 'X', 'X', 'X', 'X', 'X', 'X', 'X', '4'],
    ['3', 'X', 'X', 'X', 'X', 'X', 'X', 'X', 'X', '3'],
    ['2', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P', '2'],
    ['1', 'R', 'N', 'B', 'Q', 'K', 'B', 'N', 'R', '1'],
    [' ', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', ' '],
  ])

  const annotations = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h']

  return (
    <div className="h-full">
      <Board board={board} annotations={annotations} setBoard={setBoard} />;
    </div>
  )
}

export default Play
