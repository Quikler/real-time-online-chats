import api from "../utilities/axios-instance";
import { CreateChatFormData } from "../components/pages/chats/create-chat-form";
import { ChatRoutes } from "../routes/api-routes";
import { observableWithAbort } from "../helpers/wrappers";

export abstract class ChatService {
  static createChat(data: CreateChatFormData) {
    return observableWithAbort((abortController, observer) => {
      api
        .post(ChatRoutes.base, data, {
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

  static getChat(chatId: string) {
    return observableWithAbort((abortController, observer) => {
      api
        .get(`${ChatRoutes.base}/${chatId}`, {
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
