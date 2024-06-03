module OrdersSystem.Models.DishEvents
open OrdersSystem.Models.Dish
open OrdersSystem.Commons
open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core
open Ingredient

type DishEvents =
    | IngredientAndQuantityAdded of IngredientAndQuantity
    | IngredientRemoved of Guid
    | NameUpdated of String
    | InvisibleSet
    | VisibleSet
    | Deactivated
    | DishTypeSet of Guid
    | Updated of string * Guid * List<IngredientAndQuantity> * bool * bool * decimal * List<Guid>
    
        interface Event<Dish>  with
            member this.Process dish =
                match this with
                | IngredientAndQuantityAdded ingredient ->
                    dish.AddIngredientAndQuantity ingredient
                | IngredientRemoved ingredient ->
                    dish.RemoveIngredient ingredient
                | NameUpdated name ->
                    dish.UpdateName name
                | Deactivated -> 
                    dish.Deactivate ()
                | InvisibleSet ->
                    dish.SetInvisible ()
                | VisibleSet ->
                    dish.SetVisible ()
                | DishTypeSet guid ->
                    dish.SetDishType guid
                | Updated (name, dishType, ingredientRefs, active, visible, price, standardComments) ->
                    dish.Update (name, dishType, ingredientRefs, active, visible, price, standardComments)
    
    member this.Serialize =
        globalSerializer.Serialize this
    static member Deserialize json =
        globalSerializer.Deserialize<DishEvents> json