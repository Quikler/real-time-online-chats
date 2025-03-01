import api from "@services/axios/instance";
import { ChatRoutes } from "@src/services/api/ApiRoutes";
import { AxiosRequestConfig } from "axios";
import { CreateChatResponse } from "@src/models/dtos/Chat";
import { throwIfErrorNotCancelError } from "@src/utils/helpers";
import { CreateChatFormData } from "@src/pages/chats/CreateChatForm";

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

  static async deleteChat(chatId: string, config?: AxiosRequestConfig<any> | undefined) {
    try {
      const response = await api.delete(`${ChatRoutes.base}/${chatId}`, config);
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async updateChatOwner(
    chatId: string,
    newOwnerId: string,
    config?: AxiosRequestConfig<any> | undefined
  ) {
    try {
      const response = await api.patch(`${ChatRoutes.base}/${chatId}/owner`, newOwnerId, config);
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async addMemberMe(chatId: string, config?: AxiosRequestConfig<any> | undefined) {
    try {
      const response = await api.post(`${ChatRoutes.base}/${chatId}/members/me`, {}, config);
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async deleteMember(
    chatId: string,
    memberId: string,
    config?: AxiosRequestConfig<any> | undefined
  ) {
    try {
      const response = await api.delete(`${ChatRoutes.base}/${chatId}/members/${memberId}`, config);
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }

  static async deleteMemberMe(chatId: string, config?: AxiosRequestConfig<any> | undefined) {
    try {
      const response = await api.delete(`${ChatRoutes.base}/${chatId}/members/me`, config);
      return response.data;
    } catch (e) {
      throwIfErrorNotCancelError(e);
    }
  }
}
