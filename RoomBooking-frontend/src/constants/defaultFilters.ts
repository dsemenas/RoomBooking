import type { ReservationFilters } from "../types/reservation";
import type { RoomFilters } from "../types/room";

export const DEFAULT_ROOM_FILTERS: RoomFilters = {
  minCapacity: null,
  floor: null,
  equipment: [],
};

export const DEFAULT_RESERVATION_FILTERS: ReservationFilters = {
  roomId: null,
  organizerId: null,
  status: null,
  from: null,
  to: null,
};
