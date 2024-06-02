
namespace OrdersSystem.Contexts.RestaurantCommands
open OrdersSystem.Contexts.Restaurant
open OrdersSystem.Contexts.Restaurant.Restaurant
open OrdersSystem.Contexts.RestaurantEvents
open FSharpPlus
open OrdersSystem.Commons
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
    | AddRoleRef of Guid
    | RemoveRoleRef of Guid
    | AddOrderItemRef of Guid
    | RemoveOrderItemRef of Guid
    | AddIngredientType of IngredientType
    | AddDishType of DishType

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
            | AddRoleRef id ->
                restaurant.AddRoleRef id
                |> Result.map (fun r -> [RoleRefAdded id])
            | RemoveRoleRef id ->
                restaurant.RemoveRoleRef id
                |> Result.map (fun r -> [RoleRefRemoved id])
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

        member this.Undoer = None