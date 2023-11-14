# Vital

# This is ReadMe how to spin up the API

To run the API, you need to run the command:
```bash
docker run --name vital -p 5432:5432 -e POSTGRES_PASSWORD=password -e POSTGRES_USER=root -d postgres:14
```

## Database

### Seed Database

We are using EF Core for working with the database.  
Seeding of the database will remove the database and create a new one, this will ensure that you have the latest version of the database.  
**To seed the database with data use the command:**
```bash
dotnet watch run --db-init
```
