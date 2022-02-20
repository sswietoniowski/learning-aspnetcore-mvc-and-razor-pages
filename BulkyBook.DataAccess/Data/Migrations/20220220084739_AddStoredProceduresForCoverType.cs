using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyBook.DataAccess.Migrations
{
    public partial class AddStoredProceduresForCoverType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
@"
CREATE PROCEDURE dbo.usp_CoverTypes_SelectAll
AS
BEGIN
	SELECT Id, Name
	FROM dbo.CoverTypes;
END;
"
            );

            migrationBuilder.Sql(
@"
CREATE PROCEDURE dbo.usp_CoverTypes_Select
	@Id int
AS
BEGIN
	SELECT Id, Name
	FROM dbo.CoverTypes
	WHERE Id = @Id;
END;
"
            );

            migrationBuilder.Sql(
@"
CREATE PROCEDURE dbo.usp_CoverTypes_Insert
	@Name nvarchar(32)
AS
BEGIN
	INSERT INTO dbo.CoverTypes (Name)
	VALUES (@Name)
	SELECT SCOPE_IDENTITY();
END;
"
            );

            migrationBuilder.Sql(
@"
CREATE PROCEDURE dbo.usp_CoverTypes_Update
	@Id int,
	@Name nvarchar(32)
AS
BEGIN
	UPDATE dbo.CoverTypes
	SET Name = @Name
	WHERE Id = @Id;
END;
"
            );

            migrationBuilder.Sql(
@"
CREATE PROCEDURE dbo.usp_CoverTypes_Delete
	@Id int
AS
BEGIN
	DELETE FROM dbo.CoverTypes
	WHERE Id = @Id;
END;
"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE dbo.usp_CoverTypes_SelectAll");
            migrationBuilder.Sql(@"DROP PROCEDURE dbo.usp_CoverTypes_Select");
            migrationBuilder.Sql(@"DROP PROCEDURE dbo.usp_CoverTypes_Insert");
            migrationBuilder.Sql(@"DROP PROCEDURE dbo.usp_CoverTypes_Update");
            migrationBuilder.Sql(@"DROP PROCEDURE dbo.usp_CoverTypes_Delete");
        }
    }
}
