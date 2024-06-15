import { cn } from '@/lib/utils'
import { HoverCard, HoverCardContent, HoverCardTrigger } from '../ui/hover-card'

function Message({ message, className }) {
  const timestamp = new Date(message.timestamp)
  const formattedTimestamp = timestamp.toLocaleString()

  return (
    <div
      className={cn(
        className,
        'flex cursor-pointer items-center break-all rounded-lg p-3 text-white',
      )}
    >
      <HoverCard openDelay={0} closeDelay={0}>
        <HoverCardTrigger>
          <p className="text-sm">{message.content}</p>
        </HoverCardTrigger>
        <HoverCardContent>{formattedTimestamp}</HoverCardContent>
      </HoverCard>
    </div>
  )
}

export default Message
