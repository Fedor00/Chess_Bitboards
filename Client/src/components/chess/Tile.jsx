import React from 'react'
import { getPathForPiece } from '../../services/Utils'

function TileComponent({
  cell,
  i,
  j,
  isHighlighted,
  notation,
  onMouseDown,
  onTouchStart,
  isPieceSelected,
  isFromSquare,
  isToSquare,
}) {
  const calculateColor = () => {
    if ((i + j) % 2 === 0) {
      return 'bg-slate-700'
    } else {
      return 'bg-slate-400'
    }
  }

  const color = calculateColor()

  return (
    <div
      className={`relative flex aspect-square select-none items-center justify-center ${color}  `}
      onMouseDown={onMouseDown}
      onTouchStart={onTouchStart}
    >
      {/* Render notation on the side if needed */}
      {j === 0 && (
        <div className="absolute left-0 top-0 flex h-[20%] w-[20%] items-center justify-center text-xs md:text-sm lg:text-lg">
          {notation.col}
        </div>
      )}

      {i === 7 && (
        <div className="absolute bottom-0 right-0 flex h-[30%] w-[20%] items-center justify-center text-xs md:text-sm lg:text-lg">
          {notation.row}
        </div>
      )}

      {(isFromSquare || isToSquare) && (
        <div
          className={`absolute inset-0 m-auto block h-[90%] w-[90%] border ${isFromSquare ? 'border-white' : 'border-black'} rounded-[20%]`}
        ></div>
      )}

      {cell !== 'X' ? (
        <img
          src={getPathForPiece(cell)}
          draggable="false"
          alt=""
          className={`h-full w-full object-cover  ${isPieceSelected ? 'opacity-40' : ''} ${isHighlighted ? 'animate-bounce' : ''}`}
        />
      ) : (
        isHighlighted && (
          <div
            className={`absolute inset-0 m-auto block h-[90%] w-[90%] rounded-full border-4 border-black`}
          ></div>
        )
      )}
    </div>
  )
}

const Tile = React.memo(TileComponent)
export default Tile
