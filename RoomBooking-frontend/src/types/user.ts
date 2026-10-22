export type User = {
  id: number;
  firstName: string;
  lastName: string;
  activeReservationsCount: number;
};

export type GetAllUsersResponse = {
  error: {
    errorCode: string;
    errorMessage: string;
  } | null;
  users: Array<User> | null;
};
