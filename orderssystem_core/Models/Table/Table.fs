module OrdersSystem.Models.Table
open OrdersSystem.Commons
open System
open Sharpino
open Sharpino.Core
open FSharpPlus
open FSharpPlus.Operators
open FsToolkit.ErrorHandling

type Table private (id: Guid, description: Option<string>, number: int, seats: int, active: bool) =

    member this.Id = id
    member this.Number = number
    member this.Seats = seats
    member this.Active = active
    member this.Description = description

    private new (id: Guid, description: Option<string>, number: int, seats: int) =
        Table (id, description, number, seats, true)

    new (number: int, seats: int) =
        Table (Guid.NewGuid(), None, number, seats, true)

    member this.ChangeSeats(newSeats: int) =
        Table (id, description, number, newSeats, active) |> Ok

    member this.Deactivate () =
        Table (id, description, number, seats, false) |> Ok

    override this.ToString() = sprintf "Table(%A)" this.Id

    override this.Equals(obj) =
        match obj with
        | :? Table as t -> t.Id = this.Id && t.Number = this.Number
        | _ -> false
    override this.GetHashCode() = 
        hash (this.Id, this.Number)

    static member Deserialize(json: string) =
        globalSerializer.Deserialize<Table> json

    static member StorageName = "_tables"
    static member Version = "_01"
    static member SnapshotsInterval = 15

    member this.Serialize =
        globalSerializer.Serialize this 

    interface Aggregate<string> with
        member this.Id = this.Id
        member this.Serialize = this.Serialize