import axios from "axios";

const API_URL = "http://localhost:5195/api/account";

const login = async (email, password) => {
  try {
    const resp = await axios.post(
      `${API_URL}/login`,
      { email, password },
      {
        headers: {
          "Content-Type": "application/json",
        },
      }
    );
    return resp.data;
  } catch (error) {
    console.error(error?.response?.data);
    throw new Error(error?.response?.data);
  }
};
const register = async (email, password,phone,username) => {
  try {
    const resp = await axios.post(
      `${API_URL}/register`,
      { email, password, phone, username },
      {
        headers: {
          "Content-Type": "application/json",
        },
      }
    );
    return resp.data;
  } catch (error) {
    console.error(error?.response?.data);
    throw new Error(error?.response?.data);
  }
};
export { login,register };
