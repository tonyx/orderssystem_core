module OrdersSystem.Models.IngredientCommands
open OrdersSystem.Models.Ingredient
open OrdersSystem.Models.IngredientEvents

open OrdersSystem.Commons
open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core

type IngredientCommands =
    | SetIngredientType of Guid
    | UpdateName of String
    | AddIngredientMeasureType of IngredientMeasureType
    | RemoveIngredientMeasureType of IngredientMeasureType
    | AddIngredientPrice of IngredientPrice
    | RemoveIngredientPrice of IngredientPrice
    | Deactivate
    | SetAllergen of bool
    | SetUpdatePolicy of UpdatePolicy
    | IncreaseStock of float
    | DecreaseStock of float
    | SetVisibility of bool
    
        interface Command<Ingredient, IngredientEvents>  with
            member this.Execute ingredient = 
                match this with
                | SetIngredientType guid -> 
                    ingredient.SetIngredientTypeId guid
                    |> Result.map (fun _ -> [IngredientTypeSet guid])
                | UpdateName name -> 
                    ingredient.UpdateName name
                    |> Result.map (fun _ -> [NameUpdated name])
                | AddIngredientMeasureType ingredientMeasure -> 
                    ingredient.AddIngredientMeasureType ingredientMeasure
                    |> Result.map (fun _ -> [IngredientMeasureTypeAdded ingredientMeasure])
                | RemoveIngredientMeasureType ingredientMeasure -> 
                    ingredient.RemoveIngredientMeasureType ingredientMeasure
                    |> Result.map (fun _ -> [IngredientMeasureTypeRemoved ingredientMeasure])
                | AddIngredientPrice ingredientPrice ->
                    ingredient.AddIngredientPrice ingredientPrice
                    |> Result.map (fun _ -> [IngredientPriceAdded ingredientPrice])
                | RemoveIngredientPrice ingredientPrice ->
                    ingredient.RemoveIngredientPrice ingredientPrice
                    |> Result.map (fun _ -> [IngredientPriceRemoved ingredientPrice])
                | Deactivate -> 
                    ingredient.Deactivate ()
                    |> Result.map (fun _ -> [Deactivated])
                | SetAllergen x ->
                    ingredient.SetAllergen x
                    |> Result.map (fun _ -> [AllergenSet x])
                | SetUpdatePolicy x ->
                    ingredient.SetUpdatePolicy x
                    |> Result.map (fun _ -> [UpdatePolicySet x])
                | IncreaseStock x ->
                    ingredient.IncreaseStock x
                    |> Result.map (fun _ -> [StockIncreased x])
                | DecreaseStock x ->
                    ingredient.DecreaseStock x
                    |> Result.map (fun _ -> [StockDecreased x])
                | SetVisibility x ->
                    ingredient.SetVisibility x
                    |> Result.map (fun _ -> [VisibilitySet x])
                    
            member this.Undoer = None