import Alert from "@mui/material/Alert";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Container from "@mui/material/Container";
import MenuItem from "@mui/material/MenuItem";
import Paper from "@mui/material/Paper";
import Select from "@mui/material/Select";
import Stack from "@mui/material/Stack";
import Typography from "@mui/material/Typography";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import dayjs from "dayjs";

import CreateReservationMenu from "../components/CreateReservationMenu";
import ReservationCard from "../components/ReservationCard";
import { DEFAULT_ROOM_FILTERS } from "../constants/defaultFilters";
import { useReservationFilters } from "../hooks/useReservationFilters";
import { useReservations } from "../hooks/useReservations";
import { useRooms } from "../hooks/useRooms";
import { useUsers } from "../hooks/useUsers";
import { APP_TIME_ZONE } from "../utils/dates";

const STATUS_CHOICES = ["Booked", "Completed", "Cancelled"];

export default function ReservationsPage() {
  const { filters, setFilters, resetFilters } = useReservationFilters();

  const usersQuery = useUsers();
  const roomsQuery = useRooms(DEFAULT_ROOM_FILTERS);
  const reservationQuery = useReservations(filters);

  if (usersQuery.isPending || roomsQuery.isPending) {
    return (
      <Container maxWidth="xl" sx={{ py: 3 }}>
        <Typography variant="h1">Loading...</Typography>
      </Container>
    );
  }

  if (usersQuery.error || roomsQuery.error) {
    return (
      <Container maxWidth="xl" sx={{ py: 3 }}>
        <Typography variant="h1">Something went wrong...</Typography>
      </Container>
    );
  }

  return (
    <Container maxWidth="xl" sx={{ py: 3 }}>
      <Stack spacing={5}>
        <CreateReservationMenu
          rooms={roomsQuery.data}
          users={usersQuery.data}
        />
        <Paper sx={{ px: { xs: 3, lg: 5 }, py: { xs: 2, lg: 3 } }}>
          <Box
            sx={{
              display: "flex",
              flexDirection: { xs: "column", xl: "row" },
              justifyContent: "space-between",
              alignItems: "start",
              gap: { xs: 3, xl: 0 },
            }}
          >
            <Box
              sx={{
                display: "flex",
                flexDirection: { xs: "column", lg: "row" },
                justifyContent: "start",
                alignItems: { xs: "start", lg: "center" },
                gap: { xs: 3, lg: 5 },
              }}
            >
              <Box>
                <Typography variant="body2" sx={{ mb: 1, fontWeight: 700 }}>
                  Room
                </Typography>
                <Select
                  size="small"
                  fullWidth
                  displayEmpty={true}
                  value={filters.roomId ?? ""}
                  onChange={(e) =>
                    setFilters({ ...filters, roomId: e.target.value })
                  }
                  sx={{ minWidth: "150px" }}
                >
                  <MenuItem value="">All Rooms</MenuItem>
                  {roomsQuery?.data?.map((room) => (
                    <MenuItem key={room.id} value={room.id}>
                      {room.name}
                    </MenuItem>
                  ))}
                </Select>
              </Box>
              <Box>
                <Typography variant="body2" sx={{ mb: 1, fontWeight: 700 }}>
                  Organizer
                </Typography>
                <Select
                  size="small"
                  fullWidth
                  displayEmpty={true}
                  value={filters.organizerId ?? ""}
                  onChange={(e) =>
                    setFilters({ ...filters, organizerId: e.target.value })
                  }
                  sx={{ minWidth: "150px" }}
                >
                  <MenuItem value="">Any</MenuItem>
                  {usersQuery?.data?.map((user) => (
                    <MenuItem key={user.id} value={user.id}>
                      {`${user.firstName} ${user.lastName}`}
                    </MenuItem>
                  ))}
                </Select>
              </Box>
              <Box>
                <Typography variant="body2" sx={{ mb: 1, fontWeight: 700 }}>
                  Status
                </Typography>
                <Select
                  size="small"
                  fullWidth
                  displayEmpty={true}
                  value={filters.status ?? ""}
                  onChange={(e) =>
                    setFilters({ ...filters, status: e.target.value })
                  }
                  sx={{ minWidth: "150px" }}
                >
                  <MenuItem value="">Any</MenuItem>
                  {STATUS_CHOICES.map((status, index) => (
                    <MenuItem key={index} value={status}>
                      {status}
                    </MenuItem>
                  ))}
                </Select>
              </Box>
              <Box>
                <Typography variant="body2" sx={{ mb: 1, fontWeight: 700 }}>
                  From
                </Typography>
                <DatePicker
                  format="YYYY/MM/DD"
                  value={filters.from ? dayjs(filters.from) : null}
                  timezone={APP_TIME_ZONE}
                  onChange={(dateFrom) => {
                    if (dateFrom && !dateFrom.isValid()) return;
                    setFilters({
                      ...filters,
                      from: dateFrom?.toISOString() ?? null,
                    });
                  }}
                  slotProps={{
                    field: { readOnly: true },
                    textField: {
                      size: "small",
                      fullWidth: true,
                      sx: {
                        "& .MuiPickersOutlinedInput-root": {
                          borderRadius: "999px",
                          backgroundColor: "background.paper",
                        },
                      },
                    },
                  }}
                />
              </Box>
              <Box>
                <Typography variant="body2" sx={{ mb: 1, fontWeight: 700 }}>
                  To
                </Typography>
                <DatePicker
                  format="YYYY/MM/DD"
                  value={filters.to ? dayjs(filters.to) : null}
                  timezone={APP_TIME_ZONE}
                  onChange={(dateTo) => {
                    if (dateTo && !dateTo.isValid()) return;
                    setFilters({
                      ...filters,
                      to: dateTo?.toISOString() ?? null,
                    });
                  }}
                  slotProps={{
                    field: { readOnly: true },
                    textField: {
                      size: "small",
                      fullWidth: true,
                      sx: {
                        "& .MuiPickersOutlinedInput-root": {
                          borderRadius: "999px",
                          backgroundColor: "background.paper",
                        },
                      },
                    },
                  }}
                />
              </Box>
            </Box>
            <Button variant="contained" onClick={resetFilters}>
              Reset filters
            </Button>
          </Box>
        </Paper>
        {reservationQuery.isPending ? (
          <Typography variant="h2">Loading...</Typography>
        ) : reservationQuery.error ? (
          <Alert severity="error">{reservationQuery.error.message}</Alert>
        ) : reservationQuery.data?.length === 0 ? (
          <Typography variant="h2">No reservations were found...</Typography>
        ) : (
          <Box
            sx={{
              display: "grid",
              gridTemplateColumns: {
                xs: "1fr",
                sm: "repeat(2, 1fr)",
                md: "repeat(4, 1fr)",
              },
              gap: 2,
            }}
          >
            {reservationQuery.data?.map((reservation) => (
              <ReservationCard reservation={reservation} key={reservation.id} />
            ))}
          </Box>
        )}
      </Stack>
    </Container>
  );
}
