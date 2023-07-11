import {AuthDetails} from "../models/auth";

export function getAuthDetails(cookieData: string): AuthDetails | null {
  if (cookieData) {
    return JSON.parse(cookieData);
  }

  return null;
}
