
module OrdersSystem.Models.Dish
open OrdersSystem.Commons
open System
open Sharpino
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

    type Dish private (id: Guid, name: string, dishTypes: List<DishTypes>, ingredientRefs: List<Guid>, active: bool) =

        member this.Id = id
        member this.Name = name
        member this.IngredientRefs = ingredientRefs
        member this.DishTypes = dishTypes

        member this.Active = active


        new (id: Guid, name: string, dishTypes: List<DishTypes>, ingredientRefs: List<Guid>) =
            Dish (id, name, dishTypes, ingredientRefs, true) 

        member this.AddDishType (dishType: DishTypes) =
            result {
                do! 
                    this.DishTypes 
                    |> List.contains dishType
                    |> not
                    |> Result.ofBool "DishType already exists"
                return Dish (id, name, dishType :: dishTypes, this.IngredientRefs)
            }

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
                return Dish (id, name, dishTypes |> List.filter ((<>) dishType), this.IngredientRefs)
            }

        member this.Deactivate () =
            Dish (id, name, dishTypes, ingredientRefs, false) |> Ok
        member this.UpdateName (newName: string) =
            result {
                do! 
                    newName
                    |> String.IsNullOrWhiteSpace
                    |> not
                    |> Result.ofBool "Name cannot be empty"
                return Dish (id, newName, dishTypes, this.IngredientRefs)
            }
        member this.AddIngredient (ingredientId: Guid) =
            result {
                do! 
                    this.IngredientRefs 
                    |> List.contains ingredientId
                    |> not
                    |> Result.ofBool "Ingredient already exists"
                return Dish (id, name, dishTypes, ingredientId :: ingredientRefs)
            }
        member this.RemoveIngredient (ingredientId: Guid) =
            result {
                do! 
                    this.IngredientRefs 
                    |> List.contains ingredientId
                    |> Result.ofBool "Ingredient does not exist"
                return Dish (id, name, dishTypes, ingredientRefs |> List.filter ((<>) ingredientId))
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
