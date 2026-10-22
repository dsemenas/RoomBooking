import { useSearchParams } from "react-router";

import { DEFAULT_ROOM_FILTERS } from "../constants/defaultFilters";
import type { RoomFilters } from "../types/room";

export function useRoomFilters() {
  const [searchParams, setSearchParams] = useSearchParams();

  const minCapacity = searchParams.get("minCapacity");

  const floor = searchParams.get("floor");

  const equipment = searchParams.getAll("equipment");

  const filters: RoomFilters = {
    minCapacity: minCapacity,
    floor: floor,
    equipment: equipment,
  };

  const setFilters = (newFilters: RoomFilters) => {
    const params = new URLSearchParams();

    if (newFilters.minCapacity)
      params.set("minCapacity", newFilters.minCapacity);

    if (newFilters.floor) params.set("floor", newFilters.floor);

    newFilters.equipment?.forEach((equipmentId) =>
      params.append("equipment", equipmentId),
    );

    setSearchParams(params);
  };

  const resetFilters = () => {
    setFilters(DEFAULT_ROOM_FILTERS);
  };

  return { filters, setFilters, resetFilters };
}
