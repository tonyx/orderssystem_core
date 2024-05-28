
module OrdersSystem.Models.OrderCommands
open OrdersSystem.Models.OrderEvents
open OrdersSystem.Models.Order
open OrdersSystem.Commons
open System
open Sharpino
open Sharpino.Core
open FSharpPlus
open FSharpPlus.Operators
open FsToolkit.ErrorHandling

type OrderCommands =
    | Deactivate

        interface Command<Order, OrderEvents> with
            member this.Execute order = 
                match this with
                | Deactivate -> 
                    order.Deactivate ()
                    |> Result.map (fun _ -> [Deactivated])
            member this.Undoer = None