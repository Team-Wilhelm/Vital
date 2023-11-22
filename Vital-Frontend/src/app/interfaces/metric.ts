export interface Metric {
  name: string;
  isSelected: boolean;
  selectedValue: string | null;
  values: string[];
}

export interface MetricDto {
  id: string;
  value: string;
}
