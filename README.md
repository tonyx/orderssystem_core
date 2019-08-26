

porting from plain .net/mono version of https://github.com/tonyx/orderssystem to .net core with new features

create a database postgres named orderssystem
create a user names suave with encripted password '1234'
execute the sql script orders_system.sql
make sure you got (using nuget) the following directory structure  packages\Npgsql.2.2.1\lib\
edit the in the Db.fs the resPath variable according to the previous path

type 
	dotnet restore
	dotnet build
	dotnet run

logon to localhost to port 8083 using administrator/administrator as login and password


Issues: css is sensible to the browser window size but is far from being optimal
the printing on windows has not been tested

note: for the fiscal recaipt, a .txt file is produced in a format suitable for an external proprietary program that I don't own (ecr). More info about later.
 
