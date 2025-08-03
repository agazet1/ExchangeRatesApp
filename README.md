# ExchaneRateApp

The project is a prototype of an application that will be used to convert exchange rates and their history based on data downloaded from the NBP api.

## Features
Based on the user's selection:
- api for exchange rates (currently only NBP)
- source and target currency code
- date range 
the application returns information on the exchange rate (minimum, maximum and average value of the exchange rate from the selected period) as well as daily values of the exchange rate for the selected period.

## Architecture
- Backend: .NET 8
- Frontend: Angular 17.3.17

## Configuration

**Backend**

In the appsettings.json file, specify:
- page address
- list of api definitions with addresses
- display format of the date in the exchange rate table
- way of sorting currencies, available values (by code and name, ascending or descending): CODE_ASC / CODE_DESC / NAME_ASC / NAME_DESC

```bash
{
  "FrontendUrl": "http://localhost:4200",
  "AppConfiguration": {
    "ExchangeRateApiList": [
      {
        "Code": "NBP",
        "Name": "NBP Api",
        "Url": "https://api.nbp.pl/"
      }
    ],
    "DateFormat": "yyyy-MM-dd",
    "CurrencyOrder": "CODE_ASC"
  }
}
```

The logger configuration is stored in the *nLog.config* file  

**Frontend**

In the assets/config/config.json file, set:
```bash
{
  "apiBaseUrl": "https://localhost:7242/"
}
```

## Extending functionality

To add a new api you need to:
- add an api for exchange rates in the configuration (appsettings.json)
- add a new service which supports the api and implements the IExchangeRateService interface
- add a binding between the added configuration and the added service in class ExchangeRateApiFactory


## Run Locally

Clone the project from git.
```bash
  git clone https://link-to-project
```

**Backend:**

Make sure you have .NET 8 SDK installed.  

Project fdirectoryolder: ExchangeRatesApp\ExchangeRatesApi  
Restore NuGet Packages for main project.

Launch the backend with the command:
```bash
dotnet run
```

The API will be available by default at: https://localhost:7242/  
Swagger UI: https://localhost:7242/swagger/index.html

**Frontend:**

Project directory: ExchangeRatesApp\Angular\ExchangeRatesApp  

Go to the project directory and install dependencies
```bash
  npm install
```

Start the server
```bash
  npm run start
```
or 
```bash
  ng serve
```

The application will be available at: http://localhost:4200