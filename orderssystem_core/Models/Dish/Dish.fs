
module OrdersSystem.Models.Dish
open OrdersSystem.Commons
open System
open Sharpino
open FSharpPlus
open FsToolkit.ErrorHandling
open Sharpino.Core
    
    type DishType =
        {
            DishTypeId: Guid
            Name: string
            Visible: bool
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

    type Dish (id: Guid, name: string, dishType: Guid, ingredientAndQuantities: List<IngredientAndQuantity>, active: bool, visible: bool) =

        member this.Id = id
        member this.Name = name

        member this.IngredientAndQuantities = ingredientAndQuantities

        member this.Visible = visible
        member this.Active = active
        member this.DishType = dishType

        new (id: Guid, name: string, dishType: Guid, ingredientRefs: List<IngredientAndQuantity>) =
            Dish (id, name, dishType, ingredientRefs, true, true) 

        member this.SetVisible () =
            Dish (id, name, dishType, ingredientAndQuantities, this.Active, true) |> Ok

        member this.SetInvisible () =
            Dish (id, name, dishType, ingredientAndQuantities, this.Active, false) |> Ok            

        member this.SetDishType (newDishType: Guid) =
            Dish (id, name, newDishType, ingredientAndQuantities, this.Active, this.Visible) |> Ok 

        member this.Deactivate () =
            Dish (id, name, dishType, ingredientAndQuantities, false, this.Visible) |> Ok
        member this.UpdateName (newName: string) =
            result {
                do! 
                    newName
                    |> String.IsNullOrWhiteSpace
                    |> not
                    |> Result.ofBool "Name cannot be empty"
                return Dish (id, newName, dishType, this.IngredientAndQuantities)
            }
        member this.AddIngredientAndQuantity (ingredientAndQuantity: IngredientAndQuantity) =
            result {
                do! 
                    this.IngredientAndQuantities 
                    |>> (fun x -> x.IngredientId)
                    |> List.contains ingredientAndQuantity.IngredientId
                    |> not
                    |> Result.ofBool "Ingredient already exists"
                return Dish (id, name, dishType, ingredientAndQuantity :: ingredientAndQuantities, this.Active, this.Visible)
            }
        member this.RemoveIngredient (ingredientId: Guid) =
            result {
                do! 
                    this.IngredientAndQuantities 
                    |>> (fun x -> x.IngredientId)
                    |> List.contains ingredientId
                    |> Result.ofBool "Ingredient does not exist"
                return Dish (id, name, dishType, ingredientAndQuantities |> List.filter (fun x -> x.IngredientId <> ingredientId), this.Active, this.Visible)
            }
        // member this.ToDishTO  =
        //     { Id = id; Name = name; DishTypes = dishTypes }
        //
        // static member FromDishTO (dishTO: DishTO) =
        //     Dish (dishTO.Id, dishTO.Name, dishTO.DishTypes, [])

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
