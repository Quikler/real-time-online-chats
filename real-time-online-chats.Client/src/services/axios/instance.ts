import Environment from "@src/environment/environment";
import axios from "axios";

const api = axios.create({
  baseURL: Environment.apiUrl,
});

export default api;