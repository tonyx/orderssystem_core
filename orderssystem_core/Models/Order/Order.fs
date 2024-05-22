
module OrdersSystem.Models.Order
open OrdersSystem.Commons
open System
open Sharpino
open Sharpino.Core
open FSharpPlus
open FSharpPlus.Operators
open FsToolkit.ErrorHandling
open Dish

type IngredientChange = 
    | No of Guid 
    | Extra of Guid
    | Less of Guid
    | Substitute of Guid * IngredientAndQuantity

type OrderItemState = 
    Open | Started | Finished | Aborted of string

type OrderItem = 
    {
        Id : Guid
        DishRef: Guid
        Quantity: int
        IngredientChanges: List<IngredientChange>
        OrderitemState: OrderItemState
    }
    with 
        member 
            this.Start = 
                result {
                    do! 
                        this.OrderitemState <> Open
                        |> Result.ofBool "OrderItem is already started"
                    return { this with OrderitemState = Started }
                }
        member this.Finish = 
            result {
                do! 
                    this.OrderitemState = Started
                    |> Result.ofBool "OrderItem is not started"
                return { this with OrderitemState = Finished }
            }
        member this.Abort reason =
            {this with OrderitemState = Aborted reason} |> Ok

type Order private (id: Guid, orderItems: List<OrderItem>, table: Guid, active: bool) =

    member this.Id = id
    member this.OrderItems = orderItems |> List.sortBy (fun x -> x.Id)
    member this.Active = active

    private new (id: Guid, orderItems: List<OrderItem>, table) =
        Order (id, orderItems, table, true)

    new (orderItems: List<OrderItem>, table) =
        Order (Guid.NewGuid(), orderItems, table, true)

    member this.AddOrderItem orderItem =
        result {
            do! 
                this.OrderItems 
                |> List.map (fun x -> x.Id)
                |> List.contains orderItem.Id
                |> Result.ofBool "orderItem already exists"
            return Order (id, (orderItem :: orderItems ) |> List.sort , table, true)
        }

    member this.ChangeOrderItem (orderItem: OrderItem) =
        let sameOrderItem = fun x -> x.Id = orderItem.Id
        result {
            do! 
                this.OrderItems 
                |> List.exists (fun x -> sameOrderItem x)
                |> Result.ofBool "OrderItem does not exist"
                
            return Order (id, orderItems |> List.map (fun x -> if (sameOrderItem x) then orderItem else x), table, true)
        }
    member this.RemoveOrderItem (id : Guid) =
        result {
            do! 
                this.OrderItems 
                |> List.exists (fun x -> x.Id = id)
                |> Result.ofBool "OrderItem does not exist"
            return Order (id, orderItems |> List.filter (fun x -> x.Id <> id), table, true)
        }

    // you may want to deactiate it only if all order items are finished or aborted
    member this.Deactivate () =
        let res = 
            this.OrderItems 
            |> List.forall (fun x -> x.OrderitemState = Finished || x.OrderitemState = Aborted "")
        result {
            do! 
                this.Active
                |> Result.ofBool "Order is already deactivated"
            do! 
                res
                |> Result.ofBool "Order has unfinished order items"
            return Order (id, orderItems, table, false)
        }

    override this.ToString() = sprintf "Order(%A)" this.Id

    override this.Equals(obj) =
        match obj with
        | :? Order as o -> o.Id = this.Id && o.OrderItems = this.OrderItems
        | _ -> false
    override this.GetHashCode() = 
        hash (this.Id, this.OrderItems)

    static member Deserialize(json: string) =
        globalSerializer.Deserialize<Order> json

    static member StorageName = "_orders"
    static member Version = "_01"
    static member SnapshotsInterval = 15

    member this.Serialize =
        globalSerializer.Serialize this 

    interface Aggregate<string> with
        member this.Id = this.Id
        member this.Serialize = this.Serialize
