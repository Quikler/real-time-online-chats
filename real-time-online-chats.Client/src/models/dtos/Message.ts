export interface CreateMessageRequest {
	content: string,
}

export interface GetMessageResponse {
	id: string,
	userId: string,
	content: string,
}

export interface UpdateMessageRequest {
	content: string;
}
