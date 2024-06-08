
namespace OrdersSystem.Contexts.RestaurantCommands
open OrdersSystem.Contexts.Restaurant
open OrdersSystem.Contexts.Restaurant.Restaurant
open OrdersSystem.Contexts.RestaurantEvents
open FSharpPlus
open OrdersSystem.Commons
open OrdersSystem.Shared
open FsToolkit.ErrorHandling
open OrdersSystem.Models.Dish
open OrdersSystem.Models.Ingredient
open Sharpino.Definitions
open Sharpino.Utils
open Sharpino
open System
open Sharpino.Core

type RestaurantCommands =
    | AddDishRef of Guid
    | AddIngredientRef of Guid
    | RemoveIngredientRef of Guid
    | RemoveDishRef of Guid
    | AddPrinter of Printer
    | RemovePrinter of string
    | UpdatePrinter of Printer
    | AddTableRef of Guid
    | RemoveTableRef of Guid
    | AddOrderRef of Guid
    | RemoveOrderRef of Guid
    | AddUserRef of Guid
    | RemoveUserRef of Guid
    | AddOrderItemRef of Guid
    | RemoveOrderItemRef of Guid
    | AddIngredientType of IngredientType
    | AddDishType of DishType
    | UpdateDishType of DishType
    | AddStandardComment of string
    | UpdateStandardComment of StandardComment
    | RemoveStandardComment of Guid
    | AddStandardVariation of StandardVariation
    | RemoveStandardVariation of Guid
    | UpdateStandardVariation of StandardVariation
    | CreateUserRole of UserRole
    | UpdateUserRole of UserRole
    | DeleteUserRole of Guid
     
    interface Command<Restaurant, RestaurantEvents> with
        member this.Execute restaurant =
            match this with
            | AddDishRef id ->
                restaurant.AddDishRef id
                |> Result.map (fun r -> [DishRefAdded id])
            | AddIngredientRef id ->
                restaurant.AddIngredientRef id
                |> Result.map (fun r -> [IngredientRefAdded id])
            | RemoveIngredientRef id ->
                restaurant.RemoveIngredientRef id
                |> Result.map (fun r -> [IngredientRefRemoved id])
            | RemoveDishRef id ->
                restaurant.RemoveDishRef id
                |> Result.map (fun r -> [DishRefRemoved id])
            | AddPrinter printer ->
                restaurant.AddPrinter printer
                |> Result.map (fun r -> [PrinterAdded printer])
            | UpdatePrinter printer ->
                restaurant.UpdatePrinter printer
                |> Result.map (fun r -> [PrinterUpdated printer])
            | RemovePrinter printer ->
                restaurant.RemovePrinter printer
                |> Result.map (fun r -> [PrinterRemoved printer])
            | AddTableRef id ->
                restaurant.AddTableRef id
                |> Result.map (fun r -> [TableRefAdded id])
            | RemoveTableRef id ->
                restaurant.RemoveTableRef id
                |> Result.map (fun r -> [TableRefRemoved id])
            | AddOrderRef id ->
                restaurant.AddOrderRef id
                |> Result.map (fun r -> [OrderRefAdded id])
            | RemoveOrderRef id ->
                restaurant.RemoveOrderRef id
                |> Result.map (fun r -> [OrderRefRemoved id])
            | AddUserRef id ->
                restaurant.AddUserRef id
                |> Result.map (fun r -> [UserRefAdded id])
            | RemoveUserRef id ->
                restaurant.RemoveUserRef id
                |> Result.map (fun r -> [UserRemoved id])
            | AddOrderItemRef id ->
                restaurant.AddOrderItemRef id
                |> Result.map (fun r -> [OrderItemRefAdded id])
            | RemoveOrderItemRef id ->
                restaurant.RemoveOrderItemRef id
                |> Result.map (fun r -> [OrderItemRefRemoved id])
            | AddIngredientType ingredientType ->
                restaurant.AddIngredientType ingredientType
                |> Result.map (fun r -> [IngredientTypeAdded ingredientType])
            | AddDishType dishType ->
                restaurant.AddDishType dishType
                |> Result.map (fun r -> [DishTypeAdded dishType])
            | UpdateDishType dishType ->
                restaurant.UpdateDishType dishType
                |> Result.map (fun r -> [DishTypeUpdated dishType])
            | AddStandardComment comment ->
                restaurant.AddStandardComment comment
                |> Result.map (fun r -> [StandardCommentAdded comment])
            | UpdateStandardComment comment ->
                restaurant.UpdateStandardComment comment
                |> Result.map (fun r -> [StandardCommentUpdated comment])
            | RemoveStandardComment guid ->
                restaurant.RemoveStandardComment guid
                |> Result.map (fun r -> [StandardCommentRemoved guid])
            | AddStandardVariation standardVariation ->
                restaurant.AddStandardVariation standardVariation
                |> Result.map (fun r -> [StandardVariationAdded standardVariation])    
            | RemoveStandardVariation  guid ->
                restaurant.RemoveStandardVariation guid
                |> Result.map (fun r -> [StandardVariationRemoved guid])
            | UpdateStandardVariation standardVariation ->
                restaurant.UpdateStandardVariation standardVariation
                |> Result.map (fun r -> [StandardVariationUpdated standardVariation])
            | CreateUserRole userRole ->
                restaurant.CreateUserRole userRole
                |> Result.map (fun r -> [UserRoleAdded userRole])
            | UpdateUserRole userRole ->
                restaurant.UpdateUserRole userRole
                |> Result.map (fun r -> [UserRoleUpdated userRole])    
            | DeleteUserRole userRoleId ->
                restaurant.DeleteUserRole userRoleId
                |> Result.map (fun r -> [UserRoleDeleted userRoleId]) 
                
        member this.Undoer = None