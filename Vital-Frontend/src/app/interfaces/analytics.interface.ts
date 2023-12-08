export interface CycleAnalyticsDto {
  startDate: Date;
  endDate: Date;
  periodDays: Date[];
}

export interface PeriodCycleStatsDto {
  averageCycleLength: number,
  averagePeriodLength: number,
  currentCycleLength: number
}
