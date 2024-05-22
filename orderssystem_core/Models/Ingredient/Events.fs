module OrdersSystem.Models.IngredientEvents
open OrdersSystem.Models.Ingredient

open OrdersSystem.Commons
open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core

type IngredientEvents =
    | IngredientTypeAdded of IngredientTypes
    | IngredientTypeRemoved of IngredientTypes
    | NameUpdated of String
    | Deactivated 
    | IngredientMeasureTypeAdded of IngredientMeasureType
    | IngredientMeasureTypeRemoved of IngredientMeasureType

        interface Event<Ingredient>  with
            member this.Process ingredient =
                match this with
                | IngredientTypeAdded ingredientType -> 
                    ingredient.AddIngredientType ingredientType
                | IngredientTypeRemoved ingredientType -> 
                    ingredient.RemoveIngredientType ingredientType
                | NameUpdated name -> 
                    ingredient.UpdateName name
                | IngredientMeasureTypeAdded ingredientMeasure -> 
                    ingredient.AddIngredientMeasureType ingredientMeasure
                | IngredientMeasureTypeRemoved ingredientMeasure -> 
                    ingredient.RemoveIngredientMeasureType ingredientMeasure
                | Deactivated ->
                    ingredient.Deactivate ()
    member this.Serialize =
        globalSerializer.Serialize this
    static member Deserialize json =
        globalSerializer.Deserialize<IngredientEvents> json

