import axios from 'axios'
import { USER_API_URL } from '../config'

const getUserApi = async (userId, user) => {
  try {
    const resp = await axios.get(`${USER_API_URL}/${userId}`, {
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${user?.token}`,
      },
    })
    return resp.data
  } catch (error) {
    throw new Error('Failed to retrieve user.')
  }
}

export { getUserApi }
