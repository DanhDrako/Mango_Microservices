# Here is resource for Restore Solution.

> [!NOTE]
> For BE

```bash
-cd API:
nuget:
	<PackageReference Include="AutoMapper" Version="14.0.0" />
	<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.13" />
	<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.17" />
	<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.5" />
	<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.5" />
	<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.5">
		<PrivateAssets>all</PrivateAssets>
		<IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
	</PackageReference>
	<PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />

-cmd of API:
-- dotnet tool install --global dotnet-ef --version 9.0.4
-- dotnet ef
-- dotnet ef migrations add InitialCreate -o Data/Migrations
-- dotnet ef database update

-want to delete migrations added ?
-- dotnet ef migrations remove

-want to restore all database?
-- dotnet ef database drop

-update latest version of ef
-- dotnet tool update --global dotnet-ef
```
