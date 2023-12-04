import {Component} from "@angular/core";

@Component({
  selector: 'first-login',
  template: `
      <div class="flex flex-col items-center justify-center h-full">
          <div class="flex flex-col items-center justify-center">
              <h1 class="text-4xl font-bold text-center">Welcome to Vital</h1>
              <p class="text-center">Please set the average length of your cycle and period below</p>
          </div>
          <div class="flex flex-col items-center justify-center mt-4">
              <button class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
                      routerLink="/initial-login">
                  Set cycle and period
              </button>
          </div>
      </div>
  `
})
export class FirstLoginComponent {
  title = 'first-login';

}
