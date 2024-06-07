namespace OrdersSystem.Contexts.Restaurant
open FSharpPlus
open OrdersSystem.Commons
open OrdersSystem.Shared
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
        Types: List<Guid>
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
        ingredientTypes: List<IngredientType>,
        dishTypes: List<DishType>,
        standardComments: List<StandardComment>,
        standardVariations: List<StandardVariation>
        ) =
        
        member this.DishRefs = dishReferences
        member this.IngredientRefs = ingredientReferences
        member this.TableRefs = tablesReferences
        member this.OrderRefs = ordersReferences
        member this.UsersRefs = usersReferences
        member this.Printers = printers
        member this.RoleRefs = rolesReferences
        member this.OrderItemRefs = orderItemReferences
        member this.IngredientTypes = ingredientTypes
        member this.DishTypes = dishTypes
        member this.StandardComments = standardComments
        member this.StandardVariations = standardVariations

        static member Zero =
            Restaurant ([], [], [], [], [], [], [], [], [], [], [], [])
        member this.AddDishRef (id: Guid) =
            result {
                let! notAlreadyExists =
                    this.DishRefs
                    |> List.contains id
                    |> not
                    |> Result.ofBool (sprintf "A dish with id '%A' already exists" id)    
                return 
                    Restaurant (id :: this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes, this.DishTypes, this.StandardComments, this.StandardVariations)
            }
        member this.AddIngredientRef (id: Guid) =
            result {
                let! notAlreadyExists =
                    this.IngredientRefs
                    |> List.contains id
                    |> not
                    |> Result.ofBool (sprintf "An ingredient with id '%A' already exists" id)
                return Restaurant (this.DishRefs, id :: this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes, this.DishTypes, this.StandardComments, this.StandardVariations)
            }
        member this.RemoveIngredientRef (id: Guid) =
            result {
                let! chckExists =
                    this.IngredientRefs
                    |> List.contains id
                    |> Result.ofBool (sprintf "An ingredient with id '%A' does not exist" id)
                return Restaurant (this.DishRefs, this.IngredientRefs |> List.filter (fun  x -> x <> id), this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes, this.DishTypes, this.StandardComments, this.StandardVariations)
            }

        member this.RemoveDishRef (id: Guid) =
            result {
                let! chckExists =
                    this.DishRefs
                    |> List.contains id
                    |> Result.ofBool (sprintf "A dish with id '%A' does not exist" id)
                return Restaurant (this.DishRefs |> List.filter (fun  x -> x <> id), this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes, this.DishTypes, this.StandardComments, this.StandardVariations)
            }
        member this.AddPrinter (printer: Printer) =
            result {
                let! notAlreadyExists =
                    this.Printers
                    |> List.exists (fun x -> x.Name = printer.Name)
                    |> not
                    |> Result.ofBool (sprintf "A printer with name '%s' already exists" printer.Name)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, printer :: this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes, this.DishTypes, this.StandardComments, this.StandardVariations)
            }

        member this.RemovePrinter (name: string) =
            result {
                let! chckExists =
                    this.Printers
                    |> List.exists (fun x -> x.Name = name)
                    |> Result.ofBool (sprintf "A printer with name '%s' does not exist" name)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers |> List.filter (fun x -> x.Name <> name), this.RoleRefs, this.OrderItemRefs, this.IngredientTypes, this.DishTypes, this.StandardComments, this.StandardVariations)
            }
        member this.UpdatePrinter (printer: Printer) =
            result {
                let! chckExists =
                    this.Printers
                    |> List.exists (fun x -> x.Name = printer.Name)
                    |> Result.ofBool (sprintf "A printer with name '%s' does not exist" printer.Name)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers |> List.map (fun x -> if x.Name = printer.Name then printer else x), this.RoleRefs, this.OrderItemRefs, this.IngredientTypes, this.DishTypes, this.StandardComments, this.StandardVariations)
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
                return Restaurant (this.DishRefs, this.IngredientRefs, id :: this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes, this.DishTypes, this.StandardComments, this.StandardVariations)
            }
        member this.RemoveTableRef (id: Guid) =
            result {
                let! chckExists =
                    this.TableRefs
                    |> List.contains id
                    |> Result.ofBool (sprintf "A table with id '%A' does not exist" id)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs |> List.filter (fun  x -> x <> id), this.OrderRefs,  this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes, this.DishTypes, this.StandardComments, this.StandardVariations)
            }
        member this.AddOrderRef (id: Guid) =
            result {
                let! notAlreadyExists =
                    this.OrderRefs
                    |> List.contains id
                    |> not
                    |> Result.ofBool (sprintf "An order with id '%A' already exists" id)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, id :: this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes, this.DishTypes, this.StandardComments, this.StandardVariations)
            }
        member this.RemoveOrderRef (id: Guid) =
            result {
                let! checkExists =
                    this.OrderRefs
                    |> List.contains id
                    |> Result.ofBool (sprintf "An order with id '%A' does not exist" id)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs |> List.filter (fun  x -> x <> id), this.UsersRefs, this.Printers, this.RoleRefs , this.OrderItemRefs, this.IngredientTypes, this.DishTypes, this.StandardComments, this.StandardVariations)
            }
        member this.AddUserRef (id: Guid) =
            result {
                let! notAlreadyExists =
                    this.UsersRefs
                    |> List.contains id
                    |> not
                    |> Result.ofBool (sprintf "A user with id '%A' already exists" id)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, id :: this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes, this.DishTypes, this.StandardComments, this.StandardVariations)
            }

        member this.RemoveUserRef (id: Guid) =
            result {
                let! checkExists =
                    this.UsersRefs
                    |> List.contains id
                    |> Result.ofBool (sprintf "A user with id '%A' does not exist" id)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs |> List.filter (fun  x -> x <> id), this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes, this.DishTypes, this.StandardComments, this.StandardVariations)
            }
        member this.AddRoleRef (id: Guid) =
            result {
                let! notAlreadyExists =
                    this.RoleRefs
                    |> List.contains id
                    |> not
                    |> Result.ofBool (sprintf "A role with id '%A' already exists" id)
                return Restaurant
                    (this.DishRefs,
                     this.IngredientRefs,
                     this.TableRefs, 
                     this.OrderRefs,
                     this.UsersRefs,
                     this.Printers,
                     id :: this.RoleRefs,
                     this.OrderItemRefs,
                     this.IngredientTypes,
                     this.DishTypes, 
                     this.StandardComments,
                     this.StandardVariations)
            }

        member this.RemoveRoleRef (id: Guid) =
            result {
                let! checkExists =
                    this.RoleRefs
                    |> List.contains id
                    |> Result.ofBool (sprintf "A role with id '%A' does not exist" id)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs |> List.filter (fun  x -> x <> id), this.OrderItemRefs, this.IngredientTypes, this.DishTypes, this.StandardComments, this.StandardVariations)
            }
        member this.AddOrderItemRef (id: Guid) =
            result {
                let! notAlreadyExists =
                    this.OrderItemRefs
                    |> List.contains id
                    |> not
                    |> Result.ofBool (sprintf "An order item with id '%A' already exists" id)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, id :: this.OrderItemRefs, this.IngredientTypes, this.DishTypes, this.StandardComments, this.StandardVariations)
            }
        member this.RemoveOrderItemRef (id: Guid) =
            result {
                let! checkExists =
                    this.OrderItemRefs
                    |> List.contains id
                    |> Result.ofBool (sprintf "An order item with id '%A' does not exist" id)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs |> List.filter (fun  x -> x <> id), this.IngredientTypes, this.DishTypes, this.StandardComments, this.StandardVariations)
            }
            
        member this.AddStandardComment (text: string) =
            result {
                do!
                    text
                    |> String.IsNullOrWhiteSpace
                    |> not
                    |> Result.ofBool "Text cannot be empty"
                do!
                    this.StandardComments
                    |> List.exists (fun x -> x.Text = text)
                    |> not
                    |> Result.ofBool "Standard comment already exists"
                let id = Guid.NewGuid()
                let standardComment = {CommentId = id; Text = text}
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes, this.DishTypes, standardComment :: this.StandardComments, this.StandardVariations)
            }
            
        member this.AddStandardVariation (standardVariation:  StandardVariation) =
            result {
                do! 
                    this.StandardVariations
                    |> List.exists (fun x -> x.Id = standardVariation.Id || x.Name = standardVariation.Name)
                    |> not
                    |> Result.ofBool (sprintf "A standard variation with id '%A' or name '%A' already exists" standardVariation.Id standardVariation.Name)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes, this.DishTypes, this.StandardComments, standardVariation :: this.StandardVariations)     
            }
            
        member this.RemoveStandardVariation (id: Guid) =
            result {
                let! chckExists =
                    this.StandardVariations
                    |> List.exists (fun x -> x.Id = id)
                    |> Result.ofBool (sprintf "A standard variation with id '%A' does not exist" id)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes, this.DishTypes, this.StandardComments, this.StandardVariations |> List.filter (fun x -> x.Id <> id))
            }    
      
        member this.UpdateStandardVariation (standardVariation: StandardVariation)  =
            result {
                do! 
                    this.StandardVariations
                    |> List.exists (fun x -> x.Id = standardVariation.Id)
                    |> Result.ofBool (sprintf "A standard variation with id '%A' does not exist" standardVariation.Id)
                return Restaurant(this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes, this.DishTypes, this.StandardComments, this.StandardVariations |> List.map (fun x -> if x.Id = standardVariation.Id then standardVariation else x))    
            }
         
        member this.UpdateStandardComment (standardComment: StandardComment) =
            result {
                let! chckExists =
                    this.StandardComments
                    |> List.tryFind (fun x -> x.CommentId = standardComment.CommentId)
                    |> Result.ofOption (sprintf "A standard comment with id '%A' does not exist" standardComment.CommentId)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes, this.DishTypes, this.StandardComments |> List.map (fun x -> if x.CommentId = standardComment.CommentId then standardComment else x), this.StandardVariations)    
            }
            
        member this.GetStandardComments () =
            this.StandardComments
        
        member this.RemoveStandardComment (id: Guid) =
            result {
                let! chckExists =
                    this.StandardComments
                    |> List.exists (fun x -> x.CommentId = id)
                    |> Result.ofBool (sprintf "A standard comment with id '%A' does not exist" id)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes, this.DishTypes, this.StandardComments |> List.filter (fun x -> x.CommentId <> id), this.StandardVariations)
            }    
        
        // ingredienttype     
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
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, ingredientType :: this.IngredientTypes, this.DishTypes, this.StandardComments, this.StandardVariations)
            }
            
        // dishtypes    
        member this.AddDishType (dishType: DishType) =
            result {
                let! notAlreadyExists =
                    this.DishTypes
                    |> List.exists (fun (x: DishType) -> x.Name = dishType.Name)
                    |> not
                    |> Result.ofBool (sprintf "A dish type with name '%s' already exists" dishType.Name)
                let! isNotUsed =
                    this.DishTypes
                    |> List.exists (fun (x: DishType) -> x.DishTypeId = dishType.DishTypeId)
                    |> not
                    |> Result.ofBool (sprintf "A dish type with id '%A' already exists" dishType.DishTypeId)
                return Restaurant (this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs, this.UsersRefs, this.Printers, this.RoleRefs, this.OrderItemRefs, this.IngredientTypes, dishType :: this.DishTypes, this.StandardComments, this.StandardVariations)    
            }
        member this.UpdateDishType (dishType: DishType) =
            result {
                let! chckExists =
                    this.DishTypes
                    |> List.exists (fun x -> x.DishTypeId = dishType.DishTypeId)
                    |> Result.ofBool (sprintf "A dish type with id '%A' does not exist" dishType.DishTypeId)
                return Restaurant
                           (this.DishRefs,
                            this.IngredientRefs,
                            this.TableRefs,
                            this.OrderRefs,
                            this.UsersRefs,
                            this.Printers,
                            this.RoleRefs,
                            this.OrderItemRefs,
                            this.IngredientTypes,
                            this.DishTypes |> List.map (fun x -> if x.DishTypeId = dishType.DishTypeId then dishType else x),
                            this.StandardComments,
                            this.StandardVariations
                           )
            }
            
        member this.GetDishType (name: string) =
            this.DishTypes
            |> List.tryFind (fun x -> x.Name = name)
            |> Result.ofOption (sprintf "A dish type with name '%s' does not exist" name)    
       
        member this.GetDishType (id: Guid) =
            this.DishTypes
            |> List.tryFind (fun x -> x.DishTypeId = id)
            |> Result.ofOption (sprintf "A dish type with id '%A' does not exist" id) 
       
        member this.FindDishType (name: string) =
            this.DishTypes
            |> List.tryFind (fun x -> x.Name.Contains name) 
            
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


