import React, { useEffect, useState } from "react";
import { FiTrash2 } from "react-icons/fi";
import { useNavigate } from "react-router-dom";
import useCartApi from "../api/useCartApi";
import useProductApi from "../api/productAPI/useProductApi";
import SuccessMessage from "../components/SuccessMessage";
import ErrorMessage from "../components/ErrorMessage";

const BASE_URL = "http://localhost:5010/";

const CartViewPage = () => {
  const { getCartByUserId, removeItemFromCart, updateCartQuantities } =
    useCartApi();
  const { getProductById } = useProductApi();
  const navigate = useNavigate();
  const [cartItems, setCartItems] = useState([]);
  const [editedQuantities, setEditedQuantities] = useState({});
  const [loading, setLoading] = useState(true);
  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const [confirmDeleteId, setConfirmDeleteId] = useState(null);
  const [productDetails, setProductDetails] = useState({});

  // Move fetchCart outside useEffect so you can call it anywhere
  const fetchCart = async () => {
    try {
      const user = JSON.parse(localStorage.getItem("user"));
      const userId = user?.userId;
      if (userId) {
        const response = await getCartByUserId(userId);
        if (response.status === 200) {
          setCartItems(response.data);
          setEditedQuantities({});
          // Fetch product details for all items
          const details = {};
          await Promise.all(
            response.data.map(async (item) => {
              if (!details[item.productId]) {
                const prodRes = await getProductById(item.productId);
                if (prodRes.status === 200) {
                  details[item.productId] = prodRes.data;
                }
              }
            })
          );
          setProductDetails(details);
        }
      }
    } catch (error) {
      setCartItems([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCart();
  }, [successMessage]);

  // Handle quantity change
  const handleQuantityChange = (cartItemId, newQty) => {
    setEditedQuantities((prev) => ({
      ...prev,
      [cartItemId]: newQty,
    }));
  };

  // Prepare update array and send to backend
  const handleUpdateCart = async () => {
    const updateArray = Object.entries(editedQuantities).map(
      ([cartItemId, qty]) => {
        const item = cartItems.find((i) => i.cartItemId === Number(cartItemId));
        return {
          CartItemId: Number(cartItemId),
          NewQuantity: qty,
          newTotalPrice: item ? item.itemTotalPrice * qty : 0,
        };
      }
    );
    try {
      const response = await updateCartQuantities(updateArray);
      if (response.status === 200) {
        setSuccessMessage("Cart updated successfully!");
        setEditedQuantities({});
        await fetchCart(); // <-- Refresh cart immediately after update
      } else {
        setErrorMessage("Failed to update cart.");
      }
    } catch {
      setErrorMessage("Failed to update cart.");
    }
  };

  const handleDelete = async (itemId) => {
    try {
      const response = await removeItemFromCart(itemId);
      if (response.status === 204) {
        setSuccessMessage("Item removed from cart!");
      } else {
        setErrorMessage("Failed to remove item from cart.");
      }
    } catch {
      setErrorMessage("Failed to remove item from cart.");
    } finally {
      setConfirmDeleteId(null);
    }
  };

  const getFirstImageUrl = (contents) => {
    if (!contents || !Array.isArray(contents)) return null;
    const img = contents.find(
      (c) => c.type && c.type.toLowerCase().startsWith("image")
    );
    if (!img) return null;
    if (img.url.startsWith("http://") || img.url.startsWith("https://"))
      return img.url;
    return BASE_URL + img.url.replace(/^\/+/, "");
  };

  const total = cartItems.reduce((sum, item) => {
    const product = productDetails[item.productId] || {};
    const unitPrice = product.price ?? 0;
    const qty = editedQuantities[item.cartItemId] ?? item.quantity ?? 1;
    return sum + Number(unitPrice) * qty;
  }, 0);

  return (
    <div className="min-h-screen bg-gray-50 py-8 px-2">
      <div className="max-w-2xl mx-auto bg-white rounded-lg shadow p-6">
        <h2 className="text-2xl font-bold mb-6">Your Cart</h2>
        {successMessage && (
          <SuccessMessage
            message={successMessage}
            onClose={() => setSuccessMessage("")}
          />
        )}
        {errorMessage && (
          <ErrorMessage
            message={errorMessage}
            onClose={() => setErrorMessage("")}
          />
        )}
        {loading ? (
          <div className="text-center text-gray-500">Loading...</div>
        ) : cartItems.length === 0 ? (
          <div className="text-center text-gray-500">Cart is empty.</div>
        ) : (
          <>
            <div className="space-y-4">
              {cartItems.map((item) => {
                const product = productDetails[item.productId] || {};
                const contents = product.contents || [];
                const firstImageUrl = getFirstImageUrl(contents);
                const unitPrice = product.price ?? 0;
                return (
                  <div
                    key={item.cartItemId}
                    className="flex items-center border-b pb-4 last:border-b-0"
                  >
                    {firstImageUrl ? (
                      <img
                        src={firstImageUrl}
                        alt={product.name}
                        className="w-16 h-16 object-cover rounded mr-4 border"
                      />
                    ) : (
                      <div className="w-16 h-16 bg-gray-200 rounded mr-4 flex items-center justify-center text-gray-400 text-xs">
                        No Image
                      </div>
                    )}
                    <div className="flex-1">
                      <div className="font-semibold">
                        {product.name || "No Name"}
                      </div>
                      <div className="text-sm text-gray-500 flex items-center gap-2">
                        Quantity:{" "}
                        <button
                          className="px-2 py-0.5 border rounded text-lg"
                          onClick={() =>
                            handleQuantityChange(
                              item.cartItemId,
                              Math.max(
                                1,
                                (editedQuantities[item.cartItemId] ??
                                  item.quantity ??
                                  1) - 1
                              )
                            )
                          }
                          disabled={
                            (editedQuantities[item.cartItemId] ??
                              item.quantity ??
                              1) <= 1
                          }
                        >
                          -
                        </button>
                        <span className="font-medium">
                          {editedQuantities[item.cartItemId] ?? item.quantity}
                        </span>
                        <button
                          className="px-2 py-0.5 border rounded text-lg"
                          onClick={() =>
                            handleQuantityChange(
                              item.cartItemId,
                              (editedQuantities[item.cartItemId] ??
                                item.quantity ??
                                1) + 1
                            )
                          }
                        >
                          +
                        </button>
                      </div>
                      <div className="text-sm text-gray-500">
                        Price: {item.currency} {Number(unitPrice).toFixed(2)}
                      </div>
                      <div className="text-sm text-gray-700 font-semibold">
                        Subtotal: {item.currency}{" "}
                        {(
                          Number(unitPrice) *
                          (editedQuantities[item.cartItemId] ??
                            item.quantity ??
                            1)
                        ).toFixed(2)}
                      </div>
                    </div>
                    <button
                      className="ml-4 text-red-500 hover:text-red-700"
                      title="Remove"
                      onClick={() => setConfirmDeleteId(item.cartItemId)}
                    >
                      <FiTrash2 size={20} />
                    </button>
                    {/* Confirmation dialog */}
                    {confirmDeleteId === item.cartItemId && (
                      <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-30 z-50">
                        <div className="bg-white p-6 rounded shadow-lg">
                          <p className="mb-4">
                            Are you sure you want to remove this item from the
                            cart?
                          </p>
                          <div className="flex gap-4">
                            <button
                              className="bg-red-600 text-white px-4 py-2 rounded"
                              onClick={() => handleDelete(item.cartItemId)}
                            >
                              Yes, Remove
                            </button>
                            <button
                              className="bg-gray-300 text-black px-4 py-2 rounded"
                              onClick={() => setConfirmDeleteId(null)}
                            >
                              Cancel
                            </button>
                          </div>
                        </div>
                      </div>
                    )}
                  </div>
                );
              })}
            </div>
            {Object.keys(editedQuantities).length > 0 && (
              <div className="flex justify-end mt-4">
                <button
                  className="bg-black text-white px-6 py-2 rounded hover:bg-gray-800 transition text-base font-semibold"
                  onClick={handleUpdateCart}
                >
                  Update Cart
                </button>
              </div>
            )}
            <div className="flex flex-col sm:flex-row justify-between items-center mt-6 gap-4">
              <div className="text-lg font-bold">
                Total: USD {total.toFixed(2)}
              </div>
              <button
                className="bg-black text-white px-6 py-2 rounded hover:bg-gray-800 transition text-base font-semibold"
                onClick={() => navigate("/orderCheckout", { state: { cartItems } })}
              >
                Checkout
              </button>
            </div>
          </>
        )}
      </div>
    </div>
  );
};

export default CartViewPage;
