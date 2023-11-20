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

export interface PasswordRules {
  lengthCondition: boolean;
  digitCondition: boolean;
  lowercaseCondition: boolean;
  uppercaseCondition: boolean;
  specialCondition: boolean;
}
