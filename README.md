# Orders system
 
A system for ordering food and drink in restaurants, pubs, etc...
 
## Getting Started
 
These instructions will get you a copy of the project up and running.
 
Typical users are restaurants, pubs, bars that need to automate the process of collecting orders, printing orders, and delivering fiscal receipts.
 
## General description.
 
This program is a web-based application accessible from any device enabled to wifi access and web browsing.
Users of the program can be employees of a restaurant like waiters, managers, cookers.
Also customers, with some limitations and a quick mechanism for authorization, can use the app to make orders by themselves.
The server can access printers and, using a specific proprietary gateway, a cash drawer to provide a valid fiscal receipt.
 
There is an interface for power users allowing them to manage the menu, the ingredients, the composition of the dishes, the prices of the dishes, and, eventually, the prices to be associated with the single ingredient variations like adding of removing them from a dish.
There is an interface for ordinary users (waiters or, for limited use, customers) to make and monitor orders.
 
## Ares that can be improved
 
- optimize web interface for more devices
- optimize the order printing
- internal code optimization and refactoring
 
## Nice to have for the future
 
- a cloud version
 
## Main features:
1) managing ingredients, and categories of ingredients
2) managing dishes and categories
3) managing ingredients composing the dishes (receipts)
4) collecting orders,
5) defining users roles,
6) managing variations (adding or removing single ingredients)
7) managing price variations related to ingredient variations
8) printing orders by associating any specific printer to any specific category (example: drinks, pizza, kitchen, desserts)
9) set and view the state of orders
10) managing payment by eventually subdividing the bill
11) print a fiscal cash drawer (by using external software as a gateway).
 
 
 
### Prerequisites
 
 
```
Os: Windows 7 or higher, Mac OS (unspecified version), should also work on Linux (unspecified version)
 
dotnet core version 2.2 or higher
Postgres version 9.6 or higher
```
 
### Installing
 
1) Clone the project 
```
git clone git@github.com:tonyx/orderssystem_core.git
```
 
 
2) install a specific version old version of Npgsql (that will be used only by the Ide, you may have to create the packages directory before doing it)
```
nuget install Npgsql -version 2.2.1 -Source %cd% -OutputDirectory packages 
```
or
```
nuget install Npgsql -version 2.2.1 -Source `pwd` -OutputDirectory packages 
```
 3) In th Db.fs file you may want to assign the resPath variable to the full path of the net45 subfolder of Npgsql.2.2.1 containing the Npgsql.dll (i.e. somepath/Npgsql.2.2.1/lib/net45) 
This will help resolving the db-entity binding at compile-time, and will also help editing with ionide.
 
 
 
5) access to sql by psql command, and create a suave user:
```   
create user suave with encrypted password '1234'
```
6) still in pgsql, create the database orderssystem
```
create database orderssystem
```
7) in pgsql, grant all privileges to suave user
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
 
note: for the fiscal receipt, an ecr.txt file is produced in a format suitable for an external proprietary program that watches the dir and interprets the ecr file, sending the command to the fiscal drawer.  I don't own such a program (ecr) so I can't redistribute it. A solution could be rewriting the command for the fiscal receipt using pos API. More info about later.
 
 
 
### Authors
 
* **Antonio (Tonino) Lucca** - [Tonyx](https://github.com/tonyx)
 
 
## License
 
This project is licensed under the Gpl3 License - see the [LICENSE.md](LICENSE.md) file for details
 
 
 
 

