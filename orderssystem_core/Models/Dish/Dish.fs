
module OrdersSystem.Models.Dish
open OrdersSystem.Commons
open OrdersSystem.Shared
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

    type Dish (id: Guid, name: string, dishType: Guid, ingredientAndQuantities: List<IngredientAndQuantity>, active: bool, visible: bool, price: decimal, standardComments: List<Guid>) =

        member this.Id = id
        member this.Name = name

        member this.IngredientAndQuantities = ingredientAndQuantities

        member this.Visible = visible
        member this.Active = active
        member this.DishType = dishType
        member this.Price = price
        member this.StandardComments = standardComments
        
        new (id: Guid, name: string, dishType: Guid, ingredientRefs: List<IngredientAndQuantity>, price: decimal) =
            Dish (id, name, dishType, ingredientRefs, true, true, price, []) 

        member this.SetVisible () =
            Dish (id, name, dishType, ingredientAndQuantities, this.Active, true, price, standardComments) |> Ok

        member this.SetInvisible () =
            Dish (id, name, dishType, ingredientAndQuantities, this.Active, false, price, standardComments) |> Ok            

        member this.SetDishType (newDishType: Guid) =
            Dish (id, name, newDishType, ingredientAndQuantities, this.Active, this.Visible, price, standardComments) |> Ok 

        member this.Deactivate () =
            Dish (id, name, dishType, ingredientAndQuantities, false, this.Visible, price, standardComments) |> Ok
        member this.UpdateName (newName: string) =
            result {
                do! 
                    newName
                    |> String.IsNullOrWhiteSpace
                    |> not
                    |> Result.ofBool "Name cannot be empty"
                return Dish (id, newName, dishType,  ingredientAndQuantities, this.Active, this.Visible, price, standardComments)
            }
            
        member this.Update (name: string, dishType: Guid, ingredientAndQuantities: List<IngredientAndQuantity>, active: bool, visible: bool, price: decimal, standardComments: List<Guid>) =
            result {
                do! 
                    name
                    |> String.IsNullOrWhiteSpace
                    |> not
                    |> Result.ofBool "Name cannot be empty"
                do!
                    price > 0.0M
                    |> Result.ofBool "Price must be positive"
                return Dish (id, name, dishType, ingredientAndQuantities, active, visible, price, standardComments)
            }
            
        member this.AddIngredientAndQuantity (ingredientAndQuantity: IngredientAndQuantity) =
            result {
                do! 
                    this.IngredientAndQuantities 
                    |>> (fun x -> x.IngredientId)
                    |> List.contains ingredientAndQuantity.IngredientId
                    |> not
                    |> Result.ofBool "Ingredient already exists"
                return Dish (id, name, dishType, ingredientAndQuantity :: ingredientAndQuantities, this.Active, this.Visible, price, standardComments)
            }
        member this.RemoveIngredient (ingredientId: Guid) =
            result {
                do! 
                    this.IngredientAndQuantities 
                    |>> (fun x -> x.IngredientId)
                    |> List.contains ingredientId
                    |> Result.ofBool "Ingredient does not exist"
                return Dish (id, name, dishType, ingredientAndQuantities |> List.filter (fun x -> x.IngredientId <> ingredientId), this.Active, this.Visible, price, standardComments)
            }
        member this.AddStandardComment (standardCommentId: Guid) =
            result {
                do! 
                    this.StandardComments
                    |> List.contains standardCommentId
                    |> not
                    |> Result.ofBool "StandardComment already exists"
                return Dish (id, name, dishType, ingredientAndQuantities, this.Active, this.Visible, price, standardCommentId :: standardComments)
            }
        member this.RemoveStandardComment (standardCommentId: Guid) =
            result {
                do! 
                    this.StandardComments
                    |> List.contains standardCommentId
                    |> Result.ofBool "StandardComment does not exist"
                return Dish (id, name, dishType, ingredientAndQuantities, this.Active, this.Visible, price, standardComments |> List.filter ((<>) standardCommentId))
            }

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
