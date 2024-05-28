
module OrdersSystem.Models.UserEvents
open OrdersSystem.Models.User
open OrdersSystem.Commons
open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core

type UserEvents =
    | PasswordChanged of string
    | Deactivated
    | RoleSet of Guid
    | RoleVoided 
    | DishManagerSet
    | DishManagerUnSet
    | OrderManagerSet
    | OrderManagerUnSet
    | Updated of  Guid * string * string * bool * bool * Option<Guid> * bool * bool 
        interface Event<User>  with
            member this.Process user =
                match this with
                | PasswordChanged newPassword -> 
                    user.ChangePassword newPassword
                | Deactivated ->
                    user.Deactivate ()
                | RoleSet role ->
                    user.SetRole role
                | RoleVoided  ->
                    user.VoidRole ()
                | DishManagerSet ->
                    user.SetDishManager ()
                | DishManagerUnSet ->
                    user.UnSetDishManager ()
                | OrderManagerSet ->
                    user.SetOrderManager ()
                | OrderManagerUnSet ->
                    user.UnSetOrderManager ()
                | Updated (guid, username, newPassword, active, temporary, role, canManageAllDishes, canManageAllOrders) ->
                    user.Update(guid, username, newPassword, active, temporary, role, canManageAllDishes, canManageAllOrders)
        member this.Serialize = 
            globalSerializer.Serialize this
        static member Deserialize json = 
            globalSerializer.Deserialize<UserEvents> json