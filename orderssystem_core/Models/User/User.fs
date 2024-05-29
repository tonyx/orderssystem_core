module OrdersSystem.Models.User
open OrdersSystem.Commons
open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core

type User 
    (guid: Guid, 
    username: string, 
    password: string,
    role: string,
    active: bool, 
    temporary: bool, 
    optionalRoleId: Option<Guid>,
    canManageAllDishes: bool,
    canManageAllOrders: bool) = 

    member val CreationDate = DateTime.UtcNow with get
    member val Username = username with get
    // member val Role = role with get
    member this.Role = role
    member val Password = (passHash password) with get
    member val Id = guid with get
    member val Active = active with get
    member val Temporary = temporary with get
    member val CanManageAllDishes = canManageAllDishes with get
    member val CanManageAllOrders = canManageAllOrders with get

    member val OptionalRoleId = optionalRoleId

    member this.SetOptionalRoleId (optRolId: Guid) =
        User (guid, username, password, role, active, temporary, Some optRolId, canManageAllDishes, canManageAllOrders) |> Ok
    
    member this.SetRole (rol: string) =
        User (guid, username, password, rol, active, temporary, this.OptionalRoleId, canManageAllDishes, canManageAllOrders) |> Ok 
    
    member this.VoidOptionalRoleId () =
        User (guid, username, password, role, active, temporary, None, canManageAllDishes, canManageAllOrders) |> Ok

    static member Validate (username: string, password: string) = 
        if username.Length < 3 then
            Error "Username must be at least 3 characters long"
        elif password.Length < 6 then
            Error "Password must be at least 6 characters long"
        else
            Ok ()
    member this.Deactivate () =
        User(guid, username, password, role, false, true, this.OptionalRoleId, canManageAllDishes, canManageAllOrders) |> Ok

    member this.SetDishManager () =
        User(guid, username, password, role, active, temporary, this.OptionalRoleId, true, canManageAllOrders) |> Ok

    member this.SetOrderManager () =
        User(guid, username, password, role, active, temporary, this.OptionalRoleId, canManageAllDishes, true) |> Ok

    member this.UnSetDishManager () =
        User(guid, username, password, role, active, temporary, this.OptionalRoleId, false, canManageAllOrders) |> Ok

    member this.UnSetOrderManager () =
        User(guid, username, password, role, active, temporary, this.OptionalRoleId, canManageAllDishes, false) |> Ok

    member this.Authenticate (password: string) = 
        if this.Password = password then
            Ok this
        else
            Error "Invalid password"

    member this.ChangePassword (newPassword: string) =
        result {
            let! _ = User.Validate(this.Username, newPassword)
            return User(guid, this.Username, newPassword, role, active, temporary, this.OptionalRoleId, canManageAllDishes, canManageAllOrders)
        }
        
    member this.Update (id, username, password, role, isActive, isTemporary, optRoleId, canManageAllDishes, canManageAllOrders) =
        result {
            let! idMatches =
                this.Id = id
                |> Result.ofBool "User id does not match"
            let! _ = User.Validate(username, password)
            return User(id, username, password, role, isActive, isTemporary, optRoleId, canManageAllDishes, canManageAllOrders)
        }

    static member MkUser (username: string, password: string, role: string) =
        result {
            let! _ = User.Validate(username, password)
            return User(Guid.NewGuid(), username, password, role, active = true, temporary = false, optionalRoleId = None, canManageAllDishes = true, canManageAllOrders = true)
        }

    static member MkTemporaryUser (username: string, password: string) =
        result {
            let! _ = User.Validate(username, password)
            return User(Guid.NewGuid(), username, password, "temporary", active = true, temporary = true, optionalRoleId = None, canManageAllDishes = true, canManageAllOrders =  true)
        }

    override this.ToString() = sprintf "User(%s)" this.Username

    override this.Equals(obj) =
        match obj with
        | :? User as u ->
             u.Id = this.Id && 
             u.Username = this.Username && 
             u.Password = this.Password && 
             u.Active = this.Active && 
             u.Temporary = this.Temporary &&
             u.OptionalRoleId = this.OptionalRoleId &&
             u.CanManageAllDishes = this.CanManageAllDishes &&
             u.CanManageAllOrders = this.CanManageAllOrders
        | _ -> false
    override this.GetHashCode() = 
        hash (this.Id, this.Username, this.Password, this.Active, this.Temporary, this.OptionalRoleId, this.CanManageAllDishes, this.CanManageAllOrders)

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












