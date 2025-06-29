import { AxiosRequestConfig } from "axios";
import api from "../axios/instance";
import { UserRoutes } from "./ApiRoutes";
import { throwIfErrorNotCancelError } from "@src/utils/helpers";
import { EditUserProfileType } from "@src/pages/profile/profile.types";

export abstract class UserService {
  static async getUserProfile(userId: string, config?: AxiosRequestConfig<any> | undefined) {
    try {
      const response = await api.get(`${UserRoutes.base}/${userId}/${UserRoutes.profile}`, config);
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async updateUserProfile(
    userId: string,
    request: EditUserProfileType,
    config?: AxiosRequestConfig<any> | undefined
  ) {
    try {
      console.log(`${UserRoutes.base}/${userId}/${UserRoutes.profile}`);

      const response = await api.put(
        `${UserRoutes.base}/${userId}/${UserRoutes.profile}`,
        request,
        { ...config, headers: { "Content-Type": "multipart/form-data" } }
      );
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async getUserOwnerChats(
    userId: string,
    config?: AxiosRequestConfig<any>
  ) {
    try {
      const response = await api.get(`${UserRoutes.base}/${userId}/owner-chats`, config);
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e)
    }
  }
}
