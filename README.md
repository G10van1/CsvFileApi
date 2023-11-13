# CsvFileApi
Get list of users from a CSV file.

## Instruction
There are two project, the main project is a WEB API to upload a CSV file and search using query commands. The second project run unity tests on the WEB API. It is possible use the Swagger UI on url "https://localhost:5001/swagger/index.html". Use the Visual Studio 2022 to load and run the project. To run the main project use "dotnet run --project CsvFileApi.csproj" on command prompt. The test project is called using "dotnet test CsvFileApiTest.dll". To have a more user-friendly experience, it is recommended to use Test Manager tool in Visual Studio to run the test project.

### Post Endpoint (/api/Files)
Use the "/api/Files" endpoint to upload the CSV file.
The first test (Test1) on the Test project do this operation.
File sample:

          name,city,country,favorite_sport

          John Doe,New York,USA,Basketball
          
          Jane Smith,London,UK,Football
          
          Mike Johnson,Paris,France,Tennis
          
          Karen Lee,Tokyo,Japan,Swimming
          
          Tom Brown,Sydney,Australia,Running
          
          Emma Wilson,Berlin,Germany,Basketball

### Get Endpoint (/api/Users)
It must choose the the parameters to find users. Use q field to insert commands.

If the q value is empty it will return all users inserted.

It is possible use searches by key and value, use ':' to separate the key and value.

Example: 

            ?q=name:Mary

It return all users where the name is equal to Mary.

It is possible use '+' (and comparator) and '|' (or comparator) to join more than
one field on the search. Just a warning, don't mix '+' and '-' at the same query,
it's not possible.

Example: 

             ?q=name:Mary+city:London

             ?q=name:Mary|city:London

It is possible split the values on the search. Use the '*' to split the search string.

Example:

             ?q=name:A*b*c
             
It return all users where the name starts with 'A', contains 'b' and ends with 'c'.

The values are no case sensitive.

Endpoint Returned Codes:

          code="200" Returns the list of the users
          
          code="400" If the parameter q is invalid
          
          code="500" Server error

### Tenologies
- Visual Studio Community 2022
- .Net 6

