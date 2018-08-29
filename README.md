# vyvojari-sk
slovak community portal for developers

it all began here: http://vyvojari.sk/Forum/vyvojarisk-18180.aspx

Read our wiki in order to find out more: https://github.com/Liero/vyvojari-sk/wiki

or check latest (unstable) version: https://vyvojari-sk-test.azurewebsites.net/

## Build Status:

 dev: ![dev branch build badge](https://liero.visualstudio.com/_apis/public/build/definitions/74a5ef1e-20a5-4685-b777-a493748a0680/4/badge)
 
 master: ![master branch](https://liero.visualstudio.com/_apis/public/build/definitions/74a5ef1e-20a5-4685-b777-a493748a0680/3/badge)

 ## How to build and run?

  - Create database using Package Management Console

    ```
	Update-Database -Context EventsDbContext
	Update-Database -Context ApplicationDbContext
	Update-Database -Context DevPortalDbContext
	```