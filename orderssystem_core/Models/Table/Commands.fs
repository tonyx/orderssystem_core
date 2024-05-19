
module OrdersSystem.Models.TableCommands
open OrdersSystem.Models.Table
open OrdersSystem.Models.TableEvents
open OrdersSystem.Commons
open System
open Sharpino
open Sharpino.Core
open FSharpPlus
open FSharpPlus.Operators
open FsToolkit.ErrorHandling

type TableCommands =
    | ChangeSeats of int
    | Deactivate

        interface Command<Table, TableEvents> with
            member this.Execute table = 
                match this with
                | ChangeSeats newSeats -> 
                    table.ChangeSeats newSeats
                    |> Result.map (fun table -> [SeatsChanged newSeats])
                | Deactivate -> 
                    table.Deactivate ()
                    |> Result.map (fun table -> [Deactivated])
            member this.Undoer = None