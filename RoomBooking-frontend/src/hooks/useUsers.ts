import { useQuery } from "@tanstack/react-query";

import { getAllUsers } from "../api/user";

export function useUsers() {
  return useQuery({
    queryKey: ["users"],
    queryFn: getAllUsers,
  });
}
