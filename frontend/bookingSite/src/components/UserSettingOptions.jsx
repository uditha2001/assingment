import React from "react";
import { FiSettings, FiLogOut } from "react-icons/fi";

const UserSettingOptions = ({ onLogout, onSettings }) => {
    return (
        <div className="bg-white shadow rounded w-40 py-2 text-sm border">
            <button
                className="flex items-center w-full px-4 py-2 hover:bg-gray-100 text-left"
                onClick={onSettings}
            >
                <FiSettings className="mr-2" />
                Settings
            </button>
            <button
                className="flex items-center w-full px-4 py-2 hover:bg-gray-100 text-left text-red-600"
                onClick={onLogout}
            >
                <FiLogOut className="mr-2" />
                Logout
            </button>
        </div>
    );
};

export default UserSettingOptions;