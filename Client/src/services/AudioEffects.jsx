import { CHESS_SOUNDS } from '../config'

export const playAudio = (audio) => {
  audio?.play()
}
export const getChessAudio = (moveData) => {
  let audio
  if (moveData?.isCheckmate) audio = new Audio(`${CHESS_SOUNDS}/checkmate.mp3`)
  else if (moveData?.isCheck) audio = new Audio(`${CHESS_SOUNDS}/check.mp3`)
  else if (moveData?.isDraw || moveData.isStalemate)
    audio = new Audio(`${CHESS_SOUNDS}/draw.mp3`)
  else if (moveData?.isCapture) audio = new Audio(`${CHESS_SOUNDS}/capture.mp3`)
  else if (moveData?.isCastle) audio = new Audio(`${CHESS_SOUNDS}/castle.mp3`)
  else if (moveData?.isPromotion)
    audio = new Audio(`${CHESS_SOUNDS}/promote.mp3`)
  else audio = new Audio(`${CHESS_SOUNDS}/move.mp3`)
  return audio
}
