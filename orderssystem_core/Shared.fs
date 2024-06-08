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

type TemporaryUser =
    AssociatedTable of TableId

type State =
    | Collecting
    | Started
    | Done
    | Closed
    with
    static member
        FromString x =
            match x with
            | "Collecting" -> Collecting
            | "Started" -> Started
            | "Done" -> Done
            | "Closed" -> Closed
            | _ -> failwith "Unknown state"
    static member
        GetCases() =
            [Collecting; Started; Done; Closed]
    

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
        member this.IsManager(state: State, dishTypeId: DishTypeId) =
            this.Roles |> List.exists (fun x -> match x with | Manager (s, d) -> s = state && d = dishTypeId | _ -> false)
        member this.IsObserver(state: State, dishTypeId: DishTypeId) =
            this.Roles |> List.exists (fun x -> match x with | Observer (s, d) -> s = state && d = dishTypeId | _ -> false)
        member this.SetManager state dishTypeId =
            if
                this.Roles |> List.exists (fun x -> match x with | Manager (s, d) -> s = state && d = dishTypeId | _ -> false) then
                    this
                else 
                    { this with Roles = Manager(state, dishTypeId) :: this.Roles }
        member this.SetObserver state dishTypeId =
            if
                this.Roles |> List.exists (fun x -> match x with | Observer (s, d) -> s = state && d = dishTypeId | _ -> false) then
                    this
                else 
                    { this with Roles = Observer(state, dishTypeId) :: this.Roles }
        member this.UnSetManager state dishTypeId =
            { this with Roles = this.Roles |> List.filter (fun x -> match x with | Manager (s, d) -> not (s = state && d = dishTypeId) | _ -> true) }
        member this.UnSetObserver state dishTypeId =
            { this with Roles = this.Roles |> List.filter (fun x -> match x with | Observer (s, d) -> not (s = state && d = dishTypeId) | _ -> true) }    
        
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
