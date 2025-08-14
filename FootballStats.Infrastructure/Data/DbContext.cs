using System.Data;
using ServiceStack.OrmLite;

namespace FootballStats.Infrastructure.Data;

public class DbContext
{ 
    private readonly OrmLiteConnectionFactory _dbFactory;
    
    public DbContext(string connectionString) 
    { 
        _dbFactory = new OrmLiteConnectionFactory(connectionString, SqlServerDialect.Provider);
    }

    public IDbConnection Open()
    {
        return _dbFactory.Open();
    }
}