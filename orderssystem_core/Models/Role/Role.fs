module OrdersSystem.Models.Role
open OrdersSystem.Commons
open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core
open Dish

type Role (guid: Guid, roleName: string, enabler: List<DishTypes>, observer: List<DishTypes>, active: bool) =
    member this.Id = guid
    member this.RoleName = roleName
    member this.Enabler = enabler
    member this.Observer = observer
    member this.Active = active


    new (guid: Guid, roleName: string) =
        Role (guid, roleName, [], [], true)

    member this.AddEnabler (dishType: DishTypes) =
        result {
            do! 
                this.Enabler 
                |> List.contains dishType
                |> not
                |> Result.ofBool "DishType already exists"
            return Role (guid, roleName, dishType :: enabler, observer, active)
        }

    member this.Deactivate () =
        Role (guid, roleName, enabler, observer, false) |> Ok

    member this.AddObserver (dishType: DishTypes) =
        result {
            do! 
                this.Observer 
                |> List.contains dishType
                |> not
                |> Result.ofBool "DishType already exists"
            return Role (guid, roleName, enabler, dishType :: observer, active)
        }

    member this.RemoveEnabler (dishType: DishTypes) =
        result {
            do! 
                this.Enabler 
                |> List.contains dishType
                |> Result.ofBool "DishType does not exist"
            return Role (guid, roleName, enabler |> List.filter (fun x -> x <> dishType), observer, active)
        }

    member this.RemoveObserver (dishType: DishTypes) =
        result {
            do! 
                this.Observer 
                |> List.contains dishType
                |> Result.ofBool "DishType does not exist"
            return Role (guid, roleName, enabler, observer |> List.filter (fun x -> x <> dishType), active)
        }

    override this.ToString () = sprintf "Role: %s" roleName

    override this.Equals (obj) =
        match obj with
        | :? Role as r -> r.Id = this.Id && r.RoleName = this.RoleName && r.Enabler = this.Enabler && r.Observer = this.Observer
        | _ -> false

    override this.GetHashCode () = 
        hash (this.Id, this.RoleName, this.Enabler, this.Observer)


    static member Deserialize (json: string) =
        globalSerializer.Deserialize<Role> json

    static member StorageName = "_roles"
    static member Version = "_01"
    static member SnapshotsInterval = 15

    member this.Serialize =
        globalSerializer.Serialize this

    interface Aggregate<string> with
        member this.Id = this.Id
        member this.Serialize = this.Serialize