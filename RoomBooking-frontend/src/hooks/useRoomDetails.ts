import { useQuery } from "@tanstack/react-query";

import { getRoomDetails } from "../api/room";

export function useRoomDetails(id: number) {
  return useQuery({
    queryKey: ["room", id],
    queryFn: () => getRoomDetails(id),
  });
}
