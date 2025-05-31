import React, { useState, useRef } from "react";
import { FiSearch, FiShoppingCart, FiMenu, FiX } from "react-icons/fi";
import ButtonComponent from "./ButtonComponent";
import { useNavigate } from "react-router-dom";
import UserSettingOptions from "./UserSettingOptions";
import useCartApi from "../api/useCartApi";

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

  // Close user menu when clicking outside
  React.useEffect(() => {
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

  React.useEffect(() => {
    const user = JSON.parse(localStorage.getItem("user") || "null");
    const userId = user?.userId;
    if (userId) {
      getCartItemCount(userId).then((res) => {
        if (res.status === 200) setCartCount(res.data);
      });
    }
    setHideCartCount(false); // Reset badge when header re-mounts or user changes
  }, [getCartItemCount]);

  const handleLogout = () => {
    localStorage.removeItem("user");
    navigate("/login");
  };

  const handleSettings = () => {
    setShowUserOptions(false);
    navigate("/settings");
  };

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
        {/* Site Title */}
        <h1 className="text-2xl font-bold">BookingPlatForm</h1>
        {/* Desktop Nav */}
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
        {/* Right Side Icons and Search Bar */}
        <div className="flex items-center space-x-4 relative">
          {/* Search Bar (desktop only) */}
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
            className="focus:outline-none relative"
            aria-label="Cart"
            onClick={() => {
              setHideCartCount(true); // Hide the badge
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
          {/* User Image with dropdown */}
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
      {/* Mobile Nav */}
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
