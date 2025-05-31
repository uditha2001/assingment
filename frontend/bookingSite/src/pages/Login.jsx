import React, { useState } from "react";
import { FiUser, FiLock } from "react-icons/fi";
import { useNavigate } from "react-router-dom";
import useUserApi from "../api/userAPI/useUserApi"; 

const Login = () => {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const navigate = useNavigate();
    const {login} = useUserApi();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");
        try {
            const response=await login(username, password);
            if(response.status===200){
                console.log("Login successful:", response);
                localStorage.setItem("user", JSON.stringify(response.data));
                navigate("/home");
            }
        } catch (err) {
            setError("Invalid credentials");
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-50">
            <form
                onSubmit={handleSubmit}
                className="bg-white p-8 rounded-lg shadow-md w-full max-w-xs"
            >
                <h2 className="text-2xl font-bold mb-6 text-center">Login</h2>
                {error && (
                    <div className="mb-4 text-center text-red-600 text-sm">
                        {error}
                    </div>
                )}
                <div className="mb-4">
                    <label className="block mb-1 text-gray-700">Username</label>
                    <div className="flex items-center border rounded px-2">
                        <FiUser className="text-gray-400 mr-2" />
                        <input
                            type="text"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            className="w-full py-2 outline-none"
                            placeholder="Enter your username"
                            required
                        />
                    </div>
                </div>
                <div className="mb-6">
                    <label className="block mb-1 text-gray-700">Password</label>
                    <div className="flex items-center border rounded px-2">
                        <FiLock className="text-gray-400 mr-2" />
                        <input
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            className="w-full py-2 outline-none"
                            placeholder="Enter your password"
                            required
                        />
                    </div>
                </div>
                <button
                    type="submit"
                    className="w-full bg-black text-white py-2 rounded hover:bg-gray-800 transition"
                >
                    Login
                </button>
                <div className="mt-4 text-center text-sm">
                    Don't have an account?{" "}
                    <button
                        type="button"
                        className="text-blue-600 hover:underline"
                        onClick={() => navigate("/signup")}
                    >
                        Sign Up
                    </button>
                </div>
            </form>
        </div>
    );
};

export default Login;