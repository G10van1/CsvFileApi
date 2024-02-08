# CsvFileApi
Query a list of users from a CSV file using query filters.

## Instructions
There are two projects, the main project is a WEB API for uploading a CSV file and searching using query commands. The second project runs unit tests on the WEB API. You can use the Swagger UI at the url "https://localhost:5001/swagger/index.html". Use Visual Studio 2022 to load and run the project. To run the main project, use "dotnet run --project CsvFileApi.csproj" in the command prompt. The test project is called using "dotnet test CsvFileApiTest.dll". For a more user-friendly experience, it is recommended to use the Test Manager tool in Visual Studio to run the test project.

### Post Endpoint (/api/Files)
Use the "/api/Files" endpoint to upload the CSV file.
The first test (Test1) in this test project performs this operation.

File sample:

          name,city,country,favorite_sport

          John Doe,New York,USA,Basketball
          
          Jane Smith,London,UK,Football
          
          Mike Johnson,Paris,France,Tennis
          
          Karen Lee,Tokyo,Japan,Swimming
          
          Tom Brown,Sydney,Australia,Running
          
          Emma Wilson,Berlin,Germany,Basketball

Endpoint Returned Codes:

          code="201" Successful file upload
          
          code="500" Server error

### Get Endpoint (/api/Users)

You must choose the parameters to find users. Use the q field to enter query commands.

If the q value is empty, it will return all entered users.

It is possible to use searches by key and value, use ':' to separate the key and value.

Example: 

            ?q=name:Mary

Returns all users where the name is the same as Mary.

It is possible to use '+' (and comparator) and '|' (or comparator) to join more than
a field in the search. Just a warning, don't mix '+' and '-' in the same query,
this is not allowed.

Example: 

             ?q=name:Mary+city:London

             ?q=name:Mary|city:London

You can split the values in the search. Use the '*' character to split the search string.

Example:

             ?q=name:A*b*c
             
This query returns all users whose name starts with 'A', contains 'b' and ends with 'c'.

The values are case insensitive.

Endpoint Returned Codes:

          code="200" Returns the list of the users
          
          code="400" If the parameter q is invalid
          
          code="500" Server error

### Tenologies
- Visual Studio 2022
- .Net 6

