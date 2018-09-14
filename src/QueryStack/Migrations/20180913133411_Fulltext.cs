using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using System;

namespace DevPortal.QueryStack.Migrations
{
    /// <summary>
    /// this will not work in (localdb), but should work with SQL Express with advanced services
    /// </summary>
    public partial class Fulltext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //this should be supported out of the box in EF Core vNext (probably 2.2)
            migrationBuilder.Sql(@"
                IF (FULLTEXTSERVICEPROPERTY('IsFullTextInstalled') = 1)
                    IF EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE [name] = 'ft')
	                    PRINT 'FullText Catalog ft lready exists'
                    ELSE CREATE FULLTEXT CATALOG ft AS DEFAULT",
                suppressTransaction: true);

            migrationBuilder.Sql(@"
                IF (FULLTEXTSERVICEPROPERTY('IsFullTextInstalled') = 1)
                    IF (COLUMNPROPERTY(OBJECT_ID('ContentBase'), 'Content', 'IsFulltextIndexed') = 1)
                        PRINT 'FullText Indext ON dbo.ContentBase(Content) already exists'
                    ELSE CREATE FULLTEXT INDEX ON dbo.ContentBase(Content) KEY INDEX [PK_ContentBase] WITH STOPLIST = SYSTEM",
                suppressTransaction: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF (COLUMNPROPERTY(OBJECT_ID('ContentBase'), 'Content', 'IsFulltextIndexed') = 1)
                    DROP FULLTEXT INDEX ON ContentBase",
                suppressTransaction: true);

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE [name] = 'ft')
                    DROP FULLTEXT CATALOG ft",
                suppressTransaction: true);
        }
    }
}
