export interface CycleDayDto extends CalendarDayDto {
  cycleId: string; //guid
  isPeriod: boolean;
}

export interface PredictedPeriodDayDto extends CalendarDayDto {
  cycleId: string; //guid
}

export interface CalendarDayDto {
  id: string; //guid
  date: string; //Date
  userId: string; //guid
  selectedMetrics: any[]; // Replace 'any' with the appropriate type
  state: string;
}
