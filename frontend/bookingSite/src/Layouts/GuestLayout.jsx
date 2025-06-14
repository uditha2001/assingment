import React from "react";
import { Outlet } from "react-router-dom";
import HeaderComponent from "../components/HeaderComponent";

/**
 * GuestLayout
 *
 * Layout for unauthenticated (guest) users.
 * Renders the public header and main content area.
 */
const GuestLayout = () => {
    return (
        <div className="min-h-screen flex flex-col bg-gray-50">
            <HeaderComponent />
            <main className="flex-1">
                <Outlet />
            </main>
        </div>
    );
};

export default GuestLayout;