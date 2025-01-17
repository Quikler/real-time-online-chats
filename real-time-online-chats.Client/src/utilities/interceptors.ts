import axios from "axios";
import Environment from "../environment/environment";
import { LocalStorageService } from "../services/local-storage-service";
import { AuthService } from "../services/auth-service";

const api = axios.create({
  baseURL: Environment.apiUrl,
});

// Add a request interceptor
// api.interceptors.request.use(
//   config => {
//     const token = LocalStorageService.getToken()
//     if (token) {
//       config.headers['Authorization'] = 'Bearer ' + token
//     }
//     // config.headers['Content-Type'] = 'application/json';
//     return config
//   },
//   error => {
//     Promise.reject(error)
//   }
// )

// // Add a response interceptor
// api.interceptors.response.use(
//   (response) => {
//     console.log("Intercept respose suc");
//     return response;
//   },
//   async (error) => {
//     const originalRequest = error.config

//     if (
//       error.response.status === 401 &&
//       originalRequest.url === AuthService.refreshTokenUrl
//     ) {
//       window.location.href = "/login";
//       return Promise.reject(error)
//     }

//     if (error.response.status === 401 && !originalRequest._retry) {
//       originalRequest._retry = true
//       const refreshToken = LocalStorageService.getRefreshToken()
//       return axios
//         .post('/auth/token', {
//           refresh_token: refreshToken
//         })
//         .then(res => {
//           if (res.status === 201) {
//             LocalStorageService.setToken(res.data)
//             axios.defaults.headers.common['Authorization'] =
//               'Bearer ' + LocalStorageService.getToken()
//             return axios(originalRequest)
//           }
//         })
//     }
//     return Promise.reject(error)
//   }
// );

export default api;