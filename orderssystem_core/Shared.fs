module OrdersSystem.Shared
open System

type DishId = Guid
type IngredientId = Guid
type OrderId = Guid
type OrderItemId = Guid
type TableId = Guid
type UserId = Guid
type DishTypeId = Guid

type StandardComment =
    { 
        CommentId: Guid;
        Text: string;
    }

type State =
    | Collecting
    | Started
    | Done
    | Closed

type Role =
    | Observer of State * DishTypeId
    | Manager of State * DishTypeId
   

type UserRole =
    {
        RoleId: Guid
        Name: string
        Roles: List<Role>
    }
    with 
        static member MkUserRole name =
            {
                RoleId = Guid.NewGuid()
                Name = name
                Roles = []
            }
    
type Printer  =
    {
        Id: Guid
        Name: string
        Types: List<Guid>
    }
type DishType =
    {
        DishTypeId: DishTypeId
        Name: string
        Visible: bool
    } 
