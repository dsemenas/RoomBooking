import type { GetAllRoomsResponse, GetRoomDetailsResponse, RoomFilters } from "../types/room";
import { apiFetch } from "./client";

export const getRooms = async (filters: RoomFilters) => {
  const params = new URLSearchParams();

  if (filters.minCapacity) params.set("minCapacity", filters.minCapacity);
  if (filters.floor) params.set("floor", filters.floor);
  filters.equipment.forEach((id) => params.append("equipment", id));

  const responseData = await apiFetch<GetAllRoomsResponse>(`/room?${params}`);
  return responseData.rooms ?? [];
};

export const getRoomDetails = async (id: number) => {
  const responseData = await apiFetch<GetRoomDetailsResponse>(`/room/${id}`);

  return responseData.room ?? null;
};
