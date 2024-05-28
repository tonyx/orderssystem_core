
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
    | SetRole of Guid
    | VoidRole 
    | SetDishManager
    | SetOrderManager
    | UnSetDishManager
    | UnSetOrderManager
    | Update of  Guid * string * string * bool * bool * Option<Guid> * bool * bool 
        interface Command<User, UserEvents> with
            member this.Execute user = 
                match this with
                | ChangePassword newPassword -> 
                    user.ChangePassword newPassword
                    |> Result.map (fun user -> [PasswordChanged newPassword])
                | Deactivate -> 
                    user.Deactivate ()
                    |> Result.map (fun user -> [Deactivated])
                | SetRole role ->
                    user.SetRole role
                    |> Result.map (fun user -> [RoleSet role])
                | VoidRole ->
                    user.VoidRole ()
                    |> Result.map (fun user -> [RoleVoided ])
                | SetDishManager ->
                    user.SetDishManager ()
                    |> Result.map (fun user -> [DishManagerSet])
                | SetOrderManager ->
                    user.SetOrderManager ()
                    |> Result.map (fun user -> [OrderManagerSet])
                | UnSetDishManager ->
                    user.UnSetDishManager ()
                    |> Result.map (fun user -> [DishManagerUnSet])
                | UnSetOrderManager ->
                    user.UnSetOrderManager ()
                    |> Result.map (fun _ -> [OrderManagerUnSet])
                | Update (guid, username, newPassword, active, temporary, role, canManageAllDishes, canManageAllOrders) ->
                    user.Update (guid, username, newPassword, active, temporary, role, canManageAllDishes, canManageAllOrders) 
                    |> Result.map (fun _ -> [Updated(guid, username, newPassword, active, temporary, role, canManageAllDishes, canManageAllOrders)])

            member this.Undoer = None
                    