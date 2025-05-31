import useAxiosPrivate from "../../hooks/useAxiosPrivate";
const useProductApi = () => {
    const axiosPrivate = useAxiosPrivate();
    
    const getAllProducts = async (params) => {
        const response = await axiosPrivate.get("/product/allProducts");
        return response;
    };
    
    const createProduct = async (newProduct) => {
        const response = await axiosPrivate.post("/product", newProduct);
        return response;
    };
    const getProductCategory = async () => {
        const response = await axiosPrivate.get(`/product/category`);
        return response;
    };
   const createContent = async (productId, images) => {
    const formData = new FormData();

    images.forEach((image) => {
        formData.append('images', image); // Key must match 'images' in backend
    });

    const response = await axiosPrivate.post(
        `/product/content?productId=${productId}`,
        formData,
        {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        }
    );

    return response;
};
const getOwnerProducts = async (userId) => {
    const response = await axiosPrivate.get('/product/ownerProducts', {
        params: { userId }
    });
    return response;
};
const createAttribute = async (productId, productsAtribute) => {
    const response = await axiosPrivate.post(
        '/product/attribute',
        productsAtribute,
        { params: { productId } }
    );
    return response;
};
const getProductById = async (productId) => {
    const response = await axiosPrivate.get("/product/productById", {
      params: { productId },
    });
    return response;
};
const deleteProduct = async (productId) => {
  const response = await axiosPrivate.delete("/product", {
    params: { productId },
  });
  return response;
};
const deleteContent = async (id) => {
  const response = await axiosPrivate.delete(`/product/content/${id}`);
  return response;
};
const checkoutOrders = async (order) => {
    const response = await axiosPrivate.post("/product/checkout",order);
    return response;
  };
  const sellProducts = async (request) => {
    return await axiosPrivate.patch("/product/sellProducts", request);
  };
  
  const updateProduct = async (product) => {
    const response = await axiosPrivate.patch(
      `/product/updateProduct`,
      product
    );
    return response;
  };




    return { 
        getAllProducts, 
        createProduct, 
        getProductCategory,
        createContent, 
        getOwnerProducts, 
        createAttribute,
        getProductById,
        deleteProduct,
        deleteContent,
        checkoutOrders,
        sellProducts,
        updateProduct
    };
    }

export default useProductApi;