module OrdersSystem.Models.OrderItemCommands

open OrdersSystem.Models.OrderItem
open OrdersSystem.Models.OrderItemEvents
open OrdersSystem.Commons
open System
open OrdersSystem.Models.Dish
open Sharpino
open Sharpino.Core
open FSharpPlus
open FsToolkit.ErrorHandling
open Sharpino.Core

type OrderItemCommands =
    | AddVariation of Variation
    | RemoveVariation of Variation
    | UpdateQuantity of int
    | ReplaceDish of Guid
    | ChangeOrder of Guid
    
    interface Command<OrderItem, OrderItemEvents> with
        member this.Execute (orderItem: OrderItem) =
            match this with
            | AddVariation variation ->
                orderItem.AddVariation variation
                |> Result.map (fun _ -> [VariationAdded variation])
            | RemoveVariation variation ->
                orderItem.RemoveVariation variation
                |> Result.map (fun _ -> [VariationRemoved variation])
            | UpdateQuantity quantity ->
                orderItem.UpdateQuantity quantity
                |> Result.map (fun _ -> [QuantityUpdated quantity])
            | ReplaceDish guid ->
                orderItem.ReplaceDish guid
                |> Result.map (fun _ -> [DishReplaced guid])
            | ChangeOrder guid ->
                orderItem.ChangeOrder guid
                |> Result.map (fun _ -> [OrderChanged guid])
        member this.Undoer = None         
                
                
            
    
