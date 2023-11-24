export interface ApplicationUser {
  id: string; //guid
  name: string;
  email: string;
  passwordHash: string;
  role: string;
  currentCycleId?: string; //guid
  cycleLength: number;
  periodLength: number;
}
