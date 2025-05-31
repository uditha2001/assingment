import React, { useState, useEffect } from "react";
import { FiEdit2, FiTrash2, FiPlus, FiX } from "react-icons/fi";
import { useLocation } from "react-router-dom";
import useProductApi from "../api/productAPI/useProductApi";
import SuccessMessage from "../components/SuccessMessage";
import ErrorMessage from "../components/ErrorMessage"; // Add this import

const BASE_URL = "http://localhost:5010/";

const EditProducts = ({ onSave, onDelete }) => {
  const location = useLocation();
  const product = location.state?.product;

  const [isEditing, setIsEditing] = useState(false);
  const [editProduct, setEditProduct] = useState({ ...product });
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [categories, setCategories] = useState([]);
  const [successMessage, setSuccessMessage] = useState(""); // Add this state
  const [errorMessage, setErrorMessage] = useState(""); // Add this state
  const { getProductCategory, deleteContent, updateProduct } = useProductApi(); // <-- add updateProduct

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const response = await getProductCategory();
        setCategories(response.data);
      } catch (error) {
        setCategories([]);
      }
    };
    fetchCategories();
  }, []); // <-- Only run once on mount
  useEffect(() => {
    console.log("Product to edit:", product);
  }, [editProduct]);

  // Handlers for product fields
  const handleChange = (e) => {
    const { name, value } = e.target;
    setEditProduct((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  // Attribute handlers
  const handleAttributeChange = (idx, key, value) => {
    const updated = [...editProduct.attributes];
    updated[idx][key] = value;
    setEditProduct((prev) => ({
      ...prev,
      attributes: updated,
    }));
  };

  const handleDeleteAttribute = (idx) => {
    setEditProduct((prev) => ({
      ...prev,
      attributes: prev.attributes.filter((_, i) => i !== idx),
    }));
  };

  // Content handlers
  const handleDeleteContent = async (idx) => {
    const content = editProduct.contents[idx];
    console.log(
      "Deleting content at index:",
      content.contentId,
      "with idx:",
      idx
    );
    if (content && content.contentId) {
      try {
        const response = await deleteContent(content.contentId);
        if (response.status === 200 || response.status === 204) {
          setEditProduct((prev) => ({
            ...prev,
            contents: prev.contents.filter((_, i) => i !== idx),
          }));
        } else {
          alert("Failed to delete image from server.");
        }
      } catch (err) {
        alert("Error deleting image from server.");
      }
    } else {
      // If not saved yet, just remove from local state
      setEditProduct((prev) => ({
        ...prev,
        contents: prev.contents.filter((_, i) => i !== idx),
      }));
    }
  };

  // Add new attribute
  const [newAttr, setNewAttr] = useState({ key: "", value: "" });

  const handleAddAttribute = () => {
    if (newAttr.key && newAttr.value) {
      setEditProduct((prev) => ({
        ...prev,
        attributes: [
          ...prev.attributes,
          {
            attributeId: 0, // Always 0 for new
            provider: "", // Always empty string for new
            key: newAttr.key,
            value: newAttr.value,
          },
        ],
      }));
      setNewAttr({ key: "", value: "" });
    }
  };

  // Add new content (image)
  const handleAddContent = (e) => {
    const files = Array.from(e.target.files);
    const newContents = files.map((file) => ({
      url: URL.createObjectURL(file),
      name: file.name,
      type: "image",
      file, // keep file for upload
    }));
    setEditProduct((prev) => ({
      ...prev,
      contents: [...prev.contents, ...newContents],
    }));
  };

  // Save handler
  const handleSave = async () => {
    // Get userId for createdBy
    const user = JSON.parse(localStorage.getItem("user") || "null");
    const userId = user?.userId;

    // Prepare product object for API
    const updatedProduct = {
      name: editProduct.name,
      description: editProduct.description,
      owner: editProduct.owner,
      availableQuantity: Number(editProduct.availableQuantity),
      rate: Number(editProduct.rate) || 0,
      price: Number(editProduct.price),
      currency: editProduct.currency,
      createdBy: userId,
      productCategoryId: Number(editProduct.productCategoryId),
      attributes: editProduct.attributes,
      contents: editProduct.contents,
      id: editProduct.id, // include id if needed by backend
    };

    try {
      console.log("Saving product:", updatedProduct);
      const response = await updateProduct(updatedProduct);
      // Check if response.status is 200 and response.data is true
      if (response.status === 200 && response.data === true) {
        if (onSave) onSave(updatedProduct);
        setIsEditing(false);
        setSuccessMessage("Product updated successfully!");
        setErrorMessage("");
        setTimeout(() => setSuccessMessage(""), 2000);
      } else {
        setErrorMessage("Failed to update product.");
        setSuccessMessage("");
      }
    } catch (err) {
      setErrorMessage("Error updating product.");
      setSuccessMessage("");
    }
  };

  // Delete handler
  const handleDelete = () => {
    setShowDeleteConfirm(false);
    if (onDelete) onDelete(product.id);
  };

  // Normalize URL to full URL if relative
  const getContentUrl = (url) => {
    if (!url) return "";
    if (url.startsWith("http://") || url.startsWith("https://")) {
      return url;
    }
    return BASE_URL + url.replace(/^\/+/, "");
  };

  // Find current category name
  const currentCategory =
    categories.find((cat) => cat.id === Number(editProduct.productCategoryId))
      ?.name || "No Category";

  return (
    <div className="max-w-2xl mx-auto bg-white rounded-lg shadow p-6">
      {successMessage && <SuccessMessage message={successMessage} />}
      {errorMessage && <ErrorMessage message={errorMessage} />}
      <div className="flex justify-between items-center mb-4">
        <h2 className="text-2xl font-bold">{editProduct.name}</h2>
        <div className="flex gap-2">
          {!isEditing && (
            <>
              <button
                className="text-blue-600 hover:text-blue-800"
                onClick={() => setIsEditing(true)}
                title="Edit Product"
              >
                <FiEdit2 />
              </button>
              <button
                className="text-red-600 hover:text-red-800"
                onClick={() => setShowDeleteConfirm(true)}
                title="Delete Product"
              >
                <FiTrash2 />
              </button>
            </>
          )}
        </div>
      </div>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
        <div>
          <label className="block text-sm mb-1">Product Name</label>
          <input
            type="text"
            name="name"
            value={editProduct.name}
            onChange={handleChange}
            className="w-full border rounded px-2 py-1 mb-2"
            disabled={!isEditing}
          />
        </div>
        <div>
          <label className="block text-sm mb-1">Category</label>
          {isEditing ? (
            <select
              name="productCategoryId"
              value={editProduct.productCategoryId}
              onChange={handleChange}
              className="w-full border rounded px-2 py-1 mb-2"
            >
              <option value="">Select Category</option>
              {categories.map((cat) => (
                <option key={cat.id} value={cat.id}>
                  {cat.name}
                </option>
              ))}
            </select>
          ) : (
            <div className="w-full border rounded px-2 py-1 mb-2 bg-gray-100">
              {currentCategory}
            </div>
          )}
        </div>
        <div>
          <label className="block text-sm mb-1">Price</label>
          <input
            type="number"
            name="price"
            value={editProduct.price}
            onChange={handleChange}
            className="w-full border rounded px-2 py-1 mb-2"
            disabled={!isEditing}
          />
        </div>
        <div>
          <label className="block text-sm mb-1">Currency</label>
          <input
            type="text"
            name="currency"
            value={editProduct.currency}
            onChange={handleChange}
            className="w-full border rounded px-2 py-1 mb-2"
            disabled={!isEditing}
          />
        </div>
        <div>
          <label className="block text-sm mb-1">Available Quantity</label>
          <input
            type="number"
            name="availableQuantity"
            value={editProduct.availableQuantity}
            onChange={handleChange}
            className="w-full border rounded px-2 py-1 mb-2"
            disabled={!isEditing}
          />
        </div>
        <div>
          <label className="block text-sm mb-1">Owner</label>
          <input
            type="text"
            name="owner"
            value={editProduct.owner}
            onChange={handleChange}
            className="w-full border rounded px-2 py-1 mb-2"
            disabled={!isEditing}
          />
        </div>
      </div>
      <div>
        <label className="block text-sm mb-1">Description</label>
        <textarea
          name="description"
          value={editProduct.description}
          onChange={handleChange}
          className="w-full border rounded px-2 py-1 mb-2"
          rows={2}
          disabled={!isEditing}
        />
      </div>
      {/* Attributes */}
      <div className="mb-4">
        <label className="block text-sm mb-1">Attributes</label>
        <div className="flex flex-wrap gap-2 mb-2">
          {editProduct.attributes.map((attr, idx) => (
            <div
              key={idx}
              className="flex items-center bg-gray-100 rounded px-2 py-1 text-xs"
            >
              {isEditing ? (
                <>
                  <input
                    type="text"
                    value={attr.key}
                    onChange={(e) =>
                      handleAttributeChange(idx, "key", e.target.value)
                    }
                    className="border rounded px-1 py-0.5 mr-1"
                  />
                  <input
                    type="text"
                    value={attr.value}
                    onChange={(e) =>
                      handleAttributeChange(idx, "value", e.target.value)
                    }
                    className="border rounded px-1 py-0.5 mr-1"
                  />
                  <button
                    type="button"
                    className="text-red-500"
                    onClick={() => handleDeleteAttribute(idx)}
                    title="Remove"
                  >
                    <FiX />
                  </button>
                </>
              ) : (
                <>
                  <span className="mr-1 font-semibold">{attr.key}:</span>
                  <span>{attr.value}</span>
                </>
              )}
            </div>
          ))}
        </div>
        {isEditing && (
          <div className="flex gap-2 mb-2">
            <input
              type="text"
              value={newAttr.key}
              onChange={(e) =>
                setNewAttr((attr) => ({ ...attr, key: e.target.value }))
              }
              placeholder="Attribute Name"
              className="border rounded px-2 py-1 w-1/2"
            />
            <input
              type="text"
              value={newAttr.value}
              onChange={(e) =>
                setNewAttr((attr) => ({ ...attr, value: e.target.value }))
              }
              placeholder="Attribute Value"
              className="border rounded px-2 py-1 w-1/2"
            />
            <button
              type="button"
              className="bg-black text-white px-3 py-1 rounded flex items-center"
              onClick={handleAddAttribute}
            >
              <FiPlus />
            </button>
          </div>
        )}
      </div>
      {/* Contents */}
      <div className="mb-4">
        <label className="block text-sm mb-1">Product Images</label>
        <div className="flex flex-wrap gap-2 mb-2">
          {(isEditing
            ? editProduct.contents // Show all images in edit mode
            : editProduct.contents.filter(
                (c) => c.type?.toLowerCase() === "image"
              )
          ) // Only images in view mode
            .map((content, idx) => (
              <div
                key={idx}
                className="relative w-16 h-16 border rounded overflow-hidden flex items-center justify-center bg-gray-100"
              >
                <img
                  src={getContentUrl(content.url)}
                  alt={content.name}
                  className="object-cover w-full h-full"
                />
                {isEditing && (
                  <button
                    type="button"
                    className="absolute top-0 right-0 bg-white rounded-bl px-1 py-0.5 text-red-600"
                    onClick={() => handleDeleteContent(idx)}
                    title="Remove"
                  >
                    <FiX />
                  </button>
                )}
              </div>
            ))}
        </div>
        {isEditing && (
          <input
            type="file"
            multiple
            accept="image/*"
            onChange={handleAddContent}
            className="border rounded px-2 py-1"
          />
        )}
      </div>
      {/* Action Buttons */}
      {isEditing && (
        <div className="flex gap-2">
          <button
            className="bg-black text-white px-6 py-2 rounded hover:bg-gray-800 transition text-base font-semibold"
            onClick={handleSave}
          >
            Save
          </button>
          <button
            className="bg-gray-200 text-black px-6 py-2 rounded hover:bg-gray-300 transition text-base font-semibold"
            onClick={() => setIsEditing(false)}
          >
            Cancel
          </button>
        </div>
      )}
      {/* Delete Confirmation */}
      {showDeleteConfirm && (
        <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-30 z-50">
          <div className="bg-white p-6 rounded shadow-lg">
            <p className="mb-4">
              Are you sure you want to delete this product?
            </p>
            <div className="flex gap-4">
              <button
                className="bg-red-600 text-white px-4 py-2 rounded"
                onClick={handleDelete}
              >
                Yes, Delete
              </button>
              <button
                className="bg-gray-300 text-black px-4 py-2 rounded"
                onClick={() => setShowDeleteConfirm(false)}
              >
                Cancel
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default EditProducts;
