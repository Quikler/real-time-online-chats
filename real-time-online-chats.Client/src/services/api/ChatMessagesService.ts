import api from "@services/axios/instance";
import { CreateMessageRequest, UpdateMessageRequest } from "../../models/dtos/Message";
import { AxiosRequestConfig } from "axios";
import { throwIfErrorNotCancelError } from "@src/utils/helpers";
import { ChatMessagesRoutes } from "./ApiRoutes";

export abstract class ChatMessagesService {
  static async getAllMessages(chatId: string, config?: AxiosRequestConfig<any>) {
      try {
        const response = await api.get(ChatMessagesRoutes.byChatId(chatId), config);
        return response.data;
      } catch (e) {
        throwIfErrorNotCancelError(e);
      }
  }

  static async createMessage(
    request: CreateMessageRequest,
    config?: AxiosRequestConfig<any> | undefined
  ) {
    try {
      const response = await api.post(ChatMessagesRoutes.byChatId(request.chatId), request, {
        signal: config?.signal,
      });
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async updateMessage(
    chatId: string,
    messageId: string,
    request: UpdateMessageRequest,
    config?: AxiosRequestConfig<any> | undefined
  ) {
    try {
      const response = await api.put(ChatMessagesRoutes.byId(chatId, messageId), request, {
        signal: config?.signal,
      });
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async deleteMessage(
    chatId: string,
    messageId: string,
    config?: AxiosRequestConfig<any> | undefined
  ) {
    try {
      const response = await api.delete(ChatMessagesRoutes.byId(chatId, messageId), {
        signal: config?.signal,
      });
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }
}
