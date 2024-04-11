import {
  Sheet,
  SheetContent,
  SheetFooter,
  SheetHeader,
  SheetTitle,
} from '../ui/sheet'
import { FiSend } from 'react-icons/fi'

import { Input } from '../ui/input'
import { Button } from '../ui/button'
import { useEffect, useRef, useState } from 'react'
import PlayerName from '../chess/PlayerName'
import { useAuth } from '@/contexts/AuthContext'
import Messages from './Messages'

function Chat({ chat, showChatSheet, setShowChatSheet, onSendMessage }) {
  const [newMessage, setNewMessage] = useState('')
  const { user } = useAuth()

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
    <Sheet open={showChatSheet} onOpenChange={setShowChatSheet}>
      <SheetContent className="flex h-screen w-screen flex-col bg-stone-800 p-4 sm:max-w-[750px]">
        <SheetHeader>
          <SheetTitle className="text-center">Chat</SheetTitle>
        </SheetHeader>
        <div className="flex justify-between ">
          <PlayerName name={chat?.firstPlayer?.userName} />
          <PlayerName
            name={chat?.secondPlayer ? chat.secondPlayer.userName : 'Engine'}
          />
        </div>
        <div className="overflow-y-auto rounded-lg ">
          <div className="rounded-lg bg-stone-600">
            <div className="p-1">
              <Messages messages={chat.chatMessages} user={user} />
            </div>

            <div ref={endOfMessagesRef} />
          </div>
        </div>

        <SheetFooter className="mt-auto">
          <div className="flex w-full">
            <Input
              value={newMessage}
              onChange={(e) => setNewMessage(e.target.value)}
              placeholder="Type a message..."
              className="mr-2 flex-1"
            />
            <Button onClick={sendMessage}>
              <FiSend />
            </Button>
          </div>
        </SheetFooter>
      </SheetContent>
    </Sheet>
  )
}

export default Chat
