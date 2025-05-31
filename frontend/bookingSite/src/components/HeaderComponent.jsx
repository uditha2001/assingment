import React, { useState } from "react";
import { FiSearch, FiShoppingCart, FiMenu, FiX, FiUser } from "react-icons/fi";
import ButtonComponent from "./ButtonComponent";
import { useNavigate } from "react-router-dom";

const HeaderComponent = () => {
  const [menuOpen, setMenuOpen] = useState(false);
  const [showSearch, setShowSearch] = useState(false);
  const [searchTerm, setSearchTerm] = useState(""); // Add searchTerm state
  const navigate = useNavigate();

  // Search handler
  const handleSearch = (e) => {
    e.preventDefault();
    if (searchTerm.trim()) {
      navigate(`/?search=${encodeURIComponent(searchTerm.trim())}`);
    } else {
      navigate("/");
    }
  };

  return (
    <>
      <header className="bg-white text-black p-4 shadow-[0_4px_16px_-4px_rgba(0,0,0,0.15)] sticky top-0 z-[999]">
        <div className="flex items-center justify-between">
          {/* Site Title */}
          <h1 className="text-2xl font-bold">BookingPlatForm</h1>
          {/* Desktop Nav */}
          <nav className="hidden sm:flex space-x-8 items-center">
            <ButtonComponent
              className="border-0 hover:border-b-2 hover:border-black rounded-none transition-all"
              onClick={() => navigate("/")}
            >
              Home
            </ButtonComponent>
            <ButtonComponent className="border-0 hover:border-b-2 hover:border-black rounded-none transition-all">
              Contact
            </ButtonComponent>
            <ButtonComponent className="border-0 hover:border-b-2 hover:border-black rounded-none transition-all">
              About
            </ButtonComponent>
            <ButtonComponent
              className="border-0 hover:border-b-2 hover:border-black rounded-none transition-all"
              onClick={() => navigate("/signup")}
            >
              Sign Up
            </ButtonComponent>
          </nav>
          {/* Right Side Icons and Search Bar */}
          <div className="flex items-center space-x-4">
            {/* Search Bar (desktop only) */}
            <form
              className="hidden sm:flex items-center"
              onSubmit={handleSearch}
            >
              <input
                type="text"
                placeholder="What are you looking for?"
                className="px-2 py-1 rounded text-black border border-gray-300 w-56"
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
              />
              <button type="submit">
                <FiSearch className="text-2xl text-black ml-2" />
              </button>
            </form>
            {/* Search Icon (mobile) */}
            <button
              className="sm:hidden focus:outline-none"
              onClick={() => setShowSearch((prev) => !prev)}
              aria-label="Search"
            >
              <FiSearch className="text-2xl text-black" />
            </button>
            {/* Cart Icon */}
            <button
              className="focus:outline-none"
              aria-label="Cart"
              onClick={() => navigate("/login")}
            >
              <FiShoppingCart className="text-2xl text-black" />
            </button>
            {/* User Image/Icon */}
            <button className="focus:outline-none" aria-label="User">
              <FiUser className="text-2xl text-black" />
            </button>
            {/* Hamburger Menu (mobile) */}
            <button
              className="sm:hidden focus:outline-none ml-2"
              onClick={() => setMenuOpen((prev) => !prev)}
              aria-label="Menu"
            >
              {menuOpen ? (
                <FiX className="text-2xl text-black" />
              ) : (
                <FiMenu className="text-2xl text-black" />
              )}
            </button>
          </div>
        </div>
        {/* Mobile Search Bar */}
        {showSearch && (
          <form
            className="sm:hidden mt-4 flex items-center"
            onSubmit={handleSearch}
          >
            <input
              type="text"
              placeholder="What are you looking for?"
              className="px-2 py-1 rounded text-black border border-gray-300 w-full"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              autoFocus
            />
            <button type="submit">
              <FiSearch className="text-2xl text-black ml-2" />
            </button>
          </form>
        )}
        {/* Mobile Nav */}
        {menuOpen && (
          <div className="sm:hidden mt-4 flex flex-col items-end space-y-2">
            <ButtonComponent
              className="w-full text-right border-0 hover:border-b-2 hover:border-black rounded-none transition-all"
              onClick={() => {
                setMenuOpen(false);
                navigate("/");
              }}
            >
              Home
            </ButtonComponent>
            <ButtonComponent className="w-full text-right border-0 hover:border-b-2 hover:border-black rounded-none transition-all">
              Contact
            </ButtonComponent>
            <ButtonComponent className="w-full text-right border-0 hover:border-b-2 hover:border-black rounded-none transition-all">
              About
            </ButtonComponent>
            <ButtonComponent
              className="w-full text-right border-0 hover:border-b-2 hover:border-black rounded-none transition-all"
              onClick={() => {
                setMenuOpen(false);
                navigate("/signup");
              }}
            >
              Sign Up
            </ButtonComponent>
          </div>
        )}
      </header>
    </>
  );
};

export default HeaderComponent;
