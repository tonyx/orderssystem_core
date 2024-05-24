# Orders system
 
A system for ordering food and drink in restaurants, pubs, etc...
 
## Getting Started
 
These instructions will show you how to get you a copy of the project up and running.
 
Typical users can be managers, employees and customers of restaurants, pubs, bars that need to automate the process of collecting orders, printing orders, and delivering fiscal receipts.

## Important note:
At the moment Npgsql library and Sqlprovider must stay pinned to version 6.0.11 and 1.1.65.
For security reason only trusted users must be allowed.
 
## General description.
 
This program is a web-based application accessible from any device enabled for wifi access and web browsing.
Also customers, with some limitations and a quick mechanism for authorization, can use the app to make orders by themselves.
The server can access to printers and, using a specific proprietary gateway, a cash drawer to provide a valid fiscal receipt.
There is an interface for power users allowing them to manage the menu, the ingredients, the composition of the dishes, the prices of the dishes, and, eventually, the prices to be associated with the single ingredient variations like adding of removing them from a dish.
There is an interface for ordinary users (waiters or, for limited use, customers) to make and monitor orders.
 
## Ares that can be improved
 
- optimize web interface for more devices
- optimize the order printing
- internal code optimization and refactoring
 
## Nice to have for the future
 
- a cloud version
 
## Main features:
1) managing ingredients and categories of ingredients
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
 
2) access to sql by psql command, and create a suave user:
```   
create user suave with encrypted password '1234'
```
3) still in pgsql, create the database orderssystem
```
create database orderssystem
```
4) in pgsql, grant all privileges to suave user
```
grant all privileges on database orderssystem to suave
```
5) load the orders_system.sql schema and data to orderssystem database, you may run the psql again form the command prompt:
```
psql -d ordersystem -f orders_system.sql
```
 
6) In the App.fs source file, change the following line according to the path to the root of the application
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
 

## Intro video in **Italian Language**
* [Short presentation. less than 3. mins](https://youtu.be/3K-ohMztd2g)

* [Presentation with walk-through 14:15 mins](https://youtu.be/QiVbJZBl2Lc)
 
### Authors
 
* **Antonio (Tonino) Lucca** - [Tonyx](https://github.com/tonyx)
 
 
## License
 
This project is licensed under the Gpl3 License - see the [LICENSE.md](LICENSE.md) file for details
 
 
 
 

