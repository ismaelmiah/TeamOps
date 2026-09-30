# TeamOps
A SaaS application where different companies/organisations can manage their teams, projects, tasks, and users.

### Test database migration command
```
dotnet ef database update   --project src/TeamOps.Infrastructure   --startup-project src/TeamOps.Api --connection="Host=localhost;Port=5432;Database=teamops_test;Username=postgres;Password=mysecretpassword";
```

# Original database migration command

```
dotnet ef database update   --project src/TeamOps.Infrastructure   --startup-project src/TeamOps.Api 
```