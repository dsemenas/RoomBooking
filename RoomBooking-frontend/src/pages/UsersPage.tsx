import Alert from "@mui/material/Alert";
import Chip from "@mui/material/Chip";
import Container from "@mui/material/Container";
import Paper from "@mui/material/Paper";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableContainer from "@mui/material/TableContainer";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import Typography from "@mui/material/Typography";

import { useUsers } from "../hooks/useUsers";

export default function UsersPage() {
  const { data, isPending, error } = useUsers();

  return (
    <Container maxWidth="xl" sx={{ py: 3 }}>
      {isPending && <Typography variant="h1">Loading...</Typography>}

      {error && <Alert severity="error">{error.message}</Alert>}

      {data && (
        <TableContainer component={Paper}>
          <Table>
            <TableHead
              sx={{
                backgroundColor: "primary.main",
                "& th": { color: "white" },
              }}
            >
              <TableRow>
                <TableCell>ID</TableCell>
                <TableCell>Last name</TableCell>
                <TableCell>First name</TableCell>
                <TableCell align="right">Active reservations</TableCell>
              </TableRow>
            </TableHead>

            <TableBody>
              {data.map((user) => (
                <TableRow key={user.id} hover>
                  <TableCell>{user.id}</TableCell>
                  <TableCell>{user.lastName}</TableCell>
                  <TableCell>{user.firstName}</TableCell>
                  <TableCell align="right">
                    <Chip
                      size="small"
                      label={user.activeReservationsCount}
                      color={
                        user.activeReservationsCount > 0 ? "primary" : "default"
                      }
                    />
                  </TableCell>
                </TableRow>
              ))}

              {data.length === 0 && (
                <TableRow>
                  <TableCell colSpan={4} align="center">
                    No users found.
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Container>
  );
}
