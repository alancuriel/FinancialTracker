# Financial Tracker

## Back End
### Run Api
To run api there are two secrets needed.
The <b>Mongo DB Connection String</b> and <b>JWT secret key</b>.

Run the following commands with their needed value to add secrets.
```
dotnet user-secrets set "JWT_SECRET_KEY" "<SECRET_KEY>" 
dotnet user-secrets set "MONGO_CONN_NAME" "<MONGO_DB_CONNECTION_STRING>"
```

## Front End