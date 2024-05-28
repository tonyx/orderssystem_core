module OrdersSystem.Models.IngredientCommands
open OrdersSystem.Models.Ingredient
open OrdersSystem.Models.IngredientEvents

open OrdersSystem.Commons
open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core

type IngredientCommands =
    | AddIngredientType of IngredientTypes
    | RemoveIngredientType of IngredientTypes
    | UpdateName of String
    | AddIngredientMeasureType of IngredientMeasureType
    | RemoveIngredientMeasureType of IngredientMeasureType
    | Deactivate
    | AddIngredientPrice of IngredientPrice
    | RemoveIngredientPrice of IngredientPrice
    | UpdateIngredient of Ingredient

        interface Command<Ingredient, IngredientEvents>  with
            member this.Execute ingredient = 
                match this with
                | AddIngredientType ingredientType -> 
                    ingredient.AddIngredientType ingredientType
                    |> Result.map (fun _ -> [IngredientTypeAdded ingredientType])
                | RemoveIngredientType ingredientType -> 
                    ingredient.RemoveIngredientType ingredientType
                    |> Result.map (fun _ -> [IngredientTypeRemoved ingredientType])
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
                | UpdateIngredient ingredient ->
                    ingredient.Update ingredient
                    |> Result.map (fun _ -> [IngredientUpdated ingredient])

                | Deactivate -> 
                    ingredient.Deactivate ()
                    |> Result.map (fun _ -> [Deactivated])
            member this.Undoer = None