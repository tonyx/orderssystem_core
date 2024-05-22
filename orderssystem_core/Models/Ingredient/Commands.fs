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

        interface Command<Ingredient, IngredientEvents>  with
            member this.Execute ingredient = 
                match this with
                | AddIngredientType ingredientType -> 
                    ingredient.AddIngredientType ingredientType
                    |> Result.map (fun ingredient -> [IngredientTypeAdded ingredientType])
                | RemoveIngredientType ingredientType -> 
                    ingredient.RemoveIngredientType ingredientType
                    |> Result.map (fun ingredient -> [IngredientTypeRemoved ingredientType])
                | UpdateName name -> 
                    ingredient.UpdateName name
                    |> Result.map (fun ingredient -> [NameUpdated name])
                | AddIngredientMeasureType ingredientMeasure -> 
                    ingredient.AddIngredientMeasureType ingredientMeasure
                    |> Result.map (fun ingredient -> [IngredientMeasureTypeAdded ingredientMeasure])
                | RemoveIngredientMeasureType ingredientMeasure -> 
                    ingredient.RemoveIngredientMeasureType ingredientMeasure
                    |> Result.map (fun ingredient -> [IngredientMeasureTypeRemoved ingredientMeasure])
                | Deactivate -> 
                    ingredient.Deactivate ()
                    |> Result.map (fun ingredient -> [Deactivated])
            member this.Undoer = None