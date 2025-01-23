import api from "../utilities/axios-instance";
import { handleError } from "../helpers/error-handler";
import { CreateMessageRequest } from "../contracts/message-contract";
import { observableWithAbort } from "../helpers/wrappers";

export abstract class MessageService {
  static messageUrl = "messages";

  static createMessage(request: CreateMessageRequest) {
    return observableWithAbort((abortController, observer) => {
      api
        .post(this.messageUrl, request, { signal: abortController.signal })
        .then((response) => {
          observer.next(response);
          observer.complete();
        })
        .catch((error) => {
          handleError(error);
          observer.error(error);
        });
    });
  }
}
