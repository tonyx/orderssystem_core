namespace OrdersSystem.Contexts.Restaurant
open FSharpPlus
open OrdersSystem.Commons
open OrdersSystem.Models.Dish
open FsToolkit.ErrorHandling
open OrdersSystem.Models.Ingredient
open Sharpino.Definitions
open Sharpino.Utils
open Sharpino
open System

type Printer  =
    {
        Id: Guid
        Name: string
        Types: List<DishTypes>
    }

module Restaurant =
    type Restaurant 
        (dishReferences: List<Guid>, 
        ingredientReferences: List<Guid>, 
        tablesReferences: List<Guid>, 
        ordersReferences: List<Guid>,
        usersReferences: List<Guid>, 
        printers: List<Printer>,
        rolesReferences: List<Guid>,
        orderItemReferences: List<Guid>,
        ingredientTypes: List<IngredientType>) =

        member this.DishRefs = dishReferences
        member this.IngredientRefs = ingredientReferences
        member this.TableRefs = tablesReferences
        member this.OrderRefs = ordersReferences
        member this.UsersRefs = usersReferences
        member this.Printers = printers
        member this.RoleRefs = rolesReferences
        member this.OrderItemRefs = orderItemReferences
        member this.IngredientTypes = ingredientTypes

        static member Zero =
            Restaurant ([], [], [], [], [], [], [], [], [])
        member this.AddDishRef (id: Guid) =
            result {
                let! notAlreadyExists =
                    this.DishRefs
                    |> List.contains id
                    |> not
                    |> Result.ofBool (sprintf "A dish with id '%A' already exists" id)    
                return 
                    Restaurant (id :: this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes)
            }
        member this.AddIngredientRef (id: Guid) =
            result {
                let! notAlreadyExists =
                    this.IngredientRefs
                    |> List.contains id
                    |> not
                    |> Result.ofBool (sprintf "An ingredient with id '%A' already exists" id)
                return Restaurant (this.DishRefs, id :: this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes)
            }
        member this.RemoveIngredientRef (id: Guid) =
            result {
                let! chckExists =
                    this.IngredientRefs
                    |> List.contains id
                    |> Result.ofBool (sprintf "An ingredient with id '%A' does not exist" id)
                return Restaurant (this.DishRefs, this.IngredientRefs |> List.filter (fun  x -> x <> id), this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes)
            }

        member this.RemoveDishRef (id: Guid) =
            result {
                let! chckExists =
                    this.DishRefs
                    |> List.contains id
                    |> Result.ofBool (sprintf "A dish with id '%A' does not exist" id)
                return Restaurant (this.DishRefs |> List.filter (fun  x -> x <> id), this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes)
            }
        member this.AddPrinter (printer: Printer) =
            result {
                let! notAlreadyExists =
                    this.Printers
                    |> List.exists (fun x -> x.Name = printer.Name)
                    |> not
                    |> Result.ofBool (sprintf "A printer with name '%s' already exists" printer.Name)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, printer :: this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes)
            }

        member this.RemovePrinter (name: string) =
            result {
                let! chckExists =
                    this.Printers
                    |> List.exists (fun x -> x.Name = name)
                    |> Result.ofBool (sprintf "A printer with name '%s' does not exist" name)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers |> List.filter (fun x -> x.Name <> name), this.RoleRefs, this.OrderItemRefs, this.IngredientTypes)
            }
        member this.UpdatePrinter (printer: Printer) =
            result {
                let! chckExists =
                    this.Printers
                    |> List.exists (fun x -> x.Name = printer.Name)
                    |> Result.ofBool (sprintf "A printer with name '%s' does not exist" printer.Name)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers |> List.map (fun x -> if x.Name = printer.Name then printer else x), this.RoleRefs, this.OrderItemRefs, this.IngredientTypes)
            }
        member this.GetAllPrinters ()  =
            this.Printers
            
        member this.AddTableRef (id: Guid) =
            result {
                let! notAlreadyExists =
                    this.TableRefs
                    |> List.contains id
                    |> not
                    |> Result.ofBool (sprintf "A table with id '%A' already exists" id)
                return Restaurant (this.DishRefs, this.IngredientRefs, id :: this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes)
            }
        member this.RemoveTableRef (id: Guid) =
            result {
                let! chckExists =
                    this.TableRefs
                    |> List.contains id
                    |> Result.ofBool (sprintf "A table with id '%A' does not exist" id)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs |> List.filter (fun  x -> x <> id), this.OrderRefs,  this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes)
            }
        member this.AddOrderRef (id: Guid) =
            result {
                let! notAlreadyExists =
                    this.OrderRefs
                    |> List.contains id
                    |> not
                    |> Result.ofBool (sprintf "An order with id '%A' already exists" id)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, id :: this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes)
            }
        member this.RemoveOrderRef (id: Guid) =
            result {
                let! checkExists =
                    this.OrderRefs
                    |> List.contains id
                    |> Result.ofBool (sprintf "An order with id '%A' does not exist" id)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs |> List.filter (fun  x -> x <> id), this.UsersRefs, this.Printers, this.RoleRefs , this.OrderItemRefs, this.IngredientTypes)
            }
        member this.AddUserRef (id: Guid) =
            result {
                let! notAlreadyExists =
                    this.UsersRefs
                    |> List.contains id
                    |> not
                    |> Result.ofBool (sprintf "A user with id '%A' already exists" id)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, id :: this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes)
            }

        member this.RemoveUserRef (id: Guid) =
            result {
                let! checkExists =
                    this.UsersRefs
                    |> List.contains id
                    |> Result.ofBool (sprintf "A user with id '%A' does not exist" id)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs |> List.filter (fun  x -> x <> id), this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes)
            }
        member this.AddRoleRef (id: Guid) =
            result {
                let! notAlreadyExists =
                    this.RoleRefs
                    |> List.contains id
                    |> not
                    |> Result.ofBool (sprintf "A role with id '%A' already exists" id)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, id :: this.RoleRefs, this.OrderItemRefs, this.IngredientTypes)
            }

        member this.RemoveRoleRef (id: Guid) =
            result {
                let! checkExists =
                    this.RoleRefs
                    |> List.contains id
                    |> Result.ofBool (sprintf "A role with id '%A' does not exist" id)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs |> List.filter (fun  x -> x <> id), this.OrderItemRefs, this.IngredientTypes)
            }
        member this.AddOrderItemRef (id: Guid) =
            result {
                let! notAlreadyExists =
                    this.OrderItemRefs
                    |> List.contains id
                    |> not
                    |> Result.ofBool (sprintf "An order item with id '%A' already exists" id)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, id :: this.OrderItemRefs, this.IngredientTypes)
            }
        member this.RemoveOrderItemRef (id: Guid) =
            result {
                let! checkExists =
                    this.OrderItemRefs
                    |> List.contains id
                    |> Result.ofBool (sprintf "An order item with id '%A' does not exist" id)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs |> List.filter (fun  x -> x <> id), this.IngredientTypes)
            }
        member this.AddIngredientType  (ingredientType: IngredientType) =
            result {
                let! notAlreadyExists =
                    this.IngredientTypes
                    |> List.exists (fun x -> x.Name = ingredientType.Name)
                    |> not
                    |> Result.ofBool (sprintf "An ingredient type with name '%s' already exists" ingredientType.Name)
                let! idNotUsed =
                    this.IngredientTypes
                    |> List.exists (fun x -> x.Id = ingredientType.Id)
                    |> not
                    |> Result.ofBool (sprintf "An ingredient type with id '%A' already exists" ingredientType.Id)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, ingredientType :: this.IngredientTypes)
            }
        member this.GetIngredientType (name: string) =
            this.IngredientTypes
            |> List.tryFind (fun x -> x.Name = name)
            |> Result.ofOption (sprintf "An ingredient type with name '%s' does not exist" name)
         
        member this.GetIngredientType (id: Guid) =
            this.IngredientTypes
            |> List.tryFind (fun x -> x.Id = id)
            |> Result.ofOption (sprintf "An ingredient type with id '%A' does not exist" id)
        
        static member StorageName =
            "_restaurant"
        static member Version =
            "_01"
        static member SnapshotsInterval =
            15
        static member Deserialize json =
            globalSerializer.Deserialize<Restaurant> json
        member this.Serialize  =
            this
            |> globalSerializer.Serialize



