export type ReservationFilters = {
  roomId: string | null;
  organizerId: string | null;
  status: string | null;
  from: string | null;
  to: string | null;
};

export type CreateReservationRequest = {
  roomId: number;
  organizerId: number;
  meetingTitle: string;
  startTime: string;
  endTime: string;
  attendeeIds: number[];
};

export type ReservationDetails = {
  id: number;
  roomName: string;
  organizerFullName: string;
  meetingTitle: string;
  startTime: string;
  endTime: string;
  attendeeCount: number;
  attendeeIds: number[];
  status: string;
  canBeCancelled: boolean;
  canBeEndedEarly: boolean;
};

export type GetAllReservationsResponse = {
  error: { errorCode: string; errorMessage: string };
  reservations: ReservationDetails[] | null;
};
