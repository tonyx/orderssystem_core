
module OrdersSystem.Models.OrderEvents
open OrdersSystem.Models.Order
open OrdersSystem.Commons
open System
open Sharpino
open Sharpino.Core
open FSharpPlus
open FSharpPlus.Operators
open FsToolkit.ErrorHandling

type OrderEvents =
    | Deactivated 

        interface Event<Order>  with
            member this.Process order =
                match this with
                | Deactivated -> 
                    order.Deactivate ()
    member this.Serialize =
        globalSerializer.Serialize this
    static member Deserialize json =
        globalSerializer.Deserialize<OrderEvents> json


