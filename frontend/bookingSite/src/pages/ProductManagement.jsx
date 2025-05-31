import { use, useEffect, useState } from "react";
import { FiTrash2, FiPlus, FiX } from "react-icons/fi";
import useProductApi from "../api/productAPI/useProductApi";
import SuccessMessage from "../components/SuccessMessage";
import ErrorMessage from "../components/ErrorMessage";
import ProductDetailsView from "../components/ProductDetailsView"; // Import the new component

const initialProduct = {
  Name: "",
  Description: "",
  availableQuantity: 0,
  Price: "",
  Currency: "USD",
  ProductCategoryId: "",
  Owner: "",
  Attributes: [],
  Contents: [],
};

const ProductManagement = () => {
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const [newProduct, setNewProduct] = useState(initialProduct);
  const [attribute, setAttribute] = useState({ key: "", value: "" });
  const [contentFiles, setContentFiles] = useState([]);
  const [showAddProduct, setShowAddProduct] = useState(false);
  const [isAddProduct, setIsAddProduct] = useState(false);
  const {
    getProductCategory,
    createProduct,
    createContent,
    getOwnerProducts,
    createAttribute,
  } = useProductApi();

  useEffect(() => {
    const getProductCategories = async () => {
      try {
        const response = await getProductCategory();
        setCategories(response.data);
      } catch (error) {
        console.error("Failed to fetch categories:", error);
      }
    };
    const fetchProducts = async () => {
      try {
        const userId = localStorage.getItem("user")
          ? JSON.parse(localStorage.getItem("user")).userId
          : "";
        const response = await getOwnerProducts(userId);
        if (response.status === 200) {
          setProducts(response.data);
        } else {
          console.error("Failed to fetch products");
        }
      } catch (error) {
        console.error("Error fetching products:", error);
      }
    };
    fetchProducts();
    getProductCategories();
  }, []);
  useEffect(() => {
    console.log("Products updated:", products);
  }, [products]);

  useEffect(() => {
    if (isAddProduct) {
      const addProduct = async () => {
        try {
          const { Attributes, Contents, ...productPayload } = newProduct;
          const response = await createProduct(productPayload);

          const createdProductId = response.data;
          if (response.status === 200) {
            
            const allFiles = (newProduct.Contents || [])
              .map((c) => c.file)
              .filter(Boolean);
            if (allFiles.length > 0 && createdProductId) {
              const formData = new FormData();
              allFiles.forEach((file) => {
                formData.append("images", file);
              });
              await createContent(createdProductId, formData);
            }

            // Upload attributes if any
            let allAttrSuccess = true;
            console.log("Attributes to save:", Attributes);
            if (createdProductId && Attributes.length > 0) {
              console.log("Attributes to save:", Attributes);
              const attrResponse = await createAttribute(
                createdProductId,
                Attributes
              );
              allAttrSuccess = attrResponse.status === 200;
            }

            if (allAttrSuccess) {
              setSuccessMessage(
                "Product, images, and attributes created successfully!"
              );
            } else {
              setErrorMessage(
                "Product created, but one or more attributes failed to save."
              );
            }
          }

          setNewProduct({
            ...initialProduct,
            ProductCategoryId:
              categories.length > 0 ? Number(categories[0].id) : "",
          });
          setContentFiles([]);
        } catch (error) {
          setErrorMessage("Failed to create product.");
          console.error("Failed to create product:", error);
        } finally {
          setIsAddProduct(false);
        }
      };
      addProduct();
    }
  }, [newProduct, isAddProduct]);
 

  // Handlers for product form
  const handleProductChange = (e) => {
    const { name, value } = e.target;
    setNewProduct((prev) => ({
      ...prev,
      [name]: name === "ProductCategoryId" ? Number(value) : value,
      createdBy: localStorage.getItem("user")
        ? JSON.parse(localStorage.getItem("user")).userId
        : "",
    }));
  };

  // Attribute handlers
  const handleAttributeChange = (e) => {
    const { name, value } = e.target;
    setAttribute((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const addAttribute = () => {
    if (attribute.key && attribute.value) {
      setNewProduct((prev) => ({
        ...prev,
        Attributes: [
          ...prev.Attributes,
          {
            attributeId: 0,
            provider: "",
            key: attribute.key,
            value: attribute.value
          }
        ],
      }));
      setAttribute({ key: "", value: "" });
    }
  };

  const removeAttribute = (idx) => {
    setNewProduct((prev) => ({
      ...prev,
      Attributes: prev.Attributes.filter((_, i) => i !== idx),
    }));
  };

  // Content handlers
  const handleContentChange = (e) => {
    const files = Array.from(e.target.files);
    setContentFiles(files);
  };

  const removeContent = (idx) => {
    setNewProduct((prev) => ({
      ...prev,
      Contents: prev.Contents.filter((_, i) => i !== idx),
    }));
  };

  // Product actions (UI only)
  const handleAddProduct = (e) => {
    e.preventDefault();
    setIsAddProduct(true);
    setShowAddProduct(false);
  };

  const handleDeleteProduct = (idx) => {
    setProducts((prev) => prev.filter((_, i) => i !== idx));
  };

  // Add this handler above your component return:
  const handleAddImages = () => {
    if (contentFiles.length > 0) {
      setNewProduct((prev) => ({
        ...prev,
        Contents: [
          ...prev.Contents,
          ...contentFiles.map((file) => ({
            url: URL.createObjectURL(file),
            name: file.name,
            file,
          })),
        ],
      }));
      setContentFiles([]);
    }
  };

  return (
    <div className="max-w-5xl mx-auto py-8 px-2">
      <h2 className="text-2xl font-bold mb-6">Product Management</h2>

      {/* Show Success and Error Messages directly after the title, before the Add New Product button */}
      {successMessage && (
        <div className="mb-4">
          <SuccessMessage
            message={successMessage}
            onClose={() => setSuccessMessage("")}
          />
        </div>
      )}
      {errorMessage && (
        <div className="mb-4">
          <ErrorMessage
            message={errorMessage}
            onClose={() => setErrorMessage("")}
          />
        </div>
      )}

      <button
        className="mb-6 bg-black text-white px-6 py-2 rounded hover:bg-gray-800 transition text-base font-semibold flex items-center"
        onClick={() => setShowAddProduct((prev) => !prev)}
      >
        <FiPlus className="mr-2" /> Add New Product
      </button>
      {showAddProduct && (
        <form
          className="bg-white rounded-lg shadow p-6 mb-10"
          onSubmit={handleAddProduct}
        >
          <h3 className="text-lg font-semibold mb-4">Add New Product</h3>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm mb-1">Product Name</label>
              <input
                type="text"
                name="Name"
                value={newProduct.Name}
                onChange={handleProductChange}
                className="w-full border rounded px-2 py-1 mb-2"
                required
              />
            </div>
            <div>
              <label className="block text-sm mb-1">Category</label>
              <select
                name="ProductCategoryId"
                value={newProduct.ProductCategoryId}
                onChange={handleProductChange}
                className="w-full border rounded px-2 py-1 mb-2"
                required
              >
                <option value="">Select Category</option>
                {categories.map((cat) => (
                  <option key={cat.id} value={cat.id}>
                    {cat.name}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-sm mb-1">Price</label>
              <input
                type="number"
                name="Price"
                value={newProduct.Price}
                onChange={handleProductChange}
                className="w-full border rounded px-2 py-1 mb-2"
                required
              />
            </div>
            <div>
              <label className="block text-sm mb-1">Currency</label>
              <input
                type="text"
                name="Currency"
                value={newProduct.Currency}
                onChange={handleProductChange}
                className="w-full border rounded px-2 py-1 mb-2"
                required
              />
            </div>
            <div>
              <label className="block text-sm mb-1">Available Quantity</label>
              <input
                type="number"
                name="availableQuantity"
                value={newProduct.availableQuantity}
                onChange={handleProductChange}
                className="w-full border rounded px-2 py-1 mb-2"
                required
              />
            </div>
            <div>
              <label className="block text-sm mb-1">Owner</label>
              <input
                type="text"
                name="Owner"
                value={newProduct.Owner}
                onChange={handleProductChange}
                className="w-full border rounded px-2 py-1 mb-2"
                placeholder="Enter owner name"
              />
            </div>
          </div>
          <div>
            <label className="block text-sm mb-1">Description</label>
            <textarea
              name="Description"
              value={newProduct.Description}
              onChange={handleProductChange}
              className="w-full border rounded px-2 py-1 mb-2"
              rows={2}
              required
            />
          </div>
          {/* Attributes */}
          <div className="mb-4">
            <label className="block text-sm mb-1">Attributes</label>
            <div className="flex gap-2 mb-2">
              <input
                type="text"
                name="key"
                value={attribute.key}
                onChange={handleAttributeChange}
                placeholder="Attribute Name"
                className="border rounded px-2 py-1 w-1/2"
              />
              <input
                type="text"
                name="value"
                value={attribute.value}
                onChange={handleAttributeChange}
                placeholder="Attribute Value"
                className="border rounded px-2 py-1 w-1/2"
              />
              <button
                type="button"
                className="bg-black text-white px-3 py-1 rounded flex items-center"
                onClick={addAttribute}
              >
                <FiPlus />
              </button>
            </div>
            <div className="flex flex-wrap gap-2">
              {newProduct.Attributes.map((attr, idx) => (
                <div
                  key={idx}
                  className="flex items-center bg-gray-100 rounded px-2 py-1 text-xs"
                >
                  <span className="mr-1 font-semibold">{attr.key}:</span>
                  <span>{attr.value}</span>
                  <button
                    type="button"
                    className="ml-2 text-red-500"
                    onClick={() => removeAttribute(idx)}
                    title="Remove"
                  >
                    <FiX />
                  </button>
                </div>
              ))}
            </div>
          </div>
          {/* Contents */}
          <div className="mb-4">
            <label className="block text-sm mb-1">Product Images</label>
            <div className="flex gap-2 mb-2">
              <input
                type="file"
                multiple
                accept="image/*"
                onChange={handleContentChange}
                className="border rounded px-2 py-1"
              />
              <button
                type="button"
                className="bg-black text-white px-3 py-1 rounded flex items-center ml-2"
                onClick={handleAddImages}
                disabled={contentFiles.length === 0}
              >
                <FiPlus /> Add
              </button>
            </div>
            <div className="flex flex-wrap gap-2">
              {newProduct.Contents.map((content, idx) => (
                <div
                  key={idx}
                  className="relative w-16 h-16 border rounded overflow-hidden flex items-center justify-center bg-gray-100"
                >
                  <img
                    src={content.url}
                    alt={content.name}
                    className="object-cover w-full h-full"
                  />
                  <button
                    type="button"
                    className="absolute top-0 right-0 bg-white rounded-bl px-1 py-0.5 text-red-600"
                    onClick={() => removeContent(idx)}
                    title="Remove"
                  >
                    <FiTrash2 />
                  </button>
                </div>
              ))}
              {contentFiles.map((file, idx) => (
                <div
                  key={`preview-${idx}`}
                  className="relative w-16 h-16 border rounded overflow-hidden flex items-center justify-center bg-gray-100"
                >
                  <img
                    src={URL.createObjectURL(file)}
                    alt={file.name}
                    className="object-cover w-full h-full"
                  />
                </div>
              ))}
            </div>
          </div>
          <div className="flex gap-2">
            <button
              type="submit"
              className="bg-black text-white px-6 py-2 rounded hover:bg-gray-800 transition text-base font-semibold"
              onClick={handleAddProduct}
            >
              Add Product
            </button>
            <button
              type="button"
              className="bg-gray-200 text-black px-6 py-2 rounded hover:bg-gray-300 transition text-base font-semibold"
              onClick={() => setShowAddProduct(false)}
            >
              Cancel
            </button>
          </div>
        </form>
      )}

      {/* List of Products */}
      <div>
        <h3 className="text-lg font-semibold mb-4">Your Products</h3>
        {products.length === 0 ? (
          <div className="text-gray-500 text-center py-8">No products yet.</div>
        ) : (
          <div className="grid gap-6 grid-cols-1 sm:grid-cols-2 md:grid-cols-3">
            {products.map((product, idx) => (
              <ProductDetailsView
                key={product.id || idx}
                product={product}
                categories={categories}
                onDeleteProduct={() => handleDeleteProduct(idx)}
              />
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default ProductManagement;
