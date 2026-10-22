import { useSearchParams } from "react-router";

import { DEFAULT_RESERVATION_FILTERS } from "../constants/defaultFilters";
import type { ReservationFilters } from "../types/reservation";

export function useReservationFilters() {
  const [searchParams, setSearchParams] = useSearchParams();

  const roomId = searchParams.get("roomId");

  const organizerId = searchParams.get("organizerId");

  const status = searchParams.get("status");

  const from = searchParams.get("from");

  const to = searchParams.get("to");

  const filters: ReservationFilters = {
    roomId: roomId,
    organizerId: organizerId,
    status: status,
    from: from,
    to: to,
  };

  const setFilters = (newFilters: ReservationFilters) => {
    const params = new URLSearchParams();

    if (newFilters.roomId) {
      params.set("roomId", newFilters.roomId);
    }

    if (newFilters.organizerId) {
      params.set("organizerId", newFilters.organizerId);
    }

    if (newFilters.status) {
      params.set("status", newFilters.status);
    }

    if (newFilters.from) {
      params.set("from", newFilters.from);
    }

    if (newFilters.to) {
      params.set("to", newFilters.to);
    }

    setSearchParams(params);
  };

  const resetFilters = () => {
    setFilters(DEFAULT_RESERVATION_FILTERS);
  };

  return { filters, setFilters, resetFilters };
}
