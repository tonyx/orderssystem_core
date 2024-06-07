module OrdersSystem.Models.Ingredient
open OrdersSystem.Commons
open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core

    type IngredientMeasureType =
        | Grams
        | Kilograms
        | Liters
        | Milliliters
        | Pieces
        static member FromString (x: string) =
            match x with
            | "Grams" -> Grams
            | "Kilograms" -> Kilograms
            | "Liters" -> Liters
            | "Milliliters" -> Milliliters
            | "Pieces" -> Pieces
            | _ -> Grams
        static member GetCases() =
            ["Grams"; "Kilograms"; "Liters"; "Milliliters"; "Pieces"]    
        
    type IngredientType =
        {
            Id: Guid
            Name: string
            Visible: bool
        }

    type IngredientPrice =
        {
            Id: Guid
            Price: float
            Quantity: float
        }
        with
        static member mkIngredientPrice (price: float, quantity: float) =
            { Id = Guid.NewGuid(); Price = price; Quantity = quantity }
        
    type VariationType =
        | Abundant
        | Scarce
        | Add of Option<IngredientPrice>
        | Remove of Option<IngredientPrice>
        static member FromString (x: string) =
            match x with
            | "Abundant" -> Abundant
            | "Scarce" -> Scarce
            | "Add" -> Add None
            | "Remove" -> Remove None
            // | x when x.StartsWith "Remove" -> Remove (Some { Id = Guid.NewGuid(); Price = 0.0; Quantity = float (x.Substring 7) })
            // | x when x.StartsWith "Add" -> Add (Some { Id = Guid.NewGuid(); Price = 0.0; Quantity = float (x.Substring 4) })
            | _ -> Abundant
        override this.ToString() =
            match this with
            | Abundant -> "Abundant"
            | Scarce -> "Scarce"
            | Add None -> "Add"
            | Remove None -> "Remove"
            | Remove x when x.IsSome -> sprintf "Remove %f" x.Value.Quantity
            | Add x when x.IsSome -> sprintf "Add %f" x.Value.Quantity
        
    type IngredientVariation =
        {
            Id: Guid
            IngredientId: Guid
            VariationType: VariationType
        }
        with
        static member mkIngredientVariation (ingredientId: Guid, variationType: VariationType) =
            { Id = Guid.NewGuid(); IngredientId = ingredientId; VariationType = variationType }    
  
     
    type StandardVariation =
        {
            Id: Guid
            Name: string
            Description: Option<string>
            IngredientVariations: List<IngredientVariation>
        }
        
    type UpdatePolicy =
        | NoUpdate
        | UpdateOnOrder
        | UpdateOnStock
        | UpdateOnOrderAndStock
        with
        static member FromString (x: string) =
            match x with
            | "NoUpdate" -> NoUpdate
            | "UpdateOnOrder" -> UpdateOnOrder
            | "UpdateOnStock" -> UpdateOnStock
            | "UpdateOnOrderAndStock" -> UpdateOnOrderAndStock
            | _ ->
                NoUpdate
        static member GetCases() =
            ["NoUpdate"; "UpdateOnOrder"; "UpdateOnStock"; "UpdateOnOrderAndStock"]
                
    type CheckUpdatePolicy =
        | NoCheck
        | Alert
        | Block
        | BlockAndAlert
        with
        static member FromString (x: string) =
            match x with
            | "NoCheck" -> NoCheck
            | "Alert" -> Alert
            | "Block" -> Block
            | "BlockAndAlert" -> BlockAndAlert
            | _ -> NoCheck
        static member GetCases() =
            ["NoCheck"; "Alert"; "Block"; "BlockAndAlert"]    
     
    type Ingredient 
        (id: Guid,
         name: string,
         description: Option<string>,
         ingredientTypeId: Guid,
         ingredientMeasureType: IngredientMeasureType,
         active: bool, 
         ingredientPrices: List<IngredientPrice>,
         stock: float,
         hasAllergen: bool,
         updatePolicy: UpdatePolicy,
         checkUpdatePolicy: CheckUpdatePolicy,
         visible: bool
         ) =
        
        member this.Id = id
        member this.Name = name
        member this.IngredientTypeId = ingredientTypeId
        member this.IngredientMeasureType = ingredientMeasureType
        member this.IngredientPrices = ingredientPrices
        member this.Active = active
        member this.HasAllergen = hasAllergen
        member this.UpdatePolicy = updatePolicy
        member this.Stock = stock
        member this.Visible = visible
        member this.CheckUpdatePolicy = checkUpdatePolicy
        member this.Description = description

        member this.AddIngredientPrice (ingredientPrice: IngredientPrice) =
            result {
                do! 
                    this.IngredientPrices 
                    |> List.contains ingredientPrice
                    |> not
                    |> Result.ofBool "IngredientPrice already exists"
                return Ingredient (id, name, description, ingredientTypeId, ingredientMeasureType, active, ingredientPrice :: ingredientPrices, stock, hasAllergen, updatePolicy, checkUpdatePolicy, visible)
            }

        member this.RemoveIngredientPrice (ingredientPriceId: Guid) =
            result {
                do! 
                    this.IngredientPrices 
                    |> List.exists (fun x -> x.Id = ingredientPriceId)
                    |> Result.ofBool "IngredientPrice does not exist"
                return Ingredient (id, name, description, ingredientTypeId, ingredientMeasureType, active, ingredientPrices |> List.filter (fun x -> x.Id <> ingredientPriceId), stock, hasAllergen, updatePolicy, checkUpdatePolicy, visible)
            }    
           
        member this.SetAllergen (x: bool) =
            Ingredient (id, name, description, ingredientTypeId, ingredientMeasureType, active, ingredientPrices, stock, x, updatePolicy, checkUpdatePolicy, visible) |> Ok
            
        member this.SetUpdatePolicy (x: UpdatePolicy) =
            Ingredient (id, name, description, ingredientTypeId, ingredientMeasureType, active, ingredientPrices, stock, hasAllergen, x, checkUpdatePolicy, visible) |> Ok

        member this.SetIngredientTypeId (ingredientTypeId: Guid) =
            Ingredient (id, name, description, ingredientTypeId, ingredientMeasureType, active, ingredientPrices, stock, hasAllergen, updatePolicy, checkUpdatePolicy, visible) |> Ok

        member this.UpdateName (newName: string) =
            result {
                do! 
                    newName
                    |> String.IsNullOrWhiteSpace
                    |> not
                    |> Result.ofBool "Name cannot be empty"
                return Ingredient (id, newName, description, ingredientTypeId, ingredientMeasureType, active, ingredientPrices, stock, hasAllergen, updatePolicy, checkUpdatePolicy, visible)
            }
        member this.Update     
             (name: string,
             description: Option<string>,
             ingredientTypeId: Guid,
             ingredientMeasureType: IngredientMeasureType,
             active: bool, 
             ingredientPrices: List<IngredientPrice>,
             stock: float,
             hasAllergen: bool,
             updatePolicy: UpdatePolicy,
             checkUpdatePolicy: CheckUpdatePolicy,
             visible: bool
            ) =
            Ingredient (id, name, description, ingredientTypeId, ingredientMeasureType, active, ingredientPrices, stock, hasAllergen, updatePolicy, checkUpdatePolicy, visible) |> Ok
            
       
        member this.SetVisibility (x: bool) =
            Ingredient (id, name, description, ingredientTypeId, ingredientMeasureType, active, ingredientPrices, stock, hasAllergen, updatePolicy, checkUpdatePolicy, x) |> Ok
        
        member this.IncreaseStock (quantity: float) =
            Ingredient (id, name, description, ingredientTypeId, ingredientMeasureType, active, ingredientPrices, stock + quantity, hasAllergen, updatePolicy, checkUpdatePolicy, visible) |> Ok
        
        member this.DecreaseStock (quantity: float) =
            result {
                do!
                    match updatePolicy with
                    | NoUpdate -> true
                    | UpdateOnOrder -> true 
                    | UpdateOnStock -> 
                        stock - quantity >= 0
                    | UpdateOnOrderAndStock ->
                        stock - quantity >= 0
                    |> Result.ofBool "Stock cannot be negative"
                return Ingredient (id, name, description, ingredientTypeId, ingredientMeasureType, active, ingredientPrices, stock - quantity, hasAllergen, updatePolicy, checkUpdatePolicy, visible) 
                }

        member this.Deactivate () =
            Ingredient (id, name, description, ingredientTypeId, ingredientMeasureType, false, ingredientPrices, stock, hasAllergen, updatePolicy, checkUpdatePolicy, visible) |> Ok

        // member this.AddIngredientMeasureType (ingredientMeasure: IngredientMeasureType) =
        //     result {
        //         do! 
        //             this.IngredientMeasureType 
        //             |> List.contains ingredientMeasure
        //             |> not
        //             |> Result.ofBool "IngredientMeasure already exists"
        //         return Ingredient (id, name, description, ingredientTypeId, ingredientMeasure:: ingredientMeasureTypes, active, ingredientPrices, stock, hasAllergen, updatePolicy, checkUpdatePolicy, visible)
        //     }

        // member this.RemoveIngredientMeasureType (ingredientMeasure: IngredientMeasureType) =
        //     result {
        //         do! 
        //             this.IngredientMeasureType 
        //             |> List.contains ingredientMeasure
        //             |> Result.ofBool "IngredientMeasure does not exist"
        //         return Ingredient (id, name, description, ingredientTypeId, ingredientMeasureType |> List.filter ((<>) ingredientMeasure), active, ingredientPrices, stock, hasAllergen, updatePolicy, checkUpdatePolicy, visible)
        //     }

        static member SnapshotsInterval =
            15
        static member StorageName =
            "_ingredients"
        static member Version =
            "_01"

        member this.Serialize = 
            this |> globalSerializer.Serialize

        static member Deserialize (json: string): Result<Ingredient, string>  =
            globalSerializer.Deserialize<Ingredient> json

        interface Aggregate<string> with
            member this.Id = id
            member this.Serialize = 
                this.Serialize

