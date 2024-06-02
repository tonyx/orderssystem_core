
module OrdersSystem.Models.RoleCommands
open OrdersSystem.Models.Dish
open OrdersSystem.Models.Role
open OrdersSystem.Models.RoleEvents
open OrdersSystem.Commons
open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core
open Ingredient

type RoleCommands =
    | AddEnabler of Guid
    | AddObserver of Guid
    | RemoveEnabler of Guid
    | RemoveObserver of Guid
    | Deactivate
    interface Command<Role, RoleEvents> with
        member this.Execute role = 
            match this with
            | AddEnabler dishType -> 
                role.AddEnabler dishType
                |> Result.map (fun _  -> [EnablerAdded dishType])
            | AddObserver dishType -> 
                role.AddObserver dishType
                |> Result.map (fun _ -> [ObserverAdded dishType])
            | RemoveEnabler dishType -> 
                role.RemoveEnabler dishType
                |> Result.map (fun _ -> [EnablerRemoved dishType])
            | RemoveObserver dishType -> 
                role.RemoveObserver dishType
                |> Result.map (fun _ -> [ObserverRemoved dishType])
            | Deactivate ->
                role.Deactivate ()
                |> Result.map (fun _ -> [Deactivated])
        member this.Undoer = None