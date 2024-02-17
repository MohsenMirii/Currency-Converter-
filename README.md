# The Currency Converter
#  C#, Dot Net8, .Net Core, EF Core, Generic Repository, CQRS, Dependency Injection, Docker, Restfull API, Caching, Swagger

Write a configurable module that given a list of currency exchange rates, converts currencies to one another. Let’s carry on with an example.
Assume we have the following conversion rates:
- (USD => CAD) 1.34
- (CAD => GBP) 0.58
- (USD => EUR) 0.86

If we want to convert USD to EUR it’s pretty straight forward, we just multiply the amount by the conversion rate which is 0.86. Also, to convert 
CAD to USD we may divide the amount by the conversion rate which is 1.34. On the other hand, notice that if we want to convert CAD to EUR there’s 
no direct way to do so. Our currency converter should be able to do this by finding a “conversion path”, if you will. The conversion path requires
our converter in order to convert CAD to EUR to first convert the amount to USD, and then to EUR. Implement ICurrencyConverter with the following 
requirements in mind:
▪ The converter should find the shortest conversion path (if any).
▪ The converter is a singleton with potentially multiple threads invoking its Convert method.
▪ Because the Convert method is frequently invoked, optimization is top priority in terms of
minimum locking, less processing cost etc.

Solve this problem considering a practical production environment.
