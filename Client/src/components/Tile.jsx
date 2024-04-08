import React from 'react'
import { getPathForPiece } from '../services/Utils'
import { useRenderCount } from '@/hooks/useRenderCount'

function TileComponent({
  cell,
  i,
  j,
  isHighlighted,
  notation,
  onMouseDown,
  isPieceSelected,
}) {
  const calculateColor = () => {
    if ((i + j) % 2 === 0) {
      return 'bg-neutral-600'
    } else {
      return 'bg-neutral-400'
    }
  }
  const color = calculateColor()
  return (
    <div
      className={`relative flex aspect-square select-none items-center justify-center ${color}  `}
      onMouseDown={onMouseDown}
    >
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

      {cell !== 'X' ? (
        <img
          src={getPathForPiece(cell)}
          draggable="false"
          alt=""
          className={`h-full w-full object-cover ${isPieceSelected ? 'opacity-40' : ''} ${isHighlighted ? 'animate-pulse' : ''}`}
        />
      ) : (
        isHighlighted && (
          <div className=" h-[calc(100%/4)] w-[calc(100%/4)] rounded-full bg-black"></div>
        )
      )}
    </div>
  )
}
const Tile = React.memo(TileComponent)
export default Tile
