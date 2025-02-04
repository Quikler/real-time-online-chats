import { AxiosRequestConfig } from "axios";
import api from "../axios/instance";
import { UserRoutes } from "./ApiRoutes";
import { throwIfErrorNotCancelError } from "@src/utils/helpers";

export abstract class UserService {
  static async getUserProfile(userId: string, config?: AxiosRequestConfig<any> | undefined) {
    try {
      const response = await api.get(`${UserRoutes.base}/${userId}/${UserRoutes.profile}`, config);
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }
}
