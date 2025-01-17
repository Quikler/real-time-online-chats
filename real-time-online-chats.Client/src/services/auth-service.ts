import Environment from "../environment/environment";
import { Observable } from "rxjs";
import api from "../utilities/interceptors";
import { AuthTokenModel } from "../types/auth-types";
import { LocalStorageService } from "./local-storage-service";
import { SignupFormData } from "../components/forms/sign-up-form";
import { LoginFormData } from "../components/forms/log-in-form";
import { handleError } from "../helpers/error-handler";

export abstract class AuthService {
    static authUrl = `${Environment.apiUrl}/identity`;

    static signupUrl = `${this.authUrl}/signup`;
    static loginUrl = `${this.authUrl}/login`;
    static refreshTokenUrl = `${this.authUrl}/refresh`;
    static meUrl = `${this.authUrl}/me`;

    static signup(data: SignupFormData): Observable<any> {
      return new Observable((observer) => {
        api.post(this.signupUrl, data)
          .then((response) => {
            LocalStorageService.setAuthTokenModel(response.data as AuthTokenModel);

            observer.next(response); // pass the result to the subscriber
            observer.complete(); // signal successful completion
          })
          .catch((error) => {
            handleError(error);
            observer.error(error); // pass the error to the subscriber
          });
      });
    }

    static login(data: LoginFormData): Observable<any> {
      return new Observable((observer) => {
        api.post(this.loginUrl, data)
          .then((response) => {
            LocalStorageService.setAuthTokenModel(response.data as AuthTokenModel);

            observer.next(response); // pass the result to the subscriber
            observer.complete(); // signal successful completion
          })
          .catch((error) => {
            handleError(error);
            observer.error(error); // pass the error to the subscriber
          });
      });
    }

    // static refreshToken(token: string, refreshToken: string): Observable<any> {
    //   return new Observable((observer) => {
    //     api.post(this.refreshTokenUrl, {
    //       token: token,
    //       refreshToken: refreshToken,
    //     })
    //       .then((response) => {
    //         console.log("[Success]: AuthService.refreshToken", response.data);
    //         const authTokenModel = response.data as AuthTokenModel;
    //         LocalStorageService.setAuthTokenModel(authTokenModel);

    //         observer.next(response); // pass the result to the subscriber
    //         observer.complete(); // signal successful completion
    //       })
    //       .catch((error) => {
    //         console.log("[Error]: AuthService.refreshToken", error);
    //         observer.error(error); // pass the error to the subscriber
    //       });
    //   });
    // }

    // static isTokenExpired(token: string): boolean {
    //   try {
    //     const decodedToken: { exp?: number } = jwtDecode(token);
    
    //     if (!decodedToken.exp) {
    //       throw new Error("Token does not have an expiration time.");
    //     }
    
    //     const currentTime = Math.floor(Date.now() / 1000);
    
    //     return decodedToken.exp < currentTime;
    //   } catch (error) {
    //     console.error("Error decoding token or checking expiration:", error);
    //     return true;
    //   }
    // };
}