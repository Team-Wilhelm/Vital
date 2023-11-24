import {CalendarDay, CycleDay} from './day.interface';
import {ApplicationUser} from "./user.interface";

export interface Cycle {
  id: string; //guid
  startDate: string; //Date
  endDate?: string; //Date
  userId: string; //guid
  user?: ApplicationUser;
  cycleDays: CycleDay[];
}
