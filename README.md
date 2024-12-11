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

4. Currently it runs on HTTPS as well as HTTP, for ease of use, please test with http://localhost:5187, and ignore any SSL Certifcate Validation.
