import ProductViewComponent from "../components/productViewComponent";
import useProductApi from "../api/productAPI/useProductApi";
import { useState, useEffect } from "react";
import SuccessMessage from "../components/SuccessMessage";
import ErrorMessage from "../components/ErrorMessage";
import { useSearchParams, useNavigate } from "react-router-dom";

const HomePage = () => {
  const [products, setProducts] = useState([]);
  const [filteredProducts, setFilteredProducts] = useState([]);
  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const { getAllProducts } = useProductApi();

  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const searchTerm = searchParams.get("search")?.toLowerCase() || "";

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const response = await getAllProducts();
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
  }, []);

  useEffect(() => {
    if (searchTerm) {
      const filtered = products.filter((product) =>
        product.name.toLowerCase().includes(searchTerm)
      );
      setFilteredProducts(filtered);
      return () => clearTimeout(timeout);
    } else {
      setFilteredProducts(products);
    }
  }, [searchTerm, products, navigate]);

  return (
    <div className="min-h-screen bg-gray-50 py-8 px-2">
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
      <div className="max-w-7xl mx-auto grid gap-6 grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4">
        {filteredProducts.map((product, idx) => (
          <ProductViewComponent
            key={idx}
            {...product}
            setSuccess={setSuccessMessage}
            setError={setErrorMessage}
          />
        ))}
      </div>
    </div>
  );
};

export default HomePage;
