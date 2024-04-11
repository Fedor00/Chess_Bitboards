import axios from 'axios'
import { CHAT_API_URL } from '../config'
const getCurrentGameChatApi = async (user, gameId) => {
  try {
    const resp = await axios.get(`${CHAT_API_URL}/${gameId}`, {
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${user?.token}`,
      },
    })
    return resp.data
  } catch (error) {
    console.log(error)
    throw new Error('Failed to retrieve chat.')
  }
}

const addMessageApi = async (user, content, gameId) => {
  try {
    const resp = await axios.post(
      `${CHAT_API_URL}`,
      { senderId: user?.id, content, gameId },
      {
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${user?.token}`,
        },
      },
    )
    return resp.data
  } catch (error) {
    console.log(error)
    throw new Error('Failed to add message.')
  }
}
export { getCurrentGameChatApi, addMessageApi }
