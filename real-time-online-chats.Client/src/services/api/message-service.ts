import api from "@services/axios/instance";
import { CreateMessageRequest } from "../../models/dtos/Message";
import { AxiosRequestConfig } from "axios";
import { throwIfErrorNotCancelError } from "@src/utils/helpers";

export abstract class MessageService {
  static messageUrl = "messages";

  static async createMessage(
    request: CreateMessageRequest,
    config?: AxiosRequestConfig<any> | undefined
  ) {
    try {
      const response = await api.post(this.messageUrl, request, {
        signal: config?.signal,
      });
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }
}
