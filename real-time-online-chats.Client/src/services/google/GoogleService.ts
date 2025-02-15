import { throwIfErrorNotCancelError } from "@src/utils/helpers";
import api from "../axios/instance";
import { AxiosRequestConfig } from "axios";

export abstract class GoogleService {
  static async login(credential: string, config?: AxiosRequestConfig<any>) {
    try {
      const response = await api.post(
        `google/login?credential=${credential}`,
        {},
        {
          withCredentials: true,
          ...config,
        }
      );
      return response.data;
    } catch (e: any) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async signup(credential: string, config?: AxiosRequestConfig<any>) {
    try {
      const response = await api.post(
        `google/signup?credential=${credential}`,
        {},
        {
          withCredentials: true,
          ...config,
        }
      );
      return response.data;
    } catch (e: any) {
      throwIfErrorNotCancelError(e);
    }
  }
}
