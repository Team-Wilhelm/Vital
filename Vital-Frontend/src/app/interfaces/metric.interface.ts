export interface Metrics {
  id: string; //guid
  name: string;
  values: MetricValue[];
}

export interface MetricValue {
  id: string; //guid
  metricsId: string; //guid
  name: string;
}
