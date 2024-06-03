
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
    | AddIngredientAndQuantity of IngredientAndQuantity
    | RemoveIngredient of Guid
    | UpdateName of String
    | Deactivate
    | SetVisible
    | SetInvisible
    | SetDishType of Guid
    | Update of string * Guid * List<IngredientAndQuantity> * bool * bool * decimal * List<Guid>

        interface Command<Dish, DishEvents>  with
            member this.Execute dish = 
                match this with
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
                | SetVisible ->
                    dish.SetVisible ()
                    |> Result.map (fun _ -> [VisibleSet])
                | SetInvisible ->
                    dish.SetInvisible ()
                    |> Result.map (fun _ -> [InvisibleSet])
                | SetDishType guid ->
                    dish.SetDishType guid
                    |> Result.map (fun _ -> [DishTypeSet guid])
                | Update (name, dishType, ingredientRefs, active, visible, price, standardComments ) ->
                    dish.Update (name, dishType, ingredientRefs, active, visible, price, standardComments)
                    |> Result.map (fun _ -> [Updated (name, dishType, ingredientRefs, active, visible, price, standardComments )])
            member this.Undoer = None
            