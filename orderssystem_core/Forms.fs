module OrdersSystem.Form

open Suave.Form

[<Literal>]
let YES = "YES"
[<Literal>]
let NO = "NO"

[<Literal>]
let ABSTRACT = "ABSTRACT"

[<Literal>]
let VISIBLE = "VISIBLE"

let pattern = passwordRegex @"(\w){6,20}"

let decimalNumberPattern =  matches  @"-{0,1}\d*(\.{0,1}\d*)"

type Logon = {
    Username : string
    Password : Password
}

let logon : Form<Logon> = Form ([],[])

type Course = {
    Name : string
    Description :  string option
    Price : decimal
    Visibile: string 
    CategoryId: string
}

let course : Form<Course> = 
    Form ([ 
            TextProp ((fun f -> <@ f.Name @>), [ maxLength 100 ])
            DecimalProp ((fun f -> <@ f.Price @>), [ min 0.01M; step 0.01M ])
            TextProp ((fun f -> <@ f.CategoryId @>), [ ])
            ],
        [])

type PriceAdjustment = {
    PercentOrValue: string 
    Value: string 
}

let priceAdjustment: Form<PriceAdjustment> = 
    Form 
        ([
            TextProp ((fun f -> <@ f.PercentOrValue  @>), [])
            TextProp ((fun f -> <@ f.Value  @>), [decimalNumberPattern])
        ],
        [] )

type IngredientPriceForm = {
    AddPrice: decimal
    // SubtractPrice: decimal
    // IsDefaultAdd: string
    // IsDefaultSubtract: string
    Quantity: decimal
}

let ingredientPrice: Form<IngredientPriceForm> =
    Form (
        [ 
            DecimalProp ((fun f -> <@ f.AddPrice @>), [ min 0.01M; step 0.01M ])
        ], [] )

type OrderItem = {
    CourseId: decimal
    Comment: string option
    Quantity: decimal
    Price: decimal 
    GroupOut: decimal 
}

type OrderItemRejection = {
    Motivation: string
}

let orderItemRejection: Form<OrderItemRejection> =
    Form ([
        TextProp ((fun f -> <@ f.Motivation @>), [])
    ],[])


type StrippedOrderItem = {
    CourseId: decimal
    Comment: string option
    Quantity: decimal
    GroupOut: decimal
}

let orderItem: Form<OrderItem > =
    Form 
        ([ 
            DecimalProp ((fun f -> <@ f.Quantity @>), [])
            DecimalProp ((fun f -> <@ f.CourseId @>), [])
            DecimalProp ((fun f -> <@ f.Price @>), [ min 0.01M; step 0.01M ])
            ],
        [])
let strippedOrderItem: Form<StrippedOrderItem> =
    Form ([ DecimalProp ((fun f -> <@ f.Quantity @>), [])
            DecimalProp ((fun f -> <@ f.CourseId @>), [])
            ],
        [])

type Order = {
    Table: string
    Person: string option
}

let order: Form<Order> =
    Form ( [ ], [])

type IngredientCategory = {
    Name: string
    Comment: string option
    Visibility: string
}

let ingredientCategory: Form<IngredientCategory> = 
    Form ([
        TextProp ((fun f -> <@ f.Name @>), [])
    ],[])

type IngredientForm = {
    Name: string
    Description: string option
    Visibility: string
    Allergene: string
    // AvailableQuantity: decimal
    UpdatePolicy: string
    CheckUpdatePolicy: string
    IngredientMeasureType: string
}

let ingredient:Form<IngredientForm> =
    Form ([
        TextProp ((fun f -> <@ f.Name@>), [])
        // DecimalProp ((fun f -> <@ f.AvailableQuantity@>), [min 0.0M; step 0.01M])
    ],[]
    )

type IngredientSelector = {
    IngredientBySelect: string 
//    IngredientByText: string
    Quantity: decimal option
}

type EnablersObserverForStateAbilities = {
    CategoryId: decimal
    RoleId: decimal
    ObserverCOLLECTING: string option
    ObserverTOBEWORKED: string option
    ObserverSTARTEDWORKING: string option
    ObserverREADYFORDELIVERY: string option
    ObserverDELIVERED: string option
    ObserverDONE: string option

    EnablerCOLLECTING: string option
    EnablerTOBEWORKED: string option
    EnablerSTARTEDWORKING: string option
    EnablerREADYFORDELIVERY: string option
    EnablerDELIVERED: string option
    EnablerDONE: string option
}
let enablersObserverForStateAbilities:Form<EnablersObserverForStateAbilities> = 
    Form ([],[])

type PrinterOutGroupForStates = {
    CategoryId: decimal
    COLLECTING: string option
    TOBEWORKED: string option
    STARTEDWORKING: string option
    READYFORDELIVERY: string option
    DELIVERED: string option
    DONE: string option
    PRINTRECEIPT: string option
    PRINTINVOICE: string option
}

let printerOutGroupForStates:Form<PrinterOutGroupForStates> = 
    Form ([],[])

let ingredientSelector:Form<IngredientSelector> = 
    Form ([
        TextProp ((fun f -> <@f.IngredientBySelect@> ),[])
    ],[]
    )

type AddIngredient = {
    IngredientBySelect: decimal
    Quantity: string 
} 

let addIngredient:Form<AddIngredient> = 
    Form ([
        DecimalProp ((fun f -> <@f.IngredientBySelect @> ),[])
    ],[])

type IngredientVariationForm = {
    IngredientBySelect: string
    Quantity: string
}

let ingredientVariation:Form<IngredientVariationForm> = 
    Form ([
        TextProp ((fun f -> <@f.IngredientBySelect @> ),[])
    ],[])

type IngredientEdit = {
    Name: string
    Comment: string option
    Visibility: string
    Category: string
    Allergene: string
    UpdateAvailabilityFlag: string
    IngredientMeasureType: string
    CheckAvailabilityFlag: string
}

let ingredientEdit:Form<IngredientEdit> =
    Form ([
        TextProp ((fun f -> <@ f.Name@>), [])
        // DecimalProp ((fun f -> <@ f.Category @>), [])
    ],[]
    )

type IngredientLoad = {
    Quantity: decimal
    Comment: string
}

let ingredientLoad:Form<IngredientLoad> =
    Form (
        [
            // DecimalProp ((fun f -> <@ f.Quantity @>),[min 0.01M; step 0.01M])
            // System.Guid ((fun f -> <@ f.Quantity @>), []) // [min 0.01M; step 0.01M])
        ],
        []
    )
type Date = {
    Date: string
}
let date: Form<Date> = (
    Form ([],[])
)

type ChangePassword = {
    OldPassword: Password
    Password: Password
    ConfirmPassword: Password
}

let passwordsMatch2 = 
    (fun f -> f.Password = f.ConfirmPassword), "Passwords must match"

let changePassword: Form<ChangePassword> =
    Form ([
        PasswordProp ((fun f -> <@ f.OldPassword @>), [  ] )
        PasswordProp ((fun f -> <@ f.Password @>), [ pattern ] )
        PasswordProp ((fun f -> <@ f.ConfirmPassword @>), [ pattern ] )
    ],[passwordsMatch2])

type Register = {
    Username : string
    Password : Password
    ConfirmPassword : Password
    CanManageAllorders : string
    CanChangeThePrices : string
    CanManageAllCourses : string
    Role: decimal
}

let passwordsMatch = 
    (fun f -> f.Password = f.ConfirmPassword), "Passwords must match"

let register : Form<Register> = 
    Form (
        [  
            TextProp ((fun f -> <@ f.Username @>), [ maxLength 30 ] )
            DecimalProp ((fun f -> <@f.Role @>), [])
            PasswordProp ((fun f -> <@ f.Password @>), [ pattern ] )
            PasswordProp ((fun f -> <@ f.ConfirmPassword @>), [ pattern ] )
        ],
        [ passwordsMatch ]
        )

type UserEdit = {
    Enabled: string
    CanVoidOrder: string
    CanManageAllorders: string
    CanChangeThePrices: string
    CanManageAllCourses: string
}

type CourseCategory = {
    Name: string
    Visibility: string
}

let courseCategoryEdit : Form<CourseCategory> = 
    Form (
        [ 
            TextProp ((fun f -> <@ f.Name @>), [ maxLength 30 ] )
            TextProp ((fun f -> <@ f.Visibility @>), [] )
            ],[ ]
    )

type InvoiceForm = {
    CompanyId: decimal
    Comment: string
    ShowDetails: string option
    InvoiceNumber: decimal
    CompanyName: string
    StoreCompany: string option
}

let invoiceForm: Form<InvoiceForm> =
    Form (
        [
            TextProp ((fun f -> <@ f.Comment @>), [])
            TextProp ((fun f -> <@ f.CompanyName @>), [])
        ], []
    )

type QrUser = {
    TableName: string
}

let qrUser: Form<QrUser> =
    Form (
        [
            TextProp ((fun f -> <@ f.TableName @>), [])
        ], []
    )

type Role = {
    Name: string
    // Comment: string option
}

let role: Form<Role> =
    Form (
        [ 
            TextProp ((fun f -> <@ f.Name @>), [])
        ], []
    )

type SearchCourse = {
    Name: string
}
let searchCourse: Form<SearchCourse> =
    Form (
        [
            TextProp ((fun f -> <@ f.Name @>), [])
        ], []
    )

type Comment = {
    Comment: string
}

let comment: Form<Comment> =
    Form (
        [
            TextProp ((fun f -> <@ f.Comment@>),[])
        ], []
    )

type SubCourseCategory = {
    Name: string
    Visibility: string
}

let subCourseCategory : Form<SubCourseCategory> =
    Form (
        [
            TextProp ((fun f -> <@ f.Name @>), [ maxLength 30 ] )
            TextProp ((fun f -> <@ f.Visibility @>), [] )
        ],[ ]
    )

type CommentForCourse = {
    CommentForCourse: string
}

let commentForCourse:Form<CommentForCourse>  =
    Form (
        [TextProp ((fun f -> <@ f.CommentForCourse @>),[])],[]
    )

type VariationForCourse = {
    VariationForCourse: string
}

let variationForCourse:Form<VariationForCourse>  =
    Form (
        [TextProp ((fun f -> <@ f.VariationForCourse @>),[])],[]
    )

type StandardVariationForm = {
    Name: string
}

let standardVariation: Form<StandardVariationForm> =
    Form (
        [
            TextProp ((fun f -> <@f.Name@>),[])
        ], []
    )

type SearchIngredient = {
    Name: string
}

let searchIngredient: Form<SearchIngredient> =
    Form (
        [
            TextProp ((fun f -> <@ f.Name @>),[])
        ], []
    )

type RoleStateCategory = {
    RoleId: decimal
    StateId: decimal
    CategoryId: decimal
}

let roleStateCategory: Form<RoleStateCategory> = 
    Form ([
            DecimalProp ((fun f -> <@ f.RoleId @>), [])
            DecimalProp ((fun f -> <@ f.StateId @>), [])
            DecimalProp ((fun f -> <@ f.CategoryId@>), [])
    ], []
    )

type CourseDeletion = {
    CourseId: decimal
    CourseName: string
}

let courseDeletion: Form<CourseDeletion> =
    Form ([
        DecimalProp((fun f -> <@ f.CourseId @>), [])
        TextProp ((fun f -> <@ f.CourseName@>),[])
    ], [])

type IngredientDeletion = {
    IngredientId: decimal
    IngredientName: string
}

let ingredientDeletion: Form<IngredientDeletion> =
    Form ([
        DecimalProp((fun f -> <@ f.IngredientId @>), [])
        TextProp ((fun f -> <@ f.IngredientName@>),[])
    ], [])

let userEdit: Form<UserEdit> =
    Form ( [ 
            TextProp ((fun f -> <@ f.Enabled @>), [])
            TextProp ((fun f -> <@ f.CanVoidOrder @>), [])
            TextProp ((fun f -> <@ f.CanManageAllCourses @>), [])
        ],
    [])

