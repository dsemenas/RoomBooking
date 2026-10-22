import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Chip from "@mui/material/Chip";
import Paper from "@mui/material/Paper";
import Stack from "@mui/material/Stack";
import Typography from "@mui/material/Typography";
import { Link as RouterLink } from "react-router";

import type { Room } from "../types/room";

export default function RoomCard(props: { room: Room }) {
  const isActive = props.room.status.toLowerCase() === "active";
  const isOftenBookedAndRarelyUsed = props.room.isOftenBookedAndRarelyUsed;

  return (
    <Paper sx={{ p: 2, minWidth: "350px" }}>
      <Stack
        sx={{ justifyContent: "space-between", rowGap: 2, height: "100%" }}
      >
        <Stack spacing={2}>
          <Box
            sx={{
              display: "flex",
              justifyContent: "space-between",
              alignItems: "start",
            }}
          >
            <Box>
              <Typography variant="h6" sx={{ fontWeight: 700 }}>
                {props.room.name}
              </Typography>
              <Box>
                <Typography variant="body2">R-{props.room.id}</Typography>
                <Typography variant="body2">
                  {props.room.capacity} People
                </Typography>
                <Typography variant="body2">
                  {props.room.floor} Floor
                </Typography>
              </Box>
            </Box>
            <Stack
              sx={{
                display: "flex",
                gap: 0.5,
                alignItems: "end",
              }}
            >
              <Chip
                label={props.room.status}
                size="small"
                color={isActive ? "success" : "default"}
              />
              {isOftenBookedAndRarelyUsed ? (
                <Chip
                  label="Often booked, rarely used"
                  size="small"
                  color="warning"
                />
              ) : null}
            </Stack>
          </Box>
          <Stack direction="row" sx={{ flexWrap: "wrap", gap: 1 }}>
            {props.room.equipment.map((item) => (
              <Chip key={item} label={item} />
            ))}
          </Stack>
        </Stack>
        <Button
          fullWidth
          variant="contained"
          component={RouterLink}
          to={`/rooms/${props.room.id}`}
          sx={{ mt: "auto" }}
        >
          View details
        </Button>
      </Stack>
    </Paper>
  );
}
