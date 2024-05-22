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

    type IngredientTypes =
        | Meat
        | Fish
        | Vegetable
        | Fruit
        | Dairy
        | Grain
        | Other of string

    type IngredientTO =
        {
            Id: Guid
            Name: string
            IngredientTypes: List<IngredientTypes>
            IngredientMeasureTypes: List<IngredientMeasureType>
        }

    type Ingredient private (id: Guid, name: string, ingredientTypes: List<IngredientTypes>, ingredientMeasureTypes: List<IngredientMeasureType>, active: bool) =

        let stateId = Guid.NewGuid()
        member this.Id = id
        member this.Name = name
        member this.IngredientTypes = ingredientTypes
        member this.IngredientMeasureTypes = ingredientMeasureTypes
        member this.Active = active

        new (id: Guid, name: string, ingredientTypes: List<IngredientTypes>, ingredientMeasures: List<IngredientMeasureType>) =
            Ingredient (id, name, ingredientTypes, ingredientMeasures, true)

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

        member this.Deactivate () =
            Ingredient (id, name, ingredientTypes, ingredientMeasureTypes, false) |> Ok

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
            }
        static member FromIngredientTO (ingredientTO: IngredientTO) =
            Ingredient (ingredientTO.Id, ingredientTO.Name, ingredientTO.IngredientTypes, ingredientTO.IngredientMeasureTypes)

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

