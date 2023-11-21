export interface CalendarDay {
  id: string; //guid
  date: Date;
  UserId: string; //guid
  user: ApplicationUser;
  metrics: Metrics[];
  state: string;
}

export interface CycleDay extends CalendarDay {
  cycleId: string; //guid
  cycle: Cycle;
  isPeriod: boolean;
}

export interface ApplicationUser {
  id: string; //guid
  name: string;
  email: string;
  passwordHash: string;
  role: string;
}

export interface Metrics {
  id: string; //guid
  name: string;
  description?: string;
  values: string[];
  selectedValue: string;
}

export interface Cycle {
  id: string; //guid
  startDate: Date;
  endDate: Date;
  userId: string; //guid
  user: ApplicationUser;
  cycleDays: CycleDay[];
  period?: Period; //
}

export interface Period {
  cycleDays: CycleDay[];
}


