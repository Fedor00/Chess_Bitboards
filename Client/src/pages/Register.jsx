import { Form, Container, Button, Alert } from "react-bootstrap";
import { useAuth } from "../contexts/AuthContext";
import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import styles from "./Login.module.css";
import HomeNavbar from "../components/HomeNavbar";
import { register } from "../services/AccountApi";
function Register() {
  const { user } = useAuth();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [phoneNumber, setPhoneNumber] = useState("");
  const [username, setUsername] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    if (user) {
      navigate(`/${user?.roles[0]}-play`, { replace: true });
    }
  }, [user, navigate]);
  const validateInput = () => {
    if (password !== confirmPassword) {
      setError("Passwords do not match");
      return false;
    }
    return true;
  };
  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateInput()) {
      return;
    }
    try {
      setError("");
      await register(email, password, phoneNumber, username);
    } catch (err) {
      console.log(err?.message);
      setError(err?.message);
    }
    navigate(`/user-play`, { replace: true });
  };

  return (
    <>
      <HomeNavbar />
      <Container className="d-flex justify-content-center align-items-center min-vh-100">
        <Form onSubmit={handleSubmit}>
          {error && <Alert variant="danger">{error}</Alert>}
          <Form.Group>
            <Form.Label className={styles.emailLabel}>Email Address</Form.Label>
            <Form.Control
              type="email"
              placeholder="name@example.com"
              size="lg"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            ></Form.Control>
          </Form.Group>
          <Form.Group>
            <Form.Label className={styles.emailLabel}>Username</Form.Label>
            <Form.Control
              type="text"
              placeholder="Username"
              size="lg"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
            ></Form.Control>
          </Form.Group>
          <Form.Group>
            <Form.Label className={styles.emailLabel}>Phone</Form.Label>
            <Form.Control
              type="phone"
              placeholder="Phone"
              size="lg"
              value={phoneNumber}
              onChange={(e) => setPhoneNumber(e.target.value)}
            ></Form.Control>
          </Form.Group>

          <Form.Group>
            <Form.Label className={styles.passwordLabel}>Password</Form.Label>
            <Form.Control
              size="lg"
              type="password"
              placeholder="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            ></Form.Control>
          </Form.Group>
          <Form.Group>
            <Form.Label className={styles.passwordLabel}>Password</Form.Label>
            <Form.Control
              size="lg"
              type="password"
              placeholder="confirm password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
            ></Form.Control>
          </Form.Group>
          <br />
          <Button size="lg" type="submit" style={{ width: "100%" }}>
            Sign in
          </Button>
        </Form>
      </Container>
    </>
  );
}

export default Register;
