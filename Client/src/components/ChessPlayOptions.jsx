import { useState } from 'react'

function ChessPlayOptions({
  handlePlayRandom,
  handleCreatePrivate,
  handleJoinPrivate,
  handlePlayAi,
}) {
  const [playFriend, setPlayFriend] = useState(false)
  const [gameId, setGameId] = useState('')

  return (
    <div className="mt-2  flex h-[92vh] w-screen justify-center">
      {!playFriend ? (
        <div className="grid grid-cols-1 place-items-center">
          <div className="w-full max-w-xl px-5">
            <button
              onClick={handlePlayRandom}
              className="mb-1 w-full rounded bg-stone-900 px-7 py-4 font-bold text-white hover:bg-stone-800"
            >
              Play Random
            </button>
            <button
              onClick={() => setPlayFriend(true)}
              className="mb-1 w-full rounded bg-stone-900 px-7 py-4 font-bold text-white hover:bg-stone-800"
            >
              Play Against Friend
            </button>
            <button
              onClick={handlePlayAi}
              className="mb-1 w-full rounded bg-stone-900 px-7 py-4 font-bold text-white hover:bg-stone-800"
            >
              Play Against Ai
            </button>
          </div>
        </div>
      ) : (
        <div className="flex w-full max-w-lg flex-col content-center justify-center">
          <div className="w-full">
            {playFriend && (
              <div className="mb-1">
                <div className="relative flex h-10 w-full ">
                  <button
                    className="peer-placeholder-shown:bg-blue-gray-500 !absolute right-1 top-1 z-10 select-none rounded bg-stone-700 px-4 py-2 text-center align-middle font-sans text-xs font-bold uppercase text-white shadow-md shadow-pink-500/20 transition-all hover:shadow-lg hover:shadow-pink-500/40 focus:opacity-[0.85] focus:shadow-none active:opacity-[0.85] active:shadow-none peer-placeholder-shown:pointer-events-none peer-placeholder-shown:opacity-50 peer-placeholder-shown:shadow-none"
                    type="button"
                    data-ripple-light="true"
                    onClick={() => handleJoinPrivate(gameId)}
                  >
                    Join
                  </button>
                  <input
                    type="email"
                    className="border-blue-gray-200 text-blue-gray-700 placeholder-shown:border-blue-gray-200 placeholder-shown:border-t-blue-gray-200 disabled:bg-blue-gray-50 peer h-full w-full rounded-[7px] border bg-transparent px-3 py-2.5 pr-20 font-sans text-sm font-normal text-white outline outline-0 transition-all placeholder-shown:border focus:border-2 focus:border-pink-500 focus:border-t-transparent focus:outline-0 disabled:border-0"
                    placeholder="Type Game Id"
                    required
                    value={gameId}
                    onChange={(e) => setGameId(e.target.value)}
                  />
                </div>
              </div>
            )}
            <button
              onClick={handleCreatePrivate}
              className="mb-1 w-full rounded bg-stone-900 px-6 py-4 font-bold text-white hover:bg-stone-800"
            >
              Create
            </button>

            <button
              onClick={() => setPlayFriend(false)}
              className="w-full rounded bg-stone-900 px-6 py-4 font-bold text-white hover:bg-stone-800"
            >
              Cancel
            </button>
          </div>
        </div>
      )}
    </div>
  )
}

export default ChessPlayOptions
