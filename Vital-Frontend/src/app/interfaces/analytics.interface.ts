export interface CycleAnalyticsDto {
  StartDate: Date;
  EndDate: Date;
  PeriodDays: Date[];
}

export interface PeriodCycleStatsDto {
  averageCycleLength: number,
  averagePeriodLength: number,
  currentCycleLength: number
}
