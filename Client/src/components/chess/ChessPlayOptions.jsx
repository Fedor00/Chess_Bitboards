import { useState } from 'react'
import { Button } from '../ui/button'
import { Input } from '../ui/input'

function ChessPlayOptions({
  handlePlayRandom,
  handleCreatePrivate,
  handleJoinPrivate,
  handlePlayAi,
}) {
  const [playFriend, setPlayFriend] = useState(false)
  const [gameId, setGameId] = useState('')
  const [stockfishDepth, setStockfishDepth] = useState(5)
  const [fedorDepth, setFedorDepth] = useState(5)

  return (
    <div className="flex h-[90vh] w-full justify-center">
      {!playFriend ? (
        <div className="grid grid-cols-1 place-items-center">
          <div className="w-full max-w-xl space-y-1">
            <Button onClick={handlePlayRandom} className="w-full py-8">
              Play Random
            </Button>
            <Button onClick={() => setPlayFriend(true)} className="w-full py-8">
              Play Against Friend
            </Button>
            <Button
              onClick={() => handlePlayAi('dummy')}
              className="w-full py-8"
            >
              Play Against Dummy AI
            </Button>
            <div className="rounded-lg bg-black bg-opacity-60 p-4 shadow">
              <div>
                <label className="text-slate-300">
                  Stockfish Level ({stockfishDepth})
                </label>
                <input
                  type="range"
                  min="1"
                  max="20"
                  value={stockfishDepth}
                  onChange={(e) => setStockfishDepth(parseInt(e.target.value))}
                  className="w-full"
                />
                <Button
                  onClick={() => handlePlayAi('stockfish', stockfishDepth)}
                  className="w-full py-8"
                >
                  Play Against Stockfish
                </Button>
              </div>
            </div>
            <div className="rounded-lg bg-black bg-opacity-60 p-4 shadow">
              <div>
                <label className="text-slate-300">
                  Fedor Level ({fedorDepth})
                </label>
                <input
                  type="range"
                  min="1"
                  max="10"
                  value={fedorDepth}
                  onChange={(e) => setFedorDepth(parseInt(e.target.value))}
                  className="w-full"
                />
                <Button
                  onClick={() => handlePlayAi('fedor', fedorDepth)}
                  className="w-full py-8"
                >
                  Play Against Fedor
                </Button>
              </div>
            </div>
          </div>
        </div>
      ) : (
        <div className="flex w-full max-w-lg flex-col content-center justify-center">
          <div className="w-full space-y-1">
            {playFriend && (
              <div className="relative flex h-10 w-full space-x-1">
                <Button
                  onClick={() => handleJoinPrivate(gameId)}
                  className="px-5"
                >
                  Join
                </Button>
                <Input
                  type="text"
                  className="h-full w-full rounded-lg bg-stone-800 px-2 py-1 text-slate-200"
                  placeholder="Type Game Id"
                  required
                  value={gameId}
                  onChange={(e) => setGameId(e.target.value)}
                />
              </div>
            )}
            <Button onClick={handleCreatePrivate} className="w-full py-8">
              Create
            </Button>
            <Button
              onClick={() => setPlayFriend(false)}
              className="w-full py-8"
            >
              Cancel
            </Button>
          </div>
        </div>
      )}
    </div>
  )
}

export default ChessPlayOptions
