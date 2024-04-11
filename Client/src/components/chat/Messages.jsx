import { cn } from '@/lib/utils'
import Message from './Message'

function Messages({ messages, user }) {
  return (
    <>
      {messages.map((message, index) => (
        <div key={index}>
          <div
            className={cn(
              {
                ' ml-3 justify-end': message.senderId === user.id,
                ' mr-3 justify-start': message.senderId !== user.id,
              },
              'mb-1 mt-1 flex',
            )}
          >
            <Message
              message={message}
              className={cn({
                'bg-stone-800': message.senderId === user.id,
                ' bg-stone-900': message.senderId !== user.id,
              })}
            />
          </div>
        </div>
      ))}
    </>
  )
}

export default Messages
