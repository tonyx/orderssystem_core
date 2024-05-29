
module OrdersSystem.Models.UserCommands

open OrdersSystem.Models.User
open OrdersSystem.Models.UserEvents

open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core

type UserCommands =
    | ChangePassword of string
    | Deactivate
    | SetRole of string
    | SetOptionalRoleId of Guid
    | VoidRole 
    | SetDishManager
    | SetOrderManager
    | UnSetDishManager
    | UnSetOrderManager
    | Update of  Guid * string * string * string * bool * bool * Option<Guid> * bool * bool 
        interface Command<User, UserEvents> with
            member this.Execute user = 
                match this with
                | ChangePassword newPassword -> 
                    user.ChangePassword newPassword
                    |> Result.map (fun _ -> [PasswordChanged newPassword])
                | Deactivate -> 
                    user.Deactivate ()
                    |> Result.map (fun _ -> [Deactivated])
                | SetRole role ->
                    user.SetRole role
                    |> Result.map (fun _ -> [RoleSet role])
                | SetOptionalRoleId role ->
                    user.SetOptionalRoleId role
                    |> Result.map (fun _ -> [OptionalRoleIdSet role])
                | VoidRole ->
                    user.VoidOptionalRoleId ()
                    |> Result.map (fun _ -> [OptionalRoleIdVoided ])
                | SetDishManager ->
                    user.SetDishManager ()
                    |> Result.map (fun _ -> [DishManagerSet])
                | SetOrderManager ->
                    user.SetOrderManager ()
                    |> Result.map (fun _ -> [OrderManagerSet])
                | UnSetDishManager ->
                    user.UnSetDishManager ()
                    |> Result.map (fun _ -> [DishManagerUnSet])
                | UnSetOrderManager ->
                    user.UnSetOrderManager ()
                    |> Result.map (fun _ -> [OrderManagerUnSet])
                | Update (guid, username, newPassword, role, active, temporary, optRoleId, canManageAllDishes, canManageAllOrders) ->
                    user.Update (guid, username, newPassword, role, active, temporary, optRoleId, canManageAllDishes, canManageAllOrders) 
                    |> Result.map (fun _ -> [Updated(guid, username, newPassword, role, active, temporary, optRoleId, canManageAllDishes, canManageAllOrders)])

            member this.Undoer = None
                    