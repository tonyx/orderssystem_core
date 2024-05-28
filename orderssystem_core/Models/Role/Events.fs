
module OrdersSystem.Models.RoleEvents
open OrdersSystem.Models.Dish
open OrdersSystem.Models.Role
open OrdersSystem.Commons
open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core
open Ingredient

type RoleEvents =
    | EnablerAdded of DishTypes
    | ObserverAdded of DishTypes
    | EnablerRemoved of DishTypes
    | ObserverRemoved of DishTypes
    | Deactivated

    interface Event<Role> with
        member this.Process role =
            match this with
            | EnablerAdded dishType ->
                role.AddEnabler dishType
            | ObserverAdded dishType ->
                role.AddObserver dishType
            | EnablerRemoved dishType ->
                role.RemoveEnabler dishType
            | ObserverRemoved dishType ->
                role.RemoveObserver dishType
            | Deactivated ->
                role.Deactivate ()

    member this.Serialize =
        globalSerializer.Serialize this
    static member Deserialize json =
        globalSerializer.Deserialize<RoleEvents> json