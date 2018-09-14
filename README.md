# vyvojari-sk
slovak community portal for developers

it all began here: http://vyvojari.sk/Forum/vyvojarisk-18180.aspx

Read our wiki in order to find out more: https://github.com/Liero/vyvojari-sk/wiki

or check latest (unstable) version: https://vyvojari.azurewebsites.net/

## Build Status:
 
 master: ![master branch]([![Build Status](https://vyvojari.visualstudio.com/vyvojari/_apis/build/status/vyvojari%20-%20CI)](https://vyvojari.visualstudio.com/vyvojari/_build/latest?definitionId=1))

 ## How to build and run?

  - Create database using Package Management Console or use InMemory database provider

    ```
	Update-Database -Context EventsDbContext
	Update-Database -Context ApplicationDbContext
	Update-Database -Context DevPortalDbContext
	```
	or
	```
	dotnet ef database update --context DevPortalDbContext
	...
	```
	or just configure UseInMemoryDatabase with limited functionality.
