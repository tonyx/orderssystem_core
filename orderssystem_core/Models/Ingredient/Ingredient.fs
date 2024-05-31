module OrdersSystem.Models.Ingredient
open OrdersSystem.Commons
open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core

open Microsoft.FSharp.Reflection

    type IngredientMeasureType =
        | Grams
        | Kilograms
        | Liters
        | Milliliters
        | Pieces
        | Other of string
        
    type IngredientType =
        {
            Id: Guid
            Name: string
            Visible: bool
        }

    type IngredientPrice =
        {
            IngredientId: Guid
            Price: float
            Quantity: float
            MeasuringSystem: IngredientMeasureType
        }
        with 
        static member mkIngredientPrice (ingredientId: Guid, price: float, quantity: float, measuringSystem: IngredientMeasureType) =
            { IngredientId = ingredientId; Price = price; Quantity = quantity; MeasuringSystem = measuringSystem }

    type UpdatePolicy =
        | NoUpdate
        | UpdateOnOrder
        | UpdateOnStock
        | UpdateOnOrderAndStock
        with
        static member FromString (x: string) =
            match x with
            | "NoUpdate" -> NoUpdate
            | "UpdateOnOrder" -> UpdateOnOrder
            | "UpdateOnStock" -> UpdateOnStock
            | "UpdateOnOrderAndStock" -> UpdateOnOrderAndStock
            | _ ->
                NoUpdate
        static member GetCases() =
            ["NoUpdate"; "UpdateOnOrder"; "UpdateOnStock"; "UpdateOnOrderAndStock"]
                
    type CheckUpdatePolicy =
        | NoCheck
        | Alert
        | Block
        | BlockAndAlert
        with
        static member FromString (local: string, x: string) =
            match x with
            | "NoCheck" -> NoCheck
            | "Alert" -> Alert
            | "Block" -> Block
            | "BlockAndAlert" -> BlockAndAlert
            | _ -> NoCheck
        static member GetCases() =
            ["NoCheck"; "Alert"; "Block"; "BlockAndAlert"]    
     
    type Ingredient 
        (id: Guid,
         name: string,
         description: Option<string>,
         ingredientTypeId: Guid,
         ingredientMeasureTypes: List<IngredientMeasureType>,
         active: bool, 
         ingredientPrices: List<IngredientPrice>,
         stock: float,
         hasAllergen: bool,
         updatePolicy: UpdatePolicy,
         visible: bool
         ) =
        
        let stateId = Guid.NewGuid()
        member this.Id = id
        member this.Name = name
        member this.IngredientTypeId = ingredientTypeId
        member this.IngredientMeasureTypes = ingredientMeasureTypes
        member this.IngredientPrices = ingredientPrices
        member this.Active = active
        member this.HasAllergen = hasAllergen
        member this.UpdatePolicy = updatePolicy
        member this.Stock = stock
        member this.Visible = visible
        member this.Description = description

        member this.AddIngredientPrice (ingredientPrice: IngredientPrice) =
            result {
                let! measureSystemIsCompatible = 
                    this.IngredientMeasureTypes 
                    |> List.contains ingredientPrice.MeasuringSystem
                    |> Result.ofBool "Measuring system is not compatible with ingredient"

                do! 
                    this.IngredientPrices 
                    |> List.contains ingredientPrice
                    |> not
                    |> Result.ofBool "IngredientPrice already exists"
                return Ingredient (id, name, description, ingredientTypeId, ingredientMeasureTypes, active, ingredientPrice :: ingredientPrices, stock, hasAllergen, updatePolicy, visible)
            }

        member this.RemoveIngredientPrice (ingredientPrice: IngredientPrice) =
            result {
                do! 
                    this.IngredientPrices 
                    |> List.contains ingredientPrice
                    |> Result.ofBool "IngredientPrice does not exist"
                return Ingredient (id, name, description, ingredientTypeId, ingredientMeasureTypes, active, ingredientPrices |> List.filter ((<>) ingredientPrice), stock,  hasAllergen, updatePolicy, visible)
            }
        member this.SetAllergen (x: bool) =
            Ingredient (id, name, description, ingredientTypeId, ingredientMeasureTypes, active, ingredientPrices, stock, x, updatePolicy, visible) |> Ok
            
        member this.SetUpdatePolicy (x: UpdatePolicy) =
            Ingredient (id, name, description, ingredientTypeId, ingredientMeasureTypes, active, ingredientPrices, stock, hasAllergen, x, visible) |> Ok

        member this.SetIngredientTypeId (ingredientTypeId: Guid) =
            Ingredient (id, name, description, ingredientTypeId, ingredientMeasureTypes, active, ingredientPrices, stock, hasAllergen, updatePolicy, visible) |> Ok

        member this.UpdateName (newName: string) =
            result {
                do! 
                    newName
                    |> String.IsNullOrWhiteSpace
                    |> not
                    |> Result.ofBool "Name cannot be empty"
                return Ingredient (id, newName, description, ingredientTypeId, ingredientMeasureTypes, active, ingredientPrices, stock, hasAllergen, updatePolicy, visible)
            }
       
        member this.SetVisibility (x: bool) =
            Ingredient (id, name, description, ingredientTypeId, ingredientMeasureTypes, active, ingredientPrices, stock, hasAllergen, updatePolicy, x) |> Ok
        
        member this.IncreaseStock (quantity: float) =
            Ingredient (id, name, description, ingredientTypeId, ingredientMeasureTypes, active, ingredientPrices, stock + quantity, hasAllergen, updatePolicy, visible) |> Ok
        
        member this.DecreaseStock (quantity: float) =
            result {
                do!
                    match updatePolicy with
                    | NoUpdate -> true
                    | UpdateOnOrder -> true 
                    | UpdateOnStock -> 
                        stock - quantity >= 0
                    | UpdateOnOrderAndStock ->
                        stock - quantity >= 0
                    |> Result.ofBool "Stock cannot be negative"
                return Ingredient (id, name, description, ingredientTypeId, ingredientMeasureTypes, active, ingredientPrices, stock - quantity, hasAllergen, updatePolicy, visible) 
                }

        member this.Deactivate () =
            Ingredient (id, name, description, ingredientTypeId, ingredientMeasureTypes, false, ingredientPrices, stock, hasAllergen, updatePolicy, visible) |> Ok

        member this.AddIngredientMeasureType (ingredientMeasure: IngredientMeasureType) =
            result {
                do! 
                    this.IngredientMeasureTypes 
                    |> List.contains ingredientMeasure
                    |> not
                    |> Result.ofBool "IngredientMeasure already exists"
                return Ingredient (id, name, description, ingredientTypeId, ingredientMeasure:: ingredientMeasureTypes, active, ingredientPrices, stock, hasAllergen, updatePolicy, visible)
            }

        member this.RemoveIngredientMeasureType (ingredientMeasure: IngredientMeasureType) =
            result {
                do! 
                    this.IngredientMeasureTypes 
                    |> List.contains ingredientMeasure
                    |> Result.ofBool "IngredientMeasure does not exist"
                return Ingredient (id, name, description, ingredientTypeId, ingredientMeasureTypes |> List.filter ((<>) ingredientMeasure), active, ingredientPrices, stock, hasAllergen, updatePolicy, visible)
            }

        static member SnapshotsInterval =
            15
        static member StorageName =
            "_ingredients"
        static member Version =
            "_01"

        member this.Serialize = 
            this |> globalSerializer.Serialize

        static member Deserialize (json: string): Result<Ingredient, string>  =
            globalSerializer.Deserialize<Ingredient> json

        interface Aggregate<string> with
            member this.Id = id
            member this.Serialize = 
                this.Serialize

