import { useState, useEffect, useRef } from 'react'
import { MIDNGHT_PATH } from '../config'

function Tile({ cell, i, j, whenDragged, whenDragEnd, draggedCell }) {
  const [isDragging, setIsDragging] = useState(false)
  const [mousePosition, setMousePosition] = useState({ x: 0, y: 0 })
  const imgRef = useRef(null)
  const [imgSize, setImgSize] = useState({ width: 0, height: 0 })

  const getPath = (piece) => {
    const pieceColor = piece === piece.toLowerCase() ? 'b' : 'w'
    return `${MIDNGHT_PATH}${pieceColor}${piece.toUpperCase()}.svg`
  }

  const handleMouseDown = (e) => {
    if (cell === 'X' || isMargin()) return
    whenDragged(i, j)
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
      if (draggedCell.piece === 'X') return
      whenDragEnd(e.clientX, e.clientY)
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
  }, [draggedCell, i, isDragging, j, whenDragEnd])

  const isMargin = () => i === 0 || i === 9 || j === 0 || j === 9

  return (
    <div
      className={`flex aspect-square select-none items-center justify-center ${isMargin() ? 'bg-brown-500 text-white' : (i + j) % 2 === 0 ? 'bg-neutral-600' : 'bg-neutral-400'}`}
      onMouseDown={handleMouseDown}
    >
      {isMargin() ? (
        <span className="text-start font-bold">{cell}</span>
      ) : (
        cell !== 'X' && (
          <img
            ref={imgRef}
            src={getPath(cell)}
            draggable="false"
            alt=""
            className="object-contain"
          />
        )
      )}

      {isDragging && draggedCell && (
        <img
          src={getPath(draggedCell)}
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
