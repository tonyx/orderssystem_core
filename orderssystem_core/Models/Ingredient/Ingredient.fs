module OrdersSystem.Models.Ingredient
open OrdersSystem.Commons
open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core

    type IngredientMeasures =
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
            IngredientMeasures: List<IngredientMeasures>
        }


    type Ingredient private (id: Guid, name: string, ingredientTypes: List<IngredientTypes>, ingredientMeasures: List<IngredientMeasures>, active: bool) =

        let stateId = Guid.NewGuid()
        member this.Id = id
        member this.Name = name
        member this.IngredientTypes = ingredientTypes
        member this.IngredientMeasures = ingredientMeasures
        member this.Active = active

        new (id: Guid, name: string, ingredientTypes: List<IngredientTypes>, ingredientMeasures: List<IngredientMeasures>) =
            Ingredient (id, name, ingredientTypes, ingredientMeasures, true)

        member this.AddIngredientType (ingredientType: IngredientTypes) =
            result {
                do! 
                    this.IngredientTypes 
                    |> List.contains ingredientType
                    |> not
                    |> Result.ofBool "IngredientType already exists"
                return Ingredient (id, name, ingredientType :: ingredientTypes, ingredientMeasures)
            }

        member this.RemoveIngredientType (ingredientType: IngredientTypes) =
            result {
                do! 
                    this.IngredientTypes 
                    |> List.contains ingredientType
                    |> Result.ofBool "IngredientType does not exist"
                return Ingredient (id, name, ingredientTypes |> List.filter ((<>) ingredientType), ingredientMeasures)
            }

        member this.UpdateName (newName: string) =
            result {
                do! 
                    newName
                    |> String.IsNullOrWhiteSpace
                    |> not
                    |> Result.ofBool "Name cannot be empty"
                return Ingredient (id, newName, ingredientTypes, ingredientMeasures)
            }

        member this.Deactivate () =
            Ingredient (id, name, ingredientTypes, ingredientMeasures, false) |> Ok

        member this.AddIngredientMeasure (ingredientMeasure: IngredientMeasures) =
            result {
                do! 
                    this.IngredientMeasures 
                    |> List.contains ingredientMeasure
                    |> not
                    |> Result.ofBool "IngredientMeasure already exists"
                return Ingredient (id, name, ingredientTypes, ingredientMeasure :: ingredientMeasures)
            }

        member this.RemoveIngredientMeasure (ingredientMeasure: IngredientMeasures) =
            result {
                do! 
                    this.IngredientMeasures 
                    |> List.contains ingredientMeasure
                    |> Result.ofBool "IngredientMeasure does not exist"
                return Ingredient (id, name, ingredientTypes, ingredientMeasures |> List.filter ((<>) ingredientMeasure))
            }
        member this.ToIngredienTO  =
            {   
                Id = this.Id; 
                Name = this.Name; 
                IngredientTypes = this.IngredientTypes; 
                IngredientMeasures = this.IngredientMeasures 
            }
        static member FromIngredientTO (ingredientTO: IngredientTO) =
            Ingredient (ingredientTO.Id, ingredientTO.Name, ingredientTO.IngredientTypes, ingredientTO.IngredientMeasures)

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

