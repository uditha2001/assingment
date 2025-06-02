import React, { useState, useRef, useEffect } from "react";
import { FiSearch, FiShoppingCart, FiMenu, FiX } from "react-icons/fi";
import ButtonComponent from "./ButtonComponent";
import { useNavigate } from "react-router-dom";
import UserSettingOptions from "./UserSettingOptions";
import useCartApi from "../api/useCartApi";

/**
 * AuhtenticatedHeader
 *
 * Renders the main navigation header for authenticated users.
 * Features:
 * - Responsive navigation (desktop & mobile)
 * - Search bar (desktop & mobile)
 * - Cart icon with item count badge
 * - User avatar with dropdown for settings/logout
 * - Hamburger menu for mobile navigation
 */
const AuhtenticatedHeader = () => {
  const [menuOpen, setMenuOpen] = useState(false);
  const [showSearch, setShowSearch] = useState(false);
  const [showUserOptions, setShowUserOptions] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");
  const userMenuRef = useRef(null);
  const { getCartItemCount } = useCartApi();
  const [cartCount, setCartCount] = useState(0);
  const [hideCartCount, setHideCartCount] = useState(false);

  const navigate = useNavigate();
  const user = JSON.parse(localStorage.getItem("user") || "null");
  const userImage = user?.imageUrl || "https://ui-avatars.com/api/?name=User";

  // Handles closing the user dropdown when clicking outside
  useEffect(() => {
    function handleClickOutside(event) {
      if (userMenuRef.current && !userMenuRef.current.contains(event.target)) {
        setShowUserOptions(false);
      }
    }
    if (showUserOptions) {
      document.addEventListener("mousedown", handleClickOutside);
    } else {
      document.removeEventListener("mousedown", handleClickOutside);
    }
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [showUserOptions]);

  // Fetch cart item count on mount or when user changes
  useEffect(() => {
    const user = JSON.parse(localStorage.getItem("user") || "null");
    const userId = user?.userId;
    if (userId) {
      getCartItemCount(userId).then((res) => {
        if (res.status === 200) setCartCount(res.data);
      });
    }
    setHideCartCount(false);
  }, [getCartItemCount]);

  // Logs out the user and navigates to login
  const handleLogout = () => {
    localStorage.removeItem("user");
    navigate("/login");
  };

  // Navigates to user settings
  const handleSettings = () => {
    setShowUserOptions(false);
    navigate("/settings");
  };

  // Handles search form submission
  const handleSearch = (e) => {
    e.preventDefault();
    if (searchTerm.trim()) {
      navigate(`/home?search=${encodeURIComponent(searchTerm.trim())}`);
    } else {
      navigate("/home", { replace: true });
    }
  };

  return (
    <header className="bg-white text-black p-4 shadow-[0_4px_16px_-4px_rgba(0,0,0,0.15)] sticky top-0 z-[999]">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">BookingPlatForm</h1>
        <nav className="hidden sm:flex space-x-8 items-center">
          <ButtonComponent
            className="border-0 hover:border-b-2 hover:border-black rounded-none transition-all"
            onClick={() => navigate("/home")}
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
            onClick={() => navigate("/products")}
          >
            Products
          </ButtonComponent>
          <ButtonComponent
            className="border-0 hover:border-b-2 hover:border-black rounded-none transition-all"
            onClick={() => navigate("/orders")}
          >
            Orders
          </ButtonComponent>
        </nav>
        <div className="flex items-center space-x-4 relative">
          <form className="hidden sm:flex items-center" onSubmit={handleSearch}>
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
          <button
            className="sm:hidden focus:outline-none"
            onClick={() => setShowSearch((prev) => !prev)}
            aria-label="Search"
          >
            <FiSearch className="text-2xl text-black" />
          </button>
          <button
            className="focus:outline-none relative"
            aria-label="Cart"
            onClick={() => {
              setHideCartCount(true);
              navigate("/cart");
            }}
          >
            <FiShoppingCart className="text-2xl text-black" />
            {cartCount > 0 && !hideCartCount && (
              <span className="absolute -top-2 -right-2 bg-red-600 text-white text-xs rounded-full px-1.5 py-0.5">
                {cartCount}
              </span>
            )}
          </button>
          <div className="relative" ref={userMenuRef}>
            <img
              src={userImage}
              alt="User"
              className="w-8 h-8 rounded-full border object-cover cursor-pointer"
              onClick={() => setShowUserOptions((prev) => !prev)}
            />
            {showUserOptions && (
              <div className="absolute right-0 mt-2 z-50">
                <UserSettingOptions
                  onLogout={handleLogout}
                  onSettings={handleSettings}
                />
              </div>
            )}
          </div>
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
      {showSearch && (
        <div className="sm:hidden mt-4 flex items-center">
          <input
            type="text"
            placeholder="What are you looking for?"
            className="px-2 py-1 rounded text-black border border-gray-300 w-full"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            autoFocus
          />
        </div>
      )}
      {menuOpen && (
        <div className="sm:hidden mt-4 flex flex-col items-end space-y-2">
          <ButtonComponent
            className="w-full text-right border-0 hover:border-b-2 hover:border-black rounded-none transition-all"
            onClick={() => {
              setMenuOpen(false);
              navigate("/home");
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
              navigate("/products");
            }}
          >
            Products
          </ButtonComponent>
          <ButtonComponent
            className="w-full text-right border-0 hover:border-b-2 hover:border-black rounded-none transition-all"
            onClick={() => {
              setMenuOpen(false);
              navigate("/orders");
            }}
          >
            Orders
          </ButtonComponent>
        </div>
      )}
    </header>
  );
};

export default AuhtenticatedHeader;
