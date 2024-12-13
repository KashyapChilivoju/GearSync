# GearSync
This project is intended for the Technical Test component of OracleCMS hiring process. 


## Running the Application

By default, the application runs on both HTTP and HTTPS:
- HTTP: `http://localhost:5187`
- HTTPS: `https://localhost:7054`

To use HTTPS locally:
1. Ensure you have .NET developer HTTPS certificates installed:
   ```bash
   dotnet dev-certs https --trust

2. run command in terminal: sh start-projects.sh

3. feel free to test sqlite3 on GearSync.db

4. Currently it runs on HTTPS as well as HTTP, for ease of use, please test with http://localhost:5187 if testing only API.


## Testing the Endpoints

for all intents and purposes please use http://localhost:5187 and ignore SSL Certificate Validation.

Body Type application/json

1. Register

Endpoint: http://localhost:5187/register

Request Type: POST

Request Body: 
{
   "DealerName": "string",
   "Password": "string"
}

NOTE: DealerName is unique, cannot have the same name of existing dealer. No Password requirements for ease.


Response: 
{
  "DealerID": 1
}

returns Dealer ID effectively the username from here onwards. The User is required to provide this DealerID for logging in.

2. Login

Endpoint: http://localhost:5187/login

Request Type: POST

Request Body: 
{
  "DealerID": 123,
  "Password": "string"
}


Response: 
{
  "Token": "string"
}

3. Add Car

Endpoint: http://localhost:5187/cars/add

Request Type: POST

Request Body: 
{
  "Make": "string",
  "Model": "string",
  "Year": 2023,
  "Colours": "Red, Blue, Black",
  "Body": "SUV",
  "Transmission": "Automatic",
  "FuelType": "Petrol",
  "Seats": 5,
  "Doors": 4
}

Response: 
{
  "Success": true
}

4. Remove Car

Endpoint: http://localhost:5187/cars/remove

Request Type: DELETE

Request Body: 
{
  "Make": "string",
  "Model": "string",
  "Year": 2023,
  "Colours": "Red, Blue, Black",
  "Body": "SUV",
  "Transmission": "Automatic",
  "FuelType": "Petrol",
  "Seats": 5,
  "Doors": 4
}

Response: 
{
  "Success": true
}

5. Get all Cars

Endpoint: http://localhost:5187/cars

Request Type: GET

Request Body: 
{
  "Make": "string",
  "Model": "string",
  "Year": 2023,
  "Colours": "Red, Blue, Black",
  "Body": "SUV",
  "Transmission": "Automatic",
  "FuelType": "Petrol",
  "Seats": 5,
  "Doors": 4
}

Response: 
{
  "Success": true
}

6. Add Stock

Endpoint: http://localhost:5187/stock/add

Request Type: POST

Request Body: 
{
  "CarID": "string",
  "DealerID": int,
  "Token": "JWT Token",
  "StockLevel": int,
  "Colour": "String"
}

Response: 
{
  "Success": true,
  "Error" : "String"
}

7. Remove Stock

Endpoint: http://localhost:5187/stock/remove

Request Type: DELETE

Request Body: 
{
  "CarID": "string",
  "DealerID": int,
  "Token": "JWT Token"
}

Response: 
{
  "Success": true,
  "Error" : "String"
}

8. Update Stock

Endpoint: http://localhost:5187/stock/update

Request Type: PUT

Request Body: 
{
    "CarID": 3,
    "DealerID": 1000,
    "Token": "MTAwMC00YmMzYTA5NS0xMTg1LTQwZGUtODE5OS05MDM1YmNkOGY4MWM=",
    "StockLevel" : 200,
    "Colour": "String"
}


Response: 
{
  "Success": true,
  "Error": "String"
}

9. Get Dealer Stock

Endpoint: http://localhost:5187/stock/dealer

Request Type: GET

Request Body: 
{
  "DealerID": "string",
  "Token": "string",
}

Response: 
{
  "Success": true,
  "Error": "String",
   [
  {
    "CarID": 1,
    "Make": "Toyota",
    "Model": "Camry",
    "Year": 2023,
    "Colour": "Red",
    "StockLevel": 10
    "Body" :  "String",
    "Transmission" : "String",
    "FuelType" : "String",
    "Seats" : int
    "Doors" : int
  },...
]

}

10. Filter Cars


Endpoint: http://localhost:5187/cars/filter

Request Type: GET

Request Body: 
{
  "Make": "string",
  "Model": "string",
  "Year": 2023,
  "Colours": "Red, Blue, Black",
  "Body": "SUV",
  "Transmission": "Automatic",
  "FuelType": "Petrol",
  "Seats": 5,
  "Doors": 4
}

NOTE: All these filters are optional, when no filters are applied, it acts the same as get all cars.

Response: 
{
   [
  {
    "CarID": 1,
    "Make": "Toyota",
    "Model": "Camry",
    "Year": 2023,
    "Colour": "Red",
    "StockLevel": 10
    "Body" :  "String",
    "Transmission" : "String",
    "FuelType" : "String",
    "Seats" : int
    "Doors" : int
  },...
]

}


10. Filter Cars by dealer stock


Endpoint: http://localhost:5187/stock/filter

Request Type: GET

Request Body: 
{
  "DealerID": int,
  "Token": "JWT Token",
  "Make": "string",
  "Model": "string",
  "Year": 2023,
  "Colours": "Red, Blue, Black",
  "Body": "SUV",
  "Transmission": "Automatic",
  "FuelType": "Petrol",
  "Seats": 5,
  "Doors": 4
}

NOTE: dealerID and token are mandatory, remaining filters are optional same as Filters Cars

Response: 
{
   [
  {
    "CarID": 1,
    "Make": "Toyota",
    "Model": "Camry",
    "Year": 2023,
    "Colour": "Red",
    "StockLevel": 10
    "Body" :  "String",
    "Transmission" : "String",
    "FuelType" : "String",
    "Seats" : int
    "Doors" : int
  },...
]

}


