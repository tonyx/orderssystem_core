
module OrdersSystem.Models.Order
open OrdersSystem.Commons
open System
open Sharpino
open Sharpino.Core
open FSharpPlus
open FSharpPlus.Operators
open FsToolkit.ErrorHandling

type IngredientChange = 
    | Add of Guid 
    | AddALittle of Guid
    | AddExceed of Guid
    | Remove of Guid
    | Exceed of Guid
    | RemoveAlittle of Guid

type OrderItem = 
    {
        Id: Guid
        DishRef: Guid
        Quantity: int
        IngredientChanges: List<IngredientChange>
    }

type Order private (id: Guid, orderItems: List<OrderItem>, table: Guid, active: bool) =

    member this.Id = id
    member this.OrderItems = orderItems |> List.sortBy (fun x -> x.DishRef)
    member this.Active = active

    private new (id: Guid, orderItems: List<OrderItem>, table) =
        Order (id, orderItems, table, true)

    new (orderItems: List<OrderItem>, table) =
        Order (Guid.NewGuid(), orderItems, table, true)

    member this.AddOrderItem orderItem =
        result {
            do! 
                this.OrderItems 
                |> List.map (fun x -> x.DishRef)
                |> List.contains orderItem.DishRef
                |> Result.ofBool "DishRef already exists"
            return Order (id, orderItem :: orderItems, table, true)
        }

    member this.RemoveOrderItem (orderItem: OrderItem) =
        result {
            do! 
                this.OrderItems 
                |> List.map (fun x -> x.DishRef)
                |> List.contains orderItem.DishRef
                |> Result.ofBool "DishRef does not exist"
            return Order (id, orderItems |> List.filter (fun x -> x.DishRef <> orderItem.DishRef), table, true)
        }

    member this.ChangeOrderItem (orderItem: OrderItem) =
        result {
            do! 
                this.OrderItems 
                |> List.exists (fun x -> x.Id = orderItem.Id)
                |> Result.ofBool "OrderItem does not exist"
                
            return Order (id, orderItems |> List.map (fun x -> if x.Id = orderItem.Id then orderItem else x), table, true)
        }

    member this.Deactivate () =
        Order (id, orderItems, table, false) |> Ok

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
