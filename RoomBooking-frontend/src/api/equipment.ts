import type { GetAllEquipmentResponse } from "../types/equipment";
import { apiFetch } from "./client";

export const getAllEquipment = async () => {
  const res = await apiFetch<GetAllEquipmentResponse>("/equipment");

  return res.equipment ?? [];
};
