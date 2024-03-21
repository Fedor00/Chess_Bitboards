import Container from "react-bootstrap/Container";
import Nav from "react-bootstrap/Nav";
import Navbar from "react-bootstrap/Navbar";
import { useAuth } from "../contexts/AuthContext";
import { Link } from "react-router-dom";

function HomeNavbar() {
  const { handleLogout, user } = useAuth();
  return (
    <Navbar bg="dark" expand="lg" data-bs-theme="dark">
      <Container>
        <Navbar.Brand href="/">Chess</Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav" className="justify-content-end">
          {!user ? (
            <Nav>
              <Nav.Link as={Link} to="/login">
                Login
              </Nav.Link>
            </Nav>
          ) : (
            <Nav>
              <Nav.Link as={Link} to="/login" onClick={handleLogout}>
                Logout
              </Nav.Link>
              <Nav.Link as={Link} to={`/user-play`}>
                Profile
              </Nav.Link>
            </Nav>
          )}
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}

export default HomeNavbar;
