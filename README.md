# Orders system

A system for ordering food and drink in restaurant, pub etc...

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

What things you need to install the software and how to install them

```
dotnet core version 2.2 or higher
postgres version 9.6 or higher
```

### Installing

create a database postgres named orderssystem and create a user names suave with encripted password '1234'
after you downloaded all the package, download separately using nuget
the package Npgsql version 2.2.1 creating a path like packages\Npgsql.2.2.1\lib\..
edit the in the Db.fs the resPath variable according to the previous path, otherwise it won't compile
edit the App.fs file and change the home field of the cfg record according to path of sources, otherwise it will not access the css and html liquid templates files

Now you can type the commands

```
	dotnet restore
	dotnet build
	dotnet run
```



logon to localhost to port 8083 using administrator/administrator as login and password


Issues: css is sensible to the browser window size but is far from being optimal, 
the printing on windows has not been tested

note: for the fiscal receipt, a ecr.txt file is produced in a format suitable for an external proprietary program which watch the dir and interpret the ecr file, sending the command to the fiscal drawer.  I don't own such program (ecr) so I can't redistribute it. A solution could be rewriting the command for the fiscal receipt using pos api. More info about later.
 


### Authors

* **Antonio (Tonino) Lucca** - [Tonyx](https://github.com/tonyx)
 

## License

This project is licensed under the Gpl3 License - see the [LICENSE.md](LICENSE.md) file for details



