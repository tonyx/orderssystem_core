
namespace OrdersSystem.Contexts.RestaurantEvents
open OrdersSystem.Contexts.Restaurant
open FSharpPlus
open OrdersSystem.Commons
open OrdersSystem.Shared
open OrdersSystem.Models.Dish
open OrdersSystem.Models.Ingredient
open Sharpino.Definitions
open Sharpino.Utils
open Sharpino
open System
open Sharpino.Core
open OrdersSystem.Contexts.Restaurant.Restaurant

type RestaurantEvents =
    | DishRefAdded of Guid
    | IngredientRefAdded of Guid
    | IngredientRefRemoved of Guid
    | DishRefRemoved of Guid
    | PrinterAdded of Printer
    | PrinterRemoved of string
    | PrinterUpdated of Printer
    | TableRefAdded of Guid
    | TableRefRemoved of Guid
    | OrderRefAdded of Guid
    | OrderRefRemoved of Guid
    | UserRefAdded of Guid
    | UserRemoved of Guid
    | OrderItemRefAdded of Guid
    | OrderItemRefRemoved of Guid
    | IngredientTypeAdded of IngredientType
    | DishTypeAdded of DishType
    | DishTypeUpdated of DishType
    | StandardCommentAdded of string
    | StandardCommentUpdated of StandardComment
    | StandardCommentRemoved of Guid
    | StandardVariationAdded of StandardVariation
    | StandardVariationRemoved of Guid
    | StandardVariationUpdated of StandardVariation
    | UserRoleAdded of UserRole
    | UserRoleUpdated of UserRole
    | UserRoleDeleted of Guid

    interface Event<Restaurant> with
        member this.Process restaurant =
            match this with
            | DishRefAdded id ->
                restaurant.AddDishRef id
            | IngredientRefAdded id ->
                restaurant.AddIngredientRef id
            | IngredientRefRemoved id ->
                restaurant.RemoveIngredientRef id
            | DishRefRemoved id ->
                restaurant.RemoveDishRef id
            | PrinterAdded printer ->
                restaurant.AddPrinter printer
            | PrinterRemoved printer ->
                restaurant.RemovePrinter printer
            | PrinterUpdated printer ->
                restaurant.UpdatePrinter printer
            | TableRefAdded id ->
                restaurant.AddTableRef id    
            | TableRefRemoved id ->
                restaurant.RemoveTableRef id
            | OrderRefAdded id ->
                restaurant.AddOrderRef id
            | OrderRefRemoved id ->
                restaurant.RemoveOrderRef id
            | UserRefAdded id ->
                restaurant.AddUserRef id
            | UserRemoved id ->
                restaurant.RemoveUserRef id
            | OrderItemRefAdded id ->
                restaurant.AddOrderItemRef id
            | OrderItemRefRemoved id ->
                restaurant.RemoveOrderItemRef id
            | IngredientTypeAdded ingredientType ->
                restaurant.AddIngredientType ingredientType
            | DishTypeAdded dishType ->
                restaurant.AddDishType dishType
            | DishTypeUpdated dishType ->
                restaurant.UpdateDishType dishType
            | StandardCommentAdded text ->
                restaurant.AddStandardComment text
            | StandardCommentUpdated comment ->
                restaurant.UpdateStandardComment comment
            | StandardCommentRemoved guid ->
                restaurant.RemoveStandardComment guid
            | StandardVariationAdded standardVariation ->
                restaurant.AddStandardVariation standardVariation
            | StandardVariationRemoved guid ->
                restaurant.RemoveStandardVariation guid
            | StandardVariationUpdated standardVariation ->
                restaurant.UpdateStandardVariation standardVariation
            | UserRoleAdded userRole ->
                restaurant.AddUserRole userRole
            | UserRoleUpdated userRole ->
                restaurant.UpdateUserRole userRole    
            | UserRoleDeleted userRoleId ->
                restaurant.DeleteUserRole userRoleId

    member this.Serialize =
        globalSerializer.Serialize this
    static member Deserialize json =
        globalSerializer.Deserialize<RestaurantEvents> json

