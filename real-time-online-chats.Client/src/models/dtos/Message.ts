export interface CreateMessageRequest {
	content: string,
	contentType: string,
	chatId: string,
}

export interface GetMessageResponse {
	id: string,
	userId: string,
	content: string,
}