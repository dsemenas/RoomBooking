import Box from "@mui/material/Box";
import Container from "@mui/material/Container";
import Link from "@mui/material/Link";
import Typography from "@mui/material/Typography";
import { Link as RouterLink } from "react-router";

export default function NotFoundPage() {
  return (
    <Container maxWidth="xl" sx={{ py: 3 }}>
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          justifyContent: "center",
          alignItems: "center",
          gap: 3,
          height: "70vh",
        }}
      >
        <Typography variant="h1">404 Page Not Found</Typography>
        <Link component={RouterLink} to="/users" variant="h2">
          Back to Users Page
        </Link>
      </Box>
    </Container>
  );
}
