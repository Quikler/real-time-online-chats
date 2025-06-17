import api from "@services/axios/instance";
import { ChatUsersRoutes } from "./ApiRoutes";
import { AxiosRequestConfig } from "axios";
import { throwIfErrorNotCancelError } from "@src/utils/helpers";

export abstract class ChatUsersService {
    static async getAllUsersByChatId(chatId: string, config?: AxiosRequestConfig<any>) {
        try {
            const response = await api.get(ChatUsersRoutes.byChatId(chatId), config);
            return response.data;
        } catch (e: any) {
          throwIfErrorNotCancelError(e);
        }
    }

    static async addMe(chatId: string, config?: AxiosRequestConfig<any>) {
        try {
            const response = await api.post(ChatUsersRoutes.byId(chatId, "me"), {}, config);
            return response.data;
        } catch (e: any) {
          throwIfErrorNotCancelError(e);
        }
    }

    static async deleteMemberMe(chatId: string, config?: AxiosRequestConfig<any>) {
        try {
            const response = await api.delete(ChatUsersRoutes.byId(chatId, "me"), config);
            return response.data;
        } catch (e: any) {
          throwIfErrorNotCancelError(e);
        }
    }

    static async deleteMember(chatId: string, userId: string, config?: AxiosRequestConfig<any>) {
        try {
            const response = await api.delete(ChatUsersRoutes.byId(chatId, userId), config);
            return response.data;
        } catch (e: any) {
          throwIfErrorNotCancelError(e);
        }
    }
}
