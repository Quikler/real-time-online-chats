export interface CreateMessageRequest {
	content: string,
	chatId: string,
}

export interface GetMessageResponse {
	id: string,
	userId: string,
	content: string,
}

export interface UpdateMessageRequest {
	content: string;
	chatId: string;
}