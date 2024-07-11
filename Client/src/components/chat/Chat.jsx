import { useEffect, useRef, useState } from 'react'
import { FiSend } from 'react-icons/fi'
import { IoClose } from 'react-icons/io5'
import PlayerName from '../chess/PlayerName'
import { useAuth } from '@/contexts/AuthContext'
import Messages from './Messages'

function Chat({
  chat,
  showChatSheet,
  setShowChatSheet,
  onSendMessage,
  opponentUsername,
}) {
  const [newMessage, setNewMessage] = useState('')
  const { user } = useAuth()
  console.log(chat)
  const currentPlayerUsername =
    chat?.firstPlayer?.id === user.id
      ? chat?.firstPlayer.userName
      : chat?.secondPlayer.userName

  const endOfMessagesRef = useRef(null)

  const sendMessage = () => {
    if (newMessage) {
      onSendMessage(newMessage)
      setNewMessage('')
    }
  }

  // Scroll to the bottom of the chat whenever messages are updated
  useEffect(() => {
    endOfMessagesRef.current?.scrollIntoView({ behavior: 'smooth' })
  }, [chat.chatMessages])

  return (
    <div
      className={`fixed inset-0 flex transform flex-col bg-stone-800 p-4 transition-transform sm:max-w-[750px] ${
        showChatSheet ? 'translate-x-0' : 'translate-x-full'
      }`}
    >
      <div className="mb-4 flex items-center justify-between">
        <h2 className="text-2xl font-bold text-white">Chat</h2>
        <button
          onClick={() => setShowChatSheet(false)}
          className="text-2xl text-white"
        >
          <IoClose />
        </button>
      </div>

      <div className="mb-4 flex justify-between">
        <PlayerName name={opponentUsername} />
        <PlayerName name={currentPlayerUsername} />
      </div>

      <div className="flex-1 overflow-y-auto rounded-lg bg-stone-600 p-1">
        <Messages messages={chat.chatMessages} user={user} />
        <div ref={endOfMessagesRef} />
      </div>

      <div className="mt-4 flex">
        <input
          value={newMessage}
          onChange={(e) => setNewMessage(e.target.value)}
          placeholder="Type a message..."
          className="mr-2 flex-1 rounded border p-2 focus:outline-none"
          onKeyPress={(e) => {
            if (e.key === 'Enter') {
              sendMessage()
              e.preventDefault()
            }
          }}
        />
        <button
          onClick={sendMessage}
          className="flex items-center justify-center rounded bg-blue-600 p-2 text-white"
        >
          <FiSend />
        </button>
      </div>
    </div>
  )
}

export default Chat
