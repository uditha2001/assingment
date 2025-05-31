import useAxiosPrivate from "../hooks/useAxiosPrivate"
const useOrderApi=()=>{
 const axiosPrivate = useAxiosPrivate();
 const getAllOrders = async (userId) => {
   const response = await axiosPrivate.get(`/order`, {
     params: { userId },
   });
   return response;
 };
 const createOrder = async (order) => {
   const response = await axiosPrivate.post(`/order`, order);
   return response;
 };

 return {
    getAllOrders,
    createOrder,
 }

}
export default useOrderApi;