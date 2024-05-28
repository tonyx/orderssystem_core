
module OrdersSystem.Models.Dish
open OrdersSystem.Commons
open System
open Sharpino
open FSharpPlus
open FsToolkit.ErrorHandling
open Sharpino.Core

    type DishTypes =
        | Starter
        | Main
        | Dessert
        | Drink
        | Other of string

    type DishTO =
        {
            Id: Guid
            Name: string
            DishTypes: List<DishTypes>
        }

    type IngredientMeasureItemType =
        | Grams of float
        | Kilograms of float
        | Liters of float
        | Milliliters of float
        | Pieces of int
        | Other of string 

    type IngredientAndQuantity =
        {
            IngredientId: Guid
            PossibleAlternativeQuantities: List<IngredientMeasureItemType>
        }

    type Dish private (id: Guid, name: string, dishTypes: List<DishTypes>, ingredientAndQuantities: List<IngredientAndQuantity>, active: bool, visible: bool) =

        member this.Id = id
        member this.Name = name

        member this.IngredientAndQuantities = ingredientAndQuantities

        member this.DishTypes = dishTypes
        member this.Visible = visible
        member this.Active = active

        new (id: Guid, name: string, dishTypes: List<DishTypes>, ingredientRefs: List<IngredientAndQuantity>) =
            Dish (id, name, dishTypes, ingredientRefs, true, true) 

        member this.AddDishType (dishType: DishTypes) =
            result {
                do! 
                    this.DishTypes 
                    |> List.contains dishType
                    |> not
                    |> Result.ofBool "DishType already exists"
                return Dish (id, name, dishType :: dishTypes, this.IngredientAndQuantities, this.Active, this.Visible)
            }

        // todo: DishTO should have a visible field
        member this.Update (dishTo: DishTO) =
            result {
                do! 
                    dishTo.Id = this.Id
                    |> Result.ofBool "Dish Id does not match"
                let dish = Dish (id, dishTo.Name, dishTo.DishTypes, this.IngredientAndQuantities, this.Active, this.Visible)
                return dish
            }
            
        member this.SetVisible () =
            Dish (id, name, dishTypes, ingredientAndQuantities, this.Active, true) |> Ok

        member this.SetInvisible () =
            Dish (id, name, dishTypes, ingredientAndQuantities, this.Active, false) |> Ok            

        member this.RemoveDishType (dishType: DishTypes) =
            result {
                do! 
                    this.DishTypes 
                    |> List.contains dishType
                    |> Result.ofBool "DishType does not exist"
                do! 
                    this.DishTypes 
                    |> List.length > 1
                    |> Result.ofBool "Dish must have at least one type"
                return Dish (id, name, dishTypes |> List.filter ((<>) dishType), this.IngredientAndQuantities, this.Active, this.Visible)
            }

        member this.Deactivate () =
            Dish (id, name, dishTypes, ingredientAndQuantities, false, this.Visible) |> Ok
        member this.UpdateName (newName: string) =
            result {
                do! 
                    newName
                    |> String.IsNullOrWhiteSpace
                    |> not
                    |> Result.ofBool "Name cannot be empty"
                return Dish (id, newName, dishTypes, this.IngredientAndQuantities)
            }
        member this.AddIngredientAndQuantity (ingredientAndQuantity: IngredientAndQuantity) =
            result {
                do! 
                    this.IngredientAndQuantities 
                    |>> (fun x -> x.IngredientId)
                    |> List.contains ingredientAndQuantity.IngredientId
                    |> not
                    |> Result.ofBool "Ingredient already exists"
                return Dish (id, name, dishTypes, ingredientAndQuantity :: ingredientAndQuantities, this.Active, this.Visible)
            }
        member this.RemoveIngredient (ingredientId: Guid) =
            result {
                do! 
                    this.IngredientAndQuantities 
                    |>> (fun x -> x.IngredientId)
                    |> List.contains ingredientId
                    |> Result.ofBool "Ingredient does not exist"
                return Dish (id, name, dishTypes, ingredientAndQuantities |> List.filter (fun x -> x.IngredientId <> ingredientId), this.Active, this.Visible)
            }
        member this.ToDishTO  =
            { Id = id; Name = name; DishTypes = dishTypes }

        static member FromDishTO (dishTO: DishTO) =
            Dish (dishTO.Id, dishTO.Name, dishTO.DishTypes, [])

        member this.Serialize =
            this |> globalSerializer.Serialize

        static member SnapshotsInterval =
            15
        static member StorageName =
            "_dishes"
        static member Version =
            "_01"

        static member Deserialize x: Result<Dish, string>  =
            globalSerializer.Deserialize<Dish> x

        interface Aggregate<string> with
            member this.Id = id
            member this.Serialize =
                this.Serialize
