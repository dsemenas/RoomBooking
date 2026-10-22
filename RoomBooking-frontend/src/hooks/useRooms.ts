import { useQuery } from "@tanstack/react-query";

import { getRooms } from "../api/room";
import type { RoomFilters } from "../types/room";

export function useRooms(filters: RoomFilters) {
  return useQuery({
    queryKey: ["rooms", filters],
    queryFn: () => getRooms(filters),
  });
}
