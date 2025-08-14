import React, { createContext, useState, useEffect, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { AlertCircleIcon, CheckCircle2Icon } from "lucide-react";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const navigate = useNavigate();
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [success, setSuccess] = useState(null);

    const API_URL = "https://inventory-management-app-gvaphyaufbfsa3d0.germanywestcentral-01.azurewebsites.net/api/account";

    // Wrapper for all authenticated fetch calls
    const fetchWithAuth = async (url, options = {}) => {
        const res = await fetch(url, {
            credentials: "include",
            ...options
        });

        if (res.status === 401) {
            setUser(null);
            navigate("/login", { replace: true });
            throw new Error("Unauthorized - redirecting to login");
        }

        return res;
    };

    const handleApiError = async (res) => {
        if (!res.ok) {
            let message = "Unknown error occurred";
            try {
                const data = await res.json();
                message = data.message || message;
            } catch {
                message = res.statusText;
            }
            throw new Error(message);
        }
    };

    const login = useCallback(async (email, password) => {
        try {
            setError(null);
            const res = await fetch(`${API_URL}/login`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify({ email, password })
            });
            await handleApiError(res);
            await fetchUser();
            setSuccess("Logged in successfully!");
        } catch (err) {
            setError(err.message);
        }
    }, []);

    const register = useCallback(async (firstName, lastName, email, password) => {
        try {
            setError(null);
            const res = await fetch(`${API_URL}/register`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify({ firstName, lastName, email, password })
            });
            await handleApiError(res);
            await fetchUser();
            setSuccess("Account created successfully!");
        } catch (err) {
            setError(err.message);
        }
    }, []);

    const logout = useCallback(async () => {
        try {
            await fetch(`${API_URL}/logout`, {
                method: "POST",
                credentials: "include"
            });
        } catch {}
        setUser(null);
        navigate("/login", { replace: true });
    }, []);

    const signInWithGoogle = useCallback(() => {
        const returnUrl = encodeURIComponent(window.location.origin);
        window.location.href = `${API_URL}/login/google?returnUrl=${returnUrl}`;
    }, []);

    const fetchUser = useCallback(async () => {
        try {
            const res = await fetchWithAuth(`${API_URL}/me`);
            if (res.ok) {
                const data = await res.json();
                setUser(data);
            } else {
                setUser(null);
            }
        } catch {
            setUser(null);
        } finally {
            setLoading(false);
        }
    }, []);

    const refreshToken = useCallback(async () => {
        try {
            await fetchWithAuth(`${API_URL}/refresh`, { method: "POST" });
            await fetchUser();
        } catch (err) {
            console.error("Failed to refresh token:", err);
            setUser(null);
        }
    }, [fetchUser]);

    useEffect(() => {
        fetchUser();
    }, [fetchUser]);

    // Auto refresh every 14 minutes
    useEffect(() => {
        const interval = setInterval(() => {
            refreshToken();
        }, 14 * 60 * 1000);
        return () => clearInterval(interval);
    }, [refreshToken]);

    return (
        <AuthContext.Provider value={{
            user,
            loading,
            error,
            success,
            login,
            register,
            logout,
            signInWithGoogle
        }}>
            {/* Alerts */}
            {error && (
                <Alert variant="destructive" className="mb-4">
                    <AlertCircleIcon />
                    <AlertTitle>Error</AlertTitle>
                    <AlertDescription>{error}</AlertDescription>
                </Alert>
            )}
            {success && (
                <Alert className="mb-4">
                    <CheckCircle2Icon />
                    <AlertTitle>Success</AlertTitle>
                    <AlertDescription>{success}</AlertDescription>
                </Alert>
            )}
            {children}
        </AuthContext.Provider>
    );
};
