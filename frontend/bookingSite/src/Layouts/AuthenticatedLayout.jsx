import { Navigate, Outlet } from "react-router-dom";
import AuhtenticatedHeader from "../components/AuhtenticatedHeader";

const AuthenticatedLayout = () => {
    const user = JSON.parse(localStorage.getItem("user") || "null");
    const token = user?.acessToken;

    if (!token) {
        console.log("No token found, redirecting to login");
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