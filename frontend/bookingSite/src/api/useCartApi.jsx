import useAxiosPrivate from "../hooks/useAxiosPrivate";
const useCartApi = () => {
     const axiosPrivate = useAxiosPrivate();
     const addToCart = async (item) => {
       const response = await axiosPrivate.post("/cart/add", item);
       return response;
     };
     const getCartByUserId = async (userId) => {
       const response = await axiosPrivate.get(`/cart/${userId}`);
       return response;
     };
     const updateCartQuantities = async (updateItemList) => {
       const response = await axiosPrivate.put(
         "/cart/update-quantity",
         updateItemList
       );
       return response;
     };
     const removeItemFromCart = async (itemId) => {
       const response = await axiosPrivate.delete(`/cart/remove/${itemId}`);
       return response
     };
     const clearCart = async (userId) => {
       const response = await axiosPrivate.delete(`/cart/clear/${userId}`);
       return response
     };
     const getCartItemCount = async (userId) => {
       const response = await axiosPrivate.get("/cart/count", {
         params: { userId },
       });
       return response;
     };
  
    
    
    
    
     return{
            addToCart,
            getCartByUserId,
            updateCartQuantities,
            removeItemFromCart,
            clearCart,
            getCartItemCount
     }
}
export default useCartApi;