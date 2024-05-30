
namespace OrdersSystem.Contexts.RestaurantEvents
open OrdersSystem.Contexts.Restaurant
open FSharpPlus
open OrdersSystem.Commons
open FsToolkit.ErrorHandling
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
    | RoleRefAdded of Guid
    | RoleRefRemoved of Guid
    | OrderItemRefAdded of Guid
    | OrderItemRefRemoved of Guid
    | IngredientTypeAdded of IngredientType

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
            | RoleRefAdded id ->
                restaurant.AddRoleRef id
            | RoleRefRemoved id ->
                restaurant.RemoveRoleRef id
            | OrderItemRefAdded id ->
                restaurant.AddOrderItemRef id
            | OrderItemRefRemoved id ->
                restaurant.RemoveOrderItemRef id
            | IngredientTypeAdded ingredientType ->
                restaurant.AddIngredientType ingredientType

    member this.Serialize =
        globalSerializer.Serialize this
    static member Deserialize json =
        globalSerializer.Deserialize<RestaurantEvents> json

