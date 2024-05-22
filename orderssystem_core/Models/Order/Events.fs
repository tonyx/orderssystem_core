
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
    | OrderItemAdded of OrderItem
    | OrderItemRemoved of Guid
    | OrderItemChanged of OrderItem

        interface Event<Order>  with
            member this.Process order =
                match this with
                | Deactivated -> 
                    order.Deactivate ()
                | OrderItemAdded item -> 
                    order.AddOrderItem item
                | OrderItemRemoved item -> 
                    order.RemoveOrderItem item
                | OrderItemChanged item -> 
                    order.ChangeOrderItem item
    member this.Serialize =
        globalSerializer.Serialize this
    static member Deserialize json =
        globalSerializer.Deserialize<OrderEvents> json


