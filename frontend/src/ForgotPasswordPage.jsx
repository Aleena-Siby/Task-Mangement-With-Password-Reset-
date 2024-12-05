import React, { useState } from "react";
import axios from "axios";

const ForgotPasswordPage = () => {
  const [email, setEmail] = useState("");
  const [message, setMessage] = useState("");

  const handleForgotPassword = async (e) => {
    e.preventDefault();

    try {
      const response = await axios.post("http://localhost:5099/api/Auth/RequestPasswordReset", {
        email,
      });

      setMessage(response.data); // Response: "If the email exists, a reset link has been generated."
    } catch (error) {
      setMessage("Failed to request password reset.");
    }
  };

  return (
    <div className="forgot-password-container">
      <h2>Forgot Password</h2>
      <form onSubmit={handleForgotPassword}>
        <div>
          <label>Email</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            className="forgot-password-input"
          />
        </div>
        <button type="submit" className="forgot-password-button">
          Request Reset Link
        </button>
      </form>
      {message && <p>{message}</p>}
    </div>
  );
};

export default ForgotPasswordPage;
