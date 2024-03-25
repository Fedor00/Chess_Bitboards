
import { useAuth } from '../contexts/AuthContext'
import { useState, useEffect } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import HomeNavbar from '../components/HomeNavbar'
import { register } from '../services/AccountApi'
function Register() {
  const { user } = useAuth()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [phoneNumber, setPhoneNumber] = useState('')
  const [username, setUsername] = useState('')
  const [error, setError] = useState('')
  const navigate = useNavigate()

  useEffect(() => {
    if (user) {
      navigate(`/${user?.roles[0]}-play`, { replace: true })
    }
  }, [user, navigate])
  const validateInput = () => {
    if (password !== confirmPassword) {
      setError('Passwords do not match')
      return false
    }
    return true
  }
  const handleSubmit = async (e) => {
    e.preventDefault()
    if (!validateInput()) {
      return
    }
    try {
      setError('')
      await register(email, password, phoneNumber, username)
    } catch (err) {
      console.log(err?.message)
      setError(err?.message)
    }
    navigate(`/user-play`, { replace: true })
  }

  return (
    <>
      <HomeNavbar />
      <div className="ml-5 mr-5 mt-10 flex min-h-full flex-1 flex-col items-center justify-center rounded-lg border  bg-sky-500/[.06] px-6  sm:mx-auto sm:w-full sm:max-w-sm lg:px-8">
        <div className="sm:mx-auto sm:w-full sm:max-w-sm">
          <h2 className="mt-2 text-2xl font-bold leading-9 tracking-tight text-center text-indigo-200">
            Sign up
          </h2>
        </div>

        <div className="mt-10 sm:mx-auto sm:w-full sm:max-w-sm">
          <form className="space-y-6" onSubmit={handleSubmit}>
            {error && <div className="alert alert-danger">{error}</div>}
            <div>
              <label
                htmlFor="email"
                className="block text-sm font-medium leading-6 text-indigo-200"
              >
                Email address
              </label>
              <div>
                <input
                  name="email"
                  type="email"
                  autoComplete="email"
                  required
                  className="block w-full rounded-md border-0 py-1.5  font-bold text-black shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                />
              </div>
            </div>
            <div>
              <label
                htmlFor="username"
                className="block text-sm font-medium text-indigo-200"
              >
                Username
              </label>
              <div>
                <input
                  name="username"
                  type="text"
                  required
                  className="block w-full rounded-md border-0 py-1.5 font-bold text-black shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                  value={username}
                  onChange={(e) => setUsername(e.target.value)}
                />
              </div>
            </div>
            <div>
              <label
                htmlFor="phoneNumber"
                className="block text-sm font-medium text-indigo-200"
              >
                Phone Number
              </label>
              <div>
                <input
                  name="phoneNumber"
                  type="tel"
                  required
                  className="block w-full rounded-md border-0 py-1.5 font-bold text-black shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                  value={phoneNumber}
                  onChange={(e) => setPhoneNumber(e.target.value)}
                />
              </div>
            </div>
            <div>
              <div className="flex items-center justify-between">
                <label
                  htmlFor="password"
                  className="block text-sm font-medium leading-6 text-indigo-200"
                >
                  Password
                </label>
              </div>
              <div>
                <input
                  name="password"
                  type="password"
                  autoComplete="current-password"
                  required
                  className="block w-full rounded-md border-0 py-1.5 font-bold text-black shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                />
              </div>
            </div>
            <div>
              <div className="flex items-center justify-between">
                <label
                  htmlFor="password"
                  className="block text-sm font-medium leading-6 text-indigo-200"
                >
                  Password
                </label>
              </div>
              <div>
                <input
                  name="confirmPassword"
                  type="password"
                  autoComplete="current-password"
                  required
                  className="block w-full rounded-md border-0 py-1.5 font-bold text-black shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                />
              </div>
            </div>

            <div>
              <button
                type="submit"
                className="flex w-full justify-center rounded-md bg-indigo-600 px-3 py-1.5 text-sm font-semibold leading-6 text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600"
              >
                Sign up
              </button>
            </div>
          </form>

          <p className="mt-10 text-sm text-center text-gray-100">
            Already a member?
            <Link
              to="/login"
              className="font-semibold leading-6 text-indigo-200 hover:text-indigo-500"
            >
              Login
            </Link>
          </p>
        </div>
      </div>
    </>
  )
}

export default Register
