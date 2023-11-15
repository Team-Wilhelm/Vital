import {Component} from '@angular/core';

@Component({
  selector: 'app-current-cycle',
  template: `
    <div class="flex justify-center items-center text-center rounded-full bg-pink-300 w-24 h-24">
      <p>13.11</p>
    </div>
  `
})

export class CurrentCycleComponent {
  title = 'current-cycle';

  constructor() {

  }
}
