module OrdersSystem.Models.User
open OrdersSystem.Commons
open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core

type User private(guid: Guid, username: string, password: string) = 
    member val Username = username with get
    member val Password = (passHash password) with get
    member val Id = guid with get

    static member Validate(username: string, password: string) = 
        if username.Length < 3 then
            Error "Username must be at least 3 characters long"
        elif password.Length < 6 then
            Error "Password must be at least 6 characters long"
        else
            Ok ()

    member this.Authenticate(password: string) = 
        if this.Password = password then
            Ok this
        else
            Error "Invalid password"

    member this.ChangePassword(newPassword: string) =
        result {
            let! _ = User.Validate(this.Username, newPassword)
            return User(guid, this.Username, newPassword)
        }

    static member mkUser(username: string, password: string) =
        result {
            let! _ = User.Validate(username, password)
            return User(Guid.NewGuid(), username, password)
        }

    override this.ToString() = sprintf "User(%s)" this.Username

    override this.Equals(obj) =
        match obj with
        | :? User as u -> u.Id = this.Id && u.Username = this.Username
        | _ -> false
    override this.GetHashCode() = 
        hash (this.Id, this.Username)

    static member Deserialize(json: string) =
        globalSerializer.Deserialize<User> json

    static member StorageName = "_users"
    static member Version = "_01"
    static member SnapshotsInterval = 15

    member this.Serialize =
        globalSerializer.Serialize this 

    interface Aggregate<string> with
        member this.Id = this.Id
        member this.Serialize = this.Serialize












