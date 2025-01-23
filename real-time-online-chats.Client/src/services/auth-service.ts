import api from "../utilities/axios-instance";
import { SignupFormData } from "../components/forms/sign-up-form";
import { LoginFormData } from "../components/forms/log-in-form";
import { AuthRoutes } from "../routes/api-routes";
import { observableWithAbort } from "../helpers/wrappers";

export abstract class AuthService {
  static signup(data: SignupFormData) {
    return observableWithAbort((abortController, observer) => {
      api
        .post(AuthRoutes.signup, data, {
          withCredentials: true,
          signal: abortController.signal,
        })
        .then((response) => {
          observer.next(response);
          observer.complete();
        })
        .catch((e) => {
          if (e.name !== "CanceledError") {
            observer.error(e);
          }
        });
    });
  }

  static login(data: LoginFormData) {
    return observableWithAbort((abortController, observer) => {
      api
        .post(AuthRoutes.login, data, {
          withCredentials: true,
          signal: abortController.signal,
        })
        .then((response) => {
          observer.next(response);
          observer.complete();
        })
        .catch((e) => {
          if (e.name !== "CanceledError") {
            observer.error(e);
          }
        });
    });
  }

  static logout() {
    return observableWithAbort((abortController, observer) => {
      api
        .post(
          AuthRoutes.logout,
          {},
          { withCredentials: true, signal: abortController.signal }
        )
        .then((response) => {
          observer.next(response);
          observer.complete();
        })
        .catch((e) => {
          if (e.name !== "CanceledError") {
            observer.error(e);
          }
        });
    });
  }

  static me() {
    return observableWithAbort((abortController, observer) => {
      api
        .get(AuthRoutes.me, {
          withCredentials: true,
          signal: abortController.signal,
        })
        .then((response) => {
          observer.next(response);
          observer.complete();
        })
        .catch((e) => {
          if (e.name !== "CanceledError") {
            observer.error(e);
          }
        });
    });
  }
}
