import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import privateAxios from "../api/Axios";

/**
 * useAxiosPrivate
 *
 * Custom hook that returns an Axios instance configured with authentication headers and error handling.
 * Automatically attaches the user's access token to requests and redirects to login on 401 errors.
 *
 * Returns:
 * - AxiosInstance: Configured Axios instance for authenticated API calls.
 */
const useAxiosPrivate = () => {
    const navigate = useNavigate();

    useEffect(() => {
        const requestInterceptor = privateAxios.interceptors.request.use(
            (config) => {
                const user = JSON.parse(localStorage.getItem("user") || "null");
                const token = user?.acessToken;
                if (token && !config.headers['Authorization']) {
                    config.headers['Authorization'] = `Bearer ${token}`;
                }
                return config;
            },
            (error) => Promise.reject(error)
        );

        const responseInterceptor = privateAxios.interceptors.response.use(
            (response) => response,
            async (error) => {
                const prevRequest = error?.config;
                const status = error?.response?.status;
                if (status === 401 && !prevRequest?.sent) {
                    prevRequest.sent = true;
                    try {
                        localStorage.removeItem("user");
                        navigate("/login", { replace: true });
                        return Promise.reject(error);
                    } catch (err) {
                        console.error("Something went wrong:", err);
                        navigate("/login", { replace: true });
                        return Promise.reject(err);
                    }
                }
                return Promise.reject(error);
            }
        );

        return () => {
            privateAxios.interceptors.request.eject(requestInterceptor);
            privateAxios.interceptors.response.eject(responseInterceptor);
        };
    }, [navigate]);

    return privateAxios;
};

export default useAxiosPrivate;