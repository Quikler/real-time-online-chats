import api from "@services/axios/instance";
import {
  AuthSuccessResponse,
  LoginRequest,
  ResetPasswordRequest,
  SignupRequest,
} from "@src/models/dtos/Auth";
import { AxiosRequestConfig } from "axios";
import { throwIfErrorNotCancelError } from "@src/utils/helpers";
import { AuthRoutes } from "./ApiRoutes";

export abstract class AuthService {
  static async resetPassword(request: ResetPasswordRequest, config?: AxiosRequestConfig<any>) {
    try {
      const response = await api.post<string>(
        AuthRoutes.resetPassword,
        request,
        config,
      );

        //`${AuthRoutes.resetPassword}?token=${request.token}&email=${request.email}&newPassword=${request.newPassword}`,
      return response.data;
    } catch (e: any) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async forgotPassword(email: string, config?: AxiosRequestConfig<any>) {
    try {
      const response = await api.post<string>(
        AuthRoutes.forgotPassword,
        { email },
        config,
      );

      return response.data;
    } catch (e: any) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async signup(
    request: SignupRequest,
    config?: AxiosRequestConfig<any> | undefined
  ) {
    try {
      const response = await api.post<string>(
        AuthRoutes.signup,
        request,
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

  static async login(
    request: LoginRequest,
    config?: AxiosRequestConfig<any> | undefined
  ) {
    try {
      const response = await api.post<AuthSuccessResponse>(
        AuthRoutes.login,
        request,
        {
          withCredentials: true,
          ...config,
        }
      );
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async logout(config?: AxiosRequestConfig<any> | undefined) {
    try {
      const response = await api.post<AuthSuccessResponse>(
        AuthRoutes.logout,
        {},
        {
          withCredentials: true,
          ...config,
        }
      );
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async me(config?: AxiosRequestConfig<any> | undefined) {
    try {
      const response = await api.get<AuthSuccessResponse>(AuthRoutes.me, {
        withCredentials: true,
        ...config,
      });
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }
}
