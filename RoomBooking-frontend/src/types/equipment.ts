type Equipment = {
  id: number;
  name: string;
};

export type GetAllEquipmentResponse = {
  error: {
    errorCode: string;
    errorMessage: string;
  } | null;
  equipment: Array<Equipment> | null;
};
