import dayjs from "dayjs";
import timezone from "dayjs/plugin/timezone";
import utc from "dayjs/plugin/utc";

dayjs.extend(utc);
dayjs.extend(timezone);

export const APP_TIME_ZONE = "Europe/Vilnius";

export const formatLocal = (iso: string, fmt = "D MMM YYYY, HH:mm") =>
  dayjs(iso).tz(APP_TIME_ZONE).format(fmt);
