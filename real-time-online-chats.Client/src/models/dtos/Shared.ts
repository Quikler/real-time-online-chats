export interface PaginationResponse<T> {
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  items: T[];
}

export interface PaginationRequest {
  pageNubmer: number;
  pageSize: number;
  filter: string;
}
