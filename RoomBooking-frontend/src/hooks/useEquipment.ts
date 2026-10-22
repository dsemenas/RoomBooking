import { useQuery } from "@tanstack/react-query";

import { getAllEquipment } from "../api/equipment";

export function useEquipment() {
  return useQuery({
    queryKey: ["equipment"],
    queryFn: getAllEquipment,
  });
}
