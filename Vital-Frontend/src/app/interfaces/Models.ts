export interface CalendarDay {
  id: string; //guid
  date: Date;
  userId: string; //guid
  selectedMetrics: Metrics[];
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
  values: MetricValue[];
}

export interface MetricValue {
  id: string; //guid
  name: number;
  metricId: string; //guid
}

export interface Cycle {
  id: string; //guid
  startDate: Date;
  endDate?: Date;
  userId: string; //guid
  cycleDays: CycleDay[];
}

export interface Period {
  cycleDays: CycleDay[];
}


