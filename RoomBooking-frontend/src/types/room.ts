export type ErrorResponse = {
  code: string;
  message: string;
};

export type RoomFilters = {
  minCapacity: string | null;
  floor: string | null;
  equipment: Array<string>;
};

export type Room = {
  id: number;
  name: string;
  capacity: number;
  floor: number;
  equipment: string[];
  status: string;
  isOftenBookedAndRarelyUsed: boolean;
};

export type GetAllRoomsResponse = {
  error: { errorCode: string; errorMessage: string } | null;
  rooms: Array<Room> | null;
};

export type RoomDetailsWithStats = Room & {
  closedReservationsCount: number;
  closedReservationsTimeInMinutes: number;
  closedReservationsActuallyUsedTimeInMinutes: number;
  usageRate: number;
};

export type GetRoomDetailsResponse = {
  error: ErrorResponse | null;
  room: RoomDetailsWithStats | null;
};
