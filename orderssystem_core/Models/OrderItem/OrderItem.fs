module OrdersSystem.Models.OrderItem
open OrdersSystem.Commons
open System
open OrdersSystem.Models.Dish
open Sharpino
open FSharpPlus
open FsToolkit.ErrorHandling
open Sharpino.Core

type Variation =
    | No of Guid
    | Less of Guid
    | More of Guid
    | Extra of Guid * Option<IngredientMeasureItemType>
    
type OrderItem (id: Guid, orderId: Guid, dishId: Guid, quantity: int, comment: string, variations: List<Variation>) =
    member this.Id = id
    member this.OrderId = orderId
    member this.DishId = dishId
    member this.Quantity = quantity
    
    member this.Comment = comment
    
    member this.Variations = variations
    
    member this.AddVariation (variation: Variation) =
        OrderItem(id, orderId, dishId, quantity, comment, variations |> List.append [variation]) |> Ok
    
    member this.RemoveVariation (variation: Variation) =
        OrderItem(id, orderId, dishId, quantity, comment, variations |> List.filter (fun v -> v <> variation)) |> Ok
    
    member this.UpdateQuantity (quantity: int) =
        if (quantity <=0) then
            Error "quantity cannot be negative"
        else     
            OrderItem(id, orderId, dishId, quantity, comment, variations) |> Ok
   
    member this.UpdateComment (comment: string) =
        OrderItem(id, orderId, dishId, quantity, comment, variations) |> Ok
    
    member this.ReplaceDish (dishId: Guid) =
        OrderItem(id, orderId, dishId, quantity, comment, []) |> Ok
    
    member this.ChangeOrder (orderId: Guid) =
        OrderItem(id, orderId, dishId, quantity, comment, variations) |> Ok
    
    static member Deserialize(json: string)     =
        globalSerializer.Deserialize<OrderItem> json
        
    static member StorageName = "_orderItems"
    static member Version = "01"
    
    static member SnapshotsInterval = 15
    
    member this.Serialize =
        globalSerializer.Serialize this
   
    interface Aggregate<string> with
        member this.Id = this.Id
        member this.Serialize = this.Serialize
        
       
    
            
            


    
    



