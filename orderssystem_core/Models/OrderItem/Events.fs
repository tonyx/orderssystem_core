module OrdersSystem.Models.OrderItemEvents
open OrdersSystem.Models.OrderItem
open OrdersSystem.Commons
open System
open Sharpino.Core

type OrderItemEvents =
    | VariationAdded of Variation
    | VariationRemoved of Variation
    | QuantityUpdated of int
    | DishReplaced of Guid
    | OrderChanged of Guid
    
    interface Event<OrderItem> with
        member this.Process (orderItem: OrderItem) =
            match this with
            | VariationAdded variation -> 
                orderItem.AddVariation variation
            | VariationRemoved variation -> 
                orderItem.RemoveVariation variation
            | QuantityUpdated quantity -> 
                orderItem.UpdateQuantity quantity
            | DishReplaced dish -> 
                orderItem.ReplaceDish dish
            | OrderChanged order -> 
                orderItem.ChangeOrder order     
    
    member this.Serialize =
        globalSerializer.Serialize this
    
    static member Deserialize json =
        globalSerializer.Deserialize<OrderItemEvents> json
        