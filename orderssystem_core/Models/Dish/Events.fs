module OrdersSystem.Models.DishEvents
open OrdersSystem.Models.Dish
open OrdersSystem.Commons
open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core
open Ingredient

type DishEvents =
    | DishTypeAdded of DishTypes
    | DishTypeRemoved of DishTypes
    | IngredientAndQuantityAdded of IngredientAndQuantity
    | IngredientRemoved of Guid
    | NameUpdated of String
    | Deactivated 

        interface Event<Dish>  with
            member this.Process dish =
                match this with
                | DishTypeAdded dishType ->
                    dish.AddDishType dishType
                | DishTypeRemoved dishType ->
                    dish.RemoveDishType dishType
                | IngredientAndQuantityAdded ingredient ->
                    dish.AddIngredientAndQuantity ingredient
                | IngredientRemoved ingredient ->
                    dish.RemoveIngredient ingredient
                | NameUpdated name ->
                    dish.UpdateName name
                | Deactivated -> 
                    dish.Deactivate ()

    member this.Serialize =
        globalSerializer.Serialize this
    static member Deserialize json =
        globalSerializer.Deserialize<DishEvents> json