import {CycleDayDto} from "./day.dto.interface";

export interface UpdateCycleDto {
  startDate: string; //Date
  endDate: string; //Date
}

export interface CycleDto {
  id: string; //guid
  startDate: string; //Date
  endDate: string; //Date
  userId: string; //guid
  cycleDays: CycleDayDto[];
}

export interface CreateCycleDto {
  startDate: string; //Date
  endDate: string; //Date
}
