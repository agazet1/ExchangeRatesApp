import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CurrencyCalculatorComponent } from './components/currency-calculator/currency-calculator.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CurrencyCalculatorComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'ExchangeRatesApp';
}
