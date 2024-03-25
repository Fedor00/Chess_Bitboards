import { Form, Container, Button, Alert } from "react-bootstrap";
import { useAuth } from "../contexts/AuthContext";
import { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import styles from "./Login.module.css";
import HomeNavbar from "../components/HomeNavbar";
function Login() {
  const { handleLogin, user } = useAuth();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    if (user) {
      navigate(`/`, { replace: true });
    }
  }, [user, navigate]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      setError("");
      await handleLogin(email, password);
    } catch (err) {
      console.log(err?.message);
      setError(err?.message);
    }
  };

  return (
    <>
      {!user ? (
        <>
          <HomeNavbar />
          <Container className="d-flex justify-content-center align-items-center min-vh-100">
            <Form onSubmit={handleSubmit}>
              {error && <Alert variant="danger">{error}</Alert>}
              <Form.Group>
                <Form.Label className={styles.emailLabel}>
                  Email Address
                </Form.Label>
                <Form.Control
                  type="email"
                  placeholder="name@example.com"
                  size="lg"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                ></Form.Control>
              </Form.Group>
              <Form.Group>
                <Form.Label className={styles.passwordLabel}>
                  Password
                </Form.Label>
                <Form.Control
                  size="lg"
                  type="password"
                  placeholder="password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                ></Form.Control>
              </Form.Group>
              <br />
              <Button size="lg" type="submit" style={{ width: "100%" }}>
                Sign in
              </Button>
              <div className="mt-3 text-center">
                Don&apos;t have an account?
                <Link to="/register">Register here</Link>
              </div>
            </Form>
          </Container>
        </>
      ) : null}
    </>
  );
}

export default Login;
