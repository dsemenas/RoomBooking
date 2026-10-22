import type { GetAllUsersResponse } from "../types/user";
import { apiFetch } from "./client";

export const getAllUsers = async () => {
  const res = await apiFetch<GetAllUsersResponse>("/user");

  return res.users ?? [];
};
