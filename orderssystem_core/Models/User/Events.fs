
module OrdersSystem.Models.UserEvents
open OrdersSystem.Models.User
open OrdersSystem.Commons
open System
open OrdersSystem.Shared
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core

type UserEvents =
    | PasswordChanged of string
    | Deactivated
    | RoleSet of string
    | OptionalRoleIdSet of Guid
    | OptionalRoleIdVoided 
    | DishManagerSet
    | DishManagerUnSet
    | OrderManagerSet
    | OrderManagerUnSet
    | Updated of  Guid * string * string * string * bool * Option<TemporaryUser> * Option<Guid> * bool * bool 
        interface Event<User>  with
            member this.Process user =
                match this with
                | PasswordChanged newPassword -> 
                    user.ChangePassword newPassword
                | Deactivated ->
                    user.Deactivate ()
                | OptionalRoleIdSet role ->
                    user.SetOptionalRoleId role
                | RoleSet role ->
                    user.SetRole role
                | OptionalRoleIdVoided  ->
                    user.VoidOptionalRoleId ()
                | DishManagerSet ->
                    user.SetDishManager ()
                | DishManagerUnSet ->
                    user.UnSetDishManager ()
                | OrderManagerSet ->
                    user.SetOrderManager ()
                | OrderManagerUnSet ->
                    user.UnSetOrderManager ()
                | Updated (guid, username, newPassword, role, active, temporary, optRoleId, canManageAllDishes, canManageAllOrders) ->
                    user.Update(guid, username, newPassword, role, active, temporary, optRoleId, canManageAllDishes, canManageAllOrders)
        member this.Serialize = 
            globalSerializer.Serialize this
        static member Deserialize json = 
            globalSerializer.Deserialize<UserEvents> json