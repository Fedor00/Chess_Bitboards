/* eslint-disable react-refresh/only-export-components */
import { createContext, useContext, useReducer } from "react";
import { login } from "../services/AccountApi";
const AuthContext = createContext();
function reducer(state, action) {
   switch (action.type) {
      case "login":
         localStorage.setItem("user", JSON.stringify(action.payload));
         return { ...state, user: action.payload };
      case "logout":
         localStorage.removeItem("user");
         return { ...state, user: null };

      default:
         throw new Error("Unknown action");
   }
}
function useAuth() {
   const context = useContext(AuthContext);
   if (context === undefined)
      throw new Error("AuthContext was used outside AuthProvider");
   return context;
}
function AuthProvider({ children }) {
   const initialUser = JSON.parse(localStorage.getItem("user"));
   const [{ user }, dispatch] = useReducer(reducer, { user: initialUser });

   async function handleLogin(email, password) {
      try {
         const data = await login(email, password);
         dispatch({ type: "login", payload: data });
      } catch (error) {
         throw new Error("Email or password incorect");
      }
   }
   function handleLogout() {
      dispatch({ type: "logout" });
   }
   function isAuthenticated() {
      return localStorage.getItem("user") != null;
   }
   return (
      <AuthContext.Provider
         value={{ user, handleLogin, handleLogout, isAuthenticated }}
      >
         {children}
      </AuthContext.Provider>
   );
}
export { AuthProvider, useAuth };
