import { useAuth } from '../contexts/AuthContext'
import { useState, useEffect } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import HomeNavbar from '../components/HomeNavbar'

function Login() {
  const { handleLogin, user } = useAuth()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const navigate = useNavigate()

  useEffect(() => {
    if (user) {
      navigate(`/`, { replace: true })
    }
  }, [user, navigate])

  const handleSubmit = async (e) => {
    console.log('submit')
    e.preventDefault()
    try {
      setError('')
      await handleLogin(email, password)
    } catch (err) {
      console.log(err?.message)
      setError(err?.message)
    }
  }

  return (
    <>
      {!user ? (
        <>
          <HomeNavbar />
          <div className="ml-5 mr-5 mt-10 flex min-h-full flex-1 flex-col items-center justify-center rounded-lg border bg-cyan-950 bg-sky-500/[.06] px-6 py-12 sm:mx-auto sm:w-full sm:max-w-sm lg:px-8">
            <div className="sm:mx-auto sm:w-full sm:max-w-sm">
              <h2 className="mt-10 text-center text-2xl font-bold leading-9 tracking-tight text-indigo-200">
                Sign in to your account
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
                  <div className="mt-2">
                    <input
                      id="email"
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
                  <div className="flex items-center justify-between">
                    <label
                      htmlFor="password"
                      className="block text-sm font-medium leading-6 text-indigo-200"
                    >
                      Password
                    </label>
                    <div className="text-sm">
                      <a
                        href="/dsd"
                        className="font-semibold text-indigo-200 hover:text-indigo-500"
                      >
                        Forgot password?
                      </a>
                    </div>
                  </div>
                  <div className="mt-2">
                    <input
                      id="password"
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
                  <button
                    type="submit"
                    className="flex w-full justify-center rounded-md bg-indigo-600 px-3 py-1.5 text-sm font-semibold leading-6 text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600"
                  >
                    Sign in
                  </button>
                </div>
              </form>

              <p className="mt-10 text-center text-sm text-gray-100">
                Not a member?
                <Link
                  to="/register"
                  className="font-semibold leading-6 text-indigo-200 hover:text-indigo-500"
                >
                  Register
                </Link>
              </p>
            </div>
          </div>
        </>
      ) : null}
    </>
  )
}

export default Login
