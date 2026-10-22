import Alert from "@mui/material/Alert";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Checkbox from "@mui/material/Checkbox";
import FormControlLabel from "@mui/material/FormControlLabel";
import MenuItem from "@mui/material/MenuItem";
import Paper from "@mui/material/Paper";
import Select from "@mui/material/Select";
import TextField from "@mui/material/TextField";
import Typography from "@mui/material/Typography";
import { DateTimePicker } from "@mui/x-date-pickers";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import dayjs from "dayjs";
import type { Dayjs } from "dayjs";
import { useState } from "react";

import { createReservation } from "../api/reservation";
import type { CreateReservationRequest } from "../types/reservation";
import type { Room } from "../types/room";
import type { User } from "../types/user";
import { APP_TIME_ZONE } from "../utils/dates";

const nextQuarterHour = () =>
  dayjs()
    .add(15, "minute")
    .startOf("hour")
    .add(Math.ceil(dayjs().minute() / 15) * 15, "minute");

type ReservationForm = {
  roomId: number | "";
  organizerId: number | "";
  meetingTitle: string;
  startTime: Dayjs | null;
  endTime: Dayjs | null;
  attendeeIds: number[];
};

const INITIAL_FORM: ReservationForm = {
  roomId: "",
  organizerId: "",
  meetingTitle: "",
  startTime: nextQuarterHour(),
  endTime: nextQuarterHour().add(1, "hour"),
  attendeeIds: [],
};

export default function CreateReservationMenu(props: {
  rooms: Array<Room>;
  users: Array<User>;
}) {
  const [form, setForm] = useState<ReservationForm>(INITIAL_FORM);
  const [validationError, setValidationError] = useState("");
  const queryClient = useQueryClient();
  const createMutation = useMutation({
    mutationFn: createReservation,
    onSuccess: async () => {
      setForm(INITIAL_FORM);
      setValidationError("");
    },
    onSettled: async () => {
      await queryClient.invalidateQueries({ queryKey: ["reservations"] });
    },
  });

  const handleSubmit = () => {
    if (
      form.roomId === "" ||
      form.organizerId === "" ||
      !form.meetingTitle.trim() ||
      !form.startTime ||
      !form.endTime
    ) {
      setValidationError(
        "Complete all fields before creating the reservation.",
      );
      return;
    }

    if (form.meetingTitle.length > 50) {
      setValidationError("Meeting title must not exceed 50 characters.");
      return;
    }

    if (!form.startTime.isValid() || !form.endTime.isValid()) {
      setValidationError("Enter valid start and end times.");
      return;
    }

    if (!form.endTime.isAfter(form.startTime)) {
      setValidationError("End time must be after start time.");
      return;
    }

    if (form.attendeeIds.length === 0) {
      setValidationError("Select at least one attendee.");
      return;
    }

    setValidationError("");
    const reservation: CreateReservationRequest = {
      roomId: form.roomId,
      organizerId: form.organizerId,
      meetingTitle: form.meetingTitle.trim(),
      startTime: form.startTime.toISOString(),
      endTime: form.endTime.toISOString(),
      attendeeIds: form.attendeeIds,
    };
    createMutation.mutate(reservation);
  };

  return (
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
            flexDirection: "column",
            justifyContent: "start",
            alignItems: "start",
            gap: { xs: 3, lg: 5 },
          }}
        >
          <Box
            sx={{
              display: "flex",
              flexDirection: { xs: "column", lg: "row" },
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
                value={form.roomId}
                onChange={(e) =>
                  setForm({ ...form, roomId: e.target.value as number | "" })
                }
                sx={{ minWidth: "150px" }}
              >
                <MenuItem value="">Not selected</MenuItem>
                {props.rooms.map(
                  (room) =>
                    room.status != "Inactive" && (
                      <MenuItem key={room.id} value={room.id}>
                        {room.name}
                      </MenuItem>
                    ),
                )}
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
                value={form.organizerId}
                onChange={(e) =>
                  setForm({
                    ...form,
                    organizerId: e.target.value as number | "",
                  })
                }
                sx={{ minWidth: "150px" }}
              >
                <MenuItem value="">Not selected</MenuItem>
                {props.users.map((user) => (
                  <MenuItem key={user.id} value={user.id}>
                    {`${user.firstName} ${user.lastName}`}
                  </MenuItem>
                ))}
              </Select>
            </Box>
            <Box>
              <Typography variant="body2" sx={{ mb: 1, fontWeight: 700 }}>
                Meeting title
              </Typography>
              <TextField
                type="text"
                size="small"
                placeholder="Title goes here..."
                slotProps={{ htmlInput: { maxLength: 200 } }}
                sx={{ minWidth: "150px" }}
                value={form.meetingTitle}
                onChange={(e) =>
                  setForm({ ...form, meetingTitle: e.target.value })
                }
              />
            </Box>
            <Box>
              <Typography variant="body2" sx={{ mb: 1, fontWeight: 700 }}>
                Start time
              </Typography>
              <DateTimePicker
                value={form.startTime}
                onChange={(value) => setForm({ ...form, startTime: value })}
                timeSteps={{ minutes: 15 }}
                timezone={APP_TIME_ZONE}
                ampm={false}
                slotProps={{
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
                End time
              </Typography>
              <DateTimePicker
                value={form.endTime}
                onChange={(value) => setForm({ ...form, endTime: value })}
                timeSteps={{ minutes: 15 }}
                timezone={APP_TIME_ZONE}
                ampm={false}
                slotProps={{
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
          <Box>
            <Typography variant="body2" sx={{ mb: 0.5, fontWeight: 700 }}>
              Attendees
            </Typography>
            <Box sx={{ display: "flex", flexWrap: "wrap" }}>
              {props.users.map((user) => {
                const checked = form.attendeeIds.includes(user.id);

                return (
                  <FormControlLabel
                    key={user.id}
                    label={`${user.firstName} ${user.lastName}`}
                    control={
                      <Checkbox
                        checked={checked}
                        onChange={(event) =>
                          setForm({
                            ...form,
                            attendeeIds: event.target.checked
                              ? [...form.attendeeIds, user.id]
                              : form.attendeeIds.filter((id) => id !== user.id),
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
        <Button
          variant="contained"
          color="success"
          onClick={handleSubmit}
          disabled={createMutation.isPending}
        >
          {createMutation.isPending ? "Creating..." : "Create reservation"}
        </Button>
      </Box>
      {(validationError ||
        createMutation.isError ||
        createMutation.isSuccess) && (
        <Alert
          severity={
            validationError || createMutation.isError ? "error" : "success"
          }
          onClose={() => {
            setValidationError("");
            createMutation.reset();
          }}
          sx={{ mt: 2 }}
        >
          {validationError ||
            (createMutation.isError
              ? createMutation.error.message
              : "Reservation created successfully.")}
        </Alert>
      )}
    </Paper>
  );
}
