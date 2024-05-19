
module OrdersSystem.Models.Ingredient
open OrdersSystem.Commons
open System
open Sharpino
open FsToolkit.ErrorHandling
open Sharpino.Core

type IngredientEvents =
    | IngredientTypeAdded
