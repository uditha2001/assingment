import React, { useEffect, useState } from "react";
import useOrderApi from "../api/useOrderApi";
import useProductApi from "../api/productAPI/useProductApi";

const BASE_URL = "http://localhost:5010/";

const OrderView = () => {
  const { getAllOrders } = useOrderApi();
  const { getProductById } = useProductApi();

  const [orders, setOrders] = useState([]);
  const [productDetails, setProductDetails] = useState({});
  const [loading, setLoading] = useState(true);

  // Get userId from localStorage
  const user = JSON.parse(localStorage.getItem("user") || "null");
  const userId = user?.userId;

  useEffect(() => {
    const fetchOrdersAndProducts = async () => {
      setLoading(true);
      try {
        const ordersRes = await getAllOrders(userId);
        if (ordersRes.status === 200) {
          setOrders(ordersRes.data);

          // Collect all unique productIds from all orders
          const productIds = [
            ...new Set(
              ordersRes.data.flatMap((order) =>
                order.items.map((item) => item.productId)
              )
            ),
          ];

          // Fetch product details for each productId
          const details = {};
          await Promise.all(
            productIds.map(async (pid) => {
              try {
                const prodRes = await getProductById(pid);
                if (prodRes.status === 200) {
                  details[pid] = prodRes.data;
                }
              } catch {
                details[pid] = null;
              }
            })
          );
          setProductDetails(details);
        }
      } catch {
        setOrders([]);
      }
      setLoading(false);
    };

    if (userId) fetchOrdersAndProducts();
    // eslint-disable-next-line
  }, [userId]);

  // Helper to get normalized image url
  const getContentUrl = (url) => {
    if (!url) return "";
    if (url.startsWith("http://") || url.startsWith("https://")) {
      return url;
    }
    return BASE_URL + url.replace(/^\/+/, "");
  };

  return (
    <div className="max-w-3xl mx-auto bg-white rounded-lg shadow p-6 mt-8">
      <h2 className="text-2xl font-bold mb-6">My Orders</h2>
      {loading ? (
        <div className="text-center text-gray-500 py-8">Loading orders...</div>
      ) : orders.length === 0 ? (
        <div className="text-center text-gray-500 py-8">No orders found.</div>
      ) : (
        orders.map((order) => (
          <div key={order.orderId} className="mb-8 border-b pb-6">
            <div className="mb-2 flex justify-between items-center">
              <span className="font-semibold text-lg">
                Order #{order.orderId}
              </span>
              <span className="text-gray-700 font-semibold">
                Total: USD {order.totalOrderprice}
              </span>
            </div>
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              {order.items.map((item, idx) => {
                const product = productDetails[item.productId];
                const mainContent = product?.contents?.find(
                  (c) =>
                    c.type &&
                    typeof c.type === "string" &&
                    c.type.toLowerCase().startsWith("image")
                );
                return (
                  <div
                    key={order.orderId + "-" + idx}
                    className="flex items-center gap-4 border rounded p-3 bg-gray-50"
                  >
                    <div className="w-16 h-16 flex-shrink-0 bg-gray-200 rounded overflow-hidden flex items-center justify-center">
                      {mainContent ? (
                        <img
                          src={getContentUrl(mainContent.url)}
                          alt={product?.name || "Product"}
                          className="object-cover w-full h-full"
                        />
                      ) : (
                        <span className="text-gray-400 text-xs">No Image</span>
                      )}
                    </div>
                    <div>
                      <div className="font-semibold">
                        {product?.name || "Product"}
                      </div>
                      <div className="text-sm text-gray-600">
                        Quantity: {item.quantity}
                      </div>
                      <div className="text-sm text-gray-600">
                        Item Total: {item.itemTotalPrice}
                      </div>
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        ))
      )}
    </div>
  );
};

export default OrderView;