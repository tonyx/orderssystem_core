namespace OrdersSystem.Contexts.Restaurant
open FSharpPlus
open OrdersSystem.Commons
open FsToolkit.ErrorHandling
open Sharpino.Definitions
open Sharpino.Utils
open Sharpino
open System

module Restaurant =
    type Restaurant (dishReferences: List<Guid>, ingredientReferences: List<Guid>, tablesReferences: List<Guid>, ordersReferences: List<Guid>) =

        member this.DishRefs = dishReferences
        member this.IngredientRefs = ingredientReferences
        member this.TableRefs = tablesReferences
        member this.OrderRefs = ordersReferences

        static member Zero =
            Restaurant ([], [], [], [])
        member this.AddDishRef (id: Guid) =
            result {
                let! notAlreadyExists =
                    this.DishRefs
                    |> List.contains id
                    |> not
                    |> Result.ofBool (sprintf "A dish with id '%A' already exists" id)    
                return 
                    Restaurant (id :: this.DishRefs, this.IngredientRefs, this.TableRefs, this.OrderRefs)
            }
        member this.AddIngredientRef (id: Guid) =
            result {
                let! notAlreadyExists =
                    this.IngredientRefs
                    |> List.contains id
                    |> not
                    |> Result.ofBool (sprintf "An ingredient with id '%A' already exists" id)
                return Restaurant (this.DishRefs, id :: this.IngredientRefs, this.TableRefs, this.OrderRefs)
            }
        member this.RemoveIngredientRef (id: Guid) =
            result {
                let! chckExists =
                    this.IngredientRefs
                    |> List.contains id
                    |> Result.ofBool (sprintf "An ingredient with id '%A' does not exist" id)
                return Restaurant (this.DishRefs, this.IngredientRefs |> List.filter (fun  x -> x <> id), this.TableRefs, this.OrderRefs)
            }

        member this.RemoveDishRef (id: Guid) =
            result {
                let! chckExists =
                    this.DishRefs
                    |> List.contains id
                    |> Result.ofBool (sprintf "A dish with id '%A' does not exist" id)
                return Restaurant (this.DishRefs |> List.filter (fun  x -> x <> id), this.IngredientRefs, this.TableRefs, this.OrderRefs)
            }
        
        static member StorageName =
            "_kitchen"
        static member Version =
            "_01"
        static member SnapshotsInterval =
            15
        static member Deserialize json =
            globalSerializer.Deserialize<Restaurant> json
        member this.Serialize  =
            this
            |> globalSerializer.Serialize



