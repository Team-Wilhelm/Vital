export interface CalendarDayMetricDto {
  id: string; //guid
  calendarDayId: string; //guid
  metricsId: string; //guid
  metrics: any; // Replace 'any' with the appropriate type
  metricValueId: string; //guid
}

export interface CalendarDayMetricViewDto {
    calendarDayId: string; //guid
    metricsId: string; //guid
    metricValueId: string; //guid
}

export interface MetricsDto {
  id: string; //guid
  name: string;
  values: MetricValueDto[];
}

export interface MetricValueDto {
  id: string; //guid
  metricsId: string; //guid
  name: string;
  calendarDayIds: string[]; //guid
}

export interface MetricViewDto {
  id: string; //guid
  name: string;
  values: MetricValueViewDto[];
}

export interface MetricValueViewDto {
  id: string; //guid
  metricsId: string; //guid
  name: string;
}

export interface MetricRegisterMetricDto {
  metricsId: string; //guid
  metricValueId: string; //guid
}
