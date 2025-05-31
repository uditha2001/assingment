import React, { useState } from "react";
import { FiUser, FiMail, FiLock } from "react-icons/fi";
import { useNavigate } from "react-router-dom";
import useUserApi from "../api/userAPI/useUserApi";
import SuccessMessage from "../components/SuccessMessage";
import ErrorMessage from "../components/ErrorMessage";

const Register = () => {
  const [form, setForm] = useState({
    userName: "",
    password: "",
    confirmPassword: "",
    Email: "",
    FirstName: "",
    LastName: "",
  });
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const navigate = useNavigate();
  const { createUser } = useUserApi();

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
    setError("");
  };

  const validateEmail = (email) => {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateEmail(form.Email)) {
      setError("Please enter a valid email address.");
      setSuccess("");
      return;
    }
    if (form.password !== form.confirmPassword) {
      setError("Passwords do not match.");
      setSuccess("");
      return;
    }
    setError("");

    // Prepare user object as required by backend
    const user = {
      userName: form.userName,
      password: form.password,
      Email: form.Email,
      FirstName: form.FirstName,
      LastName: form.LastName,
    };

    try {
        console.log("Registering user:", user);
      const response = await createUser(user);
      console.log("Registration response:", response);
      if (response.status === 200 || response.status === 201) {
        setSuccess("Registration successful! Redirecting to login...");
        setError("");
        setTimeout(() => navigate("/login"), 1500);
      } else {
        setError("Registration failed. Please try again.");
        setSuccess("");
      }
    } catch (err) {
      setError("Registration failed. Please try again.");
      setSuccess("");
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 px-2">
      <form
        onSubmit={handleSubmit}
        className="bg-white p-6 rounded-lg shadow-md w-full max-w-md overflow-y-auto"
        style={{ maxHeight: "90vh" }}
      >
        <h2 className="text-xl font-bold mb-4 text-center">Register</h2>
        {success && <SuccessMessage message={success} />}
        {error && <ErrorMessage message={error} />}
        <div className="mb-2">
          <label className="block mb-1 text-gray-700">Username</label>
          <div className="flex items-center border rounded px-2">
            <FiUser className="text-gray-400 mr-2" />
            <input
              type="text"
              name="userName"
              value={form.userName}
              onChange={handleChange}
              className="w-full py-1 outline-none text-sm"
              placeholder="Enter username"
              required
            />
          </div>
        </div>
        <div className="mb-2">
          <label className="block mb-1 text-gray-700">Password</label>
          <div className="flex items-center border rounded px-2">
            <FiLock className="text-gray-400 mr-2" />
            <input
              type="password"
              name="password"
              value={form.password}
              onChange={handleChange}
              className="w-full py-1 outline-none text-sm"
              placeholder="Enter password"
              required
            />
          </div>
        </div>
        <div className="mb-2">
          <label className="block mb-1 text-gray-700">Confirm Password</label>
          <div className="flex items-center border rounded px-2">
            <FiLock className="text-gray-400 mr-2" />
            <input
              type="password"
              name="confirmPassword"
              value={form.confirmPassword}
              onChange={handleChange}
              className="w-full py-1 outline-none text-sm"
              placeholder="Re-enter password"
              required
            />
          </div>
        </div>
        <div className="mb-2">
          <label className="block mb-1 text-gray-700">Email</label>
          <div className="flex items-center border rounded px-2">
            <FiMail className="text-gray-400 mr-2" />
            <input
              type="email"
              name="Email"
              value={form.Email}
              onChange={handleChange}
              className="w-full py-1 outline-none text-sm"
              placeholder="Enter email"
              required
            />
          </div>
        </div>
        <div className="mb-2">
          <label className="block mb-1 text-gray-700">First Name</label>
          <input
            type="text"
            name="FirstName"
            value={form.FirstName}
            onChange={handleChange}
            className="w-full border rounded px-2 py-1 outline-none text-sm"
            placeholder="Enter first name"
            required
          />
        </div>
        <div className="mb-4">
          <label className="block mb-1 text-gray-700">Last Name</label>
          <input
            type="text"
            name="LastName"
            value={form.LastName}
            onChange={handleChange}
            className="w-full border rounded px-2 py-1 outline-none text-sm"
            placeholder="Enter last name"
            required
          />
        </div>
        <button
          type="submit"
          className="w-full bg-black text-white py-2 rounded hover:bg-gray-800 transition text-sm"
        >
          Register
        </button>
        <div className="mt-4 text-center text-sm">
          Already have an account?{" "}
          <button
            type="button"
            className="text-blue-600 hover:underline"
            onClick={() => navigate("/login")}
          >
            Login
          </button>
        </div>
      </form>
    </div>
  );
};

export default Register;
