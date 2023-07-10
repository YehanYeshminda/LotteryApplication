import {AuthDetails} from "../models/auth";

export function getAuthDetails(): AuthDetails | null {
  const user = sessionStorage.getItem('user');

  if (user) {
    return JSON.parse(user);
  }

  return null;
}
