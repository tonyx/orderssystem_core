module OrdersSystem.Models.Ingredient
open OrdersSystem.Commons
open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core

    type IngredientMeasureType =
        | Grams
        | Kilograms
        | Liters
        | Milliliters
        | Pieces
        | Other of string

    type IngredientCategories =
        | Meat
        | Fish
        | Vegetable
        | Fruit
        | Dairy
        | Grain
        | Other of string

    type IngredientTypes =
        | Meat
        | Fish
        | Vegetable
        | Fruit
        | Dairy
        | Grain
        | Other of string

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

    type IngredientTO =
        {
            Id: Guid
            Name: string
            IngredientTypes: List<IngredientTypes>
            IngredientMeasureTypes: List<IngredientMeasureType>
            Active: bool
            IngredientPrices: List<IngredientPrice>

        }

    type Ingredient private (id: Guid, name: string, ingredientTypes: List<IngredientTypes>, ingredientMeasureTypes: List<IngredientMeasureType>, active: bool, ingredientPrices: List<IngredientPrice>) =
        let stateId = Guid.NewGuid()
        member this.Id = id
        member this.Name = name
        member this.IngredientTypes = ingredientTypes
        member this.IngredientMeasureTypes = ingredientMeasureTypes
        member this.IngredientPrices = ingredientPrices
        member this.Active = active

        new (id: Guid, name: string, ingredientTypes: List<IngredientTypes>, ingredientMeasures: List<IngredientMeasureType>) =
            Ingredient (id, name, ingredientTypes, ingredientMeasures, true, [])


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
                return Ingredient (id, name, ingredientTypes, ingredientMeasureTypes, active, ingredientPrice :: ingredientPrices)
            }

        member this.RemoveIngredientPrice (ingredientPrice: IngredientPrice) =
            result {
                do! 
                    this.IngredientPrices 
                    |> List.contains ingredientPrice
                    |> Result.ofBool "IngredientPrice does not exist"
                return Ingredient (id, name, ingredientTypes, ingredientMeasureTypes, active, ingredientPrices |> List.filter ((<>) ingredientPrice))
            }

        member this.AddIngredientType (ingredientType: IngredientTypes) =
            result {
                do! 
                    this.IngredientTypes 
                    |> List.contains ingredientType
                    |> not
                    |> Result.ofBool "IngredientType already exists"
                return Ingredient (id, name, ingredientType :: ingredientTypes, ingredientMeasureTypes)
            }

        member this.RemoveIngredientType (ingredientType: IngredientTypes) =
            result {
                do! 
                    this.IngredientTypes 
                    |> List.contains ingredientType
                    |> Result.ofBool "IngredientType does not exist"
                return Ingredient (id, name, ingredientTypes |> List.filter ((<>) ingredientType), ingredientMeasureTypes)
            }

        member this.UpdateName (newName: string) =
            result {
                do! 
                    newName
                    |> String.IsNullOrWhiteSpace
                    |> not
                    |> Result.ofBool "Name cannot be empty"
                return Ingredient (id, newName, ingredientTypes, ingredientMeasureTypes)
            }

        member this.Update (ingredient: Ingredient) =
            result {
                do! 
                    ingredient.Id = this.Id
                    |> Result.ofBool "Ingredient Id does not match"
                return ingredient
            }

        member this.Deactivate () =
            Ingredient (id, name, ingredientTypes, ingredientMeasureTypes, false, ingredientPrices) |> Ok

        member this.AddIngredientMeasureType (ingredientMeasure: IngredientMeasureType) =
            result {
                do! 
                    this.IngredientMeasureTypes 
                    |> List.contains ingredientMeasure
                    |> not
                    |> Result.ofBool "IngredientMeasure already exists"
                return Ingredient (id, name, ingredientTypes, ingredientMeasure :: ingredientMeasureTypes)
            }

        member this.RemoveIngredientMeasureType (ingredientMeasure: IngredientMeasureType) =
            result {
                do! 
                    this.IngredientMeasureTypes 
                    |> List.contains ingredientMeasure
                    |> Result.ofBool "IngredientMeasure does not exist"
                return Ingredient (id, name, ingredientTypes, ingredientMeasureTypes |> List.filter ((<>) ingredientMeasure))
            }
        member this.ToIngredienTO  =
            {   
                Id = this.Id; 
                Name = this.Name; 
                IngredientTypes = this.IngredientTypes; 
                IngredientMeasureTypes = this.IngredientMeasureTypes 
                Active = this.Active;
                IngredientPrices = this.IngredientPrices
            }
        static member FromIngredientTO (ingredientTO: IngredientTO) =
            Ingredient (ingredientTO.Id, ingredientTO.Name, ingredientTO.IngredientTypes, ingredientTO.IngredientMeasureTypes, ingredientTO.Active, ingredientTO.IngredientPrices)

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

