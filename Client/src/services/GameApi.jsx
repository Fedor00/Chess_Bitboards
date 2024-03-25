import axios from 'axios'
import { GAME_API_URL } from '../config'

const getCurrentGame = async (user) => {
  try {
    const resp = await axios.get(`${GAME_API_URL}`, {
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${user.token}`,
      },
    })
    return resp.data
  } catch (error) {
    console.error(error?.response?.data)
    throw new Error(error?.response?.data)
  }
}
const makeMoveApi = async (user, from, to) => {
  try {
    const resp = await axios.put(
      `${GAME_API_URL}/make-move`,
      { from, to },
      {
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${user.token}`,
        },
      },
    )
    return resp.data
  } catch (error) {
    console.error(error?.response?.data)
    throw new Error(error?.response?.data)
  }
}
export { getCurrentGame, makeMoveApi }
