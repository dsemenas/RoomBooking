import Alert from "@mui/material/Alert";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Chip from "@mui/material/Chip";
import Paper from "@mui/material/Paper";
import Stack from "@mui/material/Stack";
import Typography from "@mui/material/Typography";
import { useMutation, useQueryClient } from "@tanstack/react-query";

import { cancelReservation, endReservationEarly } from "../api/reservation";
import type { ReservationDetails } from "../types/reservation";
import { formatLocal } from "../utils/dates";

type ChipColor = "primary" | "success" | "default";

const statusColor = (status: string): ChipColor => {
  switch (status.toLowerCase()) {
    case "booked":
      return "primary";
    case "completed":
      return "success";
    default:
      return "default";
  }
};

export default function ReservationCard(props: {
  reservation: ReservationDetails;
}) {
  const queryClient = useQueryClient();
  const cancelMutation = useMutation({
    mutationFn: () => cancelReservation(props.reservation.id),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: ["reservations"] }),
  });
  const endEarlyMutation = useMutation({
    mutationFn: () => endReservationEarly(props.reservation.id),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: ["reservations"] }),
  });

  const handleCancel = () => cancelMutation.mutate();
  const handleEndEarly = () => endEarlyMutation.mutate();

  return (
    <Paper sx={{ p: 2 }}>
      <Stack spacing={1} sx={{ height: "100%" }}>
        <Stack
          direction="row"
          sx={{ justifyContent: "space-between", alignItems: "flex-start" }}
          spacing={1}
        >
          <Typography variant="h6" component="h3">
            {props.reservation.meetingTitle}
          </Typography>
          <Chip
            label={props.reservation.status}
            color={statusColor(props.reservation.status)}
            size="small"
          />
        </Stack>

        <Typography variant="body2">
          {props.reservation.roomName}, organized by{" "}
          {props.reservation.organizerFullName}
        </Typography>

        <Typography variant="body2" color="text.secondary">
          Start: {formatLocal(props.reservation.startTime)}
        </Typography>
        <Typography variant="body2" color="text.secondary">
          End: {formatLocal(props.reservation.endTime)}
        </Typography>

        <Typography variant="body2" color="text.secondary">
          {props.reservation.attendeeCount}{" "}
          {props.reservation.attendeeCount === 1 ? "attendee" : "attendees"}
        </Typography>
        <Box
          sx={{
            display: "flex",
            flexDirection: { xs: "column", lg: "row" },
            justifyContent: { xs: "end", lg: "space-between" },
            gap: { xs: 1, lg: 0 },
            alignItems: { xs: "start", lg: "center" },
            height: "100%",
          }}
        >
          <Button
            color="primary"
            variant="contained"
            disabled={
              !props.reservation.canBeCancelled || cancelMutation.isPending
            }
            onClick={handleCancel}
          >
            {cancelMutation.isPending ? "Cancelling..." : "Cancel"}
          </Button>

          <Button
            color="primary"
            variant="contained"
            disabled={
              !props.reservation.canBeEndedEarly || endEarlyMutation.isPending
            }
            onClick={handleEndEarly}
          >
            {endEarlyMutation.isPending ? "Ending..." : "End early"}
          </Button>
        </Box>
        {(cancelMutation.isError || endEarlyMutation.isError) && (
          <Alert
            severity="error"
            onClose={() => {
              cancelMutation.reset();
              endEarlyMutation.reset();
            }}
          >
            {cancelMutation.isError
              ? cancelMutation.error.message
              : endEarlyMutation.error?.message}
          </Alert>
        )}
      </Stack>
    </Paper>
  );
}
