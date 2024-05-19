
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
    | AddOrderItem of OrderItem
    | RemoveOrderItem of OrderItem
    | ChangeOrderItem of OrderItem
    | Deactivate

        interface Command<Order, OrderEvents> with
            member this.Execute order = 
                match this with
                | AddOrderItem item -> 
                    order.AddOrderItem item
                    |> Result.map (fun order -> [OrderItemAdded item])
                | RemoveOrderItem item -> 
                    order.RemoveOrderItem item
                    |> Result.map (fun order -> [OrderItemRemoved item])
                | ChangeOrderItem item -> 
                    order.ChangeOrderItem item
                    |> Result.map (fun order -> [OrderItemChanged item])
                | Deactivate -> 
                    order.Deactivate ()
                    |> Result.map (fun order -> [Deactivated])
            member this.Undoer = None