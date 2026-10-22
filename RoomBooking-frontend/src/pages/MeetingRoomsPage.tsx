import Alert from "@mui/material/Alert";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Checkbox from "@mui/material/Checkbox";
import Container from "@mui/material/Container";
import FormControlLabel from "@mui/material/FormControlLabel";
import MenuItem from "@mui/material/MenuItem";
import Paper from "@mui/material/Paper";
import Select from "@mui/material/Select";
import Stack from "@mui/material/Stack";
import TextField from "@mui/material/TextField";
import Typography from "@mui/material/Typography";

import MeetingRoomCard from "../components/MeetingRoomCard";
import { useEquipment } from "../hooks/useEquipment";
import { useRoomFilters } from "../hooks/useRoomFilters";
import { useRooms } from "../hooks/useRooms";

const FLOORS = ["1", "2", "3", "4", "5"];

export default function MeetingRoomsPage() {
  const { filters, setFilters, resetFilters } = useRoomFilters();

  const equipmentQuery = useEquipment();

  const roomQuery = useRooms(filters);

  if (equipmentQuery.isPending) {
    return (
      <Container maxWidth="xl" sx={{ py: 3 }}>
        <Typography variant="h1">Loading...</Typography>
      </Container>
    );
  }

  if (equipmentQuery.error) {
    return (
      <Container maxWidth="xl" sx={{ py: 3 }}>
        <Typography variant="h1">Something went wrong...</Typography>
      </Container>
    );
  }

  return (
    <Container maxWidth="xl" sx={{ py: 3 }}>
      <Stack spacing={5}>
        <Paper sx={{ px: { xs: 3, md: 5 }, py: { xs: 2, md: 3 } }}>
          <Box
            sx={{
              display: "flex",
              flexDirection: { xs: "column", md: "row" },
              justifyContent: "space-between",
              alignItems: "start",
              gap: { xs: 3, md: 0 },
            }}
          >
            <Stack spacing={4}>
              <Box
                sx={{
                  display: "flex",
                  flexDirection: { xs: "column", md: "row" },
                  alignItems: "start",
                  gap: 5,
                }}
              >
                <Box>
                  <Typography variant="body2" sx={{ mb: 1, fontWeight: 700 }}>
                    Minimum capacity
                  </Typography>
                  <TextField
                    type="number"
                    size="small"
                    placeholder="Any"
                    sx={{
                      "& input::placeholder": {
                        opacity: 1,
                        color: "text.primary",
                      },
                    }}
                    slotProps={{ htmlInput: { min: 1 } }}
                    value={filters.minCapacity ?? "Any"}
                    onChange={(e) =>
                      setFilters({ ...filters, minCapacity: e.target.value })
                    }
                  />
                </Box>
                <Box>
                  <Typography variant="body2" sx={{ mb: 1, fontWeight: 700 }}>
                    Floor
                  </Typography>
                  <Select
                    size="small"
                    fullWidth
                    displayEmpty={true}
                    value={filters.floor ?? ""}
                    onChange={(e) =>
                      setFilters({ ...filters, floor: e.target.value })
                    }
                    sx={{ minWidth: "150px" }}
                  >
                    <MenuItem value="">Any</MenuItem>
                    {FLOORS.map((floor) => (
                      <MenuItem key={floor} value={floor}>
                        {floor}
                      </MenuItem>
                    ))}
                  </Select>
                </Box>
              </Box>
              <Box>
                <Box>
                  <Typography variant="body2" sx={{ mb: 0.5, fontWeight: 700 }}>
                    Equipment
                  </Typography>
                  <Box sx={{ display: "flex", flexWrap: "wrap" }}>
                    {equipmentQuery.data?.map((equipment) => {
                      const id = String(equipment.id);
                      const checked = filters.equipment.includes(id);

                      return (
                        <FormControlLabel
                          key={equipment.id}
                          label={equipment.name}
                          control={
                            <Checkbox
                              checked={checked}
                              onChange={() =>
                                setFilters({
                                  ...filters,
                                  equipment: checked
                                    ? filters.equipment.filter((x) => x !== id)
                                    : [...filters.equipment, id],
                                })
                              }
                            />
                          }
                        />
                      );
                    })}
                  </Box>
                </Box>
              </Box>
            </Stack>
            <Button variant="contained" onClick={resetFilters}>
              Reset filters
            </Button>
          </Box>
        </Paper>
        {roomQuery.isPending ? (
          <Typography variant="h2">Loading...</Typography>
        ) : roomQuery.error ? (
          <Alert severity="error">{roomQuery.error.message}</Alert>
        ) : roomQuery.data?.length === 0 ? (
          <Typography variant="h2">No rooms were found.</Typography>
        ) : (
          <>
            <Box
              sx={{
                display: "grid",
                gridTemplateColumns: {
                  xs: "1fr",
                  md: "repeat(2, 1fr)",
                  lg: "repeat(3, 1fr)",
                  xl: "repeat(4, 1fr)",
                },
                gap: 2,
              }}
            >
              {roomQuery.data?.map((room) => (
                <MeetingRoomCard room={room} key={room.id} />
              ))}
            </Box>
          </>
        )}
      </Stack>
    </Container>
  );
}
