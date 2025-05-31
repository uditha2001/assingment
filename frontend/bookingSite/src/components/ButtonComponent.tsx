import React from "react";

interface ButtonComponentProps {
    children: React.ReactNode;
    onClick?: () => void;
    className?: string;
    type?: "button" | "submit" | "reset";
}

const ButtonComponent: React.FC<ButtonComponentProps> = ({
    children,
    onClick,
    className = "",
    type = "button",
}) => (
    <button
        type={type}
        onClick={onClick}
        className={`bg-white text-black px-4 py-2 rounded border border-black hover:bg-gray-100 transition ${className}`}
    >
        {children}
    </button>
);

export default ButtonComponent;