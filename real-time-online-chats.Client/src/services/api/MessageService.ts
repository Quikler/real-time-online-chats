import api from "@services/axios/instance";
import { CreateMessageRequest, UpdateMessageRequest } from "../../models/dtos/Message";
import { AxiosRequestConfig } from "axios";
import { throwIfErrorNotCancelError } from "@src/utils/helpers";
import { MessageRoutes } from "./ApiRoutes";

export abstract class MessageService {
  static async createMessage(
    request: CreateMessageRequest,
    config?: AxiosRequestConfig<any> | undefined
  ) {
    try {
      const response = await api.post(MessageRoutes.base, request, {
        signal: config?.signal,
      });
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async updateMessage(
    messageId: string,
    request: UpdateMessageRequest,
    config?: AxiosRequestConfig<any> | undefined
  ) {
    try {
      const response = await api.put(`${MessageRoutes.base}/${messageId}`, request, {
        signal: config?.signal,
      });
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async deleteMessage(
    messageId: string,
    chatId: string,
    config?: AxiosRequestConfig<any> | undefined
  ) {
    try {
      const response = await api.delete(`${MessageRoutes.base}/${messageId}?chatId=${chatId}`, {
        signal: config?.signal,
      });
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }
}
