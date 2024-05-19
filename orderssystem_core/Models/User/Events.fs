
module OrdersSystem.Models.UserEvents
open OrdersSystem.Models.User
open OrdersSystem.Commons
open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core

type UserEvents =
    | PasswordChanged of string
        interface Event<User>  with
            member this.Process user =
                match this with
                | PasswordChanged newPassword -> 
                    user.ChangePassword newPassword
        member this.Serialize = 
            globalSerializer.Serialize this
        static member Deserialize json = 
            globalSerializer.Deserialize<UserEvents> json