import {Component} from '@angular/core';

@Component({
  selector: 'app-current-cycle',
  template: `
    <div class="flex justify-center items-center text-center rounded-full bg-pink-300 aspect-square
    w-10
    sm:w-16
    md:w-24">
      <p class="text-sm sm:text-lg md:text-2xl">13.11</p>
    </div>
  `
})

export class CurrentCycleComponent {
  title = 'current-cycle';

  constructor() {

  }
}
