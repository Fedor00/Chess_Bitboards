import axios from 'axios'
import { ACCOUNT_API_URL } from '../config'

const login = async (email, password) => {
  try {
    const resp = await axios.post(
      `${ACCOUNT_API_URL}/login`,
      { email, password },
      {
        headers: {
          'Content-Type': 'application/json',
        },
      },
    )
    return resp.data
  } catch (error) {
    console.error(error?.response?.data)
    throw new Error(error?.response?.data)
  }
}
const register = async (email, password, phone, username) => {
  try {
    const resp = await axios.post(
      `${ACCOUNT_API_URL}/register`,
      { email, password, phone, username },
      {
        headers: {
          'Content-Type': 'application/json',
        },
      },
    )
    return resp.data
  } catch (error) {
    console.error(error?.response?.data)
    throw new Error(error?.response?.data)
  }
}
export { login, register }
