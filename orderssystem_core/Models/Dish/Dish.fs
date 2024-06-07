
module OrdersSystem.Models.Dish
open OrdersSystem.Commons
open OrdersSystem.Shared
open OrdersSystem.Models.Ingredient
open System
open Sharpino
open FSharpPlus
open FsToolkit.ErrorHandling
open Sharpino.Core
    

    type IngredientMeasureItemType =
        | Specific of  IngredientMeasureType * float
        | Other of string

    type IngredientAndQuantity =
        {
            IngredientId: Guid
            Quantity: IngredientMeasureItemType
        }

    type Dish
        (id: Guid,
         name: string,
         description: Option<string>,
         dishType: Guid,
         ingredientAndQuantities: List<IngredientAndQuantity>,
         active: bool,
         visible: bool,
         price: decimal,
         standardComments: List<Guid>,
         standardVariations: List<Guid>
         ) =

        member this.Id = id
        member this.Name = name

        member this.IngredientAndQuantities = ingredientAndQuantities

        member this.Visible = visible
        member this.Active = active
        member this.DishType = dishType
        member this.Price = price
        member this.StandardComments = standardComments
        member this.StandardVariations = standardVariations
        member this.Description = description
        
        new (id: Guid, name: string, description: Option<string>, dishType: Guid, ingredientRefs: List<IngredientAndQuantity>, price: decimal) =
            Dish (id, name, description, dishType, ingredientRefs, true, true, price, [], []) 

        member this.SetVisible () =
            Dish (id, name, description, dishType, ingredientAndQuantities, this.Active, true, price, standardComments, []) |> Ok

        member this.SetInvisible () =
            Dish (id, name, description, dishType, ingredientAndQuantities, this.Active, false, price, standardComments, []) |> Ok            

        member this.SetDishType (newDishType: Guid) =
            Dish (id, name, description, newDishType, ingredientAndQuantities, this.Active, this.Visible, price, standardComments, []) |> Ok 

        member this.Deactivate () =
            Dish (id, name, description, dishType, ingredientAndQuantities, false, this.Visible, price, standardComments, []) |> Ok
        member this.UpdateName (newName: string) =
            result {
                do! 
                    newName
                    |> String.IsNullOrWhiteSpace
                    |> not
                    |> Result.ofBool "Name cannot be empty"
                return Dish (id, newName, description, dishType,  ingredientAndQuantities, this.Active, this.Visible, price, standardComments, [])
            }
            
        member this.Update (name: string, dishType: Guid, ingredientAndQuantities: List<IngredientAndQuantity>, active: bool, visible: bool, price: decimal, standardComments: List<Guid>, standardVariations: List<Guid> ) =
            result {
                do! 
                    name
                    |> String.IsNullOrWhiteSpace
                    |> not
                    |> Result.ofBool "Name cannot be empty"
                do!
                    price > 0.0M
                    |> Result.ofBool "Price must be positive"
                return Dish (id, name, description, dishType, ingredientAndQuantities, active, visible, price, standardComments, standardVariations)
            }
            
        member this.AddIngredientAndQuantity (ingredientAndQuantity: IngredientAndQuantity) =
            result {
                do! 
                    this.IngredientAndQuantities 
                    |>> (fun x -> x.IngredientId)
                    |> List.contains ingredientAndQuantity.IngredientId
                    |> not
                    |> Result.ofBool "Ingredient already exists"
                return Dish (id, name, description, dishType, ingredientAndQuantity :: ingredientAndQuantities, this.Active, this.Visible, price, standardComments, standardVariations)
            }
        member this.RemoveIngredient (ingredientId: Guid) =
            result {
                do! 
                    this.IngredientAndQuantities 
                    |>> (fun x -> x.IngredientId)
                    |> List.contains ingredientId
                    |> Result.ofBool "Ingredient does not exist"
                return Dish (id, name, description, dishType, ingredientAndQuantities |> List.filter (fun x -> x.IngredientId <> ingredientId), this.Active, this.Visible, price, standardComments, standardVariations)
            }
        member this.AddStandardComment (standardCommentId: Guid) =
            result {
                do! 
                    this.StandardComments
                    |> List.contains standardCommentId
                    |> not
                    |> Result.ofBool "StandardComment already exists"
                return Dish (id, name, description, dishType, ingredientAndQuantities, this.Active, this.Visible, price, standardCommentId :: standardComments, standardVariations)
            }
        member this.RemoveStandardComment (standardCommentId: Guid) =
            result {
                do! 
                    this.StandardComments
                    |> List.contains standardCommentId
                    |> Result.ofBool "StandardComment does not exist"
                return Dish (id, name, description, dishType, ingredientAndQuantities, this.Active, this.Visible, price, standardComments |> List.filter ((<>) standardCommentId), standardVariations)
            }
        member this.AddStandardVariation (standardVariationId: Guid) =
            result {
                do! 
                    this.StandardVariations
                    |> List.contains standardVariationId
                    |> not
                    |> Result.ofBool "StandardVariation already exists"
                return Dish (id, name, description, dishType, ingredientAndQuantities, this.Active, this.Visible, price, standardComments, standardVariationId :: standardVariations)
            }     

        member this.RemoveStandardVariation (standardVariationId: Guid) =
            result {
                do! 
                    this.StandardVariations
                    |> List.contains standardVariationId
                    |> Result.ofBool "StandardVariation does not exist"
                return Dish (id, name, description, dishType, ingredientAndQuantities, this.Active, this.Visible, price, standardComments, standardVariations |> List.filter ((<>) standardVariationId))
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
