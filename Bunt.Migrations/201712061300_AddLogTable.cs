using FluentMigrator;

namespace Bunt.Migrations
{
    [Migration(201712061300)]
    public class AddRequestLogTable : Migration
    {   
        public override void Up()
        {
            Execute.Sql(@"
                CREATE TABLE [Log] (
                   [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
                   [Message] nvarchar(max) NULL,
                   [MessageTemplate] nvarchar(max) NULL,
                   [Level] nvarchar(128) NULL,
                   [TimeStamp] datetimeoffset(7) NOT NULL,  -- use datetime for SQL Server pre-2008
                   [Exception] nvarchar(max) NULL,
                   [Properties] xml NULL,
                   [LogEvent] nvarchar(max) NULL            
                )
            ");
        }

        public override void Down()
        {
            Delete.Table("Log");
        }
    }
}