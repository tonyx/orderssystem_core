module OrdersSystem.Models.IngredientEvents
open OrdersSystem.Models.Ingredient

open OrdersSystem.Commons
open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core

type IngredientEvents =
    | IngredientTypeSet of Guid
    | NameUpdated of String
    | Deactivated 
    | IngredientMeasureTypeAdded of IngredientMeasureType
    | IngredientMeasureTypeRemoved of IngredientMeasureType
    | IngredientUpdated of Ingredient
    | IngredientPriceAdded of IngredientPrice
    | IngredientPriceRemoved of IngredientPrice

        interface Event<Ingredient>  with
            member this.Process ingredient =
                match this with
                | IngredientTypeSet ingredientType -> 
                    ingredient.SetIngredientTypeId ingredientType
                | NameUpdated name -> 
                    ingredient.UpdateName name
                | IngredientMeasureTypeAdded ingredientMeasure -> 
                    ingredient.AddIngredientMeasureType ingredientMeasure
                | IngredientMeasureTypeRemoved ingredientMeasure -> 
                    ingredient.RemoveIngredientMeasureType ingredientMeasure
                | Deactivated ->
                    ingredient.Deactivate ()
                | IngredientPriceAdded ingredientPrice ->
                    ingredient.AddIngredientPrice ingredientPrice
                | IngredientPriceRemoved ingredientPrice ->
                    ingredient.RemoveIngredientPrice ingredientPrice
                | IngredientUpdated ingredient ->
                    ingredient.Update ingredient
                

    member this.Serialize =
        globalSerializer.Serialize this
    static member Deserialize json =
        globalSerializer.Deserialize<IngredientEvents> json

