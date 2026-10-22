import { useQuery } from "@tanstack/react-query";

import { getReservations } from "../api/reservation";
import type { ReservationFilters } from "../types/reservation";

export function useReservations(filters: ReservationFilters) {
  return useQuery({
    queryKey: ["reservations", filters],
    queryFn: () => getReservations(filters),
  });
}
