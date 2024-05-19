
module OrdersSystem.Models.TableEvents
open OrdersSystem.Models.Table
open OrdersSystem.Commons
open System
open Sharpino
open Sharpino.Core
open FSharpPlus
open FSharpPlus.Operators
open FsToolkit.ErrorHandling

type TableEvents =
    | SeatsChanged of int
    | Deactivated
        interface Event<Table>  with
            member this.Process table =
                match this with
                | SeatsChanged newSeats -> 
                    table.ChangeSeats newSeats
                | Deactivated -> 
                    table.Deactivate ()
    member this.Serialize = 
        globalSerializer.Serialize this
    static member Deserialize json = 
        globalSerializer.Deserialize<TableEvents> json