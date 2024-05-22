
module OrdersSystem.Models.DishCommands
open OrdersSystem.Models.Dish
open OrdersSystem.Models.DishEvents
open OrdersSystem.Commons
open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core
open Ingredient

type DishCommands =
    | AddDishType of DishTypes
    | RemoveDishType of DishTypes
    | AddIngredientAndQuantity of IngredientAndQuantity
    | RemoveIngredient of Guid
    | UpdateName of String
    | Deactivate

        interface Command<Dish, DishEvents>  with
            member this.Execute dish = 
                match this with
                | AddDishType dishType -> 
                    dish.AddDishType dishType
                    |> Result.map (fun _ -> [DishTypeAdded dishType])
                | RemoveDishType dishType -> 
                    dish.RemoveDishType dishType
                    |> Result.map (fun _ -> [DishTypeRemoved dishType])
                | AddIngredientAndQuantity ingredient -> 
                    dish.AddIngredientAndQuantity ingredient
                    |> Result.map (fun _ -> [IngredientAndQuantityAdded ingredient])
                | RemoveIngredient ingredient -> 
                    dish.RemoveIngredient ingredient
                    |> Result.map (fun _ -> [IngredientRemoved ingredient])
                | UpdateName name -> 
                    dish.UpdateName name
                    |> Result.map (fun _ -> [NameUpdated name])
                | Deactivate -> 
                    dish.Deactivate ()
                    |> Result.map (fun _ -> [Deactivated])
            member this.Undoer = None