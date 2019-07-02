## Database setup

In **appsettings.json** modify the connection string:

```

"DefaultConnection": "Server=**********\\SQLEXPRESS;Database=POAMDb;Trusted_Connection=True;MultipleActiveResultSets=true"

```

Also in the **POAMDbContext.cs** file:

```

optionsBuilder.UseSqlServer("Server=**********\\SQLEXPRESS;Database=POAMDb;Trusted_Connection=True;MultipleActiveResultSets=true");

```

Powershell (to add a new migration):

```

add-migration migrationName
update-database

```

If necessary, remove the latest migration with:

```

remove-migration

```

