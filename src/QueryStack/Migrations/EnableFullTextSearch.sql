-- adding this to EF Migrations caused publis to fail, so it has to be executed manually or as part of CI/CD pipeline
CREATE FULLTEXT CATALOG ft AS DEFAULT
GO
CREATE FULLTEXT INDEX ON dbo.ContentBase(Content, Title, Description) KEY INDEX [PK_ContentBase] WITH STOPLIST = SYSTEM
GO