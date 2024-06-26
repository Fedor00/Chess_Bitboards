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
const matchGameApi = async (user, isPrivate) => {
  try {
    const resp = await axios.post(
      `${GAME_API_URL}/match-game`,
      { isPrivate },
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
const createPrivateGame = async (user, isPrivate) => {
  try {
    const resp = await axios.post(
      `${GAME_API_URL}/create-private-game`,
      { isPrivate },
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
const createEngineGame = async (user, engineName, depth = 5) => {
  try {
    const resp = await axios.post(
      `${GAME_API_URL}/create-engine-game`,
      { engineName, depth },
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
const joinPrivateGame = async (user, gameId) => {
  try {
    const resp = await axios.put(
      `${GAME_API_URL}/join-private-game`,
      { gameId },
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
const resignGameApi = async (user) => {
  try {
    const resp = await axios.put(
      `${GAME_API_URL}/resign-game`,
      {},
      {
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${user?.token}`,
        },
      },
    )
    return resp.data
  } catch (error) {
    console.error(error?.response?.data)
    throw new Error(error?.response?.data)
  }
}
export {
  getCurrentGame,
  makeMoveApi,
  matchGameApi,
  createPrivateGame,
  joinPrivateGame,
  resignGameApi,
  createEngineGame,
}
