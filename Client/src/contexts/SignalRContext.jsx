import { createContext, useContext, useState, useEffect } from 'react'
import { CHESS_HUB_URL } from '../config'
import * as signalR from '@microsoft/signalr'
import { useAuth } from './AuthContext'
const ChessSignalRContext = createContext()

function ChessSignalRProvider({ children }) {
  const [connection, setConnection] = useState(null)
  const { user } = useAuth()
  useEffect(() => {
    if (user?.token) {
      const newConnection = new signalR.HubConnectionBuilder()
        .withUrl(CHESS_HUB_URL, { accessTokenFactory: () => user?.token })
        .withAutomaticReconnect()
        .build()

      setConnection(newConnection)
    }
  }, [user])

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then(() => console.log('Connection started!'))
        .catch((err) =>
          console.log('Error while establishing connection :(', err),
        )
    }

    return () => {
      if (connection) {
        connection.stop()
      }
    }
  }, [connection])

  return (
    <ChessSignalRContext.Provider value={{ connection }}>
      {children}
    </ChessSignalRContext.Provider>
  )
}

function useChessSignal() {
  const context = useContext(ChessSignalRContext)
  if (context === undefined) {
    throw new Error('ChessSignalRContext was used outside ChessSignalRProvider')
  }
  return context
}

// eslint-disable-next-line react-refresh/only-export-components
export { ChessSignalRProvider, useChessSignal }
