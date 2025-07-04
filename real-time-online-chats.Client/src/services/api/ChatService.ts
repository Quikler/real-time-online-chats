import api from "@services/axios/instance";
import { ChatRoutes } from "@src/services/api/ApiRoutes";
import { AxiosRequestConfig } from "axios";
import { throwIfErrorNotCancelError } from "@src/utils/helpers";
import { CreateChatFormData } from "@src/pages/chats/CreateChatForm";
import { PaginationRequest } from "@src/models/dtos/Shared";
import { CreateChatResponse } from "@src/models/dtos/Chat";

export enum ChatLevel {
  Preview,
  Detail,
}

export abstract class ChatService {
  static async createChat(data: CreateChatFormData, config?: AxiosRequestConfig<any> | undefined) {
    try {
      const response = await api.post<CreateChatResponse>(ChatRoutes.base, data, config);
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async getChat(
    chatId: string,
    level: ChatLevel,
    config?: AxiosRequestConfig<any> | undefined
  ) {
    try {
      const response = await api.get(`${ChatRoutes.base}/${chatId}?level=${level}`, config);
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async getChatInfo(chatId: string, config?: AxiosRequestConfig<any> | undefined) {
      try {
        const response = await api.get(ChatRoutes.info(chatId), config)
        return response.data;
      } catch (e) {
        throwIfErrorNotCancelError(e);
      }
  }

  static async getChats(
    request: PaginationRequest,
    config?: AxiosRequestConfig<any> | undefined
  ) {
    try {
      const response = await api.get(
        `${ChatRoutes.base}?pageNumber=${request.pageNubmer}&pageSize=${request.pageSize}&titleFilter=${request.filter}`,
        config
      );
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async deleteChat(chatId: string, config?: AxiosRequestConfig<any> | undefined) {
    try {
      const response = await api.delete(`${ChatRoutes.base}/${chatId}`, config);
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async updateOwner(chatId: string, newOwnerId: string, config?: AxiosRequestConfig<any>) {
    try {
      const response = await api.patch(`${ChatRoutes.base}/${chatId}/owner`, { newOwnerId }, config);
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }
}
