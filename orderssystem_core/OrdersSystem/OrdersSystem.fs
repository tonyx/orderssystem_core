namespace OrdersSystem
open OrdersSystem.Contexts.Restaurant
open OrdersSystem.Models
open Sharpino
open Sharpino.MemoryStorage
open Sharpino.Storage
open Sharpino.CommandHandler
open FSharpPlus
open FsToolkit.ErrorHandling
open OrdersSystem.Models.Dish
open OrdersSystem.Models.User
open OrdersSystem.Models.UserCommands
open OrdersSystem.Models.Ingredient
open OrdersSystem.Contexts.RestaurantCommands
open OrdersSystem.Models.Table
open OrdersSystem.Models.Role
open OrdersSystem.Models.RoleCommands
open OrdersSystem.Models.RoleEvents
open OrdersSystem.Models.OrderItem
open OrdersSystem.Contexts.Restaurant.Restaurant
open OrdersSystem.Contexts.RestaurantEvents
open OrdersSystem.Models.User
open OrdersSystem.Models.Order
open OrdersSystem.Shared

open System

module OrdersSystem =
    open Sharpino.PgStorage
    open OrdersSystem.Models.IngredientEvents
    open OrdersSystem.Models.DishEvents
    open OrdersSystem.Models.TableEvents
    open OrdersSystem.Models.UserEvents
    open OrdersSystem.Models.OrderEvents
    open OrdersSystem.Models.OrderItemEvents
    open OrdersSystem.Models.IngredientCommands
    open OrdersSystem.Models.DishCommands
    let pgEventStoreconnection =
        "Server=127.0.0.1;"+
        "Database=orderssystem_02;" +
        "User Id=safe;"+
        "Password=safe;"
        
    let pgEventStore:IEventStore<string> = PgEventStore(pgEventStoreconnection)
    // let pgEventStore:IEventStore<string> = MemoryStorage()
    let memEventStore = MemoryStorage()

    let doNothingBroker: IEventBroker<_> =
        {
            notify = None
            notifyAggregate = None
        }

    let storageDishViewer = getAggregateStorageFreshStateViewer<Dish, DishEvents, string>  pgEventStore
    let storageIngredientViewer = getAggregateStorageFreshStateViewer<Ingredient, IngredientEvents, string>  pgEventStore
    let storageOrdersViewer = getAggregateStorageFreshStateViewer<Order, OrderEvents, string>  pgEventStore
    let storageRoleViewer = getAggregateStorageFreshStateViewer<Role, RoleEvents, string>  pgEventStore
    let storageTablesViewer = getAggregateStorageFreshStateViewer<Table, TableEvents, string>  pgEventStore
    let storageUsersViewer = getAggregateStorageFreshStateViewer<User, UserEvents, string>  pgEventStore
    let storageOrderItemViewer = getAggregateStorageFreshStateViewer<OrderItem, OrderItemEvents, string>  pgEventStore
    let storageRestaurantViewer = getStorageFreshStateViewer<Restaurant, RestaurantEvents, string>  pgEventStore


    type OrdersSystem 
        (eventStore: IEventStore<string>, 
        eventBroker: IEventBroker<string>,
        dishesViewer: AggregateViewer<Dish>,
        ingredientViewer: AggregateViewer<Ingredient>,
        tablesViewer: AggregateViewer<Table>,   
        usersViewer: AggregateViewer<User>,
        ordersViewer: AggregateViewer<Order>,
        orderItemsViewer: AggregateViewer<OrderItem>,
        restaurantViewer: StateViewer<Restaurant>
        ) =

        let createAdminIfNotExists =
            
            let res =
                result {
                    let! (_, restaurant) = restaurantViewer ()
                    let existingUsers = restaurant.UsersRefs
                    let! allUsers = 
                        existingUsers
                        |> List.traverseResultM (fun id -> usersViewer id)
                    let users = allUsers |>> snd    
                    let userExists =
                        users.Length > 0 && users |> List.exists (fun x -> x.Username = "administrator")
                    if not userExists then
                        let! firstAdministrator = User.MkUser ("administrator", "administrator", "admin")
                        let result =
                            (RestaurantCommands.AddUserRef firstAdministrator.Id)
                            |> runInitAndCommand<Restaurant, RestaurantEvents, User, string>  eventStore eventBroker firstAdministrator
                        printf "Administrator created %A\n" result
                        // return result
                        return ()
                    else
                        return ()
                }
            match res with
            | Error e -> failwith e
            | Ok _ -> ()
        
        new () =
            OrdersSystem
                (pgEventStore, 
                doNothingBroker, 
                storageDishViewer, 
                storageIngredientViewer, 
                storageTablesViewer, 
                storageUsersViewer, 
                storageOrdersViewer,
                storageOrderItemViewer,
                storageRestaurantViewer)

        member this.GetDish id =
            result {
                let! (_, restaurant) = restaurantViewer ()
                do! 
                    restaurant.DishRefs 
                    |> List.contains id 
                    |> Result.ofBool (sprintf "Dish with id '%A' does not exist" id)

                let! (_, state) = dishesViewer id
                do! 
                    state.Active
                    |> Result.ofBool (sprintf "Dish with id '%A' is not active" id)
                return state
            }

        member this.GetDishType (id: Guid) =
            result {
                let! (_, restaurant) = restaurantViewer ()
                do! 
                    restaurant.DishTypes |>> (fun x -> x.DishTypeId)
                    |> List.contains id 
                    |> Result.ofBool (sprintf "DishType with id '%A' does not exist" id)
                    
                let! result =
                    restaurant.DishTypes
                    |> List.tryFind (fun x -> x.DishTypeId = id)
                    |> Result.ofOption (sprintf "DishType with id '%A' does not exist" id)
                return result
            }
        
        member this.CreateDishType (dishType: DishType)  =
            result {
                let! existingDishTypes = this.GetallDishTypes ()
                let! notAlreadyExists =
                    existingDishTypes
                    |> List.exists (fun x -> x.Name = dishType.Name)
                    |> not
                    |> Result.ofBool (sprintf "DishType with name '%s' already exists" dishType.Name)
                
                let! result =
                    dishType
                    |> RestaurantCommands.AddDishType
                    |> runCommand<Restaurant, RestaurantEvents, string> eventStore eventBroker 
                return result
            }
        member this.UpdateDishType (dishType: DishType) =
            result {
                let! existingDishTypes = this.GetallDishTypes ()
                let! notAlreadyExists =
                    existingDishTypes
                    |> List.exists (fun x -> x.Name = dishType.Name)
                    |> not
                    |> Result.ofBool (sprintf "DishType with name '%s' already exists" dishType.Name)
                
                let! result =
                    dishType
                    |> RestaurantCommands.UpdateDishType
                    |> runCommand<Restaurant, RestaurantEvents, string> eventStore eventBroker 
                return result
            }
       
        member this.GetallDishTypes (): Result<List<DishType>, string> =
            result {
                let! (_, restaurant) = restaurantViewer ()
                return restaurant.DishTypes
            } 
            
        member this.GetDishTpe (id: Guid) =
            result {
                let! (_, restaurant) = restaurantViewer ()
                do! 
                    restaurant.DishTypes |>> (fun x -> x.DishTypeId)
                    |> List.contains id 
                    |> Result.ofBool (sprintf "DishType with id '%A' does not exist" id)
                    
                let! result =
                    restaurant.DishTypes
                    |> List.tryFind (fun x -> x.DishTypeId = id)
                    |> Result.ofOption (sprintf "DishType with id '%A' does not exist" id)
                return result
            } 
      
        member this.GetDishType (name: string)  =
            result {
                let! (_, restaurant) = restaurantViewer ()
                let dishType = 
                    restaurant.DishTypes
                    |> List.tryFind (fun x -> x.Name = name)
                return!
                    dishType
                    |> Result.ofOption (sprintf "DishType with name '%s' does not exist" name)
            }
             
        member this.GetUserByName userName: Result<User, string>  =
            result {
                let! (_, restaurant) = restaurantViewer ()
                let! users = this.GetAllUsers ()
                let user: Option<User> =
                    users 
                    |> List.tryFind (fun x -> x.Username = userName)
                return!
                    user
                    |> Result.ofOption (sprintf "User with name '%s' does not exist" userName)
            }

        member this.GetDishByName name: Result<Dish, string>  =
            result {
                let! (_, restaurant) = restaurantViewer ()
                let! dishes = this.GetAllDishes ()
                let dish: Option<Dish> =
                    dishes 
                    |> List.tryFind (fun x -> x.Name = name)
                return!
                    dish
                    |> Result.ofOption (sprintf "Dish with name '%s' does not exist" name)
            }
        
        member this.FindDishByPartialName (name: string): Result<Dish, string> =
            result {
                let! (_, restaurant) = restaurantViewer ()
                let! dishes = this.GetAllDishes ()
                let dish: Option<Dish> =
                    dishes 
                    |> List.tryFind (fun (x: Dish) -> (x.Name.Contains name))
                return!
                    dish
                    |> Result.ofOption (sprintf "Dish with name '%s' does not exist" name)
            }
        
        member this.AuthenticateUser (userName: string, password: string) =

            // note that password must already be passHashed (passHash)

            result {
                let! user = this.GetUserByName userName
                do! 
                    user.Password = password
                    |> Result.ofBool "Invalid password"
                return user
            }

        member this.RemoveUser (id: Guid) =
            result {
                let! user = this.GetUser id
                let deactivate = UserCommands.Deactivate
                let! _  =
                    deactivate
                    |> runAggregateCommand<User, UserEvents, string> user.Id eventStore eventBroker 
                let removeId = RestaurantCommands.RemoveUserRef id
                return!
                    removeId
                    |> runCommand<Restaurant, RestaurantEvents, string> eventStore eventBroker
            }

        // check that no user uses this role
        member this.RemoveRole (id: Guid) =
            result {
                let! users = this.GetAllUsers ()
                let existingRoles = 
                    users 
                    |>> (fun x -> x.OptionalRoleId)
                    |> List.fold (fun acc x -> 
                        match x with
                        | Some r -> r :: acc
                        | None -> acc) []
                do! 
                    existingRoles
                    |> List.contains id
                    |> not
                    |> Result.ofBool (sprintf "Role with id '%A' is still in use" id)

                let! role = this.GetRole id
                let deactivate = RoleCommands.Deactivate
                let! _  =
                    deactivate
                    |> runAggregateCommand<Role, RoleEvents, string> role.Id eventStore eventBroker 
                let removeId = RestaurantCommands.RemoveRoleRef id
                return!
                    removeId
                    |> runCommand<Restaurant, RestaurantEvents, string> eventStore eventBroker
            }


        member this.GetIngredient id =
            result {
                let! (_, restaurant) = restaurantViewer ()
                do! 
                    restaurant.IngredientRefs 
                    |> List.contains id 
                    |> Result.ofBool (sprintf "Ingredient with id '%A' does not exist" id)

                let! (_, state) = ingredientViewer id
                do! 
                    state.Active
                    |> Result.ofBool (sprintf "Ingredient with id '%A' is not active" id)
                return state
            }

        member this.GetIngredientsOfDish (id: Guid) =
            result {
                let! dish = this.GetDish id
                let ingredientsOfDish = dish.IngredientAndQuantities
                let! ingredients =
                    ingredientsOfDish
                    |> List.traverseResultM (fun x -> this.GetIngredient x.IngredientId) // return result
                let result: List<Ingredient * List<IngredientMeasureItemType>>  =
                    List.zip ingredients (ingredientsOfDish |>> (fun x -> x.PossibleAlternativeQuantities))
                return result    
            }

        member this.GetTable id =
            result {
                let! (_, restaurant) = restaurantViewer ()
                do! 
                    restaurant.TableRefs 
                    |> List.contains id 
                    |> Result.ofBool (sprintf " Table with id '%A' does not exist" id)

                let! (_, state) = tablesViewer id
                do! 
                    state.Active
                    |> Result.ofBool (sprintf "Table with id '%A' is not active" id)
                return state
            }

        member this.GetUser id: Result<User, string> =
            result {
                let! (_, restaurant) = restaurantViewer ()
                do! 
                    restaurant.UsersRefs 
                    |> List.contains id 
                    |> Result.ofBool (sprintf "User with id '%A' does not exist" id)

                let! (_, state) = usersViewer id
                do! 
                    state.Active
                    |> Result.ofBool (sprintf "User with id '%A' is not active" id)
                return state
            }

        member this.RemoveIngredient (id: Guid) =
            result {
                let! allDishes = this.GetAllDishes ()
                let usedIngredients  = 
                    allDishes
                    |> List.map (fun (x: Dish) -> x.IngredientAndQuantities)
                    |> List.concat
                    |>> fun x -> x.IngredientId

                let! ingredient = this.GetIngredient id

                let! isUsed =
                    usedIngredients
                    |> List.contains id
                    |> not
                    |> Result.ofBool (sprintf "Ingredient with id '%A' is still in use" id)

                let deactivate = IngredientCommands.Deactivate
                let! _  =
                    deactivate
                    |> runAggregateCommand<Ingredient, IngredientEvents, string> ingredient.Id eventStore eventBroker 
                let removeId = RestaurantCommands.RemoveIngredientRef id
                return!
                    removeId
                    |> runCommand<Restaurant, RestaurantEvents, string> eventStore eventBroker
            }

        member this.AddIngredientPrice (ingredientId: Guid, ingredientPrice: IngredientPrice) =
            result {
                let! ingredient = this.GetIngredient ingredientId
                let addIngredientPrice = IngredientCommands.AddIngredientPrice ingredientPrice
                return!
                    addIngredientPrice
                    |> runAggregateCommand<Ingredient, IngredientEvents, string> ingredient.Id eventStore eventBroker
            }

        member this.RemoveIngredientPrice (ingredientId: Guid, ingredientPrice: Guid) =
            result {
                let! ingredient = this.GetIngredient ingredientId
                let removeIngredientPrice = IngredientCommands.RemoveIngredientPrice ingredientPrice
                return!
                    removeIngredientPrice
                    |> runAggregateCommand<Ingredient, IngredientEvents, string> ingredient.Id eventStore eventBroker
            } 
        // member this.RemoveIngredientPrice (ingredientId: Guid, price: float, quantity: float, measuringSystem: IngredientMeasureType) =
        //     result {
        //         let! ingredient = this.GetIngredient ingredientId
        //         let ingredientPrice = IngredientPrice.mkIngredientPrice (ingredientId, price, quantity, measuringSystem)
        //         let removeIngredientPrice = IngredientCommands.RemoveIngredientPrice ingredientPrice
        //         return!
        //             removeIngredientPrice
        //             |> runAggregateCommand<Ingredient, IngredientEvents, string> ingredient.Id eventStore eventBroker
        //     }
        
        member this.UpdateDish (dish: Dish) =
            result {
                let! dishExists =
                    this.GetDish dish.Id
                    |> Result.map (fun _ -> ())
        
                let updateDish =
                    DishCommands.Update
                        (dish.Name,
                         dish.DishType,
                         dish.IngredientAndQuantities,
                         dish.Active,
                         dish.Visible,
                         dish.Price,
                         dish.StandardComments,
                         dish.StandardVariations
                         )
                let result =
                    updateDish
                    |> runAggregateCommand<Dish, DishEvents, string> dish.Id eventStore eventBroker
                return! result
            }    

        member this.UpdateIngredient (ingredient: Ingredient) =
            printf "going to upgrade ingredient 100\n"
            result {
                let! ingredientExists =
                    this.GetIngredient ingredient.Id
                    |> Result.map (fun _ -> ())
        
                let updateIngredient =
                    IngredientCommands.Update
                        (ingredient.Name,
                         ingredient.Description,
                         ingredient.IngredientTypeId,
                         ingredient.IngredientMeasureTypes,
                         ingredient.Active,
                         ingredient.IngredientPrices,
                         ingredient.Stock,
                         ingredient.HasAllergen,
                         ingredient.UpdatePolicy,
                         ingredient.CheckUpdatePolicy, ingredient.Visible)
                printf "going to upgrade ingredient 200\n"
                let result =
                    updateIngredient
                    |> runAggregateCommand<Ingredient, IngredientEvents, string> ingredient.Id eventStore eventBroker
                printf "upgraded ingredient  200 %A\n" result
                return! result
            }

        member this.GetOrder id =
            result {
                let! (_, restaurant) = restaurantViewer ()
                do! 
                    restaurant.OrderRefs 
                    |> List.contains id 
                    |> Result.ofBool (sprintf "Order with id '%A' does not exist" id)

                let! (_, state) = ordersViewer id
                do! 
                    state.Active
                    |> Result.ofBool (sprintf "Order with id '%A' is not active" id)
                return state
            }
        member this.GetAllOrders () =
            result {
                let! (_, restaurant) = restaurantViewer ()
                let ordersRefs = restaurant.OrderRefs
                let! orders = 
                    ordersRefs
                    |> List.traverseResultM (fun id -> ordersViewer id)
                return orders |>> snd
            }

        member this.GetOrdersOfUser (userId: Guid) =
            result {
                let! userExists =
                    this.GetUser userId
                    |> Result.map (fun _ -> ())

                let! allOrders = this.GetAllOrders ()
                let ordersOfUser = 
                    allOrders
                    |> List.filter (fun x -> x.UserId = userId)
                return ordersOfUser
            }
        member this.GetTemporaryUsers () =
            result {
                let! allUsers = this.GetAllUsers ()
                let temporaryUsers = 
                    allUsers
                    |> List.filter (fun x -> x.Temporary)
                return temporaryUsers
            }

        member this.GetOrdinaryUsers () =
            result {
                let! allUsers = this.GetAllUsers ()
                let ordinaryUsers = 
                    allUsers
                    |> List.filter (fun x -> not x.Temporary)
                return ordinaryUsers
            }

        member this.GetAllDishes () =
            result {
                let! (_, restaurant) = restaurantViewer ()
                let dishesRefs = restaurant.DishRefs
                let! dishes = 
                    dishesRefs
                    |> List.traverseResultM (fun id -> dishesViewer id)
                return dishes |>> snd
            }
        member this.FindDishByName (name: string)   =
            result {
                let! dishes = this.GetAllDishes ()
                let filtered = dishes |> List.filter (fun x -> x.Name.ToLower().Contains (name.ToLower()))
                return filtered
            }
            
        member this.GetAllDishesOfACertainType (category: Guid) =
            result {
                let! dishes = this.GetAllDishes ()
                let dishesOfCategory = 
                    dishes |> List.filter (fun x -> x.DishType  = category)
                return dishesOfCategory
            }
        
        member this.GetNumberOfDishes () =
            result {
                let! dishes = this.GetAllDishes ()
                return dishes.Length
            }
        member this.GetAllVisibleDishes ()  =
            result {
                let! dishes = this.GetAllDishes ()
                let visibleDishes = 
                    dishes |> List.filter (fun x -> x.Visible)
                return dishes
            }
        member this.GetVisibleDishesByCategory (category: Guid) =
            result {
                let! dishes = this.GetAllVisibleDishes ()
                let visibleDishes = 
                    dishes |> List.filter (fun x -> x.DishType = category)
                return dishes
            }    

        member this.GetAllIngredients () =
            result {
                let! (_, restaurant) = restaurantViewer ()
                let ingredientsRefs = restaurant.IngredientRefs
                let! ingredients = 
                    ingredientsRefs
                    |> List.traverseResultM (fun id -> ingredientViewer id)
                return ingredients |>> snd
            }

        member this.GetAllTables () =
            result {
                let! (_, restaurant) = restaurantViewer ()
                let tablesRefs = restaurant.TableRefs
                let! tables = 
                    tablesRefs
                    |> List.traverseResultM (fun id -> tablesViewer id)
                return tables |>> snd
            }

        member this.GetAllUsers () =
            result {
                let! (_, restaurant) = restaurantViewer ()
                let usersRefs = restaurant.UsersRefs
                let! users = 
                    usersRefs
                    |> List.traverseResultM (fun id -> usersViewer id)
                return users |>> snd
            }

        member this.CreateOrder (userId: Guid, tableId: Guid) =
            result {
                let! (_, restaurant) = restaurantViewer ()
                do! 
                    restaurant.TableRefs 
                    |> List.contains tableId 
                    |> Result.ofBool (sprintf "Table with id '%A' does not exist" tableId)
                let order = Order (userId, tableId)
                return!
                    order.Id
                    |> RestaurantCommands.AddOrderRef
                    |> runInitAndCommand<Restaurant, RestaurantEvents, Order, string> eventStore eventBroker order
            }

        member this.CreateRole (roleName: string) =
            result {
                let id = Guid.NewGuid()
                let role = Role (id, roleName)
                return!
                    id
                    |> RestaurantCommands.AddRoleRef
                    |> runInitAndCommand<Restaurant, RestaurantEvents, Role, string> eventStore eventBroker role
            }
        member this.GetRole id: Result<Role, string> =
            result {
                let! (_, restaurant) = restaurantViewer ()
                do! 
                    restaurant.RoleRefs 
                    |> List.contains id 
                    |> Result.ofBool (sprintf "Role with id '%A' does not exist" id)

                let! (_, state) = storageRoleViewer id
                do! 
                    state.Active
                    |> Result.ofBool (sprintf "Role with id '%A' is not active" id)
                return state
            }

        // todo: see what to do with optionalRoleIds
        member this.CreateUser (username: string, password: string, roles: List<Guid>) =
            result {
                let! existingUsers = this.GetAllUsers ()
                let! notAlreadyExists =
                    existingUsers
                    |> List.exists (fun x -> x.Username = username)
                    |> not
                    |> Result.ofBool (sprintf "User with name '%s' already exists" username)

                let! user = User.MkUser (username, password, "") // todo: see what to do with string based role 
                return!
                    user.Id
                    |> RestaurantCommands.AddUserRef
                    |> runInitAndCommand<Restaurant, RestaurantEvents, User, string> eventStore eventBroker user
            }

        member this.CreateDish (name: string, description: Option<string>, dishType: Guid, ingredientAndQuantities: List<IngredientAndQuantity>, price: decimal) =
            result {
                let! existingDishes = this.GetAllDishes ()
                let! notAlreadyExists =
                    existingDishes
                    |> List.exists (fun x -> x.Name = name)
                    |> not
                    |> Result.ofBool (sprintf "Dish with name '%s' already exists" name)

                let id = Guid.NewGuid()
                let dish = Dish (id, name, description, dishType, ingredientAndQuantities, true, true, price, [], [])

                let! result =
                    id
                    |> RestaurantCommands.AddDishRef
                    |> runInitAndCommand<Restaurant, RestaurantEvents, Dish, string> eventStore eventBroker dish

                return result
            }
        member this.CreateDish (dish: Dish) =
            result {
                let! existingDishes = this.GetAllDishes ()
                let! notAlreadyExists =
                    existingDishes
                    |> List.exists (fun x -> x.Name = dish.Name)
                    |> not
                    |> Result.ofBool (sprintf "Dish with name '%s' already exists" dish.Name)

                let! result =
                    dish.Id
                    |> RestaurantCommands.AddDishRef
                    |> runInitAndCommand<Restaurant, RestaurantEvents, Dish, string> eventStore eventBroker dish

                return result
            }
            
        member this.ChangeDishTypeToDish (dishId: Guid, typeId: Guid) =
            result {
                
                let! dishExists =
                    this.GetDish dishId
                
                let! result =
                    typeId
                    |> DishCommands.SetDishType 
                    |> runAggregateCommand<Dish, DishEvents, string> dishId eventStore eventBroker
                return result
            }    

        member this.CreateIngredient (ingredient: Ingredient)  =
            printf "Creating ingredient %A\n" ingredient
            result {
                let! existingIngredients = this.GetAllIngredients ()
                
                let! notAlreadyExists =
                    existingIngredients
                    |> List.exists (fun x -> x.Name = ingredient.Name && x.Description = ingredient.Description)
                    |> not
                    |> Result.ofBool (sprintf "Ingredient with name '%s' and description %A already exists" ingredient.Name ingredient.Description)
                
                let! result =
                    ingredient.Id    
                    |> RestaurantCommands.AddIngredientRef
                    |> runInitAndCommand<Restaurant, RestaurantEvents, Ingredient, string> eventStore eventBroker ingredient
                return result    
            }
     
        member this.GetStandardComments () =
            result {
                let! (_, restaurant) = restaurantViewer ()
                return restaurant.StandardComments
            }
        member this.GetAllStandardVariations () =
            result {
                let! (_, restaurant) = restaurantViewer ()
                return restaurant.StandardVariations
            }
        member this.GetStandardComment (id: Guid) =
            result {
                let! (_, restaurant) = restaurantViewer ()
                do! 
                    restaurant.StandardComments
                    |> List.map (fun x -> x.CommentId)
                    |> List.contains id 
                    |> Result.ofBool (sprintf "StandardComment with id '%A' does not exist" id)
                
                let comment = restaurant.StandardComments |> List.find (fun x -> x.CommentId = id)
                return comment
            }
       
        member this.GetStandardVariation (id: Guid) =
            result {
                let! (_, restaurant) = restaurantViewer ()
                do! 
                    restaurant.StandardVariations
                    |> List.map (fun x -> x.Id)
                    |> List.contains id 
                    |> Result.ofBool (sprintf "StandardVariation with id '%A' does not exist" id)
                
                let variation = restaurant.StandardVariations |> List.find (fun x -> x.Id = id)
                return variation
            }     
        
        member this.GetStandardCommentsForDish (dishId: Guid) =
            result {
                let! dish = this.GetDish dishId
                let  dishStandardCommentsIds = dish.StandardComments
                let comments =
                    dishStandardCommentsIds
                    |> List.map (fun x -> this.GetStandardComment x)
                    |> List.filter (fun x -> (x |> Result.isOk))
                    |> List.map (fun x -> x.OkValue)
                return comments    
            }
       
            
        member this.GetStandardVariationsForDish (dishId: Guid) =
            result {
                let! dish = this.GetDish dishId
                let  dishStandardVariationsIds = dish.StandardVariations
                let variations =
                    dishStandardVariationsIds
                    |> List.map (fun x -> this.GetStandardVariation x)
                    |> List.filter (fun x -> (x |> Result.isOk))
                    |> List.map (fun x -> x.OkValue)
                return variations    
            }

        member this.CreateTable (description: Option<string>, number: int, seats: int) =
            result {
                let! existingTables = this.GetAllTables ()
                let! notAlreadyExists =
                    existingTables
                    |> List.exists (fun x -> (x.Description, x.Number) = (description, number))
                    |> not
                    |> Result.ofBool (sprintf "Table with number '%d' already exists" number)

                let id = Guid.NewGuid()
                let table = Table (id, description, number, seats)

                let! result =
                    id
                    |> RestaurantCommands.AddTableRef
                    |> runInitAndCommand<Restaurant, RestaurantEvents, Table, string> eventStore eventBroker table
                return result
            }
        member this.RemoveDish (id: Guid) =
            result {
                
                // TODO: should not be used by any orderitem
                    
                let removeCommand = RestaurantCommands.RemoveDishRef id
                let! 
                    removed =
                    removeCommand
                    |> runCommand<Restaurant, RestaurantEvents, string> eventStore eventBroker
                let! _ =
                    DishCommands.Deactivate 
                    |> runAggregateCommand<Dish, DishEvents, string> id eventStore eventBroker
                return ()
            }

        member this.UserCanManageAllOrders (userId: Guid) =
            result {
                let! user = this.GetUser userId
                return user.CanManageAllOrders
            }
        
        member this.AddOrderItem (orderItem: OrderItem ) =
            result {
                let! orderExists = this.GetOrder orderItem.OrderId
                let addOrderItemRef = RestaurantCommands.AddOrderItemRef orderItem.Id
                return!
                    addOrderItemRef
                    |> runInitAndCommand<Restaurant, RestaurantEvents, OrderItem, string> eventStore eventBroker orderItem
            }
        member this.GetOrderItem (id: Guid) =
            result {
                let! (_, restaurant) = restaurantViewer ()
                do! 
                    restaurant.OrderItemRefs 
                    |> List.contains id 
                    |> Result.ofBool (sprintf "OrderItem with id '%A' does not exist" id)
                    
                let! orderItem =
                    orderItemsViewer id
                return orderItem    
            }
       
        // note this may imply the issue of having old orderItems in memory 
        member this.GetAllOrderItems () =
            result {
                let! (_, restaurant) = restaurantViewer ()
                let orderItemsRefs = restaurant.OrderItemRefs
                let! orderItems =
                    orderItemsRefs
                    |> List.traverseResultM (fun id -> orderItemsViewer id)
                return orderItems |>> snd    
            }
            
        member this.GetOrderItemsOfOrder (orderId: Guid) =
            result {
                let! orderExists = this.GetOrder orderId
                let! allOrderItems = this.GetAllOrderItems ()
                let orderItemsOfOrder = 
                    allOrderItems
                    |> List.filter (fun x -> x.OrderId = orderId)
                return orderItemsOfOrder
            }
        
        member this.AddPrinter (printer: Printer) =
            result {
                let addPrinter = RestaurantCommands.AddPrinter printer
                return!
                    addPrinter
                    |> runCommand<Restaurant, RestaurantEvents, string> eventStore eventBroker
            } 
            
        member this.GetPrinters () =
            result {
                let! (_, restaurant) = restaurantViewer ()
                return restaurant.Printers
            }
       
        member this.AddIngredientType (ingredientType: IngredientType) =
            result {
                let addIngredientType = RestaurantCommands.AddIngredientType ingredientType
                return!
                    addIngredientType
                    |> runCommand<Restaurant, RestaurantEvents, string> eventStore eventBroker
            }
            
        member this.GetAllIngredientsOfACategory (ingredientCategoryId: Guid) =
            result {
                let! existingCategories =
                    this.GetIngredientTypes ()
                do! 
                    existingCategories
                    |> List.exists (fun x -> x.Id = ingredientCategoryId)
                    |> Result.ofBool (sprintf "IngredientType QQQ with id '%A' does not exist" ingredientCategoryId)    
                
                let! ingredients = this.GetAllIngredients ()
                let ingredientsOfCategory = 
                    ingredients |> List.filter (fun x -> x.IngredientTypeId = ingredientCategoryId)
                return ingredientsOfCategory
            }
            
        member this.FindIngredientByName (name: string) =
            result {
                let! ingredients = this.GetAllIngredients ()
                let filtered = ingredients |> List.filter (fun x -> x.Name.ToLower().Contains (name.ToLower()))
                return filtered
            }
        member this.FindIngredientByNameAndDescription (name: string) (description: Option<string>) =
            result {
                let! ingredients = this.GetAllIngredients ()
                let filtered = 
                    ingredients 
                    |> List.filter (fun x -> x.Name = name && x.Description = description)
                return filtered     
            }
            
        member this.GetAllIngredientOfACategoryByPage (ingredientCategoryId: Guid, page: int, pageSize: int) =
            result {
                let! ingredients =
                    this.GetAllIngredientsOfACategory ingredientCategoryId
                let nTotals = ingredients.Length    
                let sortedIngredients = 
                    ingredients |> List.sortBy (fun x -> x.Name)    
                let ingredientsOfCategory =
                    if sortedIngredients.Length = 0 then 
                        []
                    else if (sortedIngredients.Length - (page * pageSize)) <= pageSize then
                        sortedIngredients |> List.skip (page * pageSize) 
                    else
                        sortedIngredients |> List.skip (page * pageSize) |> List.take pageSize
                return (ingredientsOfCategory, nTotals)
            }
        
        member this.GetAllIngredientOfACategoryByPageFilteredByName (ingredientCategoryId: Guid, page: int, pageSize: int, name: string ) =
            result {
                let! ingredients =
                    this.GetAllIngredientsOfACategory ingredientCategoryId
                let filtered = ingredients |> List.filter (fun x -> x.Name.ToLower().Contains (name.ToLower()))    
                let nTotals = ingredients.Length    
                let sortedIngredients = 
                    filtered |> List.sortBy (fun x -> x.Name)    
                let ingredientsOfCategory =
                    if sortedIngredients.Length = 0 then 
                        []
                    else if (sortedIngredients.Length - (page * pageSize)) <= pageSize then
                        sortedIngredients |> List.skip (page * pageSize) 
                    else
                        sortedIngredients |> List.skip (page * pageSize) |> List.take pageSize
                return (ingredientsOfCategory, nTotals)
            }
        member this.GetAllDishesOfACategory (categoryId: Guid) =
            result {
                let! dishes = this.GetAllDishes ()
                let dishesOfCategory = 
                    dishes |> List.filter (fun x -> x.DishType = categoryId)
                return dishesOfCategory
            }
        member this.GetAllDishesOfACategoryByPage (categoryId: Guid, page: int, pageSize: int) =
            result {
                let! dishes = this.GetAllDishesOfACategory categoryId
                let nTotals = dishes.Length    
                let sortedDishes = 
                    dishes |> List.sortBy (fun x -> x.Name)    
                let dishesOfCategory =
                    if sortedDishes.Length = 0 then 
                        []
                    else if (sortedDishes.Length - (page * pageSize)) <= pageSize then
                        sortedDishes |> List.skip (page * pageSize) 
                    else
                        sortedDishes |> List.skip (page * pageSize) |> List.take pageSize
                return (dishesOfCategory, nTotals)
            }    
            
        member this.GetIngredientTypes (): Result<List<IngredientType>, string> =
            result {
                let! (_, restaurant) = restaurantViewer ()
                return restaurant.IngredientTypes
            }
        member this.GetIngredientType (id: Guid) =
            result {
                let! (_, restaurant) = restaurantViewer ()
                let ingredientType = 
                    restaurant.IngredientTypes
                    |> List.tryFind (fun x -> x.Id = id)
                return!
                    ingredientType
                    |> Result.ofOption (sprintf "IngredientType with id '%A' does not exist" id)
            }
        member this.AddStandardComment (comment: string) =
            result {
                let addStandardComment = RestaurantCommands.AddStandardComment comment
                return!
                    addStandardComment
                    |> runCommand<Restaurant, RestaurantEvents, string> eventStore eventBroker
            }
            
        member this.UpdateStandardComment (standardComment: StandardComment) =
            result {
                let updateStandardComment = RestaurantCommands.UpdateStandardComment standardComment
                return!
                    updateStandardComment
                    |> runCommand<Restaurant, RestaurantEvents, string> eventStore eventBroker
            }
        member this.RemoveStandardComment (id: Guid) =
            result {
                let removeStandardComment = RestaurantCommands.RemoveStandardComment id
                return!
                    removeStandardComment
                    |> runCommand<Restaurant, RestaurantEvents, string> eventStore eventBroker
            }
