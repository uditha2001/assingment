import { FiShoppingCart, FiEye } from "react-icons/fi";
import useCartApi from "../api/useCartApi";

const BASE_URL = "http://localhost:5010/";

/**
 * StarRating
 *
 * Displays a 5-star rating based on the provided rate.
 * Props:
 * - rate: number - The rating value (0-5).
 */
const StarRating = ({ rate }) => {
  const stars = [];
  for (let i = 1; i <= 5; i++) {
    stars.push(
      <span key={i} className={i <= rate ? "text-yellow-400" : "text-gray-300"}>
        ★
      </span>
    );
  }
  return <div className="flex">{stars}</div>;
};

/**
 * ProductViewComponent
 *
 * Card component for displaying product summary, image/video, and actions.
 * Props:
 * - id: number - Product ID.
 * - name: string - Product name.
 * - description: string - Product description.
 * - owner: string - Product owner.
 * - availableQuantity: number - Quantity available.
 * - rate: number - Product rating.
 * - price: number - Product price.
 * - currency: string - Currency symbol.
 * - contents: array - Media contents (images/videos).
 * - setSuccess: function - Callback for success messages.
 * - setError: function - Callback for error messages.
 */
const ProductViewComponent = ({
  id,
  name,
  description,
  owner,
  availableQuantity,
  rate,
  price,
  currency,
  contents = [],
  setSuccess,
  setError
}) => {
  const { addToCart } = useCartApi();

  // Selects the first image or video as the main content
  const mainContent = contents.find(
    (c) =>
      (c.type && c.type.toLowerCase().startsWith("image")) ||
      (c.type && c.type.toLowerCase().startsWith("video"))
  );

  // Resolves content URL for images/videos
  const getContentUrl = (url) => {
    if (!url) return "";
    if (url.startsWith("http://") || url.startsWith("https://")) {
      return url;
    }
    return BASE_URL + url.replace(/^\/+/, "");
  };

  // Handles adding the product to the cart
  const handleAddToCart = async () => {
    const user = JSON.parse(localStorage.getItem("user"));
    const cartItem = {
      ProductId: id,
      Quantity: 1,
      userId: user?.userId,
      itemTotalPrice: price,
    };
    try {
      const response = await addToCart(cartItem);
      if (response.status === 200) {
        setSuccess("Item added to cart successfully!");
      }
    } catch (error) {
      setError("Failed to add item to cart. Please try again.");
    }
  };

  return (
    <div className="max-w-xs rounded-lg overflow-hidden shadow bg-white border border-gray-200 flex flex-col">
      <div className="w-full h-32 bg-gray-100 flex items-center justify-center">
        {mainContent ? (
          mainContent.type.toLowerCase().startsWith("image") ? (
            <img
              src={getContentUrl(mainContent.url)}
              alt={name || "Product image"}
              className="object-cover w-full h-32"
            />
          ) : (
            <video
              src={getContentUrl(mainContent.url)}
              controls
              className="object-cover w-full h-32"
            />
          )
        ) : (
          <span className="text-gray-400">No Image/Video</span>
        )}
      </div>
      <div className="p-3 flex-1 flex flex-col">
        <h2 className="text-lg font-semibold mb-1">{name}</h2>
        <p className="text-gray-600 mb-2 text-sm line-clamp-2">{description}</p>
        <div className="flex items-center mb-2">
          <StarRating rate={rate} />
          <span className="ml-2 text-xs text-gray-500">{rate}/5</span>
        </div>
        <div className="flex items-center justify-between mb-2">
          <span className="text-base font-bold">
            {currency} {price.toFixed(2)}
          </span>
          <span className="text-xs text-gray-500">Qty: {availableQuantity}</span>
        </div>
        <div className="flex items-center justify-between mt-2">
          <span className="text-xs text-gray-400">Owner: {owner}</span>
          <button className="flex items-center text-blue-600 hover:underline text-xs">
            View
            <FiEye className="ml-1 text-base" />
          </button>
        </div>
      </div>
      <div className="p-3 pt-0">
        <button
          className="w-full flex items-center justify-center bg-black text-white py-2 rounded hover:bg-gray-800 transition text-sm"
          onClick={handleAddToCart}
        >
          <FiShoppingCart className="mr-2" />
          Add to Cart
        </button>
      </div>
    </div>
  );
};

export default ProductViewComponent;
