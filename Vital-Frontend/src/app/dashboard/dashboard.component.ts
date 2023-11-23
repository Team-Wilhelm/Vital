import {AfterViewInit, Component, ElementRef, ViewChild} from '@angular/core';
import {CycleService} from "../services/cycle.service";

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html'
})

export class DashboardComponent {
  title = 'dashboard';
  nextPeriodInDays: number = 0;
  @ViewChild('hasYourPeriodStartedModal') hasYourPeriodStartedModal!: ElementRef;

  constructor(public cycleService: CycleService) {
    cycleService.getPredictedPeriod().then(() => {
      this.nextPeriodInDays = this.calculateNextPeriodInDays();
    });
  }

  private calculateNextPeriodInDays() {
    this.cycleService.predictedPeriod.sort((a, b) => a.getTime() - b.getTime());
    const today = new Date();
    let nextPeriodIndex = this.cycleService.predictedPeriod.findIndex(date => date.getTime() > today.getTime());
    if (nextPeriodIndex === -1) {
      nextPeriodIndex = 0;
    }
    const nextPeriod = this.cycleService.predictedPeriod[nextPeriodIndex];
    const diffTime = Math.abs(nextPeriod.getTime() - today.getTime());
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  }

  public displayHasYourPeriodStartedDialog() : void {
    this.hasYourPeriodStartedModal.nativeElement.showModal();
    // Focus on yes button
    setTimeout(() => {
      const yesButton = document.getElementById('yes-button');
      yesButton?.focus();
    }, 100);
  }
}
