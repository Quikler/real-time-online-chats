import api from "../axios/instance";
import { AuthRoutes } from "../api/ApiRoutes";
import { AxiosRequestConfig } from "axios";

export abstract class GoogleService {
  static async login(credential: string, config?: AxiosRequestConfig<any>) {
    try {
      const response = await api.get(
        `google/login?credential=${credential}`,
        { withCredentials: true, ...config }
      );
      return response.data;
    } catch (e: any) {
      throw new Error(e.message);
    }
  }

  static async signup(credential: string, config?: AxiosRequestConfig<any>) {
    try {
      const response = await api.get(
        `google/signup?credential=${credential}`,
        { withCredentials: true, ...config }
      );
      return response.data;
    } catch (e: any) {
      throw new Error(e.message);
    }
  }
}
