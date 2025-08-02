import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatNativeDateModule } from '@angular/material/core';
import { MatTableModule } from '@angular/material/table';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatSelectModule } from '@angular/material/select';
import { map } from 'rxjs';
import { ApiInfo } from '../../models/api.model';
import { Currency } from '../../models/currency.model';
import { Client, CurrencyDto, ExchangeRateApiTypeDto } from '../../services/api.services';

@Component({
  selector: 'app-currency-calculator',
  standalone: true,
  imports: [    
    CommonModule,
    MatSelectModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatButtonModule,
    MatTableModule,
    MatNativeDateModule,
    ReactiveFormsModule],
  providers: [Client],  
  templateUrl: './currency-calculator.component.html',
  styleUrl: './currency-calculator.component.scss'
})
export class CurrencyCalculatorComponent implements OnInit {
  form: FormGroup;

  public initialized: boolean = false;
  public apiOptions: ApiInfo[] = [];

  public selectedApi: string = '';
  public showNoData: boolean = false;

  public currencies: Currency[] = [];

  public minRate: number | null = null;
  public maxRate: number | null = null;
  public avgRate: number | null = null;
  public tableData: { date: string; rate: number }[] = [];

  constructor(
    private fb: FormBuilder, 
    private apiClient: Client
    ) {
    this.form = this.fb.group({
      api: [],
      sourceCurrency: [''],
      targetCurrency: [''],
      dateFrom: [new Date()],
      dateTo: [new Date()]
    });
  }

  ngOnInit(): void {
    this.loadApi();

    const today = new Date();
    this.form.patchValue({
      dateFrom: today,
      dateTo: today
    });
  }

  private loadApi(): void {
    this.apiClient.getExchangeRateApiList().subscribe(
      (response) => {
        this.apiOptions = response.map(
          (r: ExchangeRateApiTypeDto) => ({
            code: r.code?? "", 
            name: r.name?? ""
          }));

        this.selectedApi = this.apiOptions?.length ? this.apiOptions[0].code : '';
        this.form.patchValue({api : this.selectedApi});
        this.initialized = true;
        this.loadCurrencies();
        this.showNoData = false;
      },
      (error) => {
        if (error.status) {
        switch (error.status) {
          case 204:
            console.warn('loadApi: Brak danych (204)');
            this.apiOptions = [];
            break;
          default:
            alert("Błąd zaczytania listy api"); 
            break; 
        }
      }
    });
  }

  public loadCurrencies(): void {
    this.clearFields();
    this.apiClient.getCurrencyList(this.selectedApi).subscribe(
      response => {
       this.currencies = response.map((r: CurrencyDto) => ({
        code: r.code?? "",
        name: r.name?? ""
        }));
      },
      (error) => {
          console.log("error");
          if (error.status) {
          switch (error.status) {
            case 204:
              console.log('loadCurrencies: Brak danych (204)');
              this.apiOptions = [];
              break;
            default:
              alert("Błąd zaczytania listy api"); 
              break; 
          }
        }
    });
  }

  public onCalculate(): void {
    const formValues = this.form.value;
    const dateFrom: Date = formValues.dateFrom?? new Date();
    const dateTo: Date = formValues.dateTo?? new Date();

    const sourceCurr = formValues.sourceCurrency?.split(' - ')[0];
    const targetCurr = formValues.targetCurrency?.split(' - ')[0];

    if (!sourceCurr || !targetCurr) {
      alert('Wybierz waluty źródłową i docelową');
      return;
    }

    this.apiClient.calculateCurrencyRate(this.selectedApi, sourceCurr, targetCurr, dateFrom, dateTo).subscribe(
      response => {
        this.clearResultFields()

        if (response && response.rateList) {
          this.showNoData = false;
          const rates = response.rateList.map((r: any) => r.mid);
          this.minRate = response.minRate?? null;
          this.maxRate = response.maxRate?? null;
          this.avgRate = response.avgRate?? null;

          this.tableData = response.rateList.map((r: any) => ({
            date: r.date,
            rate: r.rate
          }));
        } else {
          this.showNoData = true;
        }
      },
     (error) => {
        console.log("rate calculation error:", error);
        this.showNoData = true;;
      }
    );
  }

  private clearFields(): void{
    this.form.patchValue({
      sourceCurrency: [''],
      targetCurrency: [''],
      dateFrom: new Date,
      dateTo: new Date
    });
    this.clearResultFields();
  }

  private clearResultFields(): void{
    this.tableData = [];
    this.minRate = this.maxRate = this.avgRate = null;
  }
}