import type { CreateReservationRequest, GetAllReservationsResponse, ReservationFilters } from "../types/reservation";
import { apiFetch } from "./client";

export async function getReservations(filters: ReservationFilters) {
  const params = new URLSearchParams();

  if (filters.organizerId) params.set("organizerId", filters.organizerId);
  if (filters.roomId) params.set("roomId", filters.roomId);
  if (filters.status) params.set("status", filters.status);
  if (filters.from) params.set("from", filters.from);
  if (filters.to) params.set("to", filters.to);

  const responseData = await apiFetch<GetAllReservationsResponse>(
    `/reservations?${params}`,
  );

  return responseData.reservations ?? [];
}

export async function createReservation(reservation: CreateReservationRequest) {
  return apiFetch<unknown>("/reservations", {
    method: "POST",
    body: JSON.stringify(reservation),
  });
}

export async function cancelReservation(reservationId: number) {
  return apiFetch<unknown>(`/reservations/${reservationId}/cancel`, {
    method: "POST",
  });
}

export async function endReservationEarly(reservationId: number) {
  return apiFetch<unknown>(`/reservations/${reservationId}/end-early`, {
    method: "POST",
  });
}
