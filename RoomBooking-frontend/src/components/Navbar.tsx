import AppBar from "@mui/material/AppBar";
import Button from "@mui/material/Button";
import Container from "@mui/material/Container";
import Toolbar from "@mui/material/Toolbar";
import { NavLink } from "react-router";

const navItems = [
  { to: "/users", label: "Users" },
  { to: "/rooms", label: "Rooms" },
  { to: "/reservations", label: "Reservations" },
];

export default function NavBar() {
  return (
    <AppBar
      position="static"
      color="inherit"
      elevation={0}
      sx={{ borderBottom: 1, borderColor: "divider" }}
    >
      <Container
        maxWidth="lg"
        sx={{ display: "flex", justifyContent: "center" }}
      >
        <Toolbar disableGutters sx={{ gap: 2 }}>
          {navItems.map((item) => (
            <Button
              key={item.to}
              component={NavLink}
              to={item.to}
              sx={{
                color: "text.secondary",
                borderRadius: "999px",
                "&.active": {
                  color: "white",
                  bgcolor: "primary.main",
                  fontWeight: 800,
                },
              }}
            >
              {item.label}
            </Button>
          ))}
        </Toolbar>
      </Container>
    </AppBar>
  );
}
