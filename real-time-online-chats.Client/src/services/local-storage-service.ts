import { AuthTokenModel } from "../types/auth-types";

export abstract class LocalStorageService {
  static getAuthTokenModel(): AuthTokenModel {
    return {
      token: this.getToken(),
      refreshToken: this.getRefreshToken(),
    }
  }

  static getToken(): string {
    return localStorage.getItem("token") ?? "";
  }

  static getRefreshToken(): string {
    return localStorage.getItem("refreshToken") ?? "";
  }

  static setAuthTokenModel(model: AuthTokenModel) {
    this.setToken(model.token);
    this.setRefreshToken(model.refreshToken);
  }

  static setToken(token: string) {
    localStorage.setItem("token", token);
  }

  static setRefreshToken(refreshToken: string) {
    localStorage.setItem("refreshToken", refreshToken);
  }
}