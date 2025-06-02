import { Navigate, Outlet } from "react-router-dom";
import AuhtenticatedHeader from "../components/AuhtenticatedHeader";

/**
 * AuthenticatedLayout
 *
 * Layout component for authenticated routes.
 * Renders the authenticated header and main content if a valid token is present.
 * Redirects to the login page if the user is not authenticated.
 */
const AuthenticatedLayout = () => {
    const user = JSON.parse(localStorage.getItem("user") || "null");
    const token = user?.acessToken;

    if (!token) {
        return <Navigate to="/" replace />;
    }

    return (
        <div className="min-h-screen flex flex-col bg-gray-50">
            <AuhtenticatedHeader />
            <main className="flex-1">
                <Outlet />
            </main>
        </div>
    );
};

export default AuthenticatedLayout;