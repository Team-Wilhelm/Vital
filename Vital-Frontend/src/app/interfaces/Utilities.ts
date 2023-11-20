export interface LoginDto {
    email: string;
    password: string;
}

export interface RegisterDto {
    email: string;
    password: string;
}

export interface ResponseDto {
    message: string;
    data: any;
}
