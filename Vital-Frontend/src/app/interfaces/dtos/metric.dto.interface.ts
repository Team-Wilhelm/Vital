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
  metricValueId?: string; //guid
  createdAt: Date;
}
