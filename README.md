# pokedex

Installations Before Running
- .Net Core 5 SDK
- .Net Core 5 Runtime

Production Design
- Poke.Api and Translation.Api would be separated projects as in microservices architecture and talk each other in different containers.
- Retry and circuit breaker would be more resilient up to business requirements(Circuit Breaker must return Service Unavailable instead of Http 500).
- HttpClient would be desgined with Decorater which is better for unit testing.
- Unit test coverage must be increased since it becomes unreliable unless it covers any single line and edge cases.
- If i start a project in production level, i'd probaby like to add Infrastructure as code like Terraform or AWS Cloudformation from the beginning.
- Instead of appsettings, parameters would be stored in centralized, reliable and secure services like AWS SSM parameter store or Secret Manager which stored them encrypted and provide key rotation. Then, the application would get all parameters once in runtime during container creation.
- Distributed and centralized cache system would be my preference in order to reduce API call network traffic and price.
- The most importantly, all warnings and exceptional cases must be pushed to log management services like Datadog, Cloudwatch, maybe splunk(expensive:) by using regarding agents if it is not managed. Useful dashboards which shows API requests status with 4XX, 5XX errors, latencies and external service CPUs, connections etc. would be good to monitor production system and In case of internal or external exceptional cases, there should be a notification service(decoupled from apps) which notifies all listeners related with the issue.
- Everytime we make a call to pokemon-species API, it returns all info which is unnecessary and network costly. Producer may design a response mapper which changes over subscribed clients.
