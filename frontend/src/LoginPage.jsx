import React, { useState } from "react";
import axios from "axios";
import { jwtDecode } from 'jwt-decode';
import { useNavigate } from 'react-router-dom';
import './LoginPage.css';  // Import the CSS file

const LoginPage = ({ setIsLoggedIn }) => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const navigate = useNavigate();

  const handleLogin = async (e) => {
    e.preventDefault();

    try {
      const response = await axios.post("http://localhost:5099/api/Auth/login", {
        username,
        password,
      });

      if (response.data) {
        const token=response.data.token;
        localStorage.setItem("token", token);
        setIsLoggedIn(true);
        alert("Logged in successfully!");
        navigate('/tasks');

        const decodedToken=jwtDecode(token);
        console.log("Decoded Token:", decodedToken);
        const userId = decodedToken.id;
        localStorage.setItem('userId', userId.toString());
        
      }
    } catch (error) {
      setErrorMessage("Login failed. Please check your credentials.");
    }
  };

  return (
    <div className="login-container">
      <div className="mydiv">
     
      <h2>Login</h2>
      {errorMessage && <p className="login-error">{errorMessage}</p>}
      <form onSubmit={handleLogin}>
        <div>
          <label>Username</label>
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
            className="login-input"
          />
        </div>
        <div>
          <label>Password</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            className="login-input"
          />
        </div>
        <button type="submit" className="login-button">
          Login
        </button>
      </form>
      <p>
        Don't have an account?{" "}
        <button onClick={() =>  navigate("/signup")} className="login-link">
          Sign Up
        </button>
      </p>
      <p
          className="forgot-password-link"
          onClick={() => navigate("/forgot-password")}
        >
          Forgot Password?
        </p>
      </div>
    </div>
  );
};

export default LoginPage;