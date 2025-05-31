import React, { useEffect } from "react";
import { FiAlertCircle } from "react-icons/fi";

const ErrorMessage = ({ message, onClose }) => {
    useEffect(() => {
        const timer = setTimeout(() => {
            if (onClose) onClose();
        }, 30000); // 30 seconds
        return () => clearTimeout(timer);
    }, [onClose]);

    if (!message) return null;

    return (
        <div className="bg-red-100 border border-red-400 text-red-800 px-6 py-3 rounded shadow flex items-center">
            <FiAlertCircle className="mr-2 text-2xl" />
            <span>{message}</span>
            <button
                className="ml-4 text-red-800 hover:text-red-600 font-bold"
                onClick={onClose}
                aria-label="Close"
            >
                ×
            </button>
        </div>
    );
};

export default ErrorMessage;