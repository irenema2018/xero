## Development Environment
- I used Visual Studio 2019 Professional as the development platform for this project.
- I upgraded the .net core version to 3.1.
- Fixed all the issues in .net core 3.1 by downloading nuget packages.
- Once the project is building and running successfully, I started making changes.

## Database
- I am more familiar with SQL Server so changed SQLite to SQL Server.
- I have changed the id columns to uniqueidentifier.
- I have added primary keys and foreign keys.

## ORM (Object Relational Mapping)
- I have used Dapper for ORM.

## Repository
- I have used a repository class to handle all the database activities.

## DTO (Data Transfer Object)
- I have used DTO objects to transfer data back and forth.

## Logging
- I have used Serilog for logging.

## Testing
- For manual testing, I used Postman.
- For writing unit tests, I used XUnit and Moq.

## API Documentation
- I have used Swagger for documenting the API endpoints.

## Further improvements
- If I have more time, I would like to use stored procedures for the SQL code.
- Add logging in the database.

## Running the application
Server: `> dotnet run`

Endpoints:

1. `GET /products` - gets all products. 
2. `GET /products?name={name}` - finds all products matching the specified name. 
3. `GET /products/{id}` - gets the project that matches the specified ID. 
4. `POST /products` - creates a new product.
5. `PUT /products/{id}` - updates a product.
6. `DELETE /products/{id}` - deletes a product and its options. 

7. `GET /products/{id}/options` - finds all options for a specified product.  
8. `GET /products/{id}/options/{optionId}` - finds the specified product option for the specified product. 
9. `POST /products/{id}/options` - adds a new product option to the specified product. 
10. `PUT /products/{id}/options/{optionId}` - updates the specified product option.
11. `DELETE /products/{id}/options/{optionId}` - deletes the specified product option. 

**Product:**
```
{
  "Id": "01234567-89ab-cdef-0123-456789abcdef",
  "Name": "Product name",
  "Description": "Product description",
  "Price": 123.45,
  "DeliveryPrice": 12.34
}
```

**Products:**
```
{
  "Items": [
    {
      // product
    },
    {
      // product
    }
  ]
}
```

**Product Option:**
```
{
  "Id": "01234567-89ab-cdef-0123-456789abcdef",
  "Name": "Product name",
  "Description": "Product description"
}
```

**Product Options:**
```
{
  "Items": [
    {
      // product option
    },
    {
      // product option
    }
  ]
}
```
