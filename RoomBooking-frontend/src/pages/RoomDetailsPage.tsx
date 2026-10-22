import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import Alert from "@mui/material/Alert";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Chip from "@mui/material/Chip";
import Container from "@mui/material/Container";
import LinearProgress from "@mui/material/LinearProgress";
import Paper from "@mui/material/Paper";
import Stack from "@mui/material/Stack";
import Typography from "@mui/material/Typography";
import { Link as RouterLink, useParams } from "react-router";

import { useRoomDetails } from "../hooks/useRoomDetails";

export default function RoomDetailsPage() {
  const formatMinutes = (totalMinutes: number) => {
    const hours = Math.floor(totalMinutes / 60);
    const minutes = totalMinutes % 60;
    return `${hours} h ${minutes} min`;
  };

  const { roomId } = useParams();
  const id = Number(roomId);
  const roomDetailQuery = useRoomDetails(id);

  if (roomDetailQuery.isPending) {
    return (
      <Container maxWidth="xl" sx={{ py: 3 }}>
        <Typography variant="h1">Loading...</Typography>
      </Container>
    );
  }

  if (roomDetailQuery.error) {
    return (
      <Container maxWidth="xl" sx={{ py: 3 }}>
        <Alert severity="error">{roomDetailQuery.error.message}</Alert>
      </Container>
    );
  }

  const room = roomDetailQuery.data;

  if (!room) {
    return (
      <Container maxWidth="md" sx={{ py: 3 }}>
        <Alert severity="info">Room not found.</Alert>
      </Container>
    );
  }

  return (
    <Container maxWidth="xl" sx={{ py: 3 }}>
      <Stack spacing={5}>
        <Button
          component={RouterLink}
          to="/rooms"
          startIcon={<ChevronLeftIcon />}
          size="large"
          sx={{ maxWidth: "200px" }}
        >
          Back to rooms page
        </Button>
        <Paper sx={{ p: 3 }}>
          <Stack spacing={4}>
            <Box
              sx={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
              }}
            >
              <Stack spacing={1}>
                <Typography variant="h4" component="h1">
                  {room.name}
                </Typography>
                <Typography variant="body1">Room ID: {room.id}</Typography>
              </Stack>
              <Box sx={{ display: "flex", gap: 1 }}>
                <Chip
                  label={room.status}
                  color={room.status == "Active" ? "success" : "default"}
                  sx={{
                    height: 50,
                    fontSize: "1rem",
                    "& .MuiChip-label": { px: 2 },
                    borderRadius: 999,
                  }}
                />
                {room.isOftenBookedAndRarelyUsed ? (
                  <Chip
                    label={"Often booked, rarely used"}
                    color="warning"
                    sx={{
                      height: 50,
                      fontSize: "1rem",
                      "& .MuiChip-label": { px: 2 },
                      borderRadius: 999,
                    }}
                  />
                ) : null}
              </Box>
            </Box>
            <Box
              sx={{
                display: "flex",
                flexDirection: { xs: "column", md: "row" },
                gap: 5,
              }}
            >
              <Paper
                sx={{
                  p: 3,
                  backgroundColor: "background.default",
                  minWidth: "200px",
                }}
              >
                <Stack spacing={2}>
                  <Typography variant="body1" sx={{ fontWeight: 700 }}>
                    Capacity
                  </Typography>
                  <Typography variant="h5" component="h3">
                    {room.capacity} People
                  </Typography>
                </Stack>
              </Paper>
              <Paper
                sx={{
                  p: 3,
                  backgroundColor: "background.default",
                  minWidth: "200px",
                }}
              >
                <Stack spacing={2}>
                  <Typography variant="body1" sx={{ fontWeight: 700 }}>
                    Floor
                  </Typography>
                  <Typography variant="h5" component="h3">
                    {room.floor}
                  </Typography>
                </Stack>
              </Paper>
              <Paper
                sx={{
                  p: 3,
                  backgroundColor: "background.default",
                  minWidth: "200px",
                }}
              >
                <Stack spacing={2}>
                  <Typography variant="body1" sx={{ fontWeight: 700 }}>
                    Equipment
                  </Typography>
                  <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1 }}>
                    {room.equipment.length > 0 ? (
                      room.equipment.map((item) => (
                        <Chip key={item} label={item} color="primary" />
                      ))
                    ) : (
                      <Typography>No equipment</Typography>
                    )}
                  </Box>
                </Stack>
              </Paper>
            </Box>
          </Stack>
        </Paper>
        <Paper sx={{ p: 3 }}>
          <Stack spacing={4}>
            <Stack spacing={1}>
              <Typography variant="h4" component="h2">
                Usage
              </Typography>
              <Typography variant="body1">
                Based on completed and cancelled reservations.
              </Typography>
            </Stack>
            <Box
              sx={{
                display: "flex",
                flexDirection: { xs: "column", md: "row" },
                gap: 5,
              }}
            >
              <Paper
                sx={{
                  p: 3,
                  backgroundColor: "background.default",
                  minWidth: "200px",
                }}
              >
                <Stack spacing={2}>
                  <Typography variant="body1" sx={{ fontWeight: 700 }}>
                    Closed reservations
                  </Typography>
                  <Typography variant="h5" component="h3">
                    {room.closedReservationsCount}
                  </Typography>
                </Stack>
              </Paper>
              <Paper
                sx={{
                  p: 3,
                  backgroundColor: "background.default",
                  minWidth: "200px",
                }}
              >
                <Stack spacing={2}>
                  <Typography variant="body1" sx={{ fontWeight: 700 }}>
                    Reserved time
                  </Typography>
                  <Typography variant="h5" component="h3">
                    {formatMinutes(room.closedReservationsTimeInMinutes)}
                  </Typography>
                </Stack>
              </Paper>
              <Paper
                sx={{
                  p: 3,
                  backgroundColor: "background.default",
                  minWidth: "200px",
                }}
              >
                <Stack spacing={2}>
                  <Typography variant="body1" sx={{ fontWeight: 700 }}>
                    Actually used
                  </Typography>
                  <Typography variant="h5" component="h3">
                    {formatMinutes(
                      room.closedReservationsActuallyUsedTimeInMinutes,
                    )}
                  </Typography>
                </Stack>
              </Paper>
            </Box>
            {room.closedReservationsCount == 0 ? null : (
              <Stack spacing={1}>
                <Box sx={{ display: "flex", justifyContent: "space-between" }}>
                  <Typography variant="h6" component="h4">
                    Usage rate
                  </Typography>
                  <Typography variant="h6" component="h4">
                    {room.usageRate}%
                  </Typography>
                </Box>
                <LinearProgress
                  variant="determinate"
                  value={Math.min(100, Math.max(0, room.usageRate))}
                  color="primary"
                  sx={{ height: 12, borderRadius: 999 }}
                />
              </Stack>
            )}
          </Stack>
        </Paper>
      </Stack>
    </Container>
  );
}
