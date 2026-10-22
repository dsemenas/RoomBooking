import { Route, Routes } from "react-router";

import NavBar from "../components/Navbar";
import MeetingRoomsPage from "../pages/MeetingRoomsPage";
import NotFoundPage from "../pages/NotFoundPage";
import ReservationsPage from "../pages/ReservationsPage";
import RoomDetailsPage from "../pages/RoomDetailsPage";
import UsersPage from "../pages/UsersPage";

function App() {
  return (
    <>
      <NavBar />
      <Routes>
        <Route path="/" element={<UsersPage />} />
        <Route path="/users" element={<UsersPage />} />
        <Route path="/reservations" element={<ReservationsPage />} />
        <Route path="/rooms" element={<MeetingRoomsPage />} />
        <Route path="/rooms/:roomId" element={<RoomDetailsPage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </>
  );
}

export default App;
