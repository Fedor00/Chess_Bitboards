import { useState, useEffect, useRef } from 'react'
import { MIDNIGHT_PATH } from '../config'

function Tile({
  cell,
  i,
  j,
  onPieceSelected,
  onDragEnd,
  selectedPiece,
  isHighlighted,
}) {
  const [isDragging, setIsDragging] = useState(false)
  const [mousePosition, setMousePosition] = useState({ x: 0, y: 0 })
  const imgRef = useRef(null)
  const [imgSize, setImgSize] = useState({ width: 0, height: 0 })

  const getPath = (piece) => {
    const pieceColor = piece === piece.toLowerCase() ? 'b' : 'w'
    return `${MIDNIGHT_PATH}${pieceColor}${piece.toUpperCase()}.svg`
  }

  const handleMouseDown = (e) => {
    if (cell === 'X' || isMargin()) return
    onPieceSelected(i, j)
    setIsDragging(true)
    setMousePosition({ x: e.clientX, y: e.clientY })
    if (imgRef.current) {
      setImgSize({
        width: imgRef.current.offsetWidth,
        height: imgRef.current.offsetHeight,
      })
    }
  }

  useEffect(() => {
    const handleMouseUp = (e) => {
      if (selectedPiece.piece === 'X') return
      onDragEnd(e.clientX, e.clientY)
      setIsDragging(false)
    }

    const handleMouseMove = (e) => {
      setMousePosition({ x: e.clientX, y: e.clientY })
    }

    if (isDragging) {
      document.addEventListener('mousemove', handleMouseMove)
      document.addEventListener('mouseup', handleMouseUp)
    }
    return () => {
      document.removeEventListener('mousemove', handleMouseMove)
      document.removeEventListener('mouseup', handleMouseUp)
    }
  }, [selectedPiece, i, isDragging, j, onDragEnd])

  const isMargin = () => i === 0 || i === 9 || j === 0 || j === 9
  const calculateColor = () => {
    if (isMargin()) {
      return ' text-white'
    } else if ((i + j) % 2 === 0) {
      return 'bg-neutral-600'
    } else {
      return 'bg-neutral-400'
    }
  }
  const borderClass = isMargin() ? '' : 'border-1 border-red-950 '

  const color = calculateColor()

  return (
    <div
      className={` flex  aspect-square select-none items-center justify-center  ${color} ${borderClass} `}
      onMouseDown={handleMouseDown}
    >
      {isMargin() ? (
        <span className="text-start font-bold">{cell}</span>
      ) : cell !== 'X' ? (
        <img
          ref={imgRef}
          src={getPath(cell)}
          draggable="false"
          alt=""
          className={isHighlighted ? 'animate-pulse' : ''}
        />
      ) : (
        isHighlighted && (
          <div className=" h-[calc(100%/4)] w-[calc(100%/4)] rounded-full bg-black"></div>
        )
      )}

      {isDragging && selectedPiece && (
        <img
          src={getPath(selectedPiece)}
          alt="Dragging piece"
          className="object-contain"
          style={{
            width: imgSize.width,
            height: imgSize.height,
            position: 'fixed',
            left: mousePosition.x - imgSize.width / 2,
            top: mousePosition.y - imgSize.height / 2,
            pointerEvents: 'none',
          }}
        />
      )}
    </div>
  )
}

export default Tile
