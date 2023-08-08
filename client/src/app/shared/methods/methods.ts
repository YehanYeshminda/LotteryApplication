import { AuthDetails } from "../models/auth";

export function getAuthDetails(cookieData: string): AuthDetails | null {
  if (cookieData) {
    return JSON.parse(cookieData);
  }

  return null;
}

export function splitNumbersByTwo(str: string): string[] {
  const regex = /\d{2}/g;
  return str.match(regex) || [];
}

export function getRandomCaptcha(): string {
  const min = 1000;
  const max = 9999;
  const randomNumber = Math.floor(Math.random() * (max - min + 1)) + min;
  return randomNumber.toString();
}
