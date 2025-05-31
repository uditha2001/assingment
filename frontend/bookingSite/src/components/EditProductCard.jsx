import { FiEdit2, FiTrash2 } from "react-icons/fi";
import { useNavigate } from "react-router-dom";

const EditProductCard = ({
    product,
    onDelete,
}) => {
    const navigate = useNavigate();

    // Show first image if available
    const imageUrl =
        product.Contents && product.Contents.length > 0
            ? product.Contents[0].url
            : "https://via.placeholder.com/150x150?text=No+Image";

    return (
        <div className="bg-white rounded-lg shadow p-4 flex flex-col relative">
            <img
                src={imageUrl}
                alt={product.Name}
                className="w-full h-40 object-cover rounded mb-3"
            />
            <div className="flex-1">
                <h3 className="font-bold text-lg mb-1">{product.Name}</h3>
                <div className="text-sm text-gray-500 mb-1">
                    {product.CategoryName || "No Category"}
                </div>
                <div className="text-xs text-gray-600 mb-2 line-clamp-2">
                    {product.Description}
                </div>
                <div className="mb-1 text-sm">
                    <span className="font-semibold">Price:</span> {product.Currency} {product.Price}
                </div>
                <div className="mb-1 text-sm">
                    <span className="font-semibold">Qty:</span> {product.availableQuantity}
                </div>
                <div className="flex flex-wrap gap-1 mt-2">
                    {product.Attributes &&
                        product.Attributes.slice(0, 2).map((attr, idx) => (
                            <span
                                key={idx}
                                className="bg-gray-100 rounded px-2 py-0.5 text-xs"
                            >
                                {attr.key}: {attr.value}
                            </span>
                        ))}
                    {product.Attributes && product.Attributes.length > 2 && (
                        <span className="text-xs text-gray-400">
                            +{product.Attributes.length - 2} more
                        </span>
                    )}
                </div>
            </div>
            <div className="flex justify-end gap-2 mt-4">
                <button
                    className="text-blue-600 hover:text-blue-800"
                    onClick={() => navigate("/editProducts")}
                    title="Edit"
                >
                    <FiEdit2 />
                </button>
                <button
                    className="text-red-600 hover:text-red-800"
                    onClick={onDelete}
                    title="Delete"
                >
                    <FiTrash2 />
                </button>
            </div>
        </div>
    );
};

export default EditProductCard;