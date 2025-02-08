import api from "../axios/instance";
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
      throw e;
    }
  }
}
