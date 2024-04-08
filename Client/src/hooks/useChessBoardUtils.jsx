function useChessBoardUtils(user, game) {
  const playerColor =
    (user?.id === game?.firstPlayer?.id && game?.isFirstPlayerWhite) ||
    (user?.id === game?.secondPlayer?.id && !game?.isFirstPlayerWhite)
      ? 'white'
      : 'black'
  const opponentUsername =
    user?.id === game?.firstPlayer?.id
      ? game?.secondPlayer?.userName
      : game?.firstPlayer?.userName
  const getNotationForTile = (i, j) => {
    const rowNotation = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h']
    const colNotation = ['8', '7', '6', '5', '4', '3', '2', '1']
    const col = playerColor === 'white' ? colNotation[i] : colNotation[7 - i]
    const row = playerColor === 'white' ? rowNotation[j] : rowNotation[7 - j]
    return { col, row }
  }
  const isPlayerTurn =
    (playerColor === 'white' && game?.isFirstPlayerWhite) ||
    (playerColor === 'black' && !game?.isFirstPlayerWhite)
  return { playerColor, opponentUsername, getNotationForTile, isPlayerTurn }
}

export default useChessBoardUtils
