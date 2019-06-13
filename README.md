porting from plain .net/mono version of https://github.com/tonyx/orderssystem to .net core
- sql is the same: you may take the .sql script from plain that version of orderssystem
- some bugs has been fixed
- localization can't work using xml based properties (becasue of problem of donet core with Fsharp.configuration). As workaround for now I have plain .fs source files for strings

info: a newer version of ordersystem .net/mono version was on my old dead laptop, so now, untill I get it from backup/time machine, I'll rewrite such features in this .net core version. I can't work easily on the .net/mono version from a cheap laptop someone lent me, so at the moment this is the official maintained version.

to run the project:
- make sure you have the postgres sql un and running, with a database named orderssystem, and a user "suave" with password '1234' with full access on it.
- load into the database the sql script you can find in https://github.com/tonyx/orderssystem.
- make sure that the Npgsql version 2.2.1 is available, and the the source Db.fs references it using absolute path.

launch dotnet run from console.
tested with dotnet core 2.2.3.

Issues: there is a mix of english italian localization (some strings are not localized, and hardwired in the code of the view).
The command for printing is unix based (lpr...) which will not work on windows.

 
