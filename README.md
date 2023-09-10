# Weather-App
This API provides information about the top 10 districts with the coolest average temperatures at 2 pm for the next 7 days. It's built using ASP.NET Core Web API (net6.0).

In this implementation, a database isn't used because the requirement is to work with the latest data every 7 days. To achieve this, all calculations are performed in-memory.

For a production-ready solution, the following approach can be considered:
#To maintain up-to-date temperature data, a periodic task can be set up using scheduling tools like Hangfire or scheduled Azure Functions.
#This task would periodically fetch new temperature data for all districts and update a database with the latest information.

It's worth noting that no third-party libraries are used in this implementation; native .NET libraries are employed. Additionally, Dependency Injection and the Repository Pattern are utilized for a structured codebase.

PS: I wasn't able to adhere to the frequent committing guideline due to time constraints. However, I compensated by adding extensive comments within the code to convey my thought process.Happy coding!
