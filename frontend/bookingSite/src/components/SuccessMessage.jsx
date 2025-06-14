import React, { useEffect } from "react";
import { FiCheckCircle } from "react-icons/fi";

/**
 * SuccessMessage
 *
 * Displays a success alert with a message and optional close button.
 * Automatically closes after 30 seconds if onClose is provided.
 *
 * Props:
 * - message: string - The success message to display.
 * - onClose?: () => void - Optional callback to close the alert.
 */
const SuccessMessage = ({ message, onClose }) => {
    useEffect(() => {
        const timer = setTimeout(() => {
            if (onClose) onClose();
        }, 30000);
        return () => clearTimeout(timer);
    }, [onClose]);

    if (!message) return null;

    return (
        <div className="bg-green-100 border border-green-400 text-green-800 px-6 py-3 rounded shadow flex items-center">
            <FiCheckCircle className="mr-2 text-2xl" />
            <span>{message}</span>
            <button
                className="ml-4 text-green-800 hover:text-green-600 font-bold"
                onClick={onClose}
                aria-label="Close"
            >
                ×
            </button>
        </div>
    );
};

export default SuccessMessage;