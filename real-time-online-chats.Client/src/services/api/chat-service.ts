import api from "@services/axios/instance";
import { ChatRoutes } from "@src/routes/api-routes";
import { AxiosRequestConfig } from "axios";
import { CreateChatResponse } from "@src/models/dtos/Chat";
import { throwIfErrorNotCancelError } from "@src/utils/helpers";
import { CreateChatFormData } from "@src/pages/chats/CreateChatForm";

export abstract class ChatService {
  static async createChat(data: CreateChatFormData, config?: AxiosRequestConfig<any> | undefined) {
    try {
      const response = await api.post<CreateChatResponse>(ChatRoutes.base, data, config);
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async getChat(chatId: string, config?: AxiosRequestConfig<any> | undefined) {
    try {
      const response = await api.get(`${ChatRoutes.base}/${chatId}`, config);
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async getChats(
    page: number,
    pageSize: number,
    config?: AxiosRequestConfig<any> | undefined
  ) {
    try {
      const response = await api.get(
        `${ChatRoutes.base}?page=${page}&pageSize=${pageSize}`,
        config
      );
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async getChatDetailed(chatId: string, config?: AxiosRequestConfig<any> | undefined) {
    try {
      const response = await api.get(`${ChatRoutes.base}/${chatId}/${ChatRoutes.detailed}`, config);
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

  static async joinChat(chatId: string, config?: AxiosRequestConfig<any> | undefined) {
    try {
      const response = await api.post(`${ChatRoutes.base}/${chatId}/join`, {}, config);
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async leaveChat(chatId: string, config?: AxiosRequestConfig<any> | undefined) {
    try {
      const response = await api.post(`${ChatRoutes.base}/${chatId}/leave`, {}, config);
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }
}
