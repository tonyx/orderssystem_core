
module OrdersSystem.Models.UserCommands

open OrdersSystem.Models.User
open OrdersSystem.Models.UserEvents

open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core

type UserCommands =
    | ChangePassword of string

        interface Command<User, UserEvents> with
            member this.Execute user = 
                match this with
                | ChangePassword newPassword -> 
                    user.ChangePassword newPassword
                    |> Result.map (fun user -> [PasswordChanged newPassword])
            member this.Undoer = None
                    


