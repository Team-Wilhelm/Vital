export interface InitialLoginGetDto {
  cycleLength: number | null;
  periodLength: number | null;
}

export interface InitialLoginPostDto {
  cycleLength: number;
  periodLength: number;
  lastPeriodStart: Date;
  lastPeriodEnd?: Date;
}


