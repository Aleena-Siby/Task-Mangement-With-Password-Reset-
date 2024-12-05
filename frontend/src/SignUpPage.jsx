// src/SignUpPage.jsx
import React, { useState } from "react";
import axios from "axios";
import './SignUpPage.css';  // Import your CSS file

const SignUpPage = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [message, setMessage] = useState("");
  const [email,setEmail]=useState("");

  const handleRegister = async (e) => {
    e.preventDefault();
    
    try {
      await axios.post("http://localhost:5099/api/Auth/register", {
        username,
        password,
        email
        
      });
      setMessage("Registration successful! Please log in.");
    } catch (error) {
      setMessage("Registration failed. Please try again.");
    }
  };

  return (
    <div className="signup-container">
      <h2>Sign Up</h2>
      {message && <p className="signup-message">{message}</p>}
      <form onSubmit={handleRegister}>
        <div>
          <label>Username</label>
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
            className="signup-input"
          />
        </div>
        <div>
          <label>Email</label>
          <input
            type="email"
            autoComplete="off"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            className="signup-input"
          />
        </div>
        <div>
          <label>Password</label>
          <input
            type="password"
            value={password}
            autoComplete="off"
            onChange={(e) => setPassword(e.target.value)}
            required
            className="signup-input"
          />
        </div>
        <button type="submit" className="signup-button">
          Sign Up
        </button>
        <button 
          type="button" 
          className="loginButton"
          onClick={() => (window.location.href = "/")}
        >
          Login
        </button>
      </form>
    </div>
  );
};

export default SignUpPage;
