# Orders system

A system for ordering food and drink in restaurant, pub etc...

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.


Typical use is restaurants collecting orders, printing orders where needed, and deliverying the fiscal receipts.


## main features:
1) managing ingredients, and categories of ingredients
2) managing courses, and categories
3) managing ingredients composing the courses (receipts)
4) collecting orders,
5) defining users roles,
6) managing variations of orders items (in term of add ons or drop off of ingredients)
7) managing price variations related to order items variations
8) printing orders by associating printer for course categories
9) displayng order items 
10) managing payment by eventually subdividing the bill
11) print a fiscal cash drawer (by using an external software as a gateway).



### Prerequisites


```
Os: Windows 7 or higher, Mac OS (unspecified version), should also work on Linux (unspecified version)

dotnet core version 2.2 or higher
postgres version 9.6 or higher
```

### Installing

1) Clone the project 
```
git clone git@github.com:tonyx/orderssystem_core.git
```


2) install a specific version old version of Npgsql (that will be use only by the Ide, you may have to create the packages directory before doing it)
```
nuget install Npgsql -version 2.2.1 -Source %cd% -OutputDirectory packages 
```
or
```
nuget install Npgsql -version 2.2.1 -Source `pwd` -OutputDirectory packages 
```
 3) In th Db.fs file you may want to assign the resPath variable to the full path of the net45 subfolder of Npgsql.2.2.1 containing the Npgsql.dll (i.e. somepath/Npgsql.2.2.1/lib/net45) 
This will help resolving the db-entity binding at compile time, and will also help editing with ionide.



5) access to sql by psql command, and create a suave user:
```   
create user suave with encrypted password '1234'
```
6) still in pgsql, create the database orderssystem
```
create database orderssystem
```
7) in pgsql, grant all priviledges to suave user
```
grant all privileges on database orderssystem to suave
```
9) load the orders_system.sql schema and data to orderssystem database, you may run the psql again form the command prompt:
```
psql -d ordersystem -f orders_system.sql
```

10) In the App.fs source file, change the following line according to the path to the root of the application
```
      homeFolder= Some @"C:\Users\username\gitprojects\orderssystem_core"
```

Now you should be able to type the commands

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



