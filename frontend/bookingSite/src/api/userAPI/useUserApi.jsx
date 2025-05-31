import useAxiosPrivate from "../../hooks/useAxiosPrivate";

const useUserApi = () => {
  const axiosPrivate = useAxiosPrivate();

const login = async (userName, password) => {
  const response = await axiosPrivate.post('/authentication', null, {
    params: { userName, password }
  });
  return response;
};

const createUser = async (user) => {
  const response = await axiosPrivate.post("/user", user);
  return response;
};




  return { login, createUser };
};

export default useUserApi;
