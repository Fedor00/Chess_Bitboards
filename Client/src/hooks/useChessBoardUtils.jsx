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
  return { playerColor, opponentUsername, isPlayerTurn }
}

export default useChessBoardUtils
