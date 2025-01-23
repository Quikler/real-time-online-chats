import axios from "axios";
import Environment from "../environment/environment";

const api = axios.create({
  baseURL: Environment.apiUrl,
});

export default api;