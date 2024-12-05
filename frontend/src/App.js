// src/App.jsx
import React, { useState } from "react";
import LoginPage from "./LoginPage";
import SignUpPage from "./SignUpPage";
import TasksPage from "./TasksPage";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import Authdebug from "./Authdebug.jsx";
import ForgotPasswordPage  from "./ForgotPasswordPage.jsx";
const App = () => {
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  return (
    <Router>
      <Routes>
        <Route path="/" element={<LoginPage setIsLoggedIn={setIsLoggedIn} />} />
        <Route path="/signup" element={<SignUpPage />} />
        <Route path="/tasks" element={<TasksPage  />} />
        <Route path="/debug" element={<Authdebug />} />
        <Route path="forgot-password" element={<ForgotPasswordPage/>} />
      </Routes>
    </Router>
  );
};

export default App;
