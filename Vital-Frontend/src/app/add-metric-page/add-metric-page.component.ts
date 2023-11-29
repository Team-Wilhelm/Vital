import {Component, ElementRef, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {DataService} from '../services/data.service';
import {MetricService} from '../services/metric.service';
import {CycleService} from "../services/cycle.service";
import {Subscription} from "rxjs";
import {Router} from "@angular/router";

@Component({
  selector: 'add-metric',
  templateUrl: './add-metric-page.component.html',
})
export class AddMetricPageComponent implements OnDestroy {
  @ViewChild('newCycleModal') newCycleModal!: ElementRef;
  private subscription: Subscription;
  private lastLoggedFlowDate: Date | null = null;
  newCycleStarted: boolean = false;

  constructor(public metricService: MetricService,
              public dataService: DataService,
              public cycleService: CycleService,
              private router: Router) {
    this.subscription = this.dataService.lastLoggedFlowDate$.subscribe(lastLoggedFlowDate => {
      if (lastLoggedFlowDate) {
        this.lastLoggedFlowDate = lastLoggedFlowDate;
      }
    });
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  // This method needs to return a promise of the user's choice, which is necessary to pause the execution of the saveMetrics() method
  displayNewCycleDialog(): Promise<boolean> {
    return new Promise(resolve => {
      // Display the modal
      this.newCycleModal.nativeElement.showModal();
      // Focus on yes button
      setTimeout(() => {
        const yesButton = document.getElementById('yes-button');
        yesButton?.focus();
      }, 100);

      // Resolve the promise with the user's choice
      document.querySelector('#yes-button')!.addEventListener('click', () => {
        resolve(true); // Resolve the promise with `true` when Yes is clicked
      });

      document.querySelector('#no-button')!.addEventListener('click', () => {
        resolve(false); // Resolve the promise with `false` when No is clicked
      });
    });
  }

  async saveMetrics() {
    // Check if the last logged flow is more than 2 days ago and less than 10 days ago, if so, ask if new cycle has started
    console.log(this.lastLoggedFlowDate);
    if (this.lastLoggedFlowDate) {
      const today = new Date();
      const diffTime = Math.abs(today.getTime() - this.lastLoggedFlowDate.getTime());
      const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

      if (diffDays > 2 && diffDays <= 10) {
        this.newCycleStarted = await this.displayNewCycleDialog();
      }
    }

    if (this.newCycleStarted) {
      await this.cycleService.startNewCycle();
    }

    // Save the metrics
    const saveSuccessful = await this.metricService.saveMetrics();

    // If saving was successful, redirect to dashboard
    if (saveSuccessful) {
      await this.router.navigate(['/dashboard']);
    }
  }
}
