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
    | IngredientPriceAdded of IngredientPrice
    | IngredientPriceRemoved of Guid
    | AllergenSet of bool
    | UpdatePolicySet of UpdatePolicy
    | StockIncreased of float
    | StockDecreased of float
    | VisibilitySet of bool
    | Updated of
        string *
        Option<string> *
        Guid *
        List<IngredientMeasureType> *
        bool *
        List<IngredientPrice> *
        float *
        bool *
        UpdatePolicy *
        CheckUpdatePolicy *
        bool
        
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
                | AllergenSet x ->
                    ingredient.SetAllergen x
                | UpdatePolicySet x ->
                    ingredient.SetUpdatePolicy x
                | StockIncreased x ->
                    ingredient.IncreaseStock x
                | StockDecreased x ->
                    ingredient.DecreaseStock x
                | VisibilitySet x ->
                    ingredient.SetVisibility x
                | Updated (name, description, ingredientTypeId, ingredientMeasureTypes, active, ingredientPrices, stock, hasAllergen, updatePolicy, checkUpdatePolicy, visible) ->
                    ingredient.Update (name, description, ingredientTypeId, ingredientMeasureTypes, active, ingredientPrices, stock, hasAllergen, updatePolicy, checkUpdatePolicy, visible)

    member this.Serialize =
        globalSerializer.Serialize this
    static member Deserialize json =
        globalSerializer.Deserialize<IngredientEvents> json

