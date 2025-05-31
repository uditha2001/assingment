import { FiTrash2, FiEdit2, FiX } from "react-icons/fi";
import { useNavigate } from "react-router-dom";
import useProductApi from "../api/productAPI/useProductApi";
import { useState } from "react";

// Simple confirmation dialog component
const ConfirmDialog = ({ message, onConfirm, onCancel }) => (
  <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-30 z-50">
    <div className="bg-white p-6 rounded shadow-lg">
      <p className="mb-4">{message}</p>
      <div className="flex gap-4">
        <button
          className="bg-red-600 text-white px-4 py-2 rounded"
          onClick={onConfirm}
        >
          Yes, Delete
        </button>
        <button
          className="bg-gray-300 text-black px-4 py-2 rounded"
          onClick={onCancel}
        >
          Cancel
        </button>
      </div>
    </div>
  </div>
);

const BASE_URL = "http://localhost:5010/";

const ProductDetailsView = ({
    product,
    categories = [],
    onDeleteProduct,
    onEditProduct,
    onDeleteContent,
    onDeleteAttribute,
    onEditContent,
    onEditAttribute,
}) => {
    const navigate = useNavigate();
    const { deleteProduct } = useProductApi();
    const [showConfirm, setShowConfirm] = useState(false);

    const handleDelete = async () => {
        setShowConfirm(true);
    };

    const confirmDelete = async () => {
        setShowConfirm(false);
        const response = await deleteProduct(product.id);
        if (response.status === 200 || response.status === 204) {
            if (onDeleteProduct) onDeleteProduct();
        } else {
            alert("Failed to delete product.");
        }
    };

    if (!product) {
        return <div className="text-center text-gray-500 py-8">No product selected.</div>;
    }

    const categoryName =
        product.categoryName ||
        categories.find((c) => c.id === Number(product.productCategoryId))?.name ||
        "No Category";

        const getContentUrl = (url) => {
        console.log("getContentUrl", url);
          if (!url) return "";
          if (url.startsWith("http://") || url.startsWith("https://")) {
            console.log("External URL", url);
            return url;
          }
          console.log("BASE_URL", BASE_URL+url.replace(/^\/+/, ""));
          return BASE_URL + url.replace(/^\/+/, "");
        };
    

    const handleEdit = () => {
        navigate("/editProducts", { state: { product } });
    };

    const images = Array.isArray(product.contents)
      ? product.contents.filter(c => c.type && c.type.toLowerCase().startsWith("image"))
      : [];
    console.log("Filtered images", images);

    return (
        <div className="max-w-2xl mx-auto bg-white rounded-lg shadow p-6">
            {showConfirm && (
                <ConfirmDialog
                    message="Are you sure you want to delete this product?"
                    onConfirm={confirmDelete}
                    onCancel={() => setShowConfirm(false)}
                />
            )}
            <div className="flex justify-between items-center mb-4">
                <h2 className="text-2xl font-bold">{product.name}</h2>
                <div className="flex gap-2">
                    <button
                        className="text-blue-600 hover:text-blue-800"
                        onClick={handleEdit}
                        title="Edit Product"
                    >
                        <FiEdit2 />
                    </button>
                    <button
                        className="text-red-600 hover:text-red-800"
                        onClick={handleDelete}
                        title="Delete Product"
                    >
                        <FiTrash2 />
                    </button>
                </div>
            </div>
            <div className="mb-2 text-gray-600">
                <span className="font-semibold">Category:</span> {categoryName}
            </div>
            <div className="mb-2">
                <span className="font-semibold">Description:</span> {product.description}
            </div>
            <div className="mb-2">
                <span className="font-semibold">Price:</span> {product.currency} {Number(product.price).toFixed(2)}
            </div>
            <div className="mb-2">
                <span className="font-semibold">Available Quantity:</span> {product.availableQuantity}
            </div>
            <div className="mb-2">
                <span className="font-semibold">Owner:</span> {product.owner}
            </div>
            {/* All Images */}
            <div className="mb-4">
                <span className="font-semibold">Images:</span>
                <div className="flex flex-wrap gap-3 mt-2">
                    {images.length > 0 ? (
                        images.map((content, idx) => (
                            <div
                                key={idx}
                                className="relative w-20 h-20 border rounded overflow-hidden flex items-center justify-center bg-gray-100"
                            >
                                <img
                                    src={getContentUrl(content.url)}
                                    alt={content.name}
                                    className="object-cover w-full h-full"
                                />
                                {onDeleteContent && (
                                    <button
                                        type="button"
                                        className="absolute top-0 right-0 bg-white rounded-bl px-1 py-0.5 text-red-600"
                                        onClick={() => onDeleteContent(idx)}
                                        title="Delete Image"
                                    >
                                        <FiX />
                                    </button>
                                )}
                                {onEditContent && (
                                    <button
                                        type="button"
                                        className="absolute bottom-0 right-0 bg-white rounded-tl px-1 py-0.5 text-blue-600"
                                        onClick={() => onEditContent(idx)}
                                        title="Edit Image"
                                    >
                                        <FiEdit2 />
                                    </button>
                                )}
                            </div>
                        ))
                    ) : (
                        <div className="text-gray-400 text-sm">No images.</div>
                    )}
                </div>
            </div>
            {/* Attributes */}
            <div className="mb-4">
                <span className="font-semibold">Attributes:</span>
                <ul className="ml-2 mt-2 list-disc text-sm">
                    {Array.isArray(product.attributes) && product.attributes.length > 0 ? (
                        product.attributes.map((attr, idx) => (
                            <li key={idx} className="flex items-center">
                                <span className="mr-1 font-semibold">{attr.key}:</span>
                                <span>{attr.value}</span>
                                {onDeleteAttribute && (
                                    <button
                                        type="button"
                                        className="ml-2 text-red-500"
                                        onClick={() => onDeleteAttribute(idx)}
                                        title="Delete Attribute"
                                    >
                                        <FiX />
                                    </button>
                                )}
                                {onEditAttribute && (
                                    <button
                                        type="button"
                                        className="ml-1 text-blue-500"
                                        onClick={() => onEditAttribute(idx)}
                                        title="Edit Attribute"
                                    >
                                        <FiEdit2 />
                                    </button>
                                )}
                            </li>
                        ))
                    ) : (
                        <li className="text-gray-400">No attributes.</li>
                    )}
                </ul>
            </div>
        </div>
    );
};

export default ProductDetailsView;