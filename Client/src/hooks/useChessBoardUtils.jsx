function useChessBoardUtils(user, game) {
  const playerColor =
    (user?.id === game?.firstPlayer?.id && game?.isFirstPlayerWhite) ||
    (user?.id === game?.secondPlayer?.id && !game?.isFirstPlayerWhite)
      ? 'white'
      : 'black'
  const opponentUsername =
    user?.id === game?.firstPlayer?.id
      ? game?.secondPlayer?.userName || game?.engine?.engineName
      : game?.firstPlayer?.userName || game?.engine?.engineName

  const isPlayerTurn =
    (playerColor === 'white' && game?.isFirstPlayerWhite) ||
    (playerColor === 'black' && !game?.isFirstPlayerWhite)
  const isPromotionMove = (fromIndex, toIndex, piece, game) => {
    if (fromIndex < 8 || fromIndex > 15) return false
    if (toIndex > 7 || toIndex < 0) return false
    if (piece === 'P')
      return game.whiteMoves.some(
        (move) => move.from === fromIndex && move.to === toIndex,
      )
    else if (piece === 'p')
      return game.blackMoves.some(
        (move) => move.from === fromIndex && move.to === toIndex,
      )
    return false
  }
  return { playerColor, opponentUsername, isPlayerTurn, isPromotionMove }
}

export default useChessBoardUtils
