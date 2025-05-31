import axios from 'axios';
const baseURL = "http://localhost:5010/api/v1" ;
console.log("API Base URL:", baseURL);

const privateAxios = axios.create({
    baseURL: baseURL,
    headers: {
        'Content-Type': 'application/json'
         
    },
    withCredentials: true
    });
    export default privateAxios;