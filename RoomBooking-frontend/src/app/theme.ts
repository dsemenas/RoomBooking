import { createTheme } from "@mui/material/styles";

const theme = createTheme({
  palette: {
    primary: {
      main: "#694aba",
    },
    secondary: {
      main: "#a38fd9",
    },
    background: {
      paper: "#fff",
      default: "#fefdff",
    },
    text: {
      primary: "#08060e",
      secondary: "#5F5B6B",
      disabled: "#8C8896",
    },
  },
  typography: {
    fontFamily:
      '"Nunito Variable", system-ui, -apple-system, "Segoe UI", Roboto, sans-serif',
    h1: { fontSize: "1.875rem", fontWeight: 800, lineHeight: 1.2 },
    h2: { fontSize: "1.375rem", fontWeight: 800, lineHeight: 1.25 },
    h3: { fontSize: "1rem", fontWeight: 800, lineHeight: 1.3 },
    body1: { fontSize: "1rem" },
    body2: { fontSize: "0.875rem" },
    button: { textTransform: "none", fontWeight: 700 },
  },
  components: {
    MuiPaper: {
      defaultProps: { variant: "outlined", elevation: 0 },
    },
    MuiAppBar: {
      defaultProps: { elevation: 0 },
      styleOverrides: {
        root: ({ theme }) => ({
          borderBottom: `1px solid ${theme.palette.divider}`,
        }),
      },
    },
    MuiButton: {
      defaultProps: { disableElevation: true },
    },
    MuiOutlinedInput: {
      styleOverrides: {
        root: ({ theme }) => ({
          borderRadius: "999px",
          backgroundColor: theme.palette.background.paper,
        }),
        multiline: { borderRadius: 16 },
      },
    },
  },
});

export default theme;
