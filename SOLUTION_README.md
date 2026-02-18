# Car Dealer Lead Processing Automation - Solution

## Overview

This is a production-grade automation system designed to process car dealer leads through a complete pipeline. The system includes API ingestion, validation, enrichment, and saving leads into a database. The application is built using C# and .NET, and is containerized using Docker for easy deployment.

---

## Setup Instructions

### Prerequisites

- Docker
- Docker Compose
- REST API testing tool (e.g., Postman)
- Web browser for accessing Swagger API documentation 
- Optional: .NET SDK for local development

### Running the Application
1. Clone the repository:
   ```bash
   git clone [repository_url]
   cd [repository_directory]
   ```
2. Build and run the application using Docker Compose:
   ```bash
    docker-compose up --build
    ```
---

## API Documentation
This project automatically generates API documentation using Swagger. Once the application is running, you can access the API documentation at:
http://localhost:5000/swagger/index.html

All endpoints, request/response schemas and sample payloads are available in the Swagger UI. This ensures that the documentation is always up to date with the code. 
         


---
## Design Decisions
### Step 1: API Ingestion
- The microservice uses the basic ASP.NET Core Web API template, which provides a robust and scalable foundation for building RESTful APIs. 
The use of services and interfaces and the dependency injection are standard practices in .NET development, which promote loose coupling and testability.
- In order to process the leads asynchronously, the API endpoint uses a background service to create a fire and forget task. This allows the API to respond immediately to the client while the lead is processed in the background. In addition, there is another controller which allows the user to check the status of the background task and cancel the task before it is completed. In a production environment, this can be easily replaced by a message broker like RabbitMQ or Azure Service Bus.
### Step 2: Validation
- General validations are implemented using ASP.NET Core's built-in model validation annotations, as they are simple to implement and work well.
- When doing more complicated validations, I chose to use FluentValidation, which is a popular validation library for .NET. 
- For email validations, I chose to first validate them using a local list of known acceptable email domains and known domains for disposable emails. 
- The reason I chose to use a local list of domains is that it is much faster than making an API call to a third-party service, and it can be easily updated as needed. Also, many of these services charge for each API call, so there is no reason to pay a separate service in order to validate that a Gmail address is not disposable.
- I chose to use isfakemail.com, because it is very easy to use and it is free. It is not the best service, so in a production environment, I would probably prefer to use a service like ZeroBounce or NeverBounce, which are more reliable and have better accuracy.
- In a production environment, I would most likely cache the results to avoid unnecessary API calls, but for the current scope, I feel this is sufficient.

### Step 3: Load Business Rules from Files
- For reading the Excel file, I chose to use the EPPlus library. 
- For reading the text file, I chose to use the built-in File.ReadAllLines method, which is simple and efficient for reading small files.
- The data from the files is loaded into memory when the application starts, and it is used to enrich the leads as they are processed. This allows for fast access to the data without having to read the files every time a lead is processed.
- The classes and methods used to read and parse the files are designed to be easily replaceable with a database or an external service in a production environment.

### Step 4: External API Enrichment
- The external API calls are made using the native HttpClient.
- In the event of a failure, the system will retry up to 3 times. A failure is defined as a timeout (5 seconds) or a status code in the 500 range. If a Bad Request response is received, no additional retries will be attempted.
- The rationale behind this approach is that if the enrichment fails due to bad lead data, there is no reason to retry. Depending on the specs of a real enrichment service or requirements by product, this can be easily modified, but I felt this was a good enough approach for the scope of this project.

### Step 5: Lead Routing & Final Processing
- The scoring outlined in the ASSIGNMENT_README allowed for a potential score of 110. This is possible with high priority lead with a high trust level email insight, verified phone wanting to buy a luxury car that is in stock. Since the max allowed score is 100, I implemented the logic to change a score above 100 to exactly 100. 
- For the final enriched lead object, I reused existing objects when possible, and I created new objects when necessary. The enriched lead object is designed to be easily extendable in the future if additional fields need to be added.
- Since the assignment specified fields that do not conform to the standard C# naming conventions, I used JsonPropertyName attributes to map them. This can also be done via Newtonsoft depending on the requirements of the system.
- In a production environment, I most likely would have chosen to use MongoDB to store the processed leads, as I find their structure to be more flexible and better suited for this type of data. However, a structured DB can also work. For the scope of this project. 
- I chose to use an in-memory dictionary to act as a "database" for simplicity, but this can easily be changed to use a db driver in production.
- I included an endpoint to get the leads from the database. It is possible to pull an individual lead by email or phone, or if those query params are left blank, it will return an array of all the leads. 
- In production, I would likely implement a separate endpoint to pull multiple leads based on a certain criteria and possibly use pagination. 
### Step 6: Implement Structured Logging
- Logging implements the standard ILogger interface provided by ASP.NET Core, which allows for structured logging and can be easily configured to use different logging providers (e.g., Serilog, NLog, etc.) in a production environment.
- In a production environment, logging will most likely be handled by a service like Coralogix or Azure  Application Insights, however, for the scope of this assignment I chose to simply have the logger write to the console.
- Since the logger is injected, it is easily replaced with a logger service. 
### Step 7: Infrastructure & Observability
- The Dockerfile is designed to be self-contained so that no additional frameworks or SDKS are necessary to run the application. 
- The original docker-compose.yml file was built with a Python service in mind, so I modified it to work with ASP.NET Core. 
- The project fully runs using ```docker-compose up```, however, for first time setup of after code changes, it is recommended to use ```docker-compose up --build``` to ensure that the latest code changes are included in the container.

### Additional Production Considerations
- In production, I would most likely use a message broker like RabbitMQ or Azure Service Bus to handle the asynchronous processing of leads, rather than using a background service. This would allow for better scalability and reliability, as well as easier monitoring and management of the processing pipeline.
- In addition, most strings and parameters would be moved to an external configuration and will be injected using IConfiguration, which would allow for easier management and modification of these values without having to change the code. 

---

## Sample Requests

### Example 1: Valid Lead

```bash
curl -X 'POST' \
  'http://localhost:5000/api/leads' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d ' {
      "BranchID": "400",
      "ProductType": "",
      "WorkerCode": "910290",
      "AskedCar": "90962_101",
      "FirstName": "דני",
      "LastName": "כהן",
      "Email": "danny.cohen@gmail.com",
      "Phone": "0542100319",
      "Remarks": "מעוניין בטסט דרייב השבוע",
      "Banner": "",
      "FromWebSite": "forthing",
      "IsAllowGetMail": "TRUE",
      "Area": "1",
      "Url": "https://forthing.co.il/models/u-tour"
    }'
```

### Example 2: Invalid Lead

```bash
curl -X 'POST' \
  'http://localhost:5000/api/leads' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '    {
      "BranchID": "400",
      "ProductType": "",
      "WorkerCode": "",
      "AskedCar": "90962_101",
      "FirstName": "ניר",
      "LastName": "פריידי",
      "Email": "invalid-email",
      "Phone": "0529998877",
      "Remarks": "This lead has invalid email format",
      "Banner": "",
      "FromWebSite": "forthing",
      "IsAllowGetMail": "FALSE",
      "Area": "1",
      "Url": ""
    }'
```

### Example 3: High-Priority Lead

```bash
    {
      "BranchID": "400",
      "ProductType": "Luxury",
      "WorkerCode": "910297",
      "AskedCar": "90968_107",
      "FirstName": "Rachel",
      "LastName": "Goldstein",
      "Email": "rachel.goldstein@icloud.com",
      "Phone": "0547778899",
      "Remarks": "Interested in premium models",
      "Banner": "instagram",
      "FromWebSite": "instagram",
      "IsAllowGetMail": "TRUE",
      "Area": "1",
      "Url": "https://instagram.com/forthing_israel"
    }
```

---


