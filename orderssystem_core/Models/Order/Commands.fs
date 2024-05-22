
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
    | RemoveOrderItem of Guid
    | ChangeOrderItem of OrderItem
    | Deactivate

        interface Command<Order, OrderEvents> with
            member this.Execute order = 
                match this with
                | AddOrderItem item -> 
                    order.AddOrderItem item
                    |> Result.map (fun _ -> [OrderItemAdded item])
                | RemoveOrderItem item -> 
                    order.RemoveOrderItem item
                    |> Result.map (fun _ -> [OrderItemRemoved item])
                | ChangeOrderItem item -> 
                    order.ChangeOrderItem item
                    |> Result.map (fun _ -> [OrderItemChanged item])
                | Deactivate -> 
                    order.Deactivate ()
                    |> Result.map (fun _ -> [Deactivated])
            member this.Undoer = None