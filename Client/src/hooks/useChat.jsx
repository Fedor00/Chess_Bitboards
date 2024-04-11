import { useAuth } from '@/contexts/AuthContext'
import { useChessSignal } from '@/contexts/SignalRContext'
import { addMessageApi, getCurrentGameChatApi } from '@/services/ChatApi'
import { useEffect, useState } from 'react'

function useChat(gameId) {
  const [chat, setChat] = useState(null)
  const [showChat, setShowChat] = useState(false)
  const [chatWithAi, setChatWithAi] = useState(false)
  const { user } = useAuth()
  const [unseenMessages, setUnseenMessages] = useState(0)
  const { connection } = useChessSignal()
  useEffect(() => {
    if (showChat) {
      setUnseenMessages(0)
    }
  }, [showChat])
  useEffect(() => {
    const setupSignalRListeners = () => {
      connection.on('ReceiveMessage', (chatMessages) => {
        console.log(chatMessages, 'chatMessages')
        setChat((chat) => ({
          ...chat,
          chatMessages: [...chat.chatMessages, chatMessages],
        }))
        if (!showChat) {
          setUnseenMessages((prev) => prev + 1)
        }
      })
    }
    if (connection) {
      setupSignalRListeners()
      return () => {
        connection.off('ReceiveMessage')
      }
    }
  }, [connection, showChat, user])
  useEffect(() => {
    const fetchChat = async () => {
      const chatData = await getCurrentGameChatApi(user, gameId)
      if (!chatData.secondUser) setChatWithAi(true)
      setChat(chatData)
    }
    if (user) fetchChat()
  }, [gameId, user])

  const addMessage = async (content) => {
    await addMessageApi(user, content, gameId)
  }

  return {
    chat,
    addMessage,
    showChat,
    setShowChat,
    chatWithAi,
    unseenMessages,
    setUnseenMessages,
  }
}

export default useChat
