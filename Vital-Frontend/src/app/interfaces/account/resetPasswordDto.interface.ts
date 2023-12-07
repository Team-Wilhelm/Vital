export interface ResetPasswordDto {
  userId: string; // UUID represented as String in TypeScript
  token: string;
  newPassword: string;
}
