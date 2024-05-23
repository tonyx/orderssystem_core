module OrdersSystem.App

open System

open System.Linq
open Xceed.Words.NET
open System.Drawing
open FSharp.Data
open Suave
open Suave.Authentication
open Suave.Cookie
open Suave.Filters
open Suave.Form
open Suave.Model.Binding
open Suave.Operators
open Suave.RequestErrors
open Suave.State.CookieStateStore
open Suave.Successful
// open Suave.Web
open Globals
open View
// open FSharp.Data.Sql
open OrdersSystem
open Suave.Writers
open QRCoder
open System.Net
open OrdersSystem.DbWrappedEntities 
open OrdersSystem.Utils
open Microsoft.Security.Application
open System.IO
open System.Text
// open QRCoder
// open OrdersSystem.Settings
// open System.Runtime

// open System.Drawing;
// open System.Drawing.Printing;
// open System.Security.Cryptography.X509Certificates
// open System

open QuestPDF.Fluent
open QuestPDF.Helpers
open QuestPDF.Infrastructure

// open QuestPDF.Fluent
// open QuestPDF.Helpers
// open QuestPDF.Infrastructure
open System.Diagnostics


//   <add key ="QuestPDF.Settings.License"  value = "LicenseType.Community" />
let _ = 
    QuestPDF.Settings.License <- LicenseType.Community
    // LicenseManager.SetLicense(ConfigurationManager.AppSettings.["QuestPDF.Settings.License"])
    // let license = ConfigurationManager.AppSettings.["QuestPDF.Settings.License"]
    // QuestPDF.LicenseManager.SetLicense(license)

type Stritem = {entry: string}
type IndexNameRecord = {name: string; index: int}
type IndexNameDataRecord = {index: int; name: string; data: string}
type NameDataRecord = {name: string; data: string}
type IndexUnitMeasureMap = {index: int; unitmeasure: string}
type TwoIndexNameRecord = {index1: int; index2: int; enablers: Stritem list;observers: Stritem list}
type PerRoleCategories = {roleid: int; categories: string list}
type ManyRolesCategories = {rolecategories: PerRoleCategories}
type OrdersLiquidModel = {orders:  OrderWrapped list; targetOrder: OrderWrapped}
type IndexNameRecordList = {indexnameitems: IndexNameRecord list }
type OrderAndOrderitemslist = {order: OrderWrapped; orderitems: OrderItemDetailsWrapped list }
type OrderaAdSuborderList = {orderandorderitems: OrderAndOrderitemslist list; targetorder: OrderWrapped}

let tenderCodes = [{index=1;name="CONTANTI"};{index=2;name="CREDITO"};{index=3;name="ASSEGNI"};{index=4;name="BUONI"};{index=5;name="CARTA DI CREDITO"}]

let log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

let reset =
    unsetPair SessionAuthCookie
    >=> unsetPair StateCookie
    >=> Redirection.FOUND Path.home

let session f = 
    statefulForSession
    >=> context (fun x -> 
        match x |> HttpContext.state with
        | None -> f NoSession
        | Some state ->
            match state.get "cartid", state.get "username", state.get "role", state.get "userid", state.get "roleid",state.get "temporary", state.get "canmanagecourses", state.get "canmanageallorders"  with 
            | Some cartId, None, None, None,None,None,None,None ->
                f (CartIdOnly cartId)
            | _, Some username, Some role, Some userid, Some roleid,Some temporary,Some canmanagecourses,Some canmanageallorders ->
                f (UserLoggedOn {Username = username; Role = role; UserId = userid;RoleId=roleid;Temporary=temporary;CanManageAllCourses=canmanagecourses;CanManageAllOrders=canmanageallorders})
            | _ -> 
                f NoSession)

let redirectWithReturnPath redirection =
    request (fun x ->
        let path = x.url.AbsolutePath
        Redirection.FOUND (redirection |> Path.withParam ("returnPath", path)))

let loggedOn f_success =
    authenticate
        Cookie.CookieLife.Session
        false
        (fun () -> Choice2Of2(redirectWithReturnPath Path.Account.logon))
        (fun _ -> Choice2Of2 reset)
        f_success

let canManageCourses f_success =
    loggedOn (session (function
        | UserLoggedOn { Role = "admin" } -> f_success
        | UserLoggedOn { CanManageAllCourses = true} -> f_success
        | UserLoggedOn _ -> FORBIDDEN "Only for admin or enabled users"
        | _ -> UNAUTHORIZED "Not logged on"
    ))

let canManageIngredients f_success =
    loggedOn (session (function
        | UserLoggedOn { Role = "admin" } -> f_success
        | UserLoggedOn { CanManageAllCourses = true} -> f_success
        | UserLoggedOn _ -> FORBIDDEN "Only for admin or enabled users"
        | _ -> UNAUTHORIZED "Not logged on"
    ))

let canManageIngredientsPassingUserLoggedOn f_success  =
    loggedOn (session (function
        | UserLoggedOn { Role = "admin"; Username=username; UserId = userid; RoleId=roleid; Temporary=false  } -> f_success { CanManageAllCourses = true; Role = "admin"; Username=username; UserId = userid; RoleId=roleid; Temporary=false; CanManageAllOrders = true  }
        | UserLoggedOn { CanManageAllCourses = true; Role=role; Username=username; UserId=userid; RoleId=roleid;Temporary=false; CanManageAllOrders = canManageAllOrders } -> f_success { CanManageAllCourses = true; Role=role; Username=username; UserId=userid; RoleId=roleid;Temporary=false;CanManageAllOrders=canManageAllOrders }
        | UserLoggedOn _ -> FORBIDDEN "Only for admin or enabled users"
        | _ -> UNAUTHORIZED "Not logged in"
    ))

let admin f_success =
    loggedOn (session (function
        | UserLoggedOn { Role = "admin" } -> f_success
        | UserLoggedOn _ -> FORBIDDEN "Only for admin"
        | _ -> UNAUTHORIZED "Not logged in"
    ))

let powerUser f_success =
    loggedOn (session (function
        | UserLoggedOn { Role = "powerUser" } -> f_success
        | UserLoggedOn { Role = "admin" } -> f_success
        | UserLoggedOn _ -> FORBIDDEN "Only for power users"
        | _ -> UNAUTHORIZED "Not logged in"
    ))

let adminWithUserName f_success =
    loggedOn (session (function
        | UserLoggedOn { Role = "admin";Username = X } -> f_success X
        | UserLoggedOn _ -> FORBIDDEN "Only for admin"
        | _ -> UNAUTHORIZED "Not logged in"
    ))

let powerUserPassingUserLoggedOn f_success =
    loggedOn (session (function
        | UserLoggedOn { Role = "powerUser";Username = username;UserId=userid;RoleId=roleid;Temporary=false;CanManageAllOrders=canManageAllOrders} -> f_success {Role="powerUser";Username=username;UserId=userid;RoleId=roleid;Temporary=false;CanManageAllCourses=true;CanManageAllOrders=canManageAllOrders}
        | UserLoggedOn { Role = "admin";Username = username;UserId=userid;RoleId=roleid;Temporary=false} -> f_success {Role="admin";Username=username;UserId=userid;RoleId=roleid;Temporary=false;CanManageAllCourses=true;CanManageAllOrders=true}
        | UserLoggedOn _ -> FORBIDDEN "Only for power users"
        | _ -> UNAUTHORIZED "Not logged in"
    ))

let temporaryUserPassingUserLoggedOn f_success =
    let ctx = Db.getContext()
    loggedOn (session (function
        | UserLoggedOn { Role = role;Username = username;UserId=userid;RoleId=roleid} -> 
            let dbUser = Db.Users.getUser(userid) ctx
            if (dbUser.Istemporary) then 
                f_success 
                    {
                        Role=role;
                        Username=username;
                        UserId=userid;
                        RoleId=roleid;
                        Temporary=true;
                        CanManageAllCourses=false;
                        CanManageAllOrders=false
                    } else 
                FORBIDDEN "user is not enabled"
        | UserLoggedOn _ -> FORBIDDEN "Only for logged on users"
        | _ -> UNAUTHORIZED "Not logged in"
    ))

let anyUserExceptTemporary f_success =
    let ctx = Db.getContext()
    loggedOn (session (function
        | UserLoggedOn { Role = role;Username = username;UserId=userid;RoleId=roleid;Temporary=false} -> 
            let dbUser = Db.Users.getUser(userid) ctx
            if (dbUser.Enabled) then f_success {Role=role;Username=username;UserId=userid;RoleId=roleid;Temporary=false;CanManageAllCourses=dbUser.Canmanagecourses;CanManageAllOrders=dbUser.Canmanageallorders} else 
                FORBIDDEN "user is not enabled"
        | UserLoggedOn _ -> FORBIDDEN "Only for logged on users"
        | _ -> UNAUTHORIZED "Not logged in"
    ))

let anyUserExceptTemporaryRef f_success =
    loggedOn (session (function
        | UserLoggedOn { Temporary=false} ->  f_success
        | UserLoggedOn _ -> FORBIDDEN "temporary user is not allowed"
        | _ -> UNAUTHORIZED "Not logged on"
    ))

let anyUserPassingUserLoggedOn f_success =
    let ctx = Db.getContext()
    loggedOn (session (function
        | UserLoggedOn { Role = role;Username = username;UserId=userid;RoleId=roleid;Temporary=X} -> 
            let dbUser = Db.Users.getUser(userid) ctx
            if (dbUser.Enabled) then f_success {Role=role;Username=username;UserId=userid;RoleId=roleid;Temporary=X;CanManageAllCourses=dbUser.Canmanagecourses;CanManageAllOrders=dbUser.Canmanageallorders} else FORBIDDEN "user is not enabled"
        | UserLoggedOn _ -> FORBIDDEN "Only for logged on users"
        | _ -> UNAUTHORIZED "Not logged in"
    ))

let adminPassingUserLoggedOn f_success =
    loggedOn (session (function
        | UserLoggedOn { Role = "admin";Username = username;UserId=userid;RoleId=roleid} -> f_success {Role="admin";Username=username;UserId=userid;RoleId=roleid;Temporary=false;CanManageAllCourses=true;CanManageAllOrders=true}
        | UserLoggedOn _ -> FORBIDDEN "Only for admins"
        | _ -> UNAUTHORIZED "Not logged in"
    ))

let passHash (pass: string) =
    use sha = Security.Cryptography.SHA256.Create()
    Text.Encoding.UTF8.GetBytes(pass)
    |> sha.ComputeHash
    |> Array.map (fun b -> b.ToString("x2"))
    |> String.concat ""

let bindToForm form handler =
    bindReq (bindForm form) handler BAD_REQUEST

let sessionStore setF = context (fun x ->
    match HttpContext.state x with
    | Some state -> setF state
    | None -> never)

let returnPathOrHome = 
    request (fun x -> 
        let path = 
            match (x.queryParam "returnPath") with
            | Choice1Of2 path -> path
            | _ -> Path.home
        Redirection.FOUND path)

let makeOrderArchived id =
    let ctx = Db.getContext() 
    Db.archiveOrder id ctx
    Db.pushArchivedOrdersInLog id ctx

let authenticateUser (user : Db.UsersView) =
    authenticated Cookie.CookieLife.Session false 
    >=> session (function
        | CartIdOnly cartId ->
            sessionStore (fun store -> store.set "cartid" "")
        | _ -> succeed)
    >=> sessionStore (fun store ->
        store.set "username" user.Username
        >=> store.set "role" user.Rolename
        >=> store.set "userid" user.Userid
        >=> store.set "roleid" user.Role
        >=> store.set "temporary" user.Istemporary
        >=> store.set "canmanagecourses" user.Canmanagecourses
        >=> store.set "canmanageallorders" user.Canmanageallorders)

    >=> returnPathOrHome

let html container =
    let result cartItems userName = 
        OK (View.index container userName) >=> Writers.setMimeType "text/html; charset=utf-8" 
    session (function 
        | UserLoggedOn User -> result 0 (Some User.Username)
        | _ -> result 0 None)

// error handled at the caller side
let makeOrderItemAsRejectedIfContainsUnavalableIngredients orderItemId ctx =
    log.Debug(sprintf "%s orerItemId: %d" "makeOrderItemAsRejectedIfContainsUnavalableIngredients" orderItemId)
    let orderItemDetail = Db.Orders.getOrderItemDetail orderItemId ctx
    let ingredientOfCourse = Db.getIngredientsOfACourse orderItemDetail.Courseid ctx
    let unavailableIngredients = ingredientOfCourse |> List.filter (fun (x:Db.IngredientOfCourse) -> x.Checkavailabilityflag && ((x.Availablequantity - (x.Quantity*(decimal)orderItemDetail.Quantity)) < (decimal)0))

    let _ = if (unavailableIngredients.Length>0) then
                let rejectionCause = unavailableIngredients |> List.fold (fun x (y:Db.IngredientOfCourse) -> x+ y.Ingredientname ) "mancano: "
                Db.SetOrderItemAsRejected orderItemId ctx
                Db.createRejectedOrderItem orderItemId orderItemDetail.Courseid rejectionCause ctx |> ignore
                ctx.SubmitUpdates()
            else ()
    ()

let makeOrderItemRejectedIfContainsInvisibleIngredients orderItemId ctx =
    log.Debug(sprintf "makeOrderItemRejectedIfContainsInvisibleIngredients %d" orderItemId) 
    let orderItemDetail = Db.Orders.getOrderItemDetail orderItemId ctx
    let ingredientOfCourse = Db.getIngredientsOfACourse orderItemDetail.Courseid ctx
    let invisibleIngredients = ingredientOfCourse |> List.filter (fun (x:Db.IngredientOfCourse ) -> (not x.Visibility || not x.Categoryvisibility)) 
    let _ = if (invisibleIngredients.Length>0) then
                let rejectionCause = invisibleIngredients |> List.fold (fun x (y:Db.IngredientOfCourse) -> x + y.Ingredientname ) "mancano: "
                Db.SetOrderItemAsRejected orderItemId ctx
                Db.createRejectedOrderItem orderItemId orderItemDetail.Courseid rejectionCause ctx |> ignore
                ctx.SubmitUpdates()
            else ()
    ()

let logonViaQrCode  = 
    log.Debug("logonViaQrCode")
    choose [
        GET >=>
            (let ctx = Db.getContext()
            request (fun r ->
                match r.queryParam "specialCode" with
                | Choice1Of2 name -> 
                    let user = Db.getTemporaryUserViewByName name ctx
                    match user with
                    | Some theUser ->
                        authenticateUser theUser
                    | None -> UNAUTHORIZED "not logged on"
                | Choice2Of2 msg -> BAD_REQUEST msg)
            )
        POST >=> 
            Redirection.FOUND Path.Orders.myOrders
    ] 

let logon =
    log.Debug("logon")
    choose [
        GET >=>
            (View.logon "" |> html)
        POST >=> bindToForm Form.logon (fun form ->
            try
                let ctx = Db.getContext()
                let (Password password) = form.Password
                match Db.Users.validateUser(form.Username, passHash password) ctx with
                | Some user ->
                    authenticateUser user
                | _ ->
                    View.logon local.LoginOrPasswordInvalid |> html
            with
            | ex ->
                log.Error("Error in logon", ex)
                View.logon local.AnErrorOccurred |> html
        )
    ]

let deleteObjects = 
    View.objectDeletionPage |> html

let error = 
    View.notFound |> html

let deleteUsers = warbler (fun _ ->
    log.Debug("deleteUsers")
    let ctx = Db.getContext()
    let ordinaryUsers = Db.Users.getOrdinaryUsersView ctx |> List.filter  (fun (x:Db.UsersView) -> x.Rolename <> "admin")
    View.userDeletionPage ordinaryUsers Path.Admin.deleteUsers |> html
)

let deleteTemporaryUsers = warbler (fun _ ->
    log.Debug("deleteTemporaryUsers")
    let ctx = Db.getContext()
    let ordinaryUsers = Db.Users.getTemporaryUsersView ctx |> List.filter  (fun (x:Db.UsersView) -> x.Rolename <> "admin")
    View.userDeletionPage ordinaryUsers Path.Admin.deleteTemporaryUsers |> html
)

let deleteUserRoles = warbler (fun _ -> (
    log.Debug("deleteUserRoles")
    let ctx = Db.getContext()
    let allUserRoles = Db.getAllRoles ctx
    View.rolesDeletionPage allUserRoles |> html))

let alignAllPricesOfAnOrder orderId =
    log.Debug("alignAllPricesOfAnOrder")
    let ctx = Db.getContext()
    let order = Db.Orders.getOrder orderId ctx
    let orderItems = order.``public.orderitems by orderid``
    let plainTotal = orderItems |> Seq.map (fun (x:Db.OrderItem) -> x.Price) |> Seq.fold (fun acc x -> acc + x) ((decimal)0.0)
    let discountedPrice = 
        match (order.Adjustispercentage,order.Adjustisplain) with
        | (true,false) -> plainTotal + (order.Percentagevariataion)/((decimal)100.0)*plainTotal
        | (false,true) -> plainTotal + order.Plaintotalvariation
        | _ -> plainTotal
    order.Total <- plainTotal
    order.Adjustedtotal <- discountedPrice

let deleteIngredientsBySelection =
    log.Debug("deleteIngredientsBySelection")
    choose [
        GET >=> 
            warbler (fun _ ->
            let ctx = Db.getContext()
            let allIngredients = Db.getAllIngredients ctx   
            View.ingredientsDeletionPageBySelect allIngredients |> html)
        POST >=> bindToForm Form.ingredientDeletion  (fun form ->
            try
                let ctx = Db.getContext()
                let ingredientName = form.IngredientName
                let ingredient = Db.tryGetIngredientByName ingredientName ctx
                match ingredient with
                | Some theCourse -> 
                    Db.safeDeleteIngredient theCourse.Ingredientid ctx
                    Redirection.FOUND Path.Admin.deleteIngredients
                | _ ->  Redirection.FOUND Path.Admin.deleteIngredients
            with
            | ex ->
                log.Error("Error in deleteIngredientsBySelection", ex)
                View.logon local.AnErrorOccurred |> html
        )
    ]

let deleteIngredients = warbler (fun _ ->
    log.Debug("deleteIngredients")
    let ctx = Db.getContext()
    let allIngredients = Db.getAllIngredients ctx
    View.ingredientsDeletionPage allIngredients |> html)

let deleteUserRole id =
    log.Debug("deleteUserRole")
    let ctx = Db.getContext()
    try
        Db.safeDeleteRole id ctx
        Redirection.found Path.Admin.deleteObjects
    with
    | ex ->
        log.Error("Error in deleteUserRole", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

let deleteIngredient id =
    log.Debug("deleteIngredient")
    let ctx = Db.getContext()
    Db.safeDeleteIngredient id ctx
    Redirection.found Path.Admin.deleteObjects 

let deletePrinter id =
    log.Debug("deletePrinter")
    let ctx = Db.getContext()
    Db.safeRemovePrinter id ctx
    Redirection.found Path.Admin.printers

let deleteIngredientPrice id =
    log.Debug("deleteIngredientPrice")
    let ctx = Db.getContext()
    try
        let ingredientPrice = Db.getIngredientPrice id ctx
        let ingredientId = ingredientPrice.Ingredientid
        Db.safeDeleteIngredientPrice ingredientPrice ctx
        Redirection.found (sprintf Path.Admin.editIngredientPrices ingredientId)
    with
    | ex ->
        log.Error("Error in deleteIngredientPrice", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

let deleteCourseCategories =
    log.Debug("deleteCourseCategories")
    choose [
        GET >=> warbler (fun _ ->
            let ctx = Db.getContext()
            let allCourseCategories = Db.Courses.getAllourseCategories ctx
            View.courseCategoriesDeletionPage allCourseCategories |> html
        )
    ]

let deleteIngredientCategories  = warbler ( fun _ ->
    log.Debug("deleteIngredientCategories")
    let ctx = Db.getContext()
    let allIngredientCategories = Db.getAllIngredientCategories ctx
    View.ingredientCategoriesDeletionPage allIngredientCategories |> html
    )

let deleteCourses = 
    log.Debug("deleteCourses")
    choose [
        GET >=> 
            warbler (fun _ ->
            let ctx = Db.getContext()
            let allCourses = Db.Courses.getAllCourses ctx   
            View.coursesDeletionPage allCourses |> html)
        POST >=> bindToForm Form.courseDeletion  (fun form ->
            try
                let ctx = Db.getContext()
                let courseName = form.CourseName
                let course = Db.Courses.tryGetCourseByName courseName ctx
                match course with
                | Some theCourse -> 
                    Db.safeDeleteCourse theCourse ctx
                    Redirection.FOUND Path.Admin.deleteCourses
                | _ ->  Redirection.FOUND Path.Admin.deleteCourses
            with
            | ex ->
                log.Error("Error in deleteCourses", ex)
                View.logon local.AnErrorOccurred |> html
        )
    ]

let emtpy = warbler (fun _ ->
    loggedOn (session (function 
    | UserLoggedOn userLoggedOn ->
        log.Debug(sprintf "empty%s " userLoggedOn.Username)
        let ctx = Db.getContext()
        let user = Db.getUserById userLoggedOn.UserId ctx
        View.empty userLoggedOn user |> html
        )
    )) 

let controlPanel (userLoggedOn:UserLoggedOnSession) = warbler (fun _ ->
    log.Debug(sprintf "controlPanel by user:%s" userLoggedOn.Username)
    let ctx = Db.getContext()
    let user = Db.getUserById userLoggedOn.UserId ctx
    View.controlPanel userLoggedOn user |> html
)

let controlPanelRef  = warbler (fun _ ->
    loggedOn (session (function 
        | UserLoggedOn userLoggedOn ->
            log.Debug(sprintf "controlPanel by user:%s" userLoggedOn.Username)
            let ctx = Db.getContext()
            let user = Db.getUserById userLoggedOn.UserId ctx
            View.controlPanel userLoggedOn user |> html
        | _ -> FORBIDDEN "not logged on"
        )
    )
)

let userEnabledToSeeWholeDoneOrers userId = 
    log.Debug(sprintf "userEnabledToSeeWholeDoneOrers %d" userId)
    let ctx = Db.getContext()
    Db.isUserEnabledToSeeWholeOrders userId ctx

let categoriesManagement (userLoggedOn:UserLoggedOnSession) = warbler (fun _ ->
    log.Debug(sprintf "categoriesManagement by user%s" userLoggedOn.Username )
    let ctx = Db.getContext()
    let allCategories = Db.Courses.getAllCategories ctx
    View.coursesAndCategoriesManagement  allCategories |> html
)

let qrOrder (userLoggedOn:UserLoggedOnSession) = warbler (fun _ ->
    log.Debug(sprintf "qrOrder by user: %s" userLoggedOn.Username)
    let ctx = Db.getContext()
    let orders = Db.Orders.getOngoingOrderDetailsByUser userLoggedOn.UserId ctx |> List.filter (fun (x:Db.Orderdetail) ->  x.Forqruserarchived<> true)
    let activeCategories = Db.Courses.getActiveCategories ctx

    let orderItemsOfOrders = 
        orders |> 
        List.map (fun (x:Db.Orderdetail) -> (x.Orderid, Db.getOrderItemDetailOfOrderDetail x ctx)) 
        |> Map.ofList
    let mapOfStates = Db.getMapOfStates ctx

    let eventualRejectionsOfOrderItems = 
        orders |>
        List.map (fun (x:Db.Orderdetail) ->  Db.getOrderItemDetailOfOrderDetail x ctx  ) |> List.fold (@) [] 
        |> List.map (fun (x:Db.OrderItemDetails) -> (x.Orderitemid,Db.getLatestRejectionOfOrderItem x.Orderitemid ctx)) |> Map.ofList
    View.qrOrder userLoggedOn orders activeCategories orderItemsOfOrders mapOfStates  eventualRejectionsOfOrderItems |> html)

let myOrders (userLoggedOn:UserLoggedOnSession) = warbler (fun _ ->
    log.Debug(sprintf "myOrders by user:%s " userLoggedOn.Username)
    let ctx = Db.getContext()
    let orders = Db.Orders.getOngoingOrderDetailsByUser userLoggedOn.UserId ctx
    let userView = Db.getUserViewById (userLoggedOn.UserId) ctx
    let activeCategories = Db.Courses.getActiveCategories ctx
    let statesEnabledForUser = Db.listOfEnabledStatesForWaiter userView.Userid ctx
    let orderItemsOfOrders = orders |> List.map (fun (x:Db.Orderdetail) -> (x.Orderid, Db.getOrderItemDetailOfOrderDetail x ctx)) |> Map.ofList
    let mapOfLinkedStates = Db.getMapOfStates ctx 
    let backUrl = Path.Orders.myOrders
    let initialStateId = (Db.States.getInitState ctx).Stateid
    let outGroupsOfOrders = orders |> List.map (fun (x:Db.Orderdetail) -> 
        (x.Orderid,Db.getOutGroupsOfOrder x.Orderid ctx)) |> Map.ofList
    let eventualRejectionsOfOrderItems = 
        orders |>
        List.map (fun (x:Db.Orderdetail) ->  Db.getOrderItemDetailOfOrderDetail x ctx  ) |> List.fold (@) [] 
        |> List.map (fun (x:Db.OrderItemDetails) -> (x.Orderitemid,Db.getLatestRejectionOfOrderItem x.Orderitemid ctx)) |> Map.ofList
    View.ordersList userView  orders activeCategories orderItemsOfOrders  mapOfLinkedStates statesEnabledForUser backUrl eventualRejectionsOfOrderItems initialStateId  outGroupsOfOrders |> html)

let myOrdersSingles (userLoggedOn:UserLoggedOnSession) = warbler (fun _ ->
    log.Debug("myOrdersSingles")
    let ctx = Db.getContext()
    let myOrders = Db.Orders.getOngoingOrderDetailsByUser userLoggedOn.UserId ctx
    let otherOrders = match userLoggedOn.CanManageAllOrders with 
                                        |  true -> Db.Orders.getOngoingOrderDetailsByAllUserExcept userLoggedOn.UserId ctx
                                        |  false -> []

    let userView = Db.getUserViewById (userLoggedOn.UserId) ctx
    let statesEnabledForUser = Db.listOfEnabledStatesForWaiter userView.Userid ctx
    let mapOfLinkedStates = Db.getMapOfStates ctx 
    let initialStateId = (Db.States.getInitState ctx).Stateid
    let outGroupsOfOrders = myOrders |> List.map (fun (x:Db.Orderdetail) -> 
        (x.Orderid,Db.getOutGroupOfOrdeHavingSomeItemsInInitialState x.Orderid ctx)) |> Map.ofList
    View.ordersListbySingles userView  myOrders  otherOrders  mapOfLinkedStates statesEnabledForUser   initialStateId  outGroupsOfOrders |> html)

let ordinaryUsers  = warbler (fun _ ->
    log.Debug("ordinaryUsers")
    let ctx = Db.getContext()
    let allUsers = Db.Users.getOrdinaryUsersView ctx
    View.userAdministrationPage  allUsers |> html)

let temporaryUsers = warbler (fun _ ->
    log.Debug("temporaryUsers")
    let ctx = Db.getContext()
    let temporaryUsers = Db.Users.getTemporaryUsersView ctx |> List.sortBy (fun (x:Db.UsersView) -> (x.Creationtime))
    View.temporaryUsersAdministrationPage  temporaryUsers |> html
)

let switchVisbilityOfIngredientCategory ingredientCatgoryId encodedBackUrl  =  
    log.Debug(sprintf "switchVisbilityOfIngredientCategory %d" ingredientCatgoryId)
    warbler (fun x ->
        let backUrl = WebUtility.UrlDecode encodedBackUrl
        let ctx = Db.getContext()
        let category = Db.tryGetIngredientCategory ingredientCatgoryId ctx
        try
            let _ = 
                match category with 
                | Some theCategory -> 
                    theCategory.Visibility <- (not theCategory.Visibility)
                    ctx.SubmitUpdates()
                | _ -> ()
            Redirection.FOUND backUrl
        with
        | ex ->
            log.Error("Error in switchVisbilityOfIngredientCategory", ex)
            Redirection.FOUND backUrl
    )

let switchVisbilityOfIngredient ingredientCategoryId ingredientId  =
    log.Debug(sprintf "switchVisbilityOfIngredient %d"  ingredientId)
    let ctx = Db.getContext()
    let ingredient = Db.tryGetIngredientById  ingredientId ctx
    try
        let _ = 
            match ingredient with 
            | Some theIngredient -> 
                theIngredient.Visibility <- (not theIngredient.Visibility); 
                ctx.SubmitUpdates()
            | _ -> ()
        let redirTo = sprintf  Path.Admin.editIngredientCategory ingredientCategoryId
        Redirection.FOUND redirTo
    with
    | ex ->
        log.Error("Error in switchVisbilityOfIngredient", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

type IngredientNameModel = {ingredientname: string}

let viewIngredientUsage ingredientId pageNumberBack =
    log.Debug(sprintf "viewIngredientUsage %d" ingredientId)
    let ctx = Db.getContext()
    let ingredient = Db.getIngredientById ingredientId ctx
    choose [
        GET >=> warbler ( fun _ ->
            let o = {ingredientname = ingredient.Name} 
            DotLiquid.page("ingredientDecrementSearch.html")  o
        )
        POST  >=> bindToForm Form.date (
            fun form -> 
                let ingredientDecrements = Db.getIngredientDecrementsStartingFromDate ingredientId form.Date ctx
                (View.seeVariationOfIngredientStartingFromDate ingredient form.Date ingredientDecrements) |> html)
    ]

let fillIngredient id pageNumberBack (user:UserLoggedOnSession) =
    log.Debug(sprintf "fillIngredient %d" id)
    let ctx = Db.getContext()
    let ingredient = Db.getIngredientById id ctx
    choose [
        GET >=> warbler (fun _ ->
            let visibleIngredientCategories = Db.getAllVisibleIngredientCategories ctx
            let theCategoryOfIngredient = Db.getIngredientCatgory ingredient.Ingredientcategoryid ctx
            let allSelectableCategories = 
                match theCategoryOfIngredient.Visibility with
                | true -> visibleIngredientCategories
                | false -> theCategoryOfIngredient::visibleIngredientCategories
            View.fillIngredient "" ingredient allSelectableCategories pageNumberBack |> html)
        POST >=> bindToForm Form.ingredientLoad (fun form ->
            try
                let quantity = form.Quantity
                let comment = form.Comment
                let increment = Db.createIngredientIncrement ingredient quantity ingredient.Unitmeasure comment user.UserId ctx
                let _ = match ingredient.Updateavailabilityflag with | true -> Db.addAmountOfingredientAvailability ingredient quantity ctx | _ -> ()
                Redirection.found (sprintf Path.Admin.editIngredientCategoryPaginated ingredient.Ingredientcategoryid pageNumberBack)
            with
            | ex ->
                log.Error("Error in fillIngredient", ex)
                View.fillIngredient local.AnErrorOccurred ingredient [] pageNumberBack |> html
        )
    ]

let editIngredientPrices id =
    log.Debug(sprintf "editIngredientPrices %d" id)
    let ctx = Db.getContext()
    let ingredient = Db.getIngredientById id ctx
    let ingredientPricesDetails = Db.getIngredientPricesDetails id ctx
    choose [
        GET >=> warbler (fun _ ->
            View.editIngredientPrices ingredient ingredientPricesDetails "" |> html
        )
        POST >=> bindToForm Form.ingredientPrice (fun form -> 
            try
                let isDefaultSubtract = (form.IsDefaultSubtract = Form.YES) //"YES"
                let isDefaultAdd = (form.IsDefaultAdd = Form.YES)
                match (Db.tryCreateIngredientPrice form.AddPrice id isDefaultAdd isDefaultSubtract form.Quantity form.SubtractPrice ctx) with
                    | Some _ -> Redirection.found (sprintf Path.Admin.editIngredientPrices id)
                    | None -> View.editIngredientPrices ingredient ingredientPricesDetails local.AlreadyExists |> html
            with
            | ex ->
                log.Error("Error in editIngredientPrices", ex)
                View.editIngredientPrices ingredient ingredientPricesDetails local.AnErrorOccurred |> html
        )
    ]

let editIngredient id  pageNumberBack =
    log.Debug(sprintf "%s %d" "editIngredient" id)
    choose [
        GET >=> warbler (fun _ ->
            let ctx = Db.getContext()
            let ingredient = Db.getIngredientById id ctx
            let visibleIngredientCategories = Db.getAllVisibleIngredientCategories ctx
            let theCategoryOfIngredient = Db.getIngredientCatgory ingredient.Ingredientcategoryid ctx
            let allSelectableCategories = 
                match theCategoryOfIngredient.Visibility with
                | true -> visibleIngredientCategories
                | false -> theCategoryOfIngredient::visibleIngredientCategories

            let ingredient = Db.getIngredientById id ctx
            View.editIngredient "" ingredient allSelectableCategories pageNumberBack |> html)
        POST >=> bindToForm Form.ingredientEdit (fun form ->
            let ctx = Db.getContext()
            let existing = Db.findIngredientByName form.Name ctx
            match existing with
            | Some theExisting when (theExisting.Ingredientid <> id) ->
                let allIngredientOfCategory = Db.getallIngredientsOfACategory theExisting.Ingredientcategoryid ctx
                let message = local.NameIsUsed
                let category = Db.getCategoryOfIngredients theExisting.Ingredientcategoryid ctx
                View.ingredientsOfACategory message category allIngredientOfCategory  |> html
            | _ ->
                try
                    let visibility = (form.Visibility = Form.VISIBLE)
                    let description = match form.Comment with | Some X -> X | _ -> ""
                    let ingredient = Db.getIngredientById id ctx
                    ingredient.Name <- form.Name
                    ingredient.Description <-description
                    ingredient.Allergen <- (form.Allergene = Form.YES) // 
                    ingredient.Visibility <- visibility
                    let oriCategory = ingredient.Ingredientcategoryid
                    ingredient.Ingredientcategoryid <- (int)form.Category
                    ingredient.Updateavailabilityflag <- (form.UpdateAvailabilityFlag = Form.YES)
                    ingredient.Checkavailabilityflag <- (form.CheckAvailabilityFlag = Form.YES)
                    ingredient.Unitmeasure <- (form.UnitOfMeasure)
                    ctx.SubmitUpdates()
                    Redirection.found (sprintf Path.Admin.editIngredientCategoryPaginated oriCategory pageNumberBack)
                with
                | ex ->
                    log.Error("Error in editIngredient", ex)
                    Redirection.FOUND Path.Errors.unableToCompleteOperation
            )
    ]

let editIngredientCategoryPaginated (idCategory:int) (pageNumber: int) =
    log.Debug(sprintf "%s %d %d" "editIngredientCategoryPaginated" idCategory pageNumber)
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ ->
            let ingredientsOfCurrentPage = Db.getAllIngredientsOfACategoryByPage idCategory pageNumber ctx
            let numberOfAllIngredientOfACategory = Db.getNumbeOfAllIngredientsOfACategory idCategory ctx
            let numberOfPages = (numberOfAllIngredientOfACategory - 1)/Globals.NUM_DB_ITEMS_IN_A_PAGE
            let ingredientCategory = Db.getCategoryOfIngredients idCategory ctx
            View.seeIngredientsOfACategoryPaginated ingredientCategory ingredientsOfCurrentPage numberOfPages pageNumber |> html)

        POST >=> bindToForm Form.searchIngredient (fun form ->
            let ingredientsOfCurrentpage = Db.getAllIngredientsOfACategoryByPageWithNameSearch idCategory 0 form.Name ctx
            let numberOfAllIngredientOfACategory = Db.getNumbeOfAllIngredientsOfACategory idCategory ctx
            let numberOfPages = (numberOfAllIngredientOfACategory - 1)/Globals.NUM_DB_ITEMS_IN_A_PAGE
            let ingredientCategory = Db.getCategoryOfIngredients idCategory ctx
            View.seeIngredientsOfACategoryPaginated ingredientCategory ingredientsOfCurrentpage numberOfPages 0 |> html
        )
    ]

let editIngredientCategory (idCategory: int) (user: UserLoggedOnSession) =
    log.Debug(sprintf "%s %d" "editIngredientCategory" idCategory)
    choose [
        GET >=> warbler (fun _ ->
            let ctx = Db.getContext()
            let allIngredientOfCategory = Db.getallIngredientsOfACategory idCategory ctx
            let category = Db.getCategoryOfIngredients idCategory ctx
            View.ingredientsOfACategory "" category allIngredientOfCategory  |> html)
        POST >=> bindToForm Form.ingredient (fun form ->
            let ctx = Db.getContext()
            match Db.findIngredientByName form.Name ctx with
            | Some existing ->
                let allIngredientOfCategory = Db.getallIngredientsOfACategory idCategory ctx
                let category = Db.getCategoryOfIngredients idCategory ctx
                let message = "ingrediente "+form.Name + " "+local.AlreadyExists
                View.ingredientsOfACategory message category allIngredientOfCategory   |> html
            | None ->
                try
                    let visibility = (form.Visibility = Form.VISIBLE)
                    let allergene = (form.Allergene = Form.YES)
                    let description = match form.Comment with | Some X -> X | _ -> ""
                    let updateAvailabilityFlag = (form.UpdateAvailabilityFlag =  Form.YES)
                    let checkAvailabilityFlag = (form.CheckAvailabilityFlag = Form.YES)
                    let unitOfMeasure = form.UnitOfMeasure
                    let newIngredient = Db.newIngredient form.Name visibility description idCategory allergene form.AvailableQuantity  updateAvailabilityFlag checkAvailabilityFlag unitOfMeasure  ctx
                    let _ =
                        match (int) (form.AvailableQuantity) with
                        | 0 -> ()
                        | _ -> 
                            let initialIncrement = Db.createIngredientIncrement newIngredient form.AvailableQuantity unitOfMeasure "inserimento iniziale" user.UserId ctx
                            ()
                    Redirection.FOUND ((sprintf  Path.Admin.editIngredientCategoryPaginated idCategory 0)) 
                with
                | ex ->
                    log.Error("Error in editIngredientCategory", ex)
                    Redirection.FOUND Path.Errors.unableToCompleteOperation
            )
    ]

let visibleingredientCategories =
    log.Debug("visibleingredientCategories")
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun x ->
            let ctx = Db.getContext()
            let visibleIngredientCategories = Db.getVisibleIngredientCategories ctx
            View.visibleIngreientCategoriesAdministrationPage visibleIngredientCategories  |> html)
        POST >=> bindToForm Form.ingredientCategory ( fun form -> 
            try
                let visibility = (form.Visibility = Form.VISIBLE)  
                let description = match form.Comment with | Some X -> X | _ -> ""
                let _ = Db.newIngredientCatgory form.Name visibility description ctx
                Redirection.FOUND Path.Admin.allIngredientCategories
            with
            | ex ->
                log.Error("Error in visibleingredientCategories", ex)
                Redirection.FOUND Path.Errors.unableToCompleteOperation
        )
    ]

type IndexStringList = {index:int; names: string list}
type MyPrinterModel = 
    {
        name: string; 
        coursecategories: IndexNameRecord list;
        catenabledforcurrentprinter: IndexNameRecord list;
        printerforstateenabled: IndexNameRecord list;
        backurl: string;
        enabledstatesforcategories: IndexStringList list;
        selectedcategory: int;
        printername: string;
        printerid: int;
        isenabledforprintinginvoices: bool;
        isenabledforprintingreceipts: bool;
    }

let managePrinter printerId categoryId =
    log.Debug(sprintf "%s %d %d" "managePrinter" printerId categoryId)
    let ctx = Db.getContext()
    let printer = Db.getPrinter printerId ctx
    choose [
        GET >=>
        warbler (fun _ -> 
            let courseCategories = Db.Courses.getVisibleCourseCategories ctx
            let printerForCategories = Db.getPrinterForCategoriesDetails printerId ctx
            let stateMappingForPrinter = 
                Db.getStatesPrinterMapping printerId ctx |>
                List.map (fun (x:Db.PrinterForCourseCategoryDetail) -> {index=x.Stateid;name = x.Statename})
            let idAndCategories = 
                courseCategories 
                |> List.map (fun (x:Db.CourseCategories) -> {index=x.Categoryid;name=x.Name})
            let idAndEnabledCategories = 
                printerForCategories 
                |> List.map (fun (x:Db.PrinterForCourseCategoryDetail) -> {index=x.Categoryid;name=x.Categoryname})
            let categoryStatesForCurrentPrinter = Db.getStatesPrinterMapping printerId ctx 
            let categoriesIds = 
                categoryStatesForCurrentPrinter 
                |> List.map (fun (x:Db.PrinterForCourseCategoryDetail) -> x.Categoryid) |> Set.ofList |> Set.toList
            let printerForReceiptAndInvoice = Db.tryGetPrinterForReceiptAndInvoice printerId ctx
            let (canPrintReceipt,canPrintInvoice) = 
                match printerForReceiptAndInvoice with    
                | Some X -> (X.Printreceipt,X.Printinvoice)
                | None -> (false,false)
            let mapOfEnabledStatesForCategories = 
                categoriesIds 
                |>  List.map 
                    (fun i -> 
                        (
                            i, 
                            categoryStatesForCurrentPrinter 
                            |> List.filter (fun (x:Db.PrinterForCourseCategoryDetail) -> x.Categoryid = i) 
                            |> List.map (fun (x:Db.PrinterForCourseCategoryDetail) -> x.Statename) ))
                        |> List.map (fun (i,j) -> {index=i;names=j}
                    )
            let selectedCategory = categoryId
            let o = 
                {
                    name = printer.Name
                    coursecategories = idAndCategories
                    catenabledforcurrentprinter=idAndEnabledCategories
                    printerforstateenabled = stateMappingForPrinter
                    backurl = Path.Admin.printers
                    enabledstatesforcategories = mapOfEnabledStatesForCategories
                    selectedcategory = selectedCategory
                    printername = printer.Name 
                    printerid = printer.Printerid
                    isenabledforprintinginvoices = canPrintInvoice
                    isenabledforprintingreceipts = canPrintReceipt
                }
            DotLiquid.page("printerStateCategoryMapping.html") o
        )
        POST >=> bindToForm Form.printerOutGroupForStates (fun form ->
            let categoryId = form.CategoryId
            let _ = 
                match form.TOBEWORKED with 
                | Some _ -> Db.createPrinterCategoryAssociation printerId ((int)categoryId) "TOBEWORKED"  ctx |> ignore
                | None -> Db.removePrinterCategoryAssociation printerId ((int)categoryId) "TOBEWORKED"  ctx |> ignore
            let _ = 
                match form.STARTEDWORKING with 
                | Some _ -> Db.createPrinterCategoryAssociation printerId ((int)categoryId) "STARTEDWORKING" ctx |> ignore
                | None  -> Db.removePrinterCategoryAssociation printerId ((int)categoryId) "STARTEDWORKING" ctx |> ignore
            let _ = 
                match form.READYFORDELIVERY with 
                | Some _ -> Db.createPrinterCategoryAssociation printerId ((int)categoryId) "READYFORDELIVERY" ctx |> ignore
                | None -> Db.removePrinterCategoryAssociation printerId ((int)categoryId) "READYFORDELIVERY" ctx |> ignore
            let _ = 
                match form.DELIVERED with 
                | Some _ -> Db.createPrinterCategoryAssociation printerId ((int)categoryId) "DELIVERED" ctx |> ignore
                | None -> Db.removePrinterCategoryAssociation printerId ((int)categoryId) "DELIVERED" ctx |> ignore
            let _ = 
                match form.DONE with 
                | Some _ -> Db.createPrinterCategoryAssociation printerId ((int)categoryId) "DONE" ctx |> ignore
                | None -> Db.removePrinterCategoryAssociation printerId ((int)categoryId) "DONE" ctx |> ignore
            let _ =
                match form.PRINTINVOICE with
                | Some _ -> Db.createOrUpdatePrinterInvoiceAssociation printerId ctx |> ignore
                | None -> Db.removePrinterInvoiceAssociationIfExists printerId ctx |> ignore
            let _ =
                match form.PRINTRECEIPT with
                | Some _ -> Db.createOrUpdatePrinterReceiptAssociation printerId ctx |> ignore
                | None -> Db.removePrinterReceiptAssociationIfExists printerId ctx |> ignore
            Redirection.FOUND (sprintf Path.Admin.managePrinter printerId ((int)categoryId)))
    ]

let recognizePrinters = 
    log.Debug("recognizePrinters")
    warbler (fun _ -> 
        try
            let ctx = Db.getContext()
            let printerNames = [for i in System.Drawing.Printing.PrinterSettings.InstalledPrinters do yield i]
            printerNames |> List.iter (fun printerName -> Db.createPrinter printerName ctx)
            Redirection.FOUND Path.Admin.printers
        with
        | ex -> 
            log.Error(sprintf "recognizePrinters %s" ex.Message)
            Redirection.FOUND Path.Admin.printers
    )

let resetPrinters =
    log.Debug("resetPrinters")
    warbler (fun _  ->
        let ctx = Db.getContext()
        Db.safeRemovePrinters ctx
        Redirection.FOUND Path.Admin.printers
    )

let adminPrinters =
    log.Debug("adminPrinters")
    let ctx = Db.getContext()
    warbler (fun _ ->
        let printers = Db.getPrinters ctx
        View.adminPrinters printers |> html
    )

let about = 
    log.Debug("about")
    warbler (fun _ ->
        View.about |> html
    )

let adminIngredientCategories = 
    log.Debug("adminIngredientCategories")
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ ->
            let ctx = Db.getContext()
            let allIngredientCategories = Db.getAllIngredientCategories ctx
            View.ingredientCatgoriesAdministrationPage allIngredientCategories  |> html)
        POST >=> bindToForm Form.ingredientCategory ( fun form -> 
            try
                let visibility = (form.Visibility = Form.VISIBLE)  
                let description = match form.Comment with | Some X -> X | _ -> ""
                let _ = Db.newIngredientCatgory form.Name visibility description ctx
                Redirection.FOUND Path.Admin.allIngredientCategories
            with
            | ex ->
                log.Error("Error in adminIngredientCategories", ex)
                Redirection.FOUND Path.Errors.unableToCompleteOperation
        )
    ]

let adminRoles  =  
    log.Debug("adminRoles")
    let ctx = Db.getContext()
    warbler (fun _ ->
            let allRoles = Db.getAllRoles ctx
            let allRolesWithObservers = Db.getObserverRoleStatusCategory ctx
            let allRolesWithEnablers = Db.getEnablerRoleStatusCategory ctx
            View.rolesAdministrationPage allRoles allRolesWithObservers allRolesWithEnablers |> html
    )

let adminCategories  = 
    log.Debug("adminCategories")
    let ctx = Db.getContext()
    choose [
            GET >=> 
                warbler (fun _ ->
                    let coursesCategories = Db.Courses.getAllRootCategories ctx
                    View.coursesAdministrationPage  coursesCategories [] |> html
                )
            POST >=> bindToForm Form.searchCourse (fun form ->
                let coursesCategories = Db.Courses.getAllRootCategories ctx
                let resultSearch = Db.Courses.searchCourses form.Name ctx
                View.coursesAdministrationPage  coursesCategories resultSearch |> html
            )
    ]

let allOrders (userLoggedOn:UserLoggedOnSession) = warbler (fun _ ->
    log.Debug("allOrders")
    let ctx = Db.getContext()
    let userView = Db.getUserViewById userLoggedOn.UserId ctx
    let activeCategories = Db.Courses.getActiveCategories ctx
    let orders = Db.Orders.getOngoingOrderdetails ctx
    let statesEnabledForUser = Db.listOfEnabledStatesForWaiter userView.Userid ctx
    let orderItemsOfOrders = orders |> List.map (fun (x:Db.Orderdetail) -> (x.Orderid, Db.getOrderItemDetailOfOrderDetail x ctx)) |> Map.ofList
    let mapOfStates = Db.getMapOfStates ctx
    let backUrl = Path.Orders.allOrders
    let initStateId = (Db.States.getInitState ctx).Stateid
    let outGroupsOfOrders = orders |> List.map (fun (x:Db.Orderdetail) -> 
        (x.Orderid,Db.getOutGroupsOfOrder x.Orderid ctx)) |> Map.ofList
    let eventualRejectionsOfOrderItems = 
        orders |>
        List.map (fun (x:Db.Orderdetail) ->  Db.getOrderItemDetailOfOrderDetail x ctx  ) |> List.fold (@) [] 
        |> List.map (fun (x:Db.OrderItemDetails) -> (x.Orderitemid,Db.getLatestRejectionOfOrderItem x.Orderitemid ctx)) |> Map.ofList
    View.ordersList userView  orders activeCategories orderItemsOfOrders mapOfStates statesEnabledForUser  backUrl  eventualRejectionsOfOrderItems initStateId outGroupsOfOrders |> html)

let allSubOrdersOfOrderDetailBelongsToAPaidSubOrder orderId =
    log.Debug(sprintf "allSubOrdersOfOrderDetailBelongsToAPaidSubOrderRef  %d" orderId)
    let ctx = Db.getContext()
    let orderItems = Db.Orders.getOrderItemDetailsOfOrder orderId ctx
    let orderItemsBelongingToSubOrder = orderItems |> List.filter(fun (x:Db.OrderItemDetails) -> Db.Orders.orderItemIsInASubOrder x.Orderitemid ctx)  |> List.filter (fun x -> Db.Orders.isSubOrderPaid x.Suborderid ctx) 
    (List.length orderItemsBelongingToSubOrder) = (List.length orderItems)

let createEcrReceiptInstructionForSubOrder subOrderId orderId =
    log.Debug(sprintf "createEcrReceiptInstructionForSubOrder subOrderId:%d orderId%d" subOrderId orderId)
    let ctx = Db.getContext()
    let subOrder = Db.Orders.getSubOrder subOrderId ctx
    let paymentItemDetailsOfSubOrder  = Db.Orders.getPaymentItemDetailsOfSubOrder subOrderId ctx
    let orderItemsOfSubOrder = Db.getOrderItemDetailsOfSubOrder subOrderId ctx
    let orderItemRows = 
        orderItemsOfSubOrder 
        |> List.map (fun (x:Db.OrderItemDetails) -> 
            (
                if (x.Quantity>1) then (sprintf Globals.ITALIAN_TEMPLATE_ROW_FOR_PAYMENT_ITEM x.Price x.Quantity x.Name ) 
                else (sprintf Globals.ITALIAN_TEMPLATE_ROW_FOR_UNITARY_PAYMENT_ITEM x.Price  x.Name ) 
            )
        )
    let paymentItemRows = paymentItemDetailsOfSubOrder |> List.map (fun x -> sprintf Globals.ITALIAN_TEMPLATE_ROW_FOR_PAYMENT_CLOSURE_ITEM ((int)x.Tendercodeidentifier) x.Amount)
    let outFile = new System.IO.StreamWriter(Settings.EcrFilePath,false,Encoding.ASCII)
    let _ = orderItemRows |> List.iter (fun x -> outFile.WriteLine(x))
    let _ = 
        match subOrder.Subtotaladjustment with
        | X when X< 0M ->
                    outFile.WriteLine("SUBTOT")
                    outFile.WriteLine(sprintf "%s %.2f" "SCONT val=" ( -  subOrder.Subtotaladjustment))
        | X when X > 0M ->
                    outFile.WriteLine("SUBTOT")
                    outFile.WriteLine(sprintf "%s %.2f, term=34" "inp num=" (subOrder.Subtotaladjustment))
        | _ -> ()

    let _ = paymentItemRows |> List.iter (fun x -> outFile.WriteLine(x))
    let _ = outFile.Close()
    Db.setSubOrderAsPaid subOrderId ctx
    let _ = if (allSubOrdersOfOrderDetailBelongsToAPaidSubOrder orderId) then (makeOrderArchived orderId) else ()
    Redirection.found (sprintf Path.Orders.subdivideDoneOrder orderId)

let createEcrReceiptInstructionForOrder  orderId =
    log.Debug(sprintf "createEcrReceiptInstructionForSubOrder %d" orderId)
    try
        let ctx = Db.getContext()
        let paymentItemDetailsOfSubOrder  = Db.Orders.getPaymentItemDetailsOfOrder orderId ctx
        let order = Db.Orders.getOrder orderId ctx
        let orderItemsOfSubOrder = Db.getOrderItemDetailsOfOrder orderId ctx
        let orderItemRows = 
            orderItemsOfSubOrder 
            |> List.map (fun (x:Db.OrderItemDetails) -> 
                (
                    if (x.Quantity>1) then (sprintf Globals.ITALIAN_TEMPLATE_ROW_FOR_PAYMENT_ITEM x.Price x.Quantity x.Name ) 
                    else (sprintf Globals.ITALIAN_TEMPLATE_ROW_FOR_UNITARY_PAYMENT_ITEM x.Price  x.Name ) 
                )
            )

        let paymentItemRows = paymentItemDetailsOfSubOrder |> List.map (fun x -> sprintf Globals.ITALIAN_TEMPLATE_ROW_FOR_PAYMENT_CLOSURE_ITEM ((int)x.Tendercodeidentifier) x.Amount)
        let outFile = new System.IO.StreamWriter(Settings.EcrFilePath,false,Encoding.ASCII)
        let _ = orderItemRows |> List.iter (fun x -> outFile.WriteLine(x))
        let _ = 
            match order.Total - order.Adjustedtotal with
                | X   when X > 0M  -> 
                    outFile.WriteLine("SUBTOT")
                    outFile.WriteLine(sprintf "%s %.2f" "SCONT val=" (order.Total - order.Adjustedtotal))
                | X   when X < 0M  -> 
                    outFile.WriteLine("SUBTOT")
                    outFile.WriteLine(sprintf "%s %.2f, term=34" "inp num=" (order.Adjustedtotal - order.Total))
                | _ -> ()
        let _ = paymentItemRows |> List.iter (fun x -> outFile.WriteLine(x))
        let _ = outFile.Close()
        makeOrderArchived orderId 
        Redirection.found  Path.Orders.seeDoneOrders 
    with
    | ex ->
        log.Error("Error in createEcrReceiptInstructionForOrder", ex)
        Redirection.found Path.Errors.unableToCompleteOperation

let defaultActionableStatesForOrderOwner = warbler (fun _ ->
    log.Debug("defaultActionableStatesForOrderOwner")
    let ctx = Db.getContext()
    let states = Db.getOrderedListOfOrdinaryStates ctx
    let defaultActionableStatesForWaiter = Db.getActionableDefaultStatesForWaiter ctx
    View.defaultActionableStatesForWaiter states defaultActionableStatesForWaiter |> html)

let tempUserDefaultActionableStates = warbler (fun _ ->
    log.Debug("tempUserDefaultActionableStates")
    let ctx = Db.getContext()
    let states = Db.getOrderedListOfOrdinaryStates ctx
    let defaultActionableStatesForTempUser = Db.getDefaultActionableStatesForTempUser ctx
    View.defaultActionableStatesForTempUser states defaultActionableStatesForTempUser |> html)

let specificActionableStateForOrderOwner id = warbler (fun _ ->
    log.Debug(sprintf "specificActionableStateForOrderOwner %d" id)
    let ctx = Db.getContext()
    let user = Db.Users.getUser id ctx
    let states = Db.getOrderedListOfOrdinaryStates ctx
    let actionableStatesForSpecificWaiter = Db.getActionableStatesForSpecificWaiter id ctx
    View.specificActionableStatesForWaiter user states actionableStatesForSpecificWaiter |> html)

let manageCourses = warbler (fun _ ->
    log.Debug("manageCourses")
    let ctx = Db.getContext()
    let allCourses = Db.Courses.getAllCourseDetails ctx
    View.seeVisibleCourses "categoria" allCourses  |> html)

let manageVisibleCoursesOfACategory categoryId =  
    log.Debug(sprintf "%s %d " "manageVisibleCoursesOfACategory" categoryId)
    let ctx = Db.getContext()
    let allCourses = Db.Courses.getVisibleCoursesDetailsByCatgory categoryId ctx
    let category = Db.Courses.tryGetCategoryById categoryId ctx
    View.seeVisibleCoursesOfACategory category allCourses |> html

let manageAllCoursesOfACategory categoryId =  
    log.Debug(sprintf "%s %d " "manageAllCoursesOfACategory" categoryId)
    let ctx = Db.getContext()
    let allCourses = Db.Courses.getAllCoursesDetailsByCatgory categoryId ctx
    let category = Db.Courses.tryGetCategoryById categoryId ctx
    View.seeAllCourses category allCourses |> html

let manageAllCoursesOfACategoryPaginated categoryId pageNumber =
    log.Debug(sprintf "%s %d %d" "manageAllCoursesOfACategoryPaginated" categoryId categoryId )
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ ->
            let coursesOfPage = Db.Courses.getAllCoursesDetailsByCategoryAndPage categoryId pageNumber ctx
            let numberOfAllCourses = Db.Courses.getNumberOfAllCoursesOfACategory categoryId ctx
            let numberOfPages = (numberOfAllCourses - 1)/Globals.NUM_DB_ITEMS_IN_A_PAGE
            let subCategories = Db.getAllSubCourseCategories categoryId ctx
            let father = Db.tryGetCourseCategoryFather categoryId ctx

            let category = Db.Courses.tryGetCategoryById categoryId ctx
            View.seeAllCoursesPaginated category subCategories father coursesOfPage numberOfPages pageNumber |> html)

        POST >=> bindToForm Form.searchCourse (fun form -> 
            let coursesOfPage = Db.Courses.getAllCoursesDetailsByCategoryWithTextNameSearch categoryId form.Name ctx
            let numberOfAllCourses = Db.Courses.getNumberOfAllCoursesOfACategory categoryId ctx
            let numberOfPages = (numberOfAllCourses - 1)/Globals.NUM_DB_ITEMS_IN_A_PAGE

            let father = Db.tryGetCourseCategoryFather categoryId ctx
            let subCategories = Db.getAllSubCourseCategories categoryId ctx
            let category = Db.Courses.tryGetCategoryById categoryId ctx
            View.seeAllCoursesPaginated category subCategories father coursesOfPage numberOfPages pageNumber |> html
        )
    ]

let manageVisibleCoursesOfACategoryPaginated categoryId pageNumber =
    log.Debug(sprintf "%s %d" "manageVisibleCoursesOfACategoryPaginated" categoryId)
    let ctx = Db.getContext()
    let coursesOfPage = Db.Courses.getVisibleCoursesDetailsByCategoryAndPage categoryId pageNumber ctx
    let numberOfAllCourses = Db.Courses.getNumberOfVisibleCoursesOfACatgory categoryId ctx
    let numberOfPages = (numberOfAllCourses - 1)/Globals.NUM_DB_ITEMS_IN_A_PAGE
    let father = Db.tryGetCourseCategoryFather categoryId ctx
    let subCategories = Db.getVisibleSubCourseCategories categoryId ctx
    let category = Db.Courses.tryGetCategoryById categoryId ctx
    View.seeVisibleCoursesPaginated category subCategories father coursesOfPage numberOfPages pageNumber |> html

let manageCategories = warbler (fun _ ->
    log.Debug("manageCategories")
    let ctx = Db.getContext()
    let allCategories = Db.Courses.getAllCategories ctx
    View.seeCategories allCategories |> html)

let editCourse id  =
    log.Debug(sprintf "%s %d " "editCourse" id)
    let ctx = Db.getContext()
    let courseCategories = Db.Courses.getVisibleCourseCategories ctx |> List.map (fun g -> decimal g.Categoryid,g.Name)
    let commentsForCourse = Db.getCommentsForCourseDetails id ctx
    let allStandardComments = Db.getAllStandardComments ctx
    let visibleIngredientCategories = Db.getVisibleIngredientCategories ctx
    let ingredientsOfTheCourse = Db.getIngredientsOfACourse id ctx
    let course = Db.Courses.getCourse id ctx
    let standardVariationForCourseDetails = Db.StandardVariations.getStandardVariationsForCourseDetails id ctx
    choose [
            GET >=> warbler (fun _ ->
                html (View.editCourse course courseCategories visibleIngredientCategories 
                    ingredientsOfTheCourse commentsForCourse allStandardComments standardVariationForCourseDetails ""))
            POST >=> bindToForm Form.course (fun form ->
                match (Db.Courses.tryFindCourseByName form.Name ctx) with
                | Some X when X.Courseid <> course.Courseid -> 
                    local.ACourseNamed + form.Name + local.AlreadyExists |>
                    View.editCourse course courseCategories visibleIngredientCategories 
                        ingredientsOfTheCourse commentsForCourse allStandardComments standardVariationForCourseDetails |> html
                | _ ->
                    try
                        let desc = match form.Description with | Some d -> d | _ -> ""
                        Db.Courses.updateCourse 
                            course
                            form.Price 
                            form.Name 
                            desc   
                            (form.Visibile = Form.VISIBLE)
                            ((int)form.CategoryId) ctx
                        let retPath = sprintf Path.Courses.manageVisibleCoursesOfACategoryPaginated ((int)form.CategoryId) 0
                        Redirection.FOUND retPath
                    with
                    | ex ->
                        log.Error("Error in editCourse", ex)
                        Redirection.FOUND Path.Errors.unableToCompleteOperation
                )
    ]
    
let editCourseCategory id =
    log.Debug(sprintf "%s %d " "editCourseCategory" id)
    let ctx = Db.getContext()
    match Db.Courses.tryGetCourseCategory id ctx with
    | Some courseCategory ->
        choose [
            GET >=> warbler (fun _ ->
                html (View.editCourseCategory courseCategory ""))
            POST >=> bindToForm Form.courseCategoryEdit (fun form ->
                let categoryExists = Db.Courses.tryGetCourseCategoryByName form.Name ctx
                match categoryExists with
                | Some X when  (X.Categoryid <> id) -> 
                    local.ACategoryNamed + form.Name + local.AlreadyExists |>(View.editCourseCategory courseCategory) |> html
                | _ ->
                    try
                        let visibility =  (form.Visibility = Form.VISIBLE)
                        let isAbstract = (form.Abstract = Form.ABSTRACT)
                        Db.updateCourseCategory 
                            courseCategory
                            form.Name
                            visibility 
                            isAbstract
                            ctx
                        Redirection.FOUND Path.Courses.adminCategories
                    with
                    | ex ->
                        log.Error("Error in editCourseCategory", ex)
                        Redirection.FOUND Path.Errors.unableToCompleteOperation
                )
        ]
    | None -> 
        never

let editUser id   =
    log.Debug(sprintf "%s %d " "editUser" id)
    let ctx = Db.getContext()
    match Db.tryFindUserById id ctx with
    | Some user ->
        choose [
            GET >=> warbler (fun _ ->
                html (View.editUser user))
            POST >=> bindToForm Form.userEdit   (fun form ->
                try
                    let enabled = match form.Enabled with | Form.YES -> true | _ -> false
                    let canVoidOrders = match form.CanVoidOrder with |  Form.YES -> true | _ -> false
                    let canManageallOrders = match form.CanManageAllorders with | Form.YES -> true | _ -> false
                    let canChangeThePrices = form.CanChangeThePrices = Form.YES
                    let canManageAllCourses = form.CanManageAllCourses =  Form.YES
                    Db.updateUserStatus id enabled canVoidOrders canManageallOrders canChangeThePrices canManageAllCourses ctx
                    Redirection.FOUND Path.Admin.allUsers
                with
                | ex ->
                    log.Error("Error in editUser", ex)
                    Redirection.FOUND Path.Errors.unableToCompleteOperation
            )
        ]
    | None -> never

let editTemporaryUser id   =
    log.Debug(sprintf "%s %d \n " "editTemporaryUser" id)
    let ctx = Db.getContext()
    match Db.tryFindUserById id ctx with
    | Some user ->
        choose [
            GET >=> warbler (fun (x:HttpContext) ->
                let hostAddress = Settings.HostAddress 
                html (View.editTemporaryUser user hostAddress ))
        ]
    | None -> never

let createCategory =
    log.Debug("createCategory")
    choose [
        GET >=> (View.createCourseCategory ""  |> html)
        POST >=> bindToForm Form.courseCategoryEdit (fun form ->
            let ctx = Db.getContext()
            match Db.Courses.tryFindCategoryByName form.Name ctx with
            | Some existing ->
                local.ACategoryNamed + form.Name + local.AlreadyExists 
                |> View.createCourseCategory
                |> html
            | None ->
                try
                    let categoryName  = form.Name |> Sanitizer.GetSafeHtmlFragment
                    let visibility = (form.Visibility = Form.VISIBLE)
                    let isAbstract = (form.Abstract = Form.ABSTRACT)
                    let _  = Db.newCategory categoryName visibility isAbstract ctx
                    Redirection.FOUND Path.Courses.adminCategories
                with
                | ex ->
                    log.Error("Error in createCategory", ex)
                    Redirection.FOUND Path.Errors.unableToCompleteOperation
            )
    ]


let setEnablerStatesForUser (statesId: int list) (userId:int) =
    log.Debug(sprintf "%s %d  " "setEnablerStatesForUser" userId)
    let ctx = Db.getContext()
    Db.setEnablerStatesForUser statesId userId ctx

let changePassword  (user:UserLoggedOnSession)= 
    let ctx = Db.getContext()
    choose [
        GET >=> (View.changePassword "" user |> html)
        POST >=>  bindToForm Form.changePassword  (fun form -> 
            try
                let (Password password) = form.Password
                Db.updatePasswordOfUser user.UserId (passHash password) ctx
                Redirection.FOUND Path.home
            with
            | ex ->
                log.Error("Error in changePassword", ex)
                Redirection.FOUND Path.Errors.unableToCompleteOperation
        )
    ]

let randomAlphanumericString() =
    let chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
    let random = Random(System.DateTime.Now.Millisecond)
    seq {
        for i in {0..11} do
            yield chars.[random.Next(chars.Length)]
    } |> Seq.toArray |> (fun x -> new String(x))

let regenTempUser userId =
    log.Debug(sprintf "%s %d" "regenTempUser" userId)
    let ctx = Db.getContext()
    choose [
        GET >=> (View.recycleQrUser |> html) 
        POST >=> bindToForm Form.qrUser (fun form ->
            try
                let dbUser = Db.Users.getUser userId ctx
                let newUserName = randomAlphanumericString()
                dbUser.Username <- newUserName
                dbUser.Creationtime <- System.DateTime.Now
                dbUser.Table <- form.TableName
                let existingOrders = Db.getAllOrdersOfUser userId ctx
                existingOrders |> Seq.iter (fun (x:Db.Order) -> x.Forqruserarchived <- true) 
                let order = Db.createOrderByUser form.TableName "" userId ctx
                ctx.SubmitUpdates()
                Redirection.FOUND Path.Admin.temporaryUsers
            with
            | ex ->
                log.Error("Error in regenTempUser", ex)
                Redirection.FOUND Path.Errors.unableToCompleteOperation
        )
    ]

let addQrUser = 
    log.Debug("addQrUser")
    let ctx = Db.getContext()
    choose [
        GET >=> (View.addQrUser |> html)
        POST >=> bindToForm Form.qrUser (fun form ->
            try
                let temporaryRole = Db.getRoleByName "temporary" ctx
                let userName  =  randomAlphanumericString()
                let canManageAllOrders = false
                let password = ""
                let canChangeThePrices = false
                let canManageCourses = false
                let defaultTempUserEnablingStates = Db.getDefaultActionableStatesForTempUser ctx
                let user = Db.newUser canChangeThePrices canManageAllOrders userName  password temporaryRole.Roleid canManageCourses ctx
                user.Creationtime <- System.DateTime.Now
                user.Istemporary<- true
                user.Table <- form.TableName
                ctx.SubmitUpdates()
                let order = Db.createOrderByUser form.TableName "" user.Userid ctx
                let actionablesStatesIds = List.map (fun (x:Db.TempUserDefaultActionableStates) -> x.Stateid) defaultTempUserEnablingStates
                Db.setEnablerStatesForUser actionablesStatesIds user.Userid ctx
                Redirection.FOUND Path.Admin.temporaryUsers
            with
            | ex ->
                log.Error("Error in addQrUser", ex)
                Redirection.FOUND Path.Errors.unableToCompleteOperation
        )

    ]

let addUser = 
    log.Debug("addUser")
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ ->  (View.register ((Db.getRoles ctx)|> List.filter (fun (x:Db.Role) -> x.Rolename <> "admin")  )"" |> html))
        POST >=> bindToForm Form.register (fun form ->
            let roles = Db.getRoles ctx
            let ctx = Db.getContext()
            match Db.tryFindUsersViewByName form.Username ctx with
            | Some existing ->
                local.UserNameIsTaken 
                |> View.register roles
                |> html
            | None ->
                try
                    let (Password password) = form.Password
                    let role = form.Role
                    let dbRole = Db.getRole ((int)role) ctx
                    if (dbRole.Rolename = "admin") then failwith local.CantAddAnotherAdmin
                    let defaultEnablingStates = Db.getActionableDefaultStatesForWaiter ctx
                    let canmanageOrders = form.CanManageAllorders = Form.YES 
                    let canChangeThePrices = form.CanChangeThePrices =  Form.YES
                    let canManageAllCourses = form.CanManageAllCourses = Form.YES
                    let user = Db.newUser canChangeThePrices canmanageOrders form.Username  (passHash password) ((int) role) canManageAllCourses ctx
                    let actionablesStatesIds = List.map (fun (x:Db.DefaultActionableState) -> x.Stateid) defaultEnablingStates
                    Db.setEnablerStatesForUser actionablesStatesIds user.Userid ctx
                    Redirection.FOUND Path.Admin.allUsers
                with
                | ex ->
                    log.Error("Error in addUser", ex)
                    Redirection.FOUND Path.Errors.unableToCompleteOperation
            )
    ]

let getViableGroupOutIdentifiers orderId =
    log.Debug(sprintf "getViableGroupOutIdentifiers %d" orderId)
    let ctx = Db.getContext()
    let outGroupsOfOrder = Db.getOutGroupsOfOrder orderId ctx
    let alreadyPrintedGroupsOfOrder = 
        outGroupsOfOrder |> List.filter (fun (x:Db.OrderOutGroup) -> x.Printcount > 0) |> 
        List.map  (fun (x:Db.OrderOutGroup) -> x.Groupidentifier)
    let availableNumbers = [1..13] |> List.filter (fun i -> not (List.contains i alreadyPrintedGroupsOfOrder))
    availableNumbers |> List.map (fun x -> ((decimal) x,(string)x))

let addOrderItemByAllCategoriesForOrdinaryUsers orderId backUrl (user:UserLoggedOnSession) =
    log.Debug(sprintf "addOrderItemByAllCategoriesForOrdinaryUsers orderId:%d " orderId)
    let ctx = Db.getContext()
    let anOrder = Db.Orders.tryGetOrder orderId ctx
    let viableGroupOutIdsForOrderItem = getViableGroupOutIdentifiers orderId
    match anOrder with
    | Some theOrder  ->
        choose [
            GET >=> warbler (fun x ->
                let visibleCourses  =
                    Db.Courses.getVisibleCourses ctx
                let coursesIdWithPrices =
                    visibleCourses |> List.map (fun g -> decimal g.Courseid, g.Price |> string)
                let coursesNames = 
                    visibleCourses  |> List.map (fun g -> decimal g.Courseid, g.Name)
                html (View.addOrderItemByAllCategories  orderId coursesNames  coursesIdWithPrices backUrl viableGroupOutIdsForOrderItem)
            )
            POST >=> bindToForm Form.orderItem (fun form ->
                try
                    let sanitizedComment = if (form.Comment.IsSome) then (form.Comment.Value |> Sanitizer.GetSafeHtmlFragment |> Some) else None
                    // let orderItem = Db.createOrderItemByCourseId ((int)(form.CourseId)) orderId ((int)(form.Quantity))  form.Comment form.Price form.GroupOut  ctx
                    let orderItem = Db.createOrderItemByCourseId ((int)(form.CourseId)) orderId ((int)(form.Quantity)) sanitizedComment  form.Price form.GroupOut  ctx
                    makeOrderItemRejectedIfContainsInvisibleIngredients orderItem.Orderitemid ctx
                    makeOrderItemAsRejectedIfContainsUnavalableIngredients orderItem.Orderitemid ctx
                    Redirection.FOUND  (sprintf Path.Orders.selectStandardCommentsAndVariationsForOrderItem orderItem.Orderitemid) 
                with
                | ex ->
                    log.Error("Error in addOrderItemByAllCategoriesForOrdinaryUsers", ex)
                    Redirection.FOUND Path.Errors.unableToCompleteOperation
            )
        ]
    | _ -> Redirection.FOUND (Path.Orders.myOrders+"#order"+(orderId|> string))

let addOrderItemByAllCategoriesForStrippedUsers orderId backUrl (user:UserLoggedOnSession) =
    log.Debug(sprintf "addOrderItemByAllCategoriesForOrdinaryUsers orderId:%d " orderId)
    let ctx = Db.getContext()
    let anOrder = Db.Orders.tryGetOrder orderId ctx
    let viableGroupOutIdsForOrderItem = getViableGroupOutIdentifiers orderId
    match anOrder with
    | Some theOrder  ->
        choose [
            GET >=> warbler (fun x ->
                let visibleCourses  =
                    Db.Courses.getVisibleCourses ctx
                let coursesIdWithPrices =
                    visibleCourses |> List.map (fun g -> decimal g.Courseid, g.Price |> string)
                let coursesNames = 
                    visibleCourses  |> List.map (fun g -> decimal g.Courseid, g.Name)
                html (View.addOrderItemByAllCategoriesForStrippedUsers  orderId coursesNames  coursesIdWithPrices backUrl viableGroupOutIdsForOrderItem)
            )
            POST >=> bindToForm Form.orderItem (fun form ->
                try
                    let sanitizedComment = if (form.Comment.IsSome) then (form.Comment.Value |> Sanitizer.GetSafeHtmlFragment |> Some) else None
                    let orderItem = Db.createOrderItemByCourseId ((int)(form.CourseId)) orderId ((int)(form.Quantity)) sanitizedComment form.Price form.GroupOut  ctx
                    makeOrderItemRejectedIfContainsInvisibleIngredients orderItem.Orderitemid ctx
                    makeOrderItemAsRejectedIfContainsUnavalableIngredients orderItem.Orderitemid ctx
                    Redirection.FOUND  (sprintf Path.Orders.selectStandardCommentsAndVariationsForOrderItem orderItem.Orderitemid) 
                with
                | ex ->
                    log.Error("Error in addOrderItemByAllCategoriesForStrippedUsers", ex)
                    Redirection.FOUND Path.Errors.unableToCompleteOperation
            )
        ]
    | _ -> Redirection.FOUND (Path.Orders.myOrders+"#order"+(orderId|> string))

let addOrderItemByCategoryForOrdinaryUsers orderId categoryId backUrl (user:UserLoggedOnSession) =
    log.Debug(sprintf "addOrderItemByCategoryForOrdinaryUsers orderId:%d categoryId:%d" orderId categoryId)
    let ctx = Db.getContext()
    let anOrder = Db.Orders.tryGetOrder orderId ctx
    let viableGroupOutIdsForOrderItem = getViableGroupOutIdentifiers orderId
    let subCategories = Db.Courses.getVisibleFatherSonCategoriesDetailsByFatherId categoryId ctx
    let fatherCategory = Db.Courses.getFatherSonCategoriesDetailsBySonId categoryId ctx
    let category = Db.Courses.getCourseCategory categoryId ctx
    match anOrder with
    | Some theOrder  ->
        choose [
            GET >=> warbler (fun x ->
                let visibleCourses  =
                    Db.Courses.getVisibleCoursesByCategoryAndSubCategories categoryId ctx
                let coursesIdWithPrices =
                    visibleCourses |> List.map (fun g -> decimal g.Courseid, g.Price |> string)
                let coursesNames = 
                    visibleCourses  |> List.map (fun g -> decimal g.Courseid, g.Name)
                html (View.addOrderItem  orderId coursesNames  coursesIdWithPrices subCategories fatherCategory category.Name backUrl viableGroupOutIdsForOrderItem)
            )
            POST >=> bindToForm Form.orderItem (fun form ->
                try
                    let sanitizedComment = if (form.Comment.IsSome) then (form.Comment.Value |> Sanitizer.GetSafeHtmlFragment |> Some) else None
                    // let orderItem = Db.createOrderItemByCourseId ((int)(form.CourseId)) orderId ((int)(form.Quantity))  form.Comment form.Price form.GroupOut  ctx
                    let orderItem = Db.createOrderItemByCourseId ((int)(form.CourseId)) orderId ((int)(form.Quantity)) sanitizedComment form.Price form.GroupOut  ctx
                    makeOrderItemRejectedIfContainsInvisibleIngredients orderItem.Orderitemid ctx
                    makeOrderItemAsRejectedIfContainsUnavalableIngredients orderItem.Orderitemid ctx
                    Redirection.FOUND  (sprintf Path.Orders.selectStandardCommentsAndVariationsForOrderItem orderItem.Orderitemid) 
                with
                | ex ->
                    log.Error("Error in addOrderItemByCategoryForOrdinaryUsers", ex)
                    Redirection.FOUND Path.Errors.unableToCompleteOperation
            )
        ]
    | _ -> Redirection.FOUND (Path.Orders.myOrders+"#order"+(orderId|> string))

let addOrderItemForOrdinaryUsers orderId backUrl (user:UserLoggedOnSession) =
    log.Debug(sprintf "addOrderItemByCategoryForOrdinaryUsers orderId:%d"  orderId )
    let ctx = Db.getContext()
    let anOrder = Db.Orders.tryGetOrder orderId ctx
    let viableGroupOutIdsForOrderItem = getViableGroupOutIdentifiers orderId
    match anOrder with
    | Some theOrder  ->
        choose [
            GET >=> warbler (fun x ->
                let visibleCourses  =
                    Db.Courses.getAllVisibleCourses  ctx
                let coursesIdWithPrices =
                    visibleCourses |> List.map (fun g -> decimal g.Courseid, g.Price |> string)
                let coursesNames = 
                    visibleCourses  |> List.map (fun g -> decimal g.Courseid, g.Name)
                // html (View.addOrderItem'  orderId cocoursesNames  coursesIdWithPrices subCategories fatherCategory category.Name backUrl viableGroupOutIdsForOrderItem)
                html (View.addOrderItem'  orderId coursesNames  coursesIdWithPrices  backUrl viableGroupOutIdsForOrderItem)
            )
            POST >=> bindToForm Form.orderItem (fun form ->
                try
                    let sanitizedComment = if (form.Comment.IsSome) then (form.Comment.Value |> Sanitizer.GetSafeHtmlFragment |> Some) else None
                    let orderItem = Db.createOrderItemByCourseId ((int)(form.CourseId)) orderId ((int)(form.Quantity)) sanitizedComment form.Price form.GroupOut  ctx
                    makeOrderItemRejectedIfContainsInvisibleIngredients orderItem.Orderitemid ctx
                    makeOrderItemAsRejectedIfContainsUnavalableIngredients orderItem.Orderitemid ctx
                    Redirection.FOUND  (sprintf Path.Orders.selectStandardCommentsAndVariationsForOrderItem orderItem.Orderitemid) 
                with
                | ex ->
                    log.Error("Error in addOrderItemForOrdinaryUsers", ex)
                    Redirection.FOUND Path.Errors.unableToCompleteOperation
            )
        ]
    | _ -> Redirection.FOUND (Path.Orders.myOrders+"#order"+(orderId|> string))
    
let addOrderItemByCategoryForStrippedUsers orderId categoryId backUrl (user:UserLoggedOnSession)=
    log.Debug(sprintf "addOrderItemByCategoryForStrippedUsers orderId:%d categoryId%d")
    let ctx = Db.getContext()
    let anOrder = Db.Orders.tryGetOrder orderId ctx
    let viableGroupOutIdsForOrderItem = getViableGroupOutIdentifiers orderId
    let subCategories = Db.Courses.getFatherSonCategoriesDetailsByFatherId categoryId ctx
    let fatherCategory = Db.Courses.getFatherSonCategoriesDetailsBySonId categoryId ctx
    let category = Db.Courses.getCourseCategory categoryId ctx
    match anOrder with
    | Some theOrder  ->
        choose [
            GET >=> warbler (fun x ->
                let visibleCourses  =
                    Db.Courses.getVisibleCoursesByCategoryAndSubCategories categoryId ctx
                let coursesIdWithPrices =
                    visibleCourses |> List.map (fun g -> decimal g.Courseid, g.Price |> string)
                let coursesNames = 
                    visibleCourses  |> List.map (fun g -> decimal g.Courseid, g.Name)
                html (View.addOrderItemForStrippedUsers orderId  coursesNames coursesIdWithPrices subCategories fatherCategory category.Name backUrl viableGroupOutIdsForOrderItem)
            )
            POST >=> bindToForm Form.strippedOrderItem (fun form ->

                try
                    let sanitizedComment = if (form.Comment.IsSome) then (form.Comment.Value |> Sanitizer.GetSafeHtmlFragment |> Some) else None
                    let course = Db.getCourseDetail ((int)form.CourseId) ctx
                    let orderItem = Db.createOrderItemByCourseId ((int)form.CourseId) orderId ((int)(form.Quantity)) sanitizedComment course.Price form.GroupOut  ctx
                    makeOrderItemRejectedIfContainsInvisibleIngredients orderItem.Orderitemid ctx
                    makeOrderItemAsRejectedIfContainsUnavalableIngredients orderItem.Orderitemid ctx
                    Redirection.FOUND  (sprintf Path.Orders.selectStandardCommentsAndVariationsForOrderItem orderItem.Orderitemid)
                with
                | ex ->
                    log.Error("Error in addOrderItemByCategoryForStrippedUsers", ex)
                    Redirection.FOUND Path.Errors.unableToCompleteOperation
            )
        ]
    | _ -> Redirection.FOUND (Path.Orders.myOrders+"#order"+(orderId|> string))

let addOrderItemForStrippedUsers orderId categoryId backUrl (user:UserLoggedOnSession)=
    log.Debug(sprintf "addOrderItemByCategoryForStrippedUsers orderId:%d categoryId%d")
    let ctx = Db.getContext()
    let anOrder = Db.Orders.tryGetOrder orderId ctx
    let viableGroupOutIdsForOrderItem = getViableGroupOutIdentifiers orderId
    let subCategories = Db.Courses.getFatherSonCategoriesDetailsByFatherId categoryId ctx
    let fatherCategory = Db.Courses.getFatherSonCategoriesDetailsBySonId categoryId ctx
    let category = Db.Courses.getCourseCategory categoryId ctx
    match anOrder with
    | Some theOrder  ->
        choose [
            GET >=> warbler (fun x ->
                let visibleCourses  =
                    Db.Courses.getVisibleCoursesByCategoryAndSubCategories categoryId ctx
                let coursesIdWithPrices =
                    visibleCourses |> List.map (fun g -> decimal g.Courseid, g.Price |> string)
                let coursesNames = 
                    visibleCourses  |> List.map (fun g -> decimal g.Courseid, g.Name)
                html (View.addOrderItemForStrippedUsers orderId  coursesNames coursesIdWithPrices subCategories fatherCategory category.Name backUrl viableGroupOutIdsForOrderItem)
            )
            POST >=> bindToForm Form.strippedOrderItem (fun form ->
                try
                    let sanitizedComment = if (form.Comment.IsSome) then (form.Comment.Value |> Sanitizer.GetSafeHtmlFragment |> Some) else None
                    let course = Db.getCourseDetail ((int)form.CourseId) ctx
                    let orderItem = Db.createOrderItemByCourseId ((int)form.CourseId) orderId ((int)(form.Quantity)) sanitizedComment course.Price form.GroupOut  ctx
                    makeOrderItemRejectedIfContainsInvisibleIngredients orderItem.Orderitemid ctx
                    makeOrderItemAsRejectedIfContainsUnavalableIngredients orderItem.Orderitemid ctx
                    Redirection.FOUND  (sprintf Path.Orders.selectStandardCommentsAndVariationsForOrderItem orderItem.Orderitemid)
                with
                | ex ->
                    log.Error("Error in addOrderItemByCategoryForStrippedUsers", ex)
                    Redirection.FOUND Path.Errors.unableToCompleteOperation
            )
        ]
    | _ -> Redirection.FOUND (Path.Orders.myOrders+"#order"+(orderId|> string))

let addOrderItemByCategoryPassingUserLoggedOn orderId categoryId urlEncodedBackUrl (user:UserLoggedOnSession) = 
    log.Debug(sprintf "addOrderItemByCategoryPassingUserLoggedOn orderId:%d categoryId:%d" orderId categoryId)
    warbler (fun (x:HttpContext) ->
        let ctx = Db.getContext()
        let dbUser = Db.getUserById user.UserId ctx
        match dbUser.Canchangetheprice with
        | true -> addOrderItemByCategoryForOrdinaryUsers orderId categoryId urlEncodedBackUrl (user:UserLoggedOnSession)
        | false -> addOrderItemByCategoryForStrippedUsers orderId categoryId urlEncodedBackUrl  (user:UserLoggedOnSession)
    )

let addOrderItemPassingUserLoggedOn orderId urlEncodedBackUrl (user:UserLoggedOnSession) = 
    log.Debug(sprintf "addOrderItemPassingUserLoggedOn orderId:%d " orderId )
    warbler (fun (x:HttpContext) ->
        let ctx = Db.getContext()
        let dbUser = Db.getUserById user.UserId ctx
        match dbUser.Canchangetheprice with
        // not exactly clear what to do here
        | _ -> addOrderItemForOrdinaryUsers orderId urlEncodedBackUrl (user:UserLoggedOnSession)
    )

let deleteOrderItem orderItemId =
    try
        let ctx = Db.getContext()
        Db.deleteOrderItem orderItemId ctx
        Redirection.FOUND Path.Orders.myOrders
    with
    | ex ->
        log.Error("Error in deleteOrderItem", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

let deleteOrderItemByUser orderItemId encodedBackUrl (user:UserLoggedOnSession) =
    log.Debug(sprintf "deleteOrderItemByUser orderIteId: %d" orderItemId)
    warbler (fun x ->
        try
            let backUrl = WebUtility.UrlDecode encodedBackUrl
            let ctx = Db.getContext()
            let theOrderItemDetail = Db.Orders.getOrderItemDetail orderItemId ctx
            let state = Db.States.getState(theOrderItemDetail.Stateid) ctx
            if ((theOrderItemDetail.Userid = user.UserId || user.Role = "admin") && (state.Isinitial))
                then Db.deleteOrderItem orderItemId ctx 
            Redirection.FOUND backUrl
        with
        | ex ->
            log.Error("Error in deleteOrderItemByUser", ex)
            Redirection.FOUND Path.Errors.unableToCompleteOperation
    )

let setStateAsActionableForTempUserByDefault stateId =
    try
        log.Debug(sprintf "setStateAsActionableForTempUserByDefault stateId:%d" stateId)
        let ctx = Db.getContext()
        let _ = Db.createTempUserActionableState stateId ctx
        Redirection.FOUND Path.Admin.tempUserDefaultActionableStates
    with
    | ex ->
        log.Error("Error in setStateAsActionableForTempUserByDefault", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

let setStateAsActiobableByDefault stateid =
    try
        log.Debug(sprintf "%s %d " "setStateAsActiobableByDefault" stateid)
        let ctx = Db.getContext()
        let _ = Db.createWaiterActionableState stateid ctx
        Redirection.FOUND Path.Admin.defaultActionableStatesForOrderOwner
    with
    | ex ->
        log.Error("Error in setStateAsActiobableByDefault", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

let unSetStateAsActiobableByDefault stateid =
    log.Debug(sprintf "unsetStateAsAciobableByDefault %d" stateid)
    try
        let ctx = Db.getContext()
        let _ = Db.deleteWaiterActionableState stateid ctx
        Redirection.FOUND Path.Admin.defaultActionableStatesForOrderOwner
    with
    | ex ->
        log.Error("Error in unSetStateAsActiobableByDefault", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

let unSetStateAsActionableForTempUserByDefault stateid =
    try
        log.Debug(sprintf "unsetStateAsActionableForTempUserByDefault %d" stateid)
        let ctx = Db.getContext()
        let _ = Db.deleteDefaultTempUserActionableState stateid ctx
        Redirection.FOUND Path.Admin.tempUserDefaultActionableStates
    with
    | ex ->
        log.Error("Error in unSetStateAsActionableForTempUserByDefault", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

let setStateAsActionableForWaiter userId stateId =
    log.Debug(sprintf "unsetStateAsActionableForWaiter userId:%d stateId%d" userId stateId)
    try
        let ctx = Db.getContext()
        let _ = Db.createSpecificWaiterAcionableState userId stateId ctx
        let redirection = sprintf Path.Admin.actionableStatesForSpecificOrderOwner userId
        Redirection.FOUND redirection
    with
    | ex ->
        log.Error("Error in setStateAsActionableForWaiter", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

let unSetStateAsAcionableForWaiter userId stateId =
    log.Debug(sprintf "unsetStateAsAcionableForWaiter userId:%d stateId%d" userId stateId)
    try
        let ctx = Db.getContext()
        let _ = Db.deleteSpecificWaiterAcionableState userId stateId ctx
        let redirection = sprintf Path.Admin.actionableStatesForSpecificOrderOwner userId
        Redirection.FOUND redirection
    with
    | ex ->
        log.Error("Error in unSetStateAsAcionableForWaiter", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

let deleteEnablerMapping id =
    log.Debug(sprintf "deleteEnablerMapping %d" id)
    try
        let ctx = Db.getContext()
        Db.deleteEnabler id ctx
        Redirection.FOUND Path.Admin.roles
    with    
    | ex ->
        log.Error("Error in deleteEnablerMapping", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

let deleteObserverMapping id =
    log.Debug(sprintf "deleteObserverMapping %d" id)
    try
        let ctx = Db.getContext()
        Db.deleteObserver id ctx
        Redirection.FOUND Path.Admin.roles
    with
    | ex ->
        log.Error("Error in deleteObserverMapping", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

let canMoveOrderItemToNextStep (orderItemDetail:Db.OrderItemDetails) (user:UserLoggedOnSession) =
    log.Debug(sprintf "canMoveOrderItemToNextStep %d" orderItemDetail.Orderitemid)
    let ctx = Db.getContext()
    let stateEnableForUserAsWaiter = Db.listOfEnabledStatesForWaiter user.UserId ctx
    ((   Db.userEnabledByRoletoChangeStateOfThisOrderItem orderItemDetail user.UserId ctx ) ||
        (((List.contains orderItemDetail.Stateid stateEnableForUserAsWaiter) &&
            (not orderItemDetail.Hasbeenrejected) )))

let moveOrderItemNextState (orderItemDetail:Db.OrderItemDetails) (user:UserLoggedOnSession)  =
    try
        let ctx = Db.getContext()
        let stateEnableForUserAsWaiter = Db.listOfEnabledStatesForWaiter user.UserId ctx
        match (Db.userEnabledByRoletoChangeStateOfThisOrderItem orderItemDetail user.RoleId ctx || 
                (orderItemDetail.Userid = user.UserId && (List.contains orderItemDetail.Stateid stateEnableForUserAsWaiter 
                && (not orderItemDetail.Hasbeenrejected)
                ))) with
        | true -> 
            Db.tryMoveOrderItemToNextState orderItemDetail.Orderitemid user.UserId ctx; 
        | false -> () 
    with
    | ex ->
        log.Error("Error in moveOrderItemNextState", ex)

let moveAllOrderItemsInBlockFromInitialState orderId (user:UserLoggedOnSession) =
    try
        log.Debug(sprintf "moveAllOrderItemsInBlockFromInitialState %d" orderId)
        let ctx = Db.getContext()
        let orderItemsDetails = Db.getOrderItemDetailOfOrderById orderId ctx
        let orderItemsToMove = 
            orderItemsDetails |> 
            List.filter (fun (x:Db.OrderItemDetails) -> (Db.isInitialState x.Stateid ctx))
        let _ = orderItemsToMove |> Seq.iter (fun (x:Db.OrderItemDetails) -> 
            match (canMoveOrderItemToNextStep x user) with
            | true -> Db.tryMoveOrderItemToNextState x.Orderitemid user.UserId ctx
            | false -> ())
        Redirection.FOUND (Path.Orders.myOrders+"#order"+(orderId|> string))
    with
    | ex ->
        log.Error("Error in moveAllOrderItemsInBlockFromInitialState", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

let removeSpooledFiles() =
    log.Debug("removeSpooledFiles()")
    let outFiles = Directory.GetFiles(".") |> Array.toList |> List.filter (fun (x:string ) -> x.EndsWith(".ok"))
    let txtFilesToRemove = outFiles |> List.map (fun x -> x.Replace(".ok",".txt"))
    let pdfFilesToRemove = outFiles |> List.map (fun x -> x.Replace(".ok",".pdf"))
    let _ = outFiles |> List.iter (fun x -> File.Delete(x))
    let _ = txtFilesToRemove |> List.iter (fun x -> File.Delete(x))
    let _ = pdfFilesToRemove |> List.iter (fun x -> File.Delete(x))
    ()


let makeDocumentOfOrderItemList 
    (header: string)
    (plainIngredientsMap: Map<int, string>)  
    (variationsIngredientsMap: Map<int, string>) 
    (orderItems:Db.OrderItemDetails list) =

    Document
        .Create(fun container ->
            container.Page (fun page ->
                page.Size(PageSizes.A4)
                page.Margin(2f, Unit.Centimetre)
                page.PageColor(Colors.White)
                page.DefaultTextStyle(fun x -> x.FontSize(20f))

                page
                    .Header()
                    .AlignCenter()
                    .Text("ordinazione")
                    .SemiBold()
                    .FontSize(36f)
                    .FontColor(Colors.Blue.Medium)
                |> ignore

                page
                    .Content()
                    .Text(fun text ->
                        text.EmptyLine() |> ignore
                        text.Line(header) |> ignore
                        List.iter (fun (x:Db.OrderItemDetails) -> 
                            text.Line(sprintf "n. %d %s \n" x.Quantity x.Name) |> ignore
                            text.Line( x.Comment |> stripComma) |> ignore
                            text.Line("ingr ricetta: " + (if (plainIngredientsMap.[x.Orderitemid]) = "" then "mancante" else  plainIngredientsMap.[x.Orderitemid])) |> ignore
                            if (variationsIngredientsMap.ContainsKey(x.Orderitemid) && (variationsIngredientsMap[x.Orderitemid]).Trim() <> String.Empty) then
                                printf "is not empty"
                                text.Line("var: " + variationsIngredientsMap.[x.Orderitemid]) |> ignore
                            text.EmptyLine() |> ignore
                            ) orderItems
                    )
            )
            |> ignore)

// todo: remove any reference to .txt generation files, bread down  and give some meaningful names to those functions
let makeFileOutForAGroupOfordersForDifferentPrinters (orderOutGroupDetail:Db.OrderOutGroupDetail) (plainIngredientsMap:Map<int,string>) (variationsIngredientsMap:Map<int,string>)  =
    log.Debug(sprintf "makeFileOutForAGroupOfordersForDifferentPrinters %d" orderOutGroupDetail.Ordergroupid)
    try
        let ctx = Db.getContext()
        let _ = removeSpooledFiles()
        let printCount = orderOutGroupDetail.Printcount
        let header = 
            local.Table  + 
            orderOutGroupDetail.Table + "\n" +
            local.ProgressivePrintCounterForThisReceipt  +  ": " +
            (printCount |> string) + "\n"+ 
            local.ExitGroup + 
            (orderOutGroupDetail.Groupidentifier |> string) +  ". " +
            local.TimeStamp + ": " +
            DateTime.Now.ToLocalTime().ToString() + ".\n"
        let orderItems = Db.getOrderItemsDetailOfOrderByOutGroup orderOutGroupDetail.Ordergroupid ctx
        let toBeWorkedState = Db.States.getStateByName("TOBEWORKED")
        let printers = Db.getPrinters ctx
        let printersForCategories = 
            printers |> 
            List.map (fun (x:Db.Printer) -> (x,x.``public.printerforcategory by printerid`` |> 
            Seq.map (fun (y:Db.PrinterForCourseCategory) -> y.Categoryid) |> Seq.toList))
        let printersForOrderItems = 
            printersForCategories |> 
            List.map (fun (x,(y: int list)) -> (x, orderItems |> 
                List.filter (fun (z:Db.OrderItemDetails) -> List.contains z.Categoryid  y))) 
        let filteredPrintersForOrderItems = printersForOrderItems |> List.filter (fun (_,y) -> ((List.length y) > 0))

        let printerfForOrderItemsPdfTexts =
            filteredPrintersForOrderItems
            |> List.map (fun (x, (y:Db.OrderItemDetails list)) -> 
                (x, y |> makeDocumentOfOrderItemList header plainIngredientsMap variationsIngredientsMap))

        let pdfFilesAndContents = printerfForOrderItemsPdfTexts |> List.map (fun (x: Db.Printer, y) -> (x.Name, y))

        let pdfFileNamesAndContents = 
            pdfFilesAndContents 
            |> List.map (fun (x, y) -> (sprintf "%s%d.pdf" x System.DateTime.Now.Ticks, y))

        let filteredPdfFileNamesAndContents = pdfFileNamesAndContents

        let _ = filteredPdfFileNamesAndContents |> List.iter (fun (x, y) ->
            y.GeneratePdf(x))
        ()
    with
    | ex ->
        log.Error("Error in makeFileOutForAGroupOfordersForDifferentPrinters", ex)

let fileDumpOrderOutGroupForDifferentPrinters (orderOutGroup:Db.OrderOutGroupDetail) =
    log.Debug(sprintf "fileDumpOrderOutGroupForDifferentPrinters %d orderOutGroup.Ordergroupid")
    let ctx = Db.getContext()
    let orderItemMyRoleCanObserve = Db.getOrderDetailsOfGroupDetail orderOutGroup ctx
    let listOfVariations = 
        orderItemMyRoleCanObserve |> 
        List.map (fun (x:Db.OrderItemDetails) -> (x.Orderitemid,(Db.getVariationDetailsOfOrderItem x.Orderitemid ctx)))  
    let variationsByStringDescription = Utils.variationsByStringDescription listOfVariations ctx
    let mapOfVariationsWithIngredientsIds = 
        listOfVariations |> 
        List.map (fun (id, variations:(Db.VariationDetail list)) -> 
            (id,variations |> List.map (fun (v:Db.VariationDetail) -> v.Ingredientid))) |> Map.ofList
    let orderItemIngredientsMap =  
        orderItemMyRoleCanObserve |> 
        List.map ( fun (x:Db.OrderItemDetails) ->  
            (x.Orderitemid, (Db.getIngredientsOfACourse x.Courseid ctx) |> List.filter (fun (y:Db.IngredientOfCourse) -> 
            not (List.contains y.Ingredientid mapOfVariationsWithIngredientsIds.[x.Orderitemid] ))))
    let orderItemIngredientsMapSequence = 
        orderItemIngredientsMap |>
        List.map (fun (orderItemId,(ingredients:Db.IngredientOfCourse list)) -> 
            (orderItemId, List.fold (fun y (x:Db.IngredientOfCourse)  ->   
                (if (x.Unitmeasure = UNITARY_MEASURE && (x.Quantity > (decimal)1)) then ((x.Quantity |> int |> string)+" ") else "")+ 
                x.Ingredientname+", " + y  ) "" ingredients)) |>
        Map.ofList
    makeFileOutForAGroupOfordersForDifferentPrinters orderOutGroup orderItemIngredientsMapSequence  variationsByStringDescription

let printOrderOutGroup orderOutGroupId  (order:Db.Order) (orderItemsDetails:Db.OrderItemDetails list) =
    log.Debug(sprintf "printOrderOutGroup %d" order.Orderid)
    try
        let ctx = Db.getContext()
        let orderOutGroup = Db.getOutGroup orderOutGroupId ctx
        orderOutGroup.Printcount <- orderOutGroup.Printcount + 1
        ctx.SubmitUpdates()
        let orderOutGroupDetail = Db.getOutGroupDetail orderOutGroup.Ordergroupid  ctx
        fileDumpOrderOutGroupForDifferentPrinters orderOutGroupDetail
    with
    | ex ->
        log.Error("Error in printOrderOutGroup", ex)

let rePrintOrderOutGroup orderId orderOutGroupId encodedBackUrl (user:UserLoggedOnSession) =
    log.Debug(sprintf "rePrintOrderOutGrop %d" orderOutGroupId)
    let ctx = Db.getContext()
    let decodedBackUrl = WebUtility.UrlDecode encodedBackUrl
    let order = Db.Orders.getOrder orderId ctx
    let orderItemsDetails = Db.getOrderItemsDetailOfOrderByOutGroup orderOutGroupId ctx
    let _ = 
        match (Settings.Print,user.UserId = order.Userid || user.Role = "admin") with
        | (true,true) -> printOrderOutGroup orderOutGroupId order orderItemsDetails
        | _ -> ()
    Redirection.FOUND decodedBackUrl

let twoVariationsAreIdentical (variation1:Db.Variation) (variation2:Db.Variation) =
    log.Debug("twoVariationsAreIdentical")
    variation1.Ingredientid = variation2.Ingredientid &&
    variation1.Tipovariazione = variation2.Tipovariazione &&
    variation1.Orderitemid = variation2.Orderitemid 

let twoListsOfVariationsAreIdentical (variations1:Db.Variation list) (variations2:Db.Variation list) =
    log.Debug("twoListsOfVariationsAreIdentical")
    let orderedVariations1 = variations1 |> List.sortBy (fun (x:Db.Variation) -> x.Ingredientid)
    let orderedVariations2 = variations2 |> List.sortBy (fun (x:Db.Variation) -> x.Ingredientid)
    (List.length orderedVariations1) = (List.length orderedVariations2)  &&
    (List.zip orderedVariations1 orderedVariations2) |> List.map (fun ((x:Db.Variation),(y:Db.Variation)) ->  twoVariationsAreIdentical x y) |> List.fold (&&) true

let twoOrderItemsAreIdentical (orderItem1: Db.OrderItem) (orderItem2: Db.OrderItem) =
    log.Debug("twoOrderItemsAreIdentical")
    let ctx = Db.getContext()
    let initialStateId = (Db.States.getInitState ctx).Stateid
    orderItem1.Stateid = initialStateId && orderItem2.Stateid = initialStateId &&
    not orderItem1.Hasbeenrejected && not orderItem2.Hasbeenrejected  &&
    orderItem1.Orderid = orderItem2.Orderid &&
    orderItem1.Price = orderItem2.Price &&
    orderItem1.Courseid = orderItem2.Courseid &&
    orderItem1.Ordergroupid = orderItem2.Ordergroupid &&
    orderItem1.Comment = orderItem2.Comment &&
    orderItem1.Archived = orderItem2.Archived 

let rec detectIdenticalOrderItemsAndMakeChunks (orderItems:Db.OrderItem list) =
    log.Debug("detectIdenticalOrderItemsAndMakeChunks")
    match orderItems with
    | [] -> []
    | H::T -> [H::(List.filter (fun (x: Db.OrderItem) -> twoOrderItemsAreIdentical x  H) T )] @ detectIdenticalOrderItemsAndMakeChunks ((List.filter (fun (x:Db.OrderItem) -> not (twoOrderItemsAreIdentical x  H)) T))

let moveInitialStateOrderItemsByOrderOutGroup orderId orderOutGroupId  encodedBackUrl (user:UserLoggedOnSession)  =
    log.Debug(sprintf "%s orderId %d orderOutGroupId %d" "moveInitialStateOrderItemsByOrderOutGroup" orderId orderOutGroupId)
    try
        let ctx = Db.getContext()
        let orderItems = Db.getOrderItemsOfOrderByOutGroup orderOutGroupId ctx
        let chunkedOrderItems = detectIdenticalOrderItemsAndMakeChunks orderItems
        let byChunksOptimizableOrderItems = chunkedOrderItems |> List.filter (fun x -> (List.length x> 1))
        let headsOfChunksWithQuantities = byChunksOptimizableOrderItems |> List.map (fun x -> (List.head x, x |> List.fold (fun acc (y:Db.OrderItem) -> y.Quantity + acc) 0 ))
        let tailsOfChunks = byChunksOptimizableOrderItems |> List.map (fun x -> List.tail x)
        let _ = headsOfChunksWithQuantities |> List.iter (fun ((x:Db.OrderItem),count ) -> x.Quantity <- count; ctx.SubmitUpdates()  )
        let _ = tailsOfChunks |> List.iter (fun (x: Db.OrderItem list) -> x |>  List.iter (fun (y:Db.OrderItem) -> Db.safeRemoveOrderItem y.Orderitemid ctx; ctx.SubmitUpdates()))
        let decodedBackUrl = WebUtility.UrlDecode encodedBackUrl
        let order = Db.Orders.getOrder orderId ctx
        let orderItemsDetails = Db.getOrderItemsDetailOfOrderByOutGroup orderOutGroupId ctx
        let orderItemsToMove = 
            orderItemsDetails |> 
            List.filter (fun (x:Db.OrderItemDetails) -> (Db.isInitialState x.Stateid ctx))
        let _ = 
            match orderItemsToMove.Length with
            | X when (X>0 && Settings.Print ) -> printOrderOutGroup orderOutGroupId order orderItemsDetails
            | _ -> ()
        let _ = 
            orderItemsToMove 
            |> Seq.iter (fun (x:Db.OrderItemDetails) -> 
                match (canMoveOrderItemToNextStep x user) with
                | true -> Db.tryMoveOrderItemToNextState x.Orderitemid user.UserId ctx
                | false -> ()
            )
        Redirection.FOUND decodedBackUrl
    with
    | ex ->
        log.Error("Error in moveInitialStateOrderItemsByOrderOutGroup", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

let adjustTotalOfOrder orderId =
    log.Debug(sprintf "adjustTotalOfOrde %d" orderId)
    try
        let ctx = Db.getContext()
        let order = Db.Orders.getOrder orderId ctx
        let orderItemsOfTheOrder = Db.Orders.getOrderItemsOfOrderItemThatAreNotInInitalState orderId ctx
        let total = List.fold (fun x  (y:Db.OrderItem) -> x + (y.Price*(decimal)y.Quantity))  ((decimal)0) orderItemsOfTheOrder
        let adjustedTotal = 
            match (order.Adjustispercentage,order.Adjustisplain) with
            |  (true,false) -> total + (order.Percentagevariataion/((decimal)100.0))*total
            |  (false,true) -> total + order.Plaintotalvariation
            | _ -> total
        Db.setTotalOfOrder orderId total ctx
        Db.setAdjstedTotalOfOrder orderId adjustedTotal ctx
    with
    | ex ->
        log.Error("Error in adjustTotalOfOrder", ex)

let setOrderToArchivedIfAllorderItemsAreDone  orderId =
    log.Debug(sprintf "setOrderToArchivedIfAllorderItemsAreDone %d" orderId)
    let ctx = Db.getContext()
    let orderItemsOfTheOrder = Db.getAllOrderItemsOfOrder orderId ctx
    let states = Db.States.getOrdinaryStates ctx
    let finalState = states |> List.find  (fun (x:Db.State) -> x.Isfinal)

    let allOrderItemsAreDone = List.forall (fun (x:Db.OrderItem) -> (x.Stateid = finalState.Stateid)) orderItemsOfTheOrder
    let ret =
        match allOrderItemsAreDone with
            | true -> 
                try
                    Db.Orders.setOrderAsDoneById orderId ctx
                    let total = List.fold (fun x  (y:Db.OrderItem) -> x + (y.Price*(decimal)y.Quantity))  ((decimal)0) orderItemsOfTheOrder
                    Db.setTotalOfOrder orderId total ctx
                    Db.setAdjstedTotalOfOrder orderId total ctx
                with 
                | ex ->
                    log.Error("Error in setOrderToArchivedIfAllorderItemsAreDone", ex)
            | false -> ()  
    ret

// deprecated (or not?)
//
let moveOrderItemNextStep orderItemId returnPath (user:UserLoggedOnSession) =
    log.Debug(sprintf "moveOrderItemNextStep orderItemId:%d userName %s" orderItemId user.Username)
    warbler (fun _ ->
        try
            let ctx = Db.getContext()
            let orderItemDetail = Db.Orders.tryGetOrderItemDetail orderItemId ctx
            match orderItemDetail with
            | Some theOrderItemDetail -> 
                moveOrderItemNextState theOrderItemDetail user
                setOrderToArchivedIfAllorderItemsAreDone  theOrderItemDetail.Orderid
            | None -> () 
        
            Redirection.FOUND returnPath
        with
        | ex ->
            log.Error("Error in moveOrderItemNextStep", ex)
            Redirection.FOUND Path.Errors.unableToCompleteOperation
    )

let moveOrderItemNextStepAndBacktoOrder orderItemId encodedBackUrl (user:UserLoggedOnSession) =
    log.Debug(sprintf "moveOrderItemNextStepAndBacktoOrder %d" orderItemId )
    warbler (fun x ->
        let retPath = WebUtility.UrlDecode encodedBackUrl
        let ctx = Db.getContext()
        let orderItemDetail = Db.Orders.tryGetOrderItemDetail orderItemId ctx
        match orderItemDetail with
        | Some theOrderItemDetail -> 
            try
                moveOrderItemNextState theOrderItemDetail user
                Redirection.FOUND retPath
            with
            | ex ->
                log.Error("Error in moveOrderItemNextStepAndBacktoOrder", ex)
                Redirection.FOUND Path.Errors.unableToCompleteOperation
        | None ->   Redirection.FOUND retPath
    )

let removeOrderItemThenGoBackToUrl orderItemId encodedBackUrl (user:UserLoggedOnSession ) =
    log.Debug(sprintf "removeOrderItemThenGoBackToUrl %d" orderItemId)
    warbler (fun x ->
        let retPath = WebUtility.UrlDecode encodedBackUrl
        let ctx = Db.getContext()
        let orderItemDetail = Db.Orders.tryGetOrderItemDetail orderItemId ctx
        match orderItemDetail with
        | Some theOrderItemDetail -> 
            Db.safeRemoveOrderItem orderItemId ctx
            Redirection.FOUND retPath
        | None ->   Redirection.FOUND retPath
    )

let orderItemProgress (user:UserLoggedOnSession) =
    log.Debug(sprintf "orderItemProgress user: %s " user.Username)
    let ctx = Db.getContext()
    let myRoleObservablesIds = 
        Db.getObserversCatgoryStateMappingForRole user.RoleId ctx |> 
            List.map (fun (x:Db.Observer) -> (x.Stateid, x.Categoryid))

    let myRoleMovablesIds = 
        Db.getEnablersCatgoryStateMappingForRole user.RoleId ctx |> 
            List.map (fun (x:Db.Enabler) -> (x.Stateid, x.Categoryid))

    let mapOfLinkedStates = Db.getStatesNextStatesPairs' ctx |> Map.ofList
    let initialState = Db.States.getInitState ctx
    let finalState = Db.States.getFinalState ctx


    let orderedStates =
        let rec sequenceOfStates (currentState: Db.State) accumul =
            let nextStateIndex = currentState.Nextstateid
            match currentState.Isfinal  with
            | true -> accumul@[currentState]
            | _ -> 
                let nextState = Db.States.getState nextStateIndex ctx
                ((sequenceOfStates nextState (accumul@ [currentState])))
        sequenceOfStates initialState []

    orderedStates |> List.iter (fun x -> printf  "%s\n" (x.Statusname))

    let orderItemMyRoleCanObserve = 
        myRoleObservablesIds
        |> List.map (fun x -> Db.getOrderItemDetailsOfAParticularStateAndAParticularCategory x ctx) 


    let orderItemMyRoleCanMove = 
        myRoleMovablesIds
        |> List.map (fun x -> Db.getOrderItemDetailsOfAParticularStateAndAParticularCategory x ctx) 

    let concatenatedOrderItemMyRoleCanObserve = 
        List.fold (fun x y -> x@y) [] orderItemMyRoleCanObserve  
        |> List.sortBy (fun (x:Db.OrderItemDetails) -> x.Startingtime)

    let concatenatedOrderItemMyRoleCanMove = 
        List.fold (fun x y -> x@y) [] orderItemMyRoleCanMove  
        |> List.sortBy (fun (x:Db.OrderItemDetails) -> x.Startingtime)

    let orderItemsPerStates = 
        orderedStates 
        |> List.map 
            (fun x -> 
                (x.Statusname, concatenatedOrderItemMyRoleCanObserve |> List.filter (fun y -> y.Stateid = x.Stateid) )
            ) 
        |> Map.ofList

    let listOfVariations = 
        concatenatedOrderItemMyRoleCanObserve |> 
        List.map (fun (x:Db.OrderItemDetails) -> (x.Orderitemid,(Db.getVariationDetailsOfOrderItem x.Orderitemid ctx))) 

    let mapOfVariations = listOfVariations |> Map.ofList

    let mapOfVariationsWithIngredientsIds = 
        listOfVariations |> 
        List.map (fun (id, variations:(Db.VariationDetail list)) -> 
            (id,variations |> List.map (fun (v:Db.VariationDetail) -> v.Ingredientid))) |> Map.ofList

    let orderItemIngredientsMap =  
        concatenatedOrderItemMyRoleCanObserve |> 
        List.map ( fun (x:Db.OrderItemDetails) ->  
            (x.Orderitemid, (Db.getIngredientsOfACourse x.Courseid ctx) |> List.filter (fun (y:Db.IngredientOfCourse) -> 
            not (List.contains y.Ingredientid mapOfVariationsWithIngredientsIds.[x.Orderitemid] ))))

    let orderItemIngredientsMapSequence = 
        orderItemIngredientsMap |>
        List.map (fun (orderItemId,(ingredients:Db.IngredientOfCourse list)) -> 
            (orderItemId, List.fold (fun y (x:Db.IngredientOfCourse)  -> x.Ingredientname+", " + y  ) "" ingredients)) |>
        Map.ofList

    let variationsStringDescriptions = Utils.variationsByStringDescription (mapOfVariations |> Map.toList) ctx

    html (View.viewableOrderItems orderItemsPerStates  mapOfLinkedStates concatenatedOrderItemMyRoleCanMove orderedStates finalState mapOfVariations orderItemIngredientsMapSequence variationsStringDescriptions) 

let getNextState stateId =
    log.Debug(sprintf "getNextState %d" stateId)
    let ctx = Db.getContext()
    let state = Db.States.getState stateId  ctx
    if (state.Isfinal) then state else
        let nextStateId = state.Nextstateid
        Db.States.getState nextStateId ctx

let editOrderItemByCategoryForStrippedUsers (orderItemId:int) categoryId landingPage  encodedBackUrl  (user:UserLoggedOnSession)=
    log.Debug(sprintf "editOrderItemByCategoryForStrippedUsers orderItemId: %d categoryId: %d" orderItemId categoryId)
    let ctx = Db.getContext()
    let orderItem = Db.Orders.tryGetOrderItemDetail orderItemId ctx
    let categories = Db.Courses.getActiveCategories ctx
    let backUrl = WebUtility.UrlDecode(encodedBackUrl)

    match orderItem with
    | Some orderItemExists ->
        let orderId = orderItemExists.Orderid
        let order = Db.Orders.getOrder(orderId) ctx
        let dbUser = Db.getUserById(user.UserId) ctx
        let ingredients = Db.getIngredientsOfCourse orderItemExists.Courseid ctx
        let outGroup = orderItemExists.Groupidentifier

        let viableGroupOutIdsForOrderItem = getViableGroupOutIdentifiers orderId

        if (user.UserId = order.Userid || dbUser.Canmanageallorders || user.Role = "admin") then
            (choose [
                GET >=> warbler (fun x ->
                    let courses = 
                        Db.Courses.getVisibleCoursesByCategory categoryId  ctx
                    html (View.editOrderItemForStrippedUsers orderItemExists courses categories ingredients outGroup backUrl viableGroupOutIdsForOrderItem))

                POST >=> bindToForm Form.strippedOrderItem (fun form ->
                    try
                        let sanitizedComment = if (form.Comment.IsSome) then (form.Comment.Value |> Sanitizer.GetSafeHtmlFragment |> Some) else None
                        let course = Db.getCourseDetail ((int)form.CourseId) ctx
                        let price = course.Price
                        Db.updateOrderItemAndPriceByCourseId  orderItemId ((int)form.CourseId) ((int)(form.Quantity)) sanitizedComment price form.GroupOut ctx
                        makeOrderItemRejectedIfContainsInvisibleIngredients orderItemId ctx
                        makeOrderItemAsRejectedIfContainsUnavalableIngredients orderItemId ctx
                        Db.deleteAnyEmptyOrderOutGroupOfOrder orderId ctx
                        Redirection.FOUND (backUrl+"#order"+(orderItemExists.Orderid |> string))
                    with
                    | ex ->
                        log.Error("Error in editOrderItemByCategoryForStrippedUsers", ex)
                        Redirection.FOUND Path.Errors.unableToCompleteOperation
                    )
                ]) 
            else 
                Redirection.FOUND landingPage 
    | None -> Redirection.FOUND landingPage

let editOrderItemByCategoryForOrdinaryUsers (orderItemId:int) categoryId landingPage encodedBackUrl  (user:UserLoggedOnSession)=
    log.Debug(sprintf "editOrderItemByCategoryForOrdinaryUsers %d %d" orderItemId categoryId)
    let ctx = Db.getContext()
    let orderItem = Db.Orders.tryGetOrderItemDetail orderItemId ctx
    let categories = Db.Courses.getActiveCategories ctx
    let backUrl = WebUtility.UrlDecode encodedBackUrl

    match orderItem with
    | Some orderItemExists ->
        let orderId = orderItemExists.Orderid
        let order = Db.Orders.getOrder(orderId) ctx
        let dbUser = Db.getUserById(user.UserId) ctx
        let ingredients = Db.getIngredientsOfCourse orderItemExists.Courseid ctx
        let outGroup = orderItemExists.Groupidentifier
        let viableGroupOutIdsForOrderItem = getViableGroupOutIdentifiers orderId
        if (user.UserId = order.Userid || dbUser.Canmanageallorders || user.Role = "admin") then

            (choose [
                GET >=> warbler (fun _ ->
                    let courses = 
                        Db.Courses.getVisibleCoursesByCategory categoryId  ctx
                    html (View.editOrderItemForOrdinaryUsers orderItemExists courses categories ingredients outGroup backUrl viableGroupOutIdsForOrderItem))

                POST >=> bindToForm Form.orderItem (fun form ->
                    try
                        let sanitizedComment = if (form.Comment.IsSome) then (form.Comment.Value |> Sanitizer.GetSafeHtmlFragment |> Some) else None
                        Db.updateOrderItemAndPriceByCourseId  orderItemId ((int)(form.CourseId)) ((int)(form.Quantity)) sanitizedComment form.Price form.GroupOut ctx
                        makeOrderItemRejectedIfContainsInvisibleIngredients orderItemId ctx
                        makeOrderItemAsRejectedIfContainsUnavalableIngredients orderItemId  ctx
                        Db.deleteAnyEmptyOrderOutGroupOfOrder orderId ctx
                        Redirection.FOUND (backUrl+"#order"+(orderItemExists.Orderid |> string))
                    with
                    | ex ->
                        log.Error("Error in editOrderItemByCategoryForOrdinaryUsers", ex)
                        Redirection.FOUND Path.Errors.unableToCompleteOperation
                    
                )
            ]) else Redirection.FOUND landingPage 
    | None -> Redirection.FOUND landingPage

let editOrderItemByCategoryPassingUserLoggedOn (orderItemId:int) categoryId landingPage (encodedBackUrl:string)  (user:UserLoggedOnSession)=
    log.Debug(sprintf "editOrderItemByCategoryPassingUserLoggedOn orderItemId: %d, categoryId:%d " orderItemId categoryId)
    let ctx = Db.getContext()
    let dbUser = Db.getUserById user.UserId ctx
    match dbUser.Canchangetheprice with 
    | true -> editOrderItemByCategoryForOrdinaryUsers  (orderItemId:int) categoryId landingPage encodedBackUrl  (user:UserLoggedOnSession)
    | false -> editOrderItemByCategoryForStrippedUsers  (orderItemId:int) categoryId landingPage encodedBackUrl   (user:UserLoggedOnSession)

let editOrderItemByCategoryPassingUserLoggedOnRef (orderItemId:int) categoryId landingPage (encodedBackUrl:string) =
    log.Debug(sprintf "editOrderItemByCategoryPassingUserLoggedOn orderItemId: %d, categoryId:%d " orderItemId categoryId)
    loggedOn (session (function 
        | UserLoggedOn User ->
            let ctx = Db.getContext()
            let dbUser = Db.getUserById User.UserId ctx
            match dbUser.Canchangetheprice with 
            | true -> editOrderItemByCategoryForOrdinaryUsers  (orderItemId:int) categoryId landingPage encodedBackUrl  User
            | false -> editOrderItemByCategoryForStrippedUsers  (orderItemId:int) categoryId landingPage encodedBackUrl   User
        | _ -> UNAUTHORIZED "Not logged on"
    ))

let resetVariationThenEditOrderItemByCat (orderItemId:int) categoryId landingPage urlEncodedBackUrl (user:UserLoggedOnSession) =
    log.Debug(sprintf "resetVariationThenEditOrderItemByCat %d" orderItemId)
    let ctx = Db.getContext()
    let _ = Db.removeAllVariationsOfOrderItem orderItemId ctx
    editOrderItemByCategoryPassingUserLoggedOn orderItemId categoryId landingPage urlEncodedBackUrl user


let resetVariationThenEditOrderItemByCatRef (orderItemId:int) categoryId landingPage urlEncodedBackUrl  =
    loggedOn (session (function 
        | UserLoggedOn User ->
            log.Debug(sprintf "resetVariationThenEditOrderItemByCat %d" orderItemId)
            let ctx = Db.getContext()
            let _ = Db.removeAllVariationsOfOrderItem orderItemId ctx
            editOrderItemByCategoryPassingUserLoggedOnRef orderItemId categoryId landingPage urlEncodedBackUrl 
        | _ -> UNAUTHORIZED "Not logged on")
    )

let createRole (user:UserLoggedOnSession ) =
    log.Debug(sprintf "createRole, logged user: %s" user.Username)
    let ctx = Db.getContext() 
    choose [
        GET >=> html (View.createRole user)
        POST >=> bindToForm Form.role (fun form ->
            Db.createRole form.Name form.Comment ctx 
            Redirection.FOUND Path.home
        )
    ]

let createOrderByUserLoggedOn (user:UserLoggedOnSession) =
    log.Debug(sprintf "createOrderByUserLoggedOn. Logged user: %s" user.Username)
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ ->
            html (View.createOrder "")
        )
        POST >=> bindToForm Form.order (fun form ->
            let thePerson = match form.Person with | Some X -> X | _ -> ""
            let _ = Db.createOrderByUser form.Table thePerson user.UserId ctx
            Redirection.FOUND Path.Orders.myOrders
        )
    ]

let createSingleOrderByUserLoggedOn (user:UserLoggedOnSession) =
    log.Debug(sprintf "createSingleOrderByUserLoggedOn. Logged user %s" user.Username)
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ ->
            html (View.createOrder "")
        )
        POST >=> bindToForm Form.order (fun form ->
            let orderAlreadyExists = Db.tableIsAlreadyInAnOpenOrder form.Table ctx
            match orderAlreadyExists with
                | true -> 
                    View.createOrder "esiste già un ordine associato a quel tavolo " |> html
                | false ->
                    let thePerson = match form.Person with | Some X -> X | _ -> ""
                    let order = Db.createOrderByUser form.Table thePerson user.UserId ctx
                    Redirection.FOUND (sprintf Path.Orders.viewOrder order.Orderid)
        )
    ]

type RolesCategoryMappings = 
    {
        roleidname: IndexNameRecord list
        categoriesidname: IndexNameRecord list
        existingrolestatemapping:  TwoIndexNameRecord list
        selectroleid: string
        selectcategoryid: string
        backurl: string
    }

let roleEnablerObserverCategoriesByCheckBoxesWithRoleAndCat roleId categoryId =
    log.Debug(sprintf "roleEnablerObserverCategoriesByCheckBoxesWithRoleAndCat roleId: %d, categoryId: %d " roleId categoryId)
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ ->
            let roleIdNameMap =  Db.getRoles ctx |>   List.map (fun (g:Db.Role) -> g.Roleid,g.Rolename)
            let roleIdNameMapRef =  Db.getRoles ctx |>   List.map (fun (g:Db.Role) -> g.Roleid,g.Rolename) |> Map.ofList
            let statesIdNameMapRef = Db.States.getOrdinaryStates ctx |> List.map (fun (g:Db.State) -> g.Stateid,g.Statusname) |> Map.ofList
            let categoriesIdNameMap = Db.Courses.getActiveConcreteCategories ctx |> List.map (fun (g:Db.CourseCategories) -> g.Categoryid,g.Name)
            let categoriesIdNameMapRef = Db.Courses.getActiveConcreteCategories ctx |> List.map (fun (g:Db.CourseCategories) -> g.Categoryid,g.Name) |> Map.ofList
            let roleIdsRef = roleIdNameMapRef |> Map.toSeq |> Seq.map (fun (id,_) -> id)
            let statesIdsRef = statesIdNameMapRef |> Map.toSeq |> Seq.map (fun (id,_) -> id)  |> Seq.toList
            let categoriesIdsRef = categoriesIdNameMapRef |> Map.toSeq |> Seq.map (fun (id,_) -> id) |> Seq.toList

            let rolecategoriesCombinations = 
                [
                    for i in roleIdsRef do 
                        for j in categoriesIdsRef do 
                            let observers = List.fold (fun acc k ->  (if (Db.isObserverRoleCatState i j k ctx) then ({entry="Observer"+(statesIdNameMapRef.[k])})::acc else acc)) [] statesIdsRef
                            let enablers = List.fold (fun acc k ->  (if (Db.isEnablerRoleCatState i j k ctx) then ({entry="Enabler"+(statesIdNameMapRef.[k])})::acc else acc)) [] statesIdsRef
                            yield {index1=i;index2=j;enablers = enablers; observers = observers}
            ]

            let o = {
                roleidname = roleIdNameMap|> List.map (fun (ind,value)-> {index=ind;name=value});
                categoriesidname = categoriesIdNameMap |> List.map (fun (ind,value) -> {index=ind;name=value});
                existingrolestatemapping=rolecategoriesCombinations; 
                selectroleid = (roleId|>string) ; 
                selectcategoryid = (categoryId |> string);
                backurl = Path.Admin.roles
            }

            DotLiquid.page("rolesCategoryMappings.html")  o
        )
        POST >=> bindToForm Form.enablersObserverForStateAbilities (fun form -> 
            let _ =  
                match form.EnablerCOLLECTING with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryEnablerByStateName ((int)form.RoleId) "COLLECTING" ((int)form.CategoryId) ctx 
                    ()
                | None -> 
                    let _ = Db.tryRemoveRoleStateCategoryEnablerByStateName ((int)form.RoleId) "COLLECTING" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.EnablerTOBEWORKED with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryEnablerByStateName ((int)form.RoleId) "TOBEWORKED" ((int)form.CategoryId) ctx
                    ()
                | None -> 
                    let _ = Db.tryRemoveRoleStateCategoryEnablerByStateName ((int)form.RoleId) "TOBEWORKED" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.EnablerSTARTEDWORKING with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryEnablerByStateName ((int)form.RoleId) "STARTEDWORKING" ((int)form.CategoryId) ctx
                    ()
                | None -> 
                    let _ = Db.tryRemoveRoleStateCategoryEnablerByStateName ((int)form.RoleId) "STARTEDWORKING" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.EnablerREADYFORDELIVERY with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryEnablerByStateName ((int)form.RoleId) "READYFORDELIVERY" ((int)form.CategoryId) ctx
                    ()
                | None -> 
                    let _ = Db.tryRemoveRoleStateCategoryEnablerByStateName ((int)form.RoleId) "READYFORDELIVERY" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.EnablerDELIVERED with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryEnablerByStateName ((int)form.RoleId) "DELIVERED" ((int)form.CategoryId) ctx
                    ()
                | _ -> 
                    let _ = Db.tryRemoveRoleStateCategoryEnablerByStateName ((int)form.RoleId) "DELIVERED" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.EnablerDONE with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryEnablerByStateName ((int)form.RoleId) "DONE" ((int)form.CategoryId) ctx
                    ()
                | _ -> 
                    let _ = Db.tryRemoveRoleStateCategoryEnablerByStateName ((int)form.RoleId) "DONE" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.ObserverCOLLECTING with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryObserverByStateName ((int)form.RoleId) "COLLECTING" ((int)form.CategoryId) ctx
                    ()
                | _ -> 
                    let _ = Db.tryRemoveRoleStateCategoryObserverByStateName ((int)form.RoleId) "COLLECTING" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.ObserverTOBEWORKED with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryObserverByStateName ((int)form.RoleId) "TOBEWORKED" ((int)form.CategoryId) ctx
                    ()
                | _ -> 
                    let _ = Db.tryRemoveRoleStateCategoryObserverByStateName ((int)form.RoleId) "TOBEWORKED" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.ObserverTOBEWORKED with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryObserverByStateName ((int)form.RoleId) "TOBEWORKED" ((int)form.CategoryId) ctx
                    ()
                | _ -> 
                    let _ = Db.tryRemoveRoleStateCategoryObserverByStateName ((int)form.RoleId) "TOBEWORKED" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.ObserverSTARTEDWORKING with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryObserverByStateName ((int)form.RoleId) "STARTEDWORKING" ((int)form.CategoryId) ctx
                    ()
                | _ -> 
                    let _ = Db.tryRemoveRoleStateCategoryObserverByStateName ((int)form.RoleId) "STARTEDWORKING" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.ObserverREADYFORDELIVERY with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryObserverByStateName ((int)form.RoleId) "READYFORDELIVERY" ((int)form.CategoryId) ctx
                    ()
                | _ -> 
                    let _ = Db.tryRemoveRoleStateCategoryObserverByStateName ((int)form.RoleId) "READYFORDELIVERY" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.ObserverDELIVERED with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryObserverByStateName ((int)form.RoleId) "DELIVERED" ((int)form.CategoryId) ctx
                    ()
                | _ -> 
                    let _ = Db.tryRemoveRoleStateCategoryObserverByStateName ((int)form.RoleId) "DELIVERED" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.ObserverDONE with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryObserverByStateName ((int)form.RoleId) "DONE" ((int)form.CategoryId) ctx
                    ()
                | _ -> 
                    let _ = Db.tryRemoveRoleStateCategoryObserverByStateName ((int)form.RoleId) "DONE" ((int)form.CategoryId) ctx
                    ()

            Redirection.found (sprintf Path.Admin.roleEnablerObserverCategoriesByCheckBoxesByRoleAndCat ((int)form.RoleId) ((int)form.CategoryId))
        )
        
    ]

let roleEnablerObserverCategoriesByCheckBoxes   =
    log.Debug("roleEnablerObserverCategoriesByCheckBoxes")
    let ctx = Db.getContext()

    let firstRole = Db.getAllRoles ctx |> List.tryHead

    let firstRoleId = 
        match firstRole with
        | Some X -> X.Roleid
        | None -> -1

    let firstCategory  = Db.Courses.getAllourseCategories ctx |> List.tryHead

    let firstCategoryId = 
        match firstCategory with
        | Some X -> X.Categoryid
        | None -> -1

    roleEnablerObserverCategoriesByCheckBoxesWithRoleAndCat firstRoleId firstCategoryId 

type NamesAndMeasures =  { names: IndexNameRecord list; measures: IndexUnitMeasureMap list; message: string}

let selectIAllngredientCatForCourse courseId  =
    log.Debug(sprintf "selectIAllngredientCatForCourse %d" courseId )

    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ ->
            log.Debug("selectIAllngredientCatForCourse GET")
            let visibleIngredients = Db.getAllVisibleIngredients  ctx
            let alreadyTakenIngredients = Db.getIngredientIdsOfACourse courseId ctx
            let selectableIngredients = visibleIngredients |> List.filter (fun (x:Db.Ingredient) -> not (List.contains x.Ingredientid alreadyTakenIngredients))
            let myMap = selectableIngredients |> List.map (fun (x:Db.Ingredient) -> {index=x.Ingredientid;name=x.Name })

            let indexUnitsOfMeasures = List.map (fun (x:Db.Ingredient) -> {index=x.Ingredientid ; unitmeasure = x.Unitmeasure}) selectableIngredients

            let o = {names = myMap; measures=indexUnitsOfMeasures; message = ""}
            let course = Db.Courses.tryGetCourse courseId ctx
            match course with
            | Some theCourse ->
                DotLiquid.page("ingredientToCourse.html") o
            | _ ->  Redirection.FOUND Path.home
        ) 
        POST >=> bindToForm Form.ingredientSelector (fun form ->
            try
                log.Debug("selectIAllngredientCatForCourse POST")
                let decQuantity = form.Quantity 
                Db.addIngredientToCourse ((int)form.IngredientBySelect) courseId decQuantity ctx

                let retPath = sprintf Path.Courses.editCourse courseId 
                Redirection.FOUND retPath
            with
            | ex ->
                log.Error("Error in selectIAllngredientCatForCourse", ex)
                Redirection.FOUND Path.Errors.unableToCompleteOperation
        )
    ]

let selectIngredientCatForCourse courseId categoryId message =
    log.Debug(sprintf "selectIngredientCatForCourse courseId: %d, categoryId: %d " categoryId)
    let ctx = Db.getContext()
    choose [

        GET >=> warbler (fun _ ->
            log.Debug("selectIngredientCatForCourse GET")
            let visibleIngredients = Db.getVisibleIngredientsOfACategory categoryId ctx
            let alreadyTakenIngredients = Db.getIngredientIdsOfACourse courseId ctx
            let selectableIngredients = 
                visibleIngredients |> 
                List.filter (fun (x:Db.Ingredient) -> not (List.contains x.Ingredientid alreadyTakenIngredients))
            let myMap = selectableIngredients |> List.map (fun (x:Db.Ingredient)   -> {index=x.Ingredientid;name=x.Name })
            let indexUnitsOfMeasures = List.map (fun (x:Db.Ingredient) -> {index=x.Ingredientid ; unitmeasure = x.Unitmeasure}) selectableIngredients

            let o = { names = myMap; measures=indexUnitsOfMeasures; message = ""}
            DotLiquid.page ("ingredientToCourse.html") o 
        )
        POST >=> bindToForm Form.ingredientSelector (fun form ->
            try
                log.Debug("selectIngredientCatForCourse POST")
                let ingredient = Db.getIngredientById ((int)form.IngredientBySelect) ctx
                let quantity = match (form.Quantity,ingredient.Unitmeasure) with
                                    | (Some X,_) -> Some X
                                    | (None,UNITARY_MEASURE) -> Some ((decimal)1.0)
                                    | _ -> form.Quantity
                Db.addIngredientToCourseById ((int)ingredient.Ingredientid) courseId quantity ctx
                let retPath = sprintf Path.Courses.editCourse courseId 
                Redirection.FOUND retPath
            with
            | ex ->
                log.Error("Error in selectIngredientCatForCourse", ex)
                Redirection.FOUND Path.Errors.unableToCompleteOperation
        )
    ]


let createCourseByCatgory id =
    log.Debug( sprintf "createCourseByCatgory %d")
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ -> 
            let visibleCategories = Db.Courses.getVisibleCourseCategories ctx |> List.map (fun (g:Db.CourseCategories) -> (decimal)g.Categoryid,g.Name)
            html (View.createCourseByCategory "" visibleCategories id))
        POST >=> bindToForm Form.course (fun form ->
            try
                let visibleCategories = Db.Courses.getVisibleCourseCategories ctx |> List.map (fun (g:Db.CourseCategories) -> (decimal)g.Categoryid,g.Name)

                match Db.Courses.tryFindCourseByName form.Name ctx with
                | Some _ -> 
                    View.createCourseByCategory (local.TheName+ form.Name + "esiste già") visibleCategories id |> html

                | None ->
                    let description = match form.Description with | Some X -> X | _ -> ""
                    let course = Db.Courses.createCourse form.Price form.Name description (form.Visibile = Form.VISIBLE) ((int)form.CategoryId) ctx
                    let retPath = sprintf Path.Courses.editCourse course.Courseid 
                    Redirection.FOUND retPath
            with
            | ex ->
                log.Error("Error in createCourseByCatgory", ex)
                Redirection.FOUND Path.Errors.unableToCompleteOperation
        )
    ]

let archiveOrder id (user:UserLoggedOnSession) =
    log.Debug(sprintf "archiveOrder %d" id)
    let ctx = Db.getContext()
    let dbUser = Db.getUserById (user.UserId) ctx
    let _ = if (dbUser.Canmanageallorders || user.Role = "admin") then
                Db.archiveOrder id ctx
                Db.pushArchivedOrdersInLog id ctx
            else ()
    Redirection.FOUND Path.Orders.seeDoneOrders

let unableToCompleteOperation aString    =
    View.cantComplete aString |> html

let voidorder orderId =
    log.Debug(sprintf "voidorder %d" orderId)
    let ctx = Db.getContext()
    Db.voidOrder orderId ctx
    Redirection.FOUND Path.home

let voidOrderByUserLoggedOn orderId urlEncodedBackUrl (user: UserLoggedOnSession) =
    log.Debug(sprintf "voidOrderByUserLoggedOn orderId: %d, user: %s" orderId user.Username)
    let ctx = Db.getContext()
    let order = Db.Orders.tryGetOrder(orderId) ctx
    let dbUser = Db.getUserById(user.UserId) ctx
    let backUrl = WebUtility.UrlDecode urlEncodedBackUrl
    try
        match order with 
        | Some theOrder -> warbler (fun x ->
            if (user.Role = "admin" || (theOrder.Userid = user.UserId && dbUser.Canvoidorders))
                then 
                    let _ = Db.voidOrder orderId ctx
                    let _ = Db.addVoidedOrderToLog orderId user.UserId ctx
                    Redirection.FOUND backUrl
            else 
                    Redirection.FOUND backUrl)
        | _ -> Redirection.FOUND Path.home 
    with
    | ex ->
        log.Error("Error in voidOrderByUserLoggedOn", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation
    
let askConfirmationVoidOrderByUserLoggedOn orderId encodedBackUrl (user: UserLoggedOnSession) =
    log.Debug(sprintf "%s order: %d  user: %s" "askConfirmationVoidOrderByUserLoggedOn" orderId user.Username)
    View.askConfirmationVoidOrderByUserLoggedOn orderId encodedBackUrl (user: UserLoggedOnSession) |> html

let deleteIngredientToCourse courseId ingredientId =
    log.Debug(sprintf "deleteIngredientToCourse courseId: %d, ingredientId: %d" courseId ingredientId)
    let ctx = Db.getContext()
    let ingredientcourse = Db.tryGetIngredientCourse courseId ingredientId ctx
    match ingredientcourse with 
    | Some theIngredientCourse -> theIngredientCourse.Delete(); ctx.SubmitUpdates()
    | None -> ()
    let retPath = sprintf Path.Courses.editCourse courseId 
    Redirection.FOUND retPath

let archiveOrderByUserId orderId  =
    log.Debug(sprintf "archiveOrderByUserId %d" orderId )
    let ctx = Db.getContext()
    let order = Db.Orders.getOrder orderId ctx
    match order.Archived with
        | true -> ()
        | false -> 
            try
                Db.archiveOrder orderId ctx
                Db.pushArchivedOrdersInLog orderId ctx
            with
            | ex ->
                log.Error("Error in archiveOrderByUserId", ex)
            ()

let archiveOrdersWithAllOrderItemsInPaidSubOrders (orders: (Db.NonArchivedOrderDetail) list)  =
    let paidOrders = orders |> List.filter (fun order -> allSubOrdersOfOrderDetailBelongsToAPaidSubOrder order.Orderid )
    let _ = paidOrders |> List.iter (fun x -> archiveOrderByUserId x.Orderid  )
    ()

let seeDoneOrders (user:UserLoggedOnSession)=
    log.Debug("seeDoneOrders")
    let ctx  = Db.getContext()

    let loadedOrders = Db.Orders.getPayableOrderDetails ctx  

    let _ = loadedOrders |> List.iter (fun (x:Db.NonArchivedOrderDetail) -> adjustTotalOfOrder x.Orderid )
    let orders = Db.Orders.getPayableOrderDetails ctx  

    let orderItemsOfOrders = orders |> List.map (fun (x:Db.NonArchivedOrderDetail) -> 
        (x.Orderid, Db.getOrderItemDetailOfOrderDetailThatAreNotInInitStateByNonEmptyOrderDetail x ctx)) |> Map.ofList 

    let dbUser = Db.getUserById(user.UserId) ctx

    let subOrdersOfOrdersMap = 
        orders |> 
        List.map (fun (x:Db.NonArchivedOrderDetail) -> (x.Orderid,Db.Orders.getSubOrdersOfOrderById x.Orderid ctx)) 

    let ordersHavingSubOrdersMap = subOrdersOfOrdersMap |> List.map (fun (x,y) -> (x,(List.length y >0)))  |> Map.ofList

    match dbUser.Canmanageallorders with
    | true -> 
        View.seeDoneOrders orders orderItemsOfOrders ordersHavingSubOrdersMap |> html
    | _ -> Redirection.found Path.home

let adjustPriceOfOrderItemByVariations orderItemId =
    log.Debug(sprintf "%s %d" "adjustPriceOfOrderItemByVariations" orderItemId)
    let ctx = Db.getContext()
    try
        let orderItem = Db.Orders.getTheOrderItemById orderItemId ctx

        let course = Db.Courses.getCourse orderItem.Courseid ctx
        let originalPrice = course.Price

        let variations = orderItem.``public.variations by orderitemid`` |> Seq.toList

        let addVariations = 
            variations 
            |> List.filter 
                (fun (x:Db.Variation) -> 
                    x.Tipovariazione = 
                        Globals.AGGIUNGIMOLTO 
                        || x.Tipovariazione = Globals.AGGIUNGINORMALE 
                        || x.Tipovariazione = Globals.AGGIUNGIPOCO 
                )

        let subtractVariations = 
            variations 
            |> List.filter (fun (x:Db.Variation) -> x.Tipovariazione = Globals.SENZA) 
        let unitaryAddOrSubtractVariations = variations |> List.filter (fun (x:Db.Variation) -> x.Tipovariazione = UNITARY_MEASURE)
        let unitaryAddOrSubtract = unitaryAddOrSubtractVariations |> List.map (fun (x:Db.Variation) -> ((decimal)x.Plailnumvariation) * (Db.getFirstPriceVariationForIngredientAddVariatonFlaggedAsDefault x.Ingredientid ctx )) |> List.fold (+) ((decimal)0.0)

        let specificIngredientPriceBasedVariations = variations |> List.filter (fun (x:Db.Variation) -> x.Tipovariazione = Globals.PER_PREZZO_INGREDIENTE)

        let amountToAddToPriceBeacuseOfAddings = 
            match Settings.InAddIngredientAdjustPrice with 
            | true ->  addVariations |> List.map (fun (x:Db.Variation) -> 
                Db.getFirstPriceVariationForIngredientAddVariatonFlaggedAsDefault x.Ingredientid ctx) |> List.fold (+) ((decimal)0.0)
            | false -> (decimal)0.0

        let amountToSubtractToPriceBeacuseOfSubtractions = 
            match Settings.InAddIngredientAdjustPrice with 
                | true ->  subtractVariations |> List.map (fun (x:Db.Variation) -> 
                    Db.getFirstPriceVariationForIngredientSubtractVariatonFlaggedAsDefault x.Ingredientid ctx) |> List.fold (+) ((decimal)0.0)
                | false -> (decimal)0.0
        let pricesGivenToTheIngredientPrices = specificIngredientPriceBasedVariations |> List.map (fun (x:Db.Variation) -> (Db.getIngredientPrice x.Ingredientpriceid ctx)) |> List.map (fun (x:Db.IngredientPrice) -> x.Addprice ) |> List.fold (+) ((decimal)0)
        let _ = orderItem.Price <- originalPrice + amountToAddToPriceBeacuseOfAddings - amountToSubtractToPriceBeacuseOfSubtractions + unitaryAddOrSubtract + pricesGivenToTheIngredientPrices
        ctx.SubmitUpdates()
    with
    | ex ->
        log.Error("Error in adjustPriceOfOrderItemByVariations", ex)

let editOrderItemVariationPassingUserLoggedOnAndIngredientList orderItemId (ingredientsYouCanAdd: Db.IngredientDetail list) encodedBackUrl (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d " "editOrderItemVariationPassingUserLoggedOnAndIngredientList" orderItemId)
    let ctx = Db.getContext()
    let theOrderItemDetail = Db.Orders.getOrderItemDetail orderItemId ctx
    let ingredientCategories = Db.getVisibleIngredientCategories ctx
    let standardVariationsForCourseDetails = Db.StandardVariations.getStandardVariationsForCourseDetails theOrderItemDetail.Courseid ctx

    let dbUser = Db.getUserById user.UserId ctx
    if (user.UserId = theOrderItemDetail.Userid || dbUser.Canmanageallorders || user.Role = "admin" ) then
      choose [
        GET >=> warbler (fun _ ->
            let ctx = Db.getContext()
            let _ = adjustPriceOfOrderItemByVariations theOrderItemDetail.Orderitemid

            let specificCustomAddQuantitiesForIngredients = ingredientsYouCanAdd |> List.map (fun (x:Db.IngredientDetail) -> (x.Ingredientid,Db.getIngredientPrices x.Ingredientid ctx)) |> Map.ofList

            (let ingredientOfCourse = Db.getIngredientsOfCourse theOrderItemDetail.Courseid ctx
             let existingVariations =  Db.getAllVariationDetailsOfOrderItem theOrderItemDetail.Orderitemid ctx

             html (View.editOrderItemVariations theOrderItemDetail ingredientOfCourse 
                existingVariations ingredientCategories ingredientsYouCanAdd specificCustomAddQuantitiesForIngredients standardVariationsForCourseDetails encodedBackUrl  ))
        )

        POST >=> bindToForm Form.addIngredient (fun form -> 
            try
                let ctx = Db.getContext()
                let _ = Db.addAddIngredientVariationById orderItemId ((int)(form.IngredientBySelect)) form.Quantity ctx
                let _ = adjustPriceOfOrderItemByVariations orderItemId
                let redirTo = (sprintf Path.Orders.editOrderItemVariation orderItemId )
                Redirection.found (redirTo)
            with
            | ex ->
                log.Error("Error in editOrderItemVariationPassingUserLoggedOnAndIngredientList", ex)
                Redirection.FOUND Path.Errors.unableToCompleteOperation
        )
     ]
     
     else (Redirection.FOUND Path.home)

let viewSingleOrderRef orderId =
    loggedOn (session (function 
        | UserLoggedOn user  ->
            log.Debug(sprintf "%s %d" "viewSingleOrderRef" orderId)
            let ctx = Db.getContext()
            Db.deleteAnyEmptyOrderOutGroupOfOrder orderId ctx
            let orderDetail = Db.getOrderDetail orderId ctx
            let myOrdersDetails = Db.Orders.getOngoingOrderDetailsByUser user.UserId ctx 
            let othersOrdersDetails = match user.CanManageAllOrders with
                                        | true -> Db.Orders.getOngoingOrderDetailsByAllUserExcept user.UserId ctx 
                                        | _ -> []
            let userView = Db.getUserViewById user.UserId ctx
            let activeCategories = Db.Courses.getAllActiveVisibleRootCategories ctx
            let orderItemDetails = Db.getOrderItemDetailOfOrderById orderId ctx
            let mapOfLinkedStates = Db.getMapOfStates ctx 
            let statesEnabledForUser = Db.listOfEnabledStatesForWaiter userView.Userid ctx
            let eventualRejectionsOfOrderItems = 
                [ orderDetail ] 
                |> List.map (fun (x:Db.Orderdetail) ->  
                    Db.getOrderItemDetailOfOrderDetail x ctx  ) 
                    |> List.fold (@) [] 
                    |> List.map (fun (x:Db.OrderItemDetails) -> 
                        (x.Orderitemid,Db.getLatestRejectionOfOrderItem x.Orderitemid ctx)) 
                        |> Map.ofList

            let outGroupsOfOrder = Db.getOutGroupsOfOrder orderId ctx

            (View.viewSingleOrder orderDetail orderItemDetails mapOfLinkedStates statesEnabledForUser 
                eventualRejectionsOfOrderItems activeCategories userView outGroupsOfOrder myOrdersDetails othersOrdersDetails user) |> html

        | _ -> UNAUTHORIZED "Not logged on"
    ))

let editOrderItemVariationPassingUserLoggedOn orderItemId  (user:UserLoggedOnSession) =
    warbler (fun  x ->  
        log.Debug(sprintf "%s %d" "editOrderItemVariationPassingUserLoggedOn" orderItemId )
        let ctx = Db.getContext()
        let theOrderItem = Db.Orders.getTheOrderItemById orderItemId ctx
        let _ = theOrderItem.Hasbeenrejected <- false
        let _ = adjustTotalOfOrder theOrderItem.Orderid
        ctx.SubmitUpdates()
        let courseOfOrderItem = Db.Courses.getCourse theOrderItem.Courseid ctx
        let ingredientIdsOfCourse = courseOfOrderItem.``public.ingredientcourse by courseid`` |> Seq.toList |> List.map (fun (x:Db.IngredientCourse) -> x.Ingredientid) 
        let ingredientDetailsYouCanAdd = Db.getAllVisibleIngredientDetails ctx
        let ingredientDetailsYouCanAddWithoutAlreadyThere =  
            ingredientDetailsYouCanAdd |> 
            List.filter (fun (x:Db.IngredientDetail) -> not (List.contains x.Ingredientid  ingredientIdsOfCourse ))
        let encodedBackUrl = WebUtility.UrlEncode (sprintf Path.Orders.viewOrder theOrderItem.Orderid)
        editOrderItemVariationPassingUserLoggedOnAndIngredientList orderItemId ingredientDetailsYouCanAddWithoutAlreadyThere encodedBackUrl user 
    )

let editOrderItemVariationByIngredientCategoryPasssingUserLoggedOn orderItemId categoryId  (user:UserLoggedOnSession) =
    warbler ( fun x ->
        log.Debug(sprintf "%s %d " "editOrderItemVariationByIngredientCategoryPasssingUserLoggedOn" orderItemId)
        let ctx = Db.getContext()
        let ingredientsDetailYouCanAdd = Db.getVisibleIngredientsDetailOfACategory categoryId ctx
        let orderItem = Db.Orders.getTheOrderItemById orderItemId ctx
        let encodedBackUrl = WebUtility.UrlEncode (sprintf Path.Orders.viewOrder orderItem.Orderid)
        let courseOfOrderItem = Db.Courses.getCourse orderItem.Courseid ctx
        let ingredientIdsOfCourse = courseOfOrderItem.``public.ingredientcourse by courseid`` |> Seq.toList |> List.map (fun (x:Db.IngredientCourse) -> x.Ingredientid) 
        let ingredientDetailsYouCanAddWithoutAlreadyThere =  
            ingredientsDetailYouCanAdd |> 
            List.filter (fun (x:Db.IngredientDetail) -> not (List.contains x.Ingredientid  ingredientIdsOfCourse ))
        editOrderItemVariationPassingUserLoggedOnAndIngredientList orderItemId ingredientDetailsYouCanAddWithoutAlreadyThere encodedBackUrl user 
    )

let addWithoutIngredientVariation orderItemId ingredientId encodedBackUrl  (user: UserLoggedOnSession) =
    log.Debug(sprintf "%s %d %d " "addWithoutIngredientVariation" orderItemId ingredientId)
    let ctx = Db.getContext()
    let orderItemDetail = Db.Orders.getOrderItemDetail orderItemId ctx
    let course = Db.Courses.getCourse orderItemDetail.Courseid ctx
    let ingredientsMap = 
        Db.getIngredientsOfACourse course.Courseid ctx |> 
        List.map (fun (x:Db.IngredientOfCourse) -> (x.Ingredientid,x)) |> Map.ofList
    let dbUser = Db.getUserById user.UserId ctx
    let _ = 
        try
            if (orderItemDetail.Userid = user.UserId || user.Role = "admin" || dbUser.Canmanageallorders) then 
                        (match ingredientsMap.[ingredientId].Unitmeasure with
                        | UNITARY_MEASURE  -> Db.addRemoveUnitaryIngredientVariationOrDecreaseByOne orderItemId ingredientId ctx
                        | _ -> (Db.addRemoveIngredientVariation orderItemId ingredientId ctx)) 
                    else 
                        ()
        with
        | ex ->
            log.Error("Error in addWithoutIngredientVariation", ex)
    let _ = adjustPriceOfOrderItemByVariations orderItemId 
    editOrderItemVariationPassingUserLoggedOn orderItemId  user

let removeAllInvisibleIngredients orderItemId encodedBackUrl (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d " "removeAllInvisibleIngredients" orderItemId)
    let ctx = Db.getContext()
    let orderItem = Db.Orders.getOrderItemById orderItemId ctx
    match orderItem with
        | Some theOrderItem ->    
            try
                let course = Db.Courses.getCourse theOrderItem.Courseid ctx   
                let ingredientsOfTheCourse = Db.getIngredientsOfCourse course.Courseid ctx
                let unavailables = ingredientsOfTheCourse |> List.filter (fun (x:Db.IngredientOfCourse) -> ( not x.Visibility))
                unavailables |> List.iter (fun (x:Db.IngredientOfCourse) -> Db.addRemoveIngredientVariation orderItemId x.Ingredientid ctx) 
            with
            | ex ->
                log.Error("Error in removeAllInvisibleIngredients", ex)
        | _ -> ()
    editOrderItemVariationPassingUserLoggedOn orderItemId  user

let removeAllAllergenic orderItemId  encodedBackUrl (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d" "removeAllAllergenic" orderItemId)
    let ctx = Db.getContext()
    let orderItem = Db.Orders.getOrderItemById orderItemId ctx
    match orderItem with 
        | Some theOrderItem ->
            try
                let course = Db.Courses.getCourse theOrderItem.Courseid ctx
                let ingredientsOfTheCourse = Db.getIngredientsOfCourse course.Courseid ctx
                let allergenics = ingredientsOfTheCourse |> List.filter (fun (x:Db.IngredientOfCourse) -> x.Allergen)
                allergenics |> List.iter (fun (x:Db.IngredientOfCourse) -> Db.addRemoveIngredientVariation orderItemId x.Ingredientid ctx)
            with
            | ex ->
                log.Error("Error in removeAllAllergenic", ex)
        | _ -> ()
    editOrderItemVariationPassingUserLoggedOn orderItemId  user

let addDiminuishIngredientVariation orderItemId ingredientId encodedBackUrl (user:UserLoggedOnSession)=
    log.Debug(sprintf "%s %d %d " "addDiminuishIngredientVariation" orderItemId ingredientId )
    let ctx = Db.getContext()
    let orderDetail = Db.Orders.getOrderItemDetail orderItemId ctx
    let dbUser = Db.getUserById user.UserId ctx
    let _ = 
        try
            if 
                (
                    orderDetail.Userid = user.UserId 
                    || user.Role = "admin" 
                    || dbUser.Canmanageallorders
                )
            then 
                (Db.addDiminuishIngredientVariattion orderItemId ingredientId ctx) 
            else 
                ()
        with
        | ex ->
            log.Error("Error in addDiminuishIngredientVariation", ex)
    editOrderItemVariationPassingUserLoggedOn orderItemId  user

let addIncreaseIngredientVariation orderItemId ingredientId encodedBackUrl (user:UserLoggedOnSession)=
    log.Debug(sprintf "%s %d %d" "addIncreaseIngredientVariation"  orderItemId  ingredientId)
    let ctx = Db.getContext()
    let orderItemDetail = Db.Orders.getOrderItemDetail orderItemId ctx
    let course = Db.Courses.getCourse orderItemDetail.Courseid ctx
    let ingredientsMap = 
        Db.getIngredientsOfACourse course.Courseid ctx |> 
        List.map (fun (x:Db.IngredientOfCourse) -> (x.Ingredientid,x)) |> Map.ofList
    let dbUser = Db.getUserById user.UserId ctx
    let _ = 
        try
            if 
                orderItemDetail.Userid = user.UserId 
                || user.Role = "admin" 
                || dbUser.Canmanageallorders
            then 
                match ingredientsMap.[ingredientId].Unitmeasure with
                | UNITARY_MEASURE -> Db.addAddUnitaryIngredientVariationOrIncreaseByOne orderItemId ingredientId ctx
                | _ -> (Db.addIncreaseIngredientVariation orderItemId ingredientId ctx) 
            else 
                ()
        with
        | ex ->
            log.Error("Error in addIncreaseIngredientVariation", ex)
    let _ = adjustPriceOfOrderItemByVariations orderItemId 
    editOrderItemVariationPassingUserLoggedOn orderItemId  user

let removeIngredientVariation variationId orderitemId  encodedBackUrl (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d %d " "removeIngredientVariation" variationId orderitemId)
    let ctx = Db.getContext()
    let orderDetail = Db.Orders.getOrderItemDetail orderitemId ctx
    let dbUser = Db.getUserById user.UserId ctx
    let _ = 
        try
            if 
                orderDetail.Userid = user.UserId 
                || user.Role = "admin" 
                || dbUser.Canmanageallorders
            then 
                Db.removeIngredientVariation  variationId ctx
            else ()
        with
        | ex ->
            log.Error("Error in removeIngredientVariation", ex)
    let _ = adjustPriceOfOrderItemByVariations orderitemId
    editOrderItemVariationPassingUserLoggedOn orderitemId  user

let increaseUnitaryIngredientVariation variationId encodedBackUrl (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d " "increaseUnitaryIngredientVariation" variationId)
    let ctx = Db.getContext()
    let variation = Db.getVariation variationId ctx
    let variationDetail = Db.getVariationDetail variationId ctx
    let dbUser = Db.getUserById user.UserId ctx
    let _ = if (variationDetail.Userid = user.UserId || user.Role = "admin" || dbUser.Canmanageallorders) then variation.Plailnumvariation <- variation.Plailnumvariation + 1; ctx.SubmitUpdates()

    // the eventual error log is in the implementation of adjustPriceOfOrderItemByVariations
    let _ = adjustPriceOfOrderItemByVariations variation.Orderitemid
    editOrderItemVariationPassingUserLoggedOn variation.Orderitemid  user

let decreaseUnitaryIngredientVariation variationId encodedBackUrl (user:UserLoggedOnSession) =
    let ctx = Db.getContext()
    let variation = Db.getVariation variationId ctx
    let variationDetail = Db.getVariationDetail variationId ctx
    let dbUser = Db.getUserById user.UserId ctx
    let _ = if (variationDetail.Userid = user.UserId || user.Role = "admin" || dbUser.Canmanageallorders) then variation.Plailnumvariation <- variation.Plailnumvariation - 1; ctx.SubmitUpdates()

    // the eventual error log is in the implementation of adjustPriceOfOrderItemByVariations
    let _ = adjustPriceOfOrderItemByVariations variation.Orderitemid
    editOrderItemVariationPassingUserLoggedOn variation.Orderitemid  user

let deleteUser id  = warbler (fun x ->
    let backUrl = match x.request.queryParam("backUrl") with | Choice1Of2 par -> par | _ -> Path.Admin.deleteObjects
    let ctx = Db.getContext()
    let user = Db.getUserViewById id ctx
    match user.Rolename with
    | "admin" -> Redirection.found backUrl // Path.Admin.deleteObjects
    | _ ->
        try
            Db.safeDeleteUser id ctx
            Redirection.found backUrl // Path.Admin.deleteObjects
        with
        | ex ->
            log.Error("Error in deleteUser", ex)
            Redirection.found Path.Errors.unableToCompleteOperation
    )

let deleteCourseCategory id =
    let ctx =  Db.getContext()
    try
        Db.safeDeleteCourseCategory id ctx
        Redirection.found Path.Admin.deleteObjects
    with
    | ex ->
        log.Error("Error in deleteCourseCategory", ex)
        Redirection.found Path.Errors.unableToCompleteOperation

let deleteIngredientCategory id = 
    let ctx = Db.getContext()
    try
        Db.safeDeleteIngredientCategory id ctx
        Redirection.found Path.Admin.deleteObjects
    with
    | ex ->
        log.Error("Error in deleteIngredientCategory", ex)
        Redirection.found Path.Errors.unableToCompleteOperation

type LiquidWrappedOrderItemsForEdit = 
    {   
        orderitemdetailswrapped: OrderItemDetailsWrapped list 
        suborderwrapped: SubOrderWrapped list
        currentsuborderid: int
    }

type invoiceModel = {daticliente: string; orderitemdetailswrapped: OrderItemDetailsWrapped list; nextavailablenumber: int; idname: IndexNameDataRecord list}
let printWholeOrderInvoiceAsWordformat orderId =
    failwith "unimplemented"

let printSubOrderInvoice subOrderId =
    log.Debug(sprintf "%s %d " "printSubOrderInvoice" subOrderId)
    let ctx = Db.getContext()

    let orderItemsDetails = Db.getOrderItemDetailsOfSubOrderThatAreNotInInitState subOrderId ctx

    let allCustomers = Db.getAllCustomers ctx
    let idNameCustomers = allCustomers |> List.map (fun (x:Db.CustomerData) -> {index=x.Customerdataid;name=x.Name;data=x.Data})

    let nextAvailableNumber = 
        match Db.latestInvoiceNumber ctx with
        | Some N -> N + 1
        | None -> 1

    let orderItemDetailsWrappedList = orderItemsDetails |> List.map (fun (x:Db.OrderItemDetails) -> DbObjectWrapper.WrapOrderItemDetails(x)  "") 

    let totalOfSuborder = orderItemsDetails |> List.map (fun (x:Db.OrderItemDetails) -> x.Price) |> List.fold (+) 0M

    choose [
        GET >=> warbler (fun _ ->
            let datiCliente = {daticliente = ""; orderitemdetailswrapped = orderItemDetailsWrappedList; nextavailablenumber = nextAvailableNumber; idname=idNameCustomers }
            DotLiquid.page("invoiceData.html") datiCliente
        )

        POST >=> bindToForm Form.invoiceForm (
            fun form -> 
                try
                    let printerForReceipts = Db.getPrintersForReceipts ctx
                    let printerNames = printerForReceipts |> List.map (fun (x:Db.Printer) -> x.Name)

                    let customerDataId = 
                        match (form.CompanyId,form.StoreCompany) with 
                        | (-1M, Some _) -> 
                            let newCustomer = Db.createCustmomerData (form.CompanyName |> Sanitizer.GetSafeHtmlFragment) (form.Comment |> Sanitizer.GetSafeHtmlFragment) ctx
                            newCustomer.Customerdataid
                        | (X,_)  when (X <> -1M) ->
                            Db.tryUpdateCustomerData ((int)X) (form.CompanyName |> Sanitizer.GetSafeHtmlFragment) (form.Comment |> Sanitizer.GetSafeHtmlFragment) ctx
                            (int)X
                        | _ -> -1


                    let _ = removeSpooledFiles()

                    let now = System.DateTime.Now.ToLocalTime()

                    let unBoundledTotal = Utils.unbundleVat totalOfSuborder Globals.ALIQUOTA_IVA_UNICA

                    let showDetails = 
                        match form.ShowDetails with 
                        | Some _ -> (orderItemsDetails |> (List.fold (fun y (x:Db.OrderItemDetails) ->  y + (sprintf "%d %-20s %-10.2f\n"    x.Quantity  x.Name  x.Price )) ""))  
                        | None -> ""

                    let text = "fattura: n."+(form.InvoiceNumber|> string)+"\n\n"+ "data:"+now.ToString() + "\n\n" + form.Comment + "\n\n" + showDetails + "\n"+(sprintf "%-22s %-40.2f\n" "totale: " totalOfSuborder)+ "\n\n" + (sprintf "imponibile: %-30.2f"  unBoundledTotal)+"\n\nIVA " + (sprintf "%.0f %%: " Globals.ALIQUOTA_IVA_UNICA) + (sprintf "%.2f" (totalOfSuborder - unBoundledTotal))+ "\n\n"

                    let invoice = 
                        try
                            match customerDataId with
                            | -1 -> Db.createInvoiceBySubOrderIdWithNoCustomerId subOrderId text ((int)form.InvoiceNumber) ctx
                            | _ ->  Db.createInvoiceBysubOrderIdAndCustomerId subOrderId customerDataId text ((int)form.InvoiceNumber) ctx                
                        with
                        | ex ->
                            log.Error("Error in printSubOrderInvoice", ex)

                    let fileName = sprintf "receipt_print%d.txt" System.DateTime.Now.Ticks
                    let outFile = new System.IO.StreamWriter(fileName,true,Encoding.UTF8)
                    let _ = outFile.WriteLine(text)
                    let _ = outFile.Close()
                    let _ = printerNames |> List.iter (fun x -> 
                        // System.Diagnostics.Process.Start(Settings.Printcommand, "-P" + x + " " + AppDomain.CurrentDomain.BaseDirectory + fileName) |> ignore
                        System.Diagnostics.Process.Start(Settings.Printcommand, Settings.PrinterSelector + x + " " + Directory.GetCurrentDirectory() + "/" + fileName) |> ignore
                        File.Copy(fileName,x+fileName.Replace(".txt",".ok"),true)
                    )

                    Db.setSubOrderAsPaid subOrderId ctx

                    let orderId = Db.getOrderIdOfSubOrder subOrderId ctx

                    Redirection.found (sprintf Path.Orders.subdivideDoneOrder orderId)
                with
                | ex ->
                    log.Error("Error in printSubOrderInvoice", ex)
                    Redirection.FOUND Path.Errors.unableToCompleteOperation
        )
    ]

let printWholeOrderInvoice orderId =
    log.Debug(sprintf "%s %d" "printWholeOrderInvoice" orderId)
    let ctx = Db.getContext()
    let orderItemsDetails = Db.getOrderItemDetailOfOrderThatArenotInInitState orderId ctx
    let order = Db.Orders.getOrder orderId ctx

    let allCustomers = Db.getAllCustomers ctx
    let idNameCustomers = allCustomers |> List.map (fun (x:Db.CustomerData) -> {index=x.Customerdataid;name=x.Name; data=x.Data})

    let nextAvailableNumber = 
        match Db.latestInvoiceNumber ctx with
        | Some N -> N + 1
        | None -> 1

    let orderItemDetailsWrappedList = orderItemsDetails |> List.map (fun (x:Db.OrderItemDetails) -> DbObjectWrapper.WrapOrderItemDetails(x) "") 

    choose [
        GET >=> warbler (fun _ ->
            let datiCliente = {daticliente = ""; orderitemdetailswrapped = orderItemDetailsWrappedList; nextavailablenumber = nextAvailableNumber; idname  = idNameCustomers }
            DotLiquid.page("invoiceData.html") datiCliente
        )

        POST >=> bindToForm Form.invoiceForm (
            fun form -> 
                try
                    let printerForReceipts = Db.getPrintersForReceipts ctx
                    let printerNames = printerForReceipts |> List.map (fun (x:Db.Printer) -> x.Name)

                    let customerDataId = 
                        match (form.CompanyId,form.StoreCompany) with 
                        | (-1M, Some _) -> 
                            let newCustomer = Db.createCustmomerData form.CompanyName form.Comment ctx
                            newCustomer.Customerdataid
                        | (X,_)  when (X <> -1M) ->
                            Db.tryUpdateCustomerData ((int)X) form.CompanyName form.Comment ctx
                            (int)X
                        | _ -> -1

                    let _ = removeSpooledFiles()

                    let textAboutTotal =   
                        match (order.Adjustispercentage,order.Adjustisplain) with
                        | (true, false) -> sprintf "sconto percentuale: %.2f%%\n%s %.2f"  order.Percentagevariataion "Totale scontato" order.Adjustedtotal
                        | (false, true) -> sprintf "sconto: %.2f\n%s %.2f"  order.Plaintotalvariation  "Totale scontato" order.Adjustedtotal
                        | _ -> ""

                    let now = System.DateTime.Now.ToLocalTime()

                    let unBoundledTotal = Utils.unbundleVat order.Adjustedtotal Globals.ALIQUOTA_IVA_UNICA

                    let showDetails = 
                        match form.ShowDetails with 
                        | Some _ -> (orderItemsDetails |> (List.fold (fun y (x:Db.OrderItemDetails) ->  y + (sprintf "%d %-20s %-10.2f\n"    x.Quantity  x.Name  x.Price )) ""))  
                        | None -> ""

                    let text = "fattura: n. "+(form.InvoiceNumber|> string)+"\n\n"+ "data:"+now.ToString() + "\n\n" + form.Comment + "\n\n"+ showDetails + "\nTotale: "+(order.Total |> string)+ "\n"+textAboutTotal + "\n\n" + "imponibile: " + (sprintf "%.2f" unBoundledTotal)+"\n\nIVA " + (sprintf "%.0f %%: " Globals.ALIQUOTA_IVA_UNICA) + (sprintf "%.2f" (order.Adjustedtotal - unBoundledTotal))+ "\n\n"

                    let invoice = 
                        try
                            match customerDataId with
                                | -1 -> Db.createInvoiceByOrderIdWithNoCustomerId orderId text ((int)form.InvoiceNumber) ctx
                                | _ ->  Db.createInvoiceByOrderIdAndCustomerId orderId customerDataId text ((int)form.InvoiceNumber) ctx
                        with
                        | ex ->
                            log.Error("Error in printWholeOrderInvoice", ex)

                    let fileName = sprintf "receipt_print%d.txt" System.DateTime.Now.Ticks
                    let outFile = new System.IO.StreamWriter(fileName,true,Encoding.UTF8)
                    let _ = outFile.WriteLine(text)
                    let _ = outFile.Close()
                    let _ = printerNames |> List.iter (fun x -> 
                        // System.Diagnostics.Process.Start(Settings.Printcommand, "-P" + x + " " + AppDomain.CurrentDomain.BaseDirectory + fileName) |> ignore
                        System.Diagnostics.Process.Start(Settings.Printcommand, Settings.PrinterSelector + x + " " + Directory.GetCurrentDirectory() + "/" + fileName) |> ignore
                        File.Copy(fileName,x+fileName.Replace(".txt",".ok"),true)
                    )
                    let _ = archiveOrderByUserId orderId

                    Redirection.found Path.Orders.seeDoneOrders
                with
                | ex -> 
                    log.Error("Error in printWholeOrderInvoice", ex)
                    Redirection.FOUND Path.Errors.unableToCompleteOperation
        )
    ]

let printWholeOrderReceipt orderId =
    try
        log.Debug(sprintf "%s %d " "printWholeOrderReceipt" orderId)
        let ctx = Db.getContext()
        let order = Db.Orders.getOrder orderId ctx
        let orderItemsDetails = Db.getOrderItemDetailOfOrderThatArenotInInitState orderId ctx
        let printerForReceipts = Db.getPrintersForReceipts ctx
        let printerNames = printerForReceipts |> List.map (fun (x:Db.Printer) -> x.Name)
        let _ = removeSpooledFiles()
        let text = Utils.textForWholeOrderReceipt orderId orderItemsDetails ctx
        let fileName = sprintf "receipt_print%d.txt" System.DateTime.Now.Ticks
        let outFile = new System.IO.StreamWriter(fileName,true,Encoding.UTF8)
        let _ = outFile.WriteLine(text)
        let _ = outFile.Close()
        let _ = printerNames |> List.iter (fun x -> 
            // System.Diagnostics.Process.Start(Settings.Printcommand, "-P" + x + " " + AppDomain.CurrentDomain.BaseDirectory + fileName) |> ignore
            System.Diagnostics.Process.Start(Settings.Printcommand, Settings.PrinterSelector + x + " " + Directory.GetCurrentDirectory() + "/" + fileName) |> ignore
            File.Copy(fileName,x+fileName.Replace(".txt",".ok"),true)
        )
        let _ = archiveOrderByUserId orderId 
        Redirection.found (Path.Orders.seeDoneOrders)
    with
    | ex ->
        log.Error("Error in printWholeOrderReceipt", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

type PaymentItemsLiquidValues = {
    wrappedSubOrder: DbWrappedEntities.SubOrderWrapped
    wrappedOrderItems: DbWrappedEntities.OrderItemDetailsWrapped list
    table: string
    wrappedPaymentItems: DbWrappedEntities.PaymentItemWrapped list;
    tenderCodes: IndexNameRecord list
    residualPaymentDue: decimal
    residualPaymentAsString: string
    orderId: int
    subOrderId:int
}

let subOrderPaymentItems subOrderId orderId  =
    log.Debug(sprintf "%s %d" "subOrderPaymentItems" subOrderId )
    let ctx = Db.getContext()
    warbler (fun x -> 

        let _ = 
            match (x.request.queryParam("tenderid"),x.request.queryParam("amount")) with 
            | (Choice1Of2 tenderid,Choice1Of2 amount) -> Db.addPaymentItemToSubOrder subOrderId ((int)tenderid) ((decimal)amount) ctx
            | _ -> ()

        let _ = 
            match (x.request.queryParam("adjustment")) with
            | (Choice1Of2 adjustment) -> 
                Db.Orders.setAdjustmentOfSubOrder ((int)subOrderId) ((decimal)adjustment) ctx
                Db.Orders.setPercentAdjustmentOfSubOrder ((int)subOrderId) 0M ctx
            | _ -> ()

        let _ = 
            match (x.request.queryParam("percentadjustment")) with
            | (Choice1Of2 percentadjustment) ->
                Db.Orders.setPercentAdjustmentOfSubOrder ((int)subOrderId) ((decimal)percentadjustment) ctx
                Db.Orders.setAdjustmentOfSubOrder ((int)subOrderId) 0M ctx;
            | _ -> ()

        let table = Db.Orders.getTableOfOrder orderId ctx
        let subOrder = Db.Orders.getSubOrder subOrderId ctx

        let subOrderItemDetails = Db.Orders.getOrderItemDetailsOfSubOrder subOrderId ctx
        let wrappedOrderItemDetails = subOrderItemDetails |>  Seq.map (fun x -> DbWrappedEntities.DbObjectWrapper.WrapOrderItemDetails(x) "") |> Seq.toList

        let tenderCodes = Db.getAllTenderCodes ctx
        let tenderCodesIndexNameList = tenderCodes |> List.map (fun (x:Db.TenderCode) -> {index=x.Tendercodesid; name=x.Tendername})
        let paymentItems = Db.Orders.getPaymentItemsOfSubOrder subOrderId ctx
        let paymentItemDetails = Db.Orders.getPaymentItemDetailsOfSubOrder subOrderId ctx
        let adjustment = if (subOrder.Subtotaladjustment <> 0M) then subOrder.Subtotaladjustment else Math.Round ((subOrder.Subtotal*(subOrder.Subtotalpercentadjustment/100M)),2,MidpointRounding.AwayFromZero)

        let residual = subOrder.Subtotal + adjustment - ((paymentItemDetails |> List.map (fun (x:Db.PaymentItemDetail) -> x.Amount ) |> List.fold (+) 0.0M))

        let wrappedPaymentItems = paymentItemDetails |> List.map (fun (x:Db.PaymentItemDetail) -> DbWrappedEntities.DbObjectWrapper.WrapPaymentItem(x))
        let wrappedSubOrder = DbWrappedEntities.DbObjectWrapper.WrapSubOrder(subOrder) ""
        let liquidModel = 
            {
                wrappedSubOrder = wrappedSubOrder
                wrappedOrderItems= wrappedOrderItemDetails
                table=table
                wrappedPaymentItems = wrappedPaymentItems
                tenderCodes = tenderCodesIndexNameList
                residualPaymentDue=residual
                residualPaymentAsString = residual |> string
                orderId = orderId
                subOrderId=subOrderId
            }
        DotLiquid.page("subOrderPaymentItem.html") liquidModel
    )

let removeAllDiscountOfSubOrder subOrderId =
    let ctx = Db.getContext()
    try
        let subOrder = Db.Orders.getSubOrder subOrderId ctx
        let _ = subOrder.Subtotaladjustment <- 0M
        let _ = subOrder.Subtotalpercentadjustment <-0M
        ctx.SubmitUpdates()
        Redirection.found (sprintf Path.Orders.subOrderPaymentItems subOrderId subOrder.Orderid)
    with
    | ex ->
        log.Error("Error in removeAllDiscountOfSubOrder", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

type PaymentItemsLiquidValuesForOrder = 
    {
        wrappedOrder: DbWrappedEntities.OrderWrapped
        wrappedOrderItems: DbWrappedEntities.OrderItemDetailsWrapped list
        table: string
        wrappedPaymentItems: DbWrappedEntities.PaymentItemWrapped list
        tenderCodes: IndexNameRecord list
        residualPaymentDue: decimal
        residualPaymentDueAsString: string
        orderId: int
    }

let wholeOrderPaymentItems  orderId  =
    log.Debug(sprintf "%s %d " "wholeOrderPaymentItems" orderId )
    let ctx = Db.getContext()
    warbler (fun x -> 
        let _ = 
            match (x.request.queryParam("tenderid"),x.request.queryParam("amount")) with 
            | (Choice1Of2 tenderid,Choice1Of2 amount) -> Db.addPaymentItemToOrder orderId ((int)tenderid) ((decimal)amount) ctx
            | _ -> ()
        let orderItemsDetails = Db.getOrderItemDetailsOfOrder orderId ctx
        let wrappedOrderItemsDetails = orderItemsDetails |> Seq.map (fun x -> DbWrappedEntities.DbObjectWrapper.WrapOrderItemDetails(x) "") |> Seq.toList;
        let order = Db.Orders.getOrder orderId ctx
        let tenderCodes = Db.getAllTenderCodes ctx
        let tenderCodesIndexNameList = tenderCodes |> List.map (fun (x:Db.TenderCode) -> {index=x.Tendercodesid; name=x.Tendername})
        let paymentItems = Db.Orders.getPaymentItemsOfOrder orderId ctx
        let paymentItemDetails = Db.Orders.getPaymentItemDetailsOfOrder orderId ctx
        let residual = order.Adjustedtotal - ((paymentItemDetails |> List.map (fun (x:Db.PaymentItemDetail) -> x.Amount ) |> List.fold (+) 0.0M))
        let wrappedPaymentItems = paymentItemDetails |> List.map (fun (x:Db.PaymentItemDetail) -> DbWrappedEntities.DbObjectWrapper.WrapPaymentItem(x))
        let wrappedOrder = DbWrappedEntities.DbObjectWrapper.WrapOrder(order) 
        let liquidModel = {
            wrappedOrder = wrappedOrder
            table = order.Table
            wrappedOrderItems = wrappedOrderItemsDetails; 
            wrappedPaymentItems = wrappedPaymentItems; 
            tenderCodes = tenderCodesIndexNameList
            residualPaymentDue = residual
            residualPaymentDueAsString = residual |> string
            orderId = orderId
        }
        DotLiquid.page("wholeOrderPaymentItem.html") liquidModel
    )

let removePaymentItemOfSubOrder paymentItemId subOrderId orderId   =
    let ctx = Db.getContext()
    try
        Db.Orders.removePaymentItem paymentItemId ctx
        Redirection.found (sprintf Path.Orders.subOrderPaymentItems subOrderId orderId)
    with
    | ex ->
        log.Error("Error in removePaymentItemOfSubOrder", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

let removePaymentItemOfOrder paymentItemId  orderId   =
    let ctx = Db.getContext()
    try
        Db.Orders.removePaymentItem paymentItemId ctx
        Redirection.found (sprintf Path.Orders.wholeOrderPaymentItems  orderId)
    with
    | ex ->
        log.Error("Error in removePaymentItemOfOrder", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

let setSubOrderAsPaid subOrderId orderId (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d " "setSubOrderAsPaid" subOrderId)
    let ctx = Db.getContext()
    try
        let subOrder = Db.Orders.getSubOrder subOrderId ctx
        let _ = subOrder.Payed <- true
        ctx.SubmitUpdates()
        Redirection.found (sprintf Path.Orders.subdivideDoneOrder orderId)
    with
    | ex ->
        log.Error("Error in setSubOrderAsPaid", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

let printReceipt subOrderId orderId =
    log.Debug(sprintf "%s %d %d" "printReceipt" subOrderId orderId )
    let ctx = Db.getContext()
    try
        let subOrder = Db.Orders.getSubOrder subOrderId ctx
        let printerForReceipts = Db.getPrintersForReceipts ctx
        let printerNames = printerForReceipts |> List.map (fun (x:Db.Printer) -> x.Name)
        let orderItemOfSubOrder = Db.getOrderItemDetailsOfSubOrderThatAreNotInInitState subOrder.Suborderid ctx
        let total = orderItemOfSubOrder |> List.fold (fun accumul (x:Db.OrderItemDetails) -> accumul + (x.Price)) ((decimal)0.0)
        let text = Utils.textForSubOrderReceipt orderItemOfSubOrder ctx
        let fileName = sprintf "receipt_print%d.txt" System.DateTime.Now.Ticks
        let outFile = new System.IO.StreamWriter(fileName,true,Encoding.UTF8)
        let _ = outFile.WriteLine(text)
        let _ = outFile.Close() |> ignore
        let _ = printerNames |> List.iter (fun x -> 
            System.Diagnostics.Process.Start(Settings.Printcommand, Settings.PrinterSelector + x + " " + AppDomain.CurrentDomain.BaseDirectory + fileName) |> ignore
            File.Copy(fileName,x+fileName.Replace(".txt",".ok"),true)
        )
        subOrder.Payed <- true
        ctx.SubmitUpdates()
        Redirection.found (sprintf Path.Orders.subdivideDoneOrder orderId)
    with
    | ex ->
        log.Error("Error in printReceipt", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

let setSubOrderAsNotPaid subOrderId orderId (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d %d"  "setSubOrderAsNotPaid" subOrderId orderId)
    let ctx = Db.getContext()
    let subOrder = Db.Orders.getSubOrder subOrderId ctx
    let _ = subOrder.Payed <- false
    ctx.SubmitUpdates()
    Redirection.found (sprintf Path.Orders.subdivideDoneOrder orderId)

let deleteSubOrder subOrderId orderId (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d %d" "deleteSubOrder" subOrderId orderId)
    try
        let ctx = Db.getContext()
        Db.safeDeleteSubOrder subOrderId ctx
        Redirection.found (sprintf Path.Orders.subdivideDoneOrder orderId)
    with
    | ex ->
        log.Error("Error in deleteSubOrder", ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

type LiquidWrappedOrderItems = {orderitemdetailswrapped: OrderItemDetailsWrapped list; 
    suborderwrapped: SubOrderWrapped list; orderid: int; table: string; encodedbackurl: string; tendercodes: TenderCodeWrapped list }

type LiquidWrapperOrderItemsWithGenericCourses = { orderitemdetailswrapped: OrderItemDetailsWrapped list; 
    suborderwrapped: SubOrderWrapped list; orderid: int; coursewrappedlist: CourseWrapped list; encodedbackurl: string; }

let recomputeSubOrderTotal (subOrder:Db.SubOrder) =
    log.Debug(sprintf "recomputeSubOrderTotal %d" subOrder.Suborderid)
    let ctx = Db.getContext()
    let orderItemsSOfSuborder = Db.Orders.getOrderItemsOfSubOrder subOrder.Suborderid ctx
    log.Debug(sprintf "list length %d" (List.length orderItemsSOfSuborder))
    let total = orderItemsSOfSuborder |> List.map (fun (x:Db.OrderItem) -> x.Price) |> List.sum
    log.Debug(sprintf "total %.2f" total)
    subOrder.Subtotal <- total
    ctx.SubmitUpdates()

let colapseDoneOrder id (user: UserLoggedOnSession) =
    log.Debug(printf "%s %d" "colapseDoneOrder" id)
    let ctx = Db.getContext()
    choose [
        GET >=> warbler ( fun (x:HttpContext) ->
            let abstractCourses = Db.Courses.getAllAbstractCourses ctx
            let abstractCoursesWrapped = abstractCourses |> List.map (fun x -> DbObjectWrapper.WrapCourse x)
            let idsOfNonUnitaryOrderItems = Db.getIdsOfNonUnitaryOrderItemsOfOrder id ctx
            let _ = idsOfNonUnitaryOrderItems |> Seq.iter (fun x -> Db.splitOrderItemInToUnitaryOrderItems x ctx)
            let order = Db.getOrderDetail id ctx
            let orderItemsdetailsOfOrder = Db.getOrderItemDetailOfOrderDetailThatAreNotInInitState order ctx
            let subOrders = 
                Db.Orders.getSubOrdersOfOrderById id ctx
            let _ = subOrders |> List.iter (fun (x:Db.SubOrder) -> recomputeSubOrderTotal x )
            let colors = Globals.getFirstNColorValues (List.length subOrders)
            let wrappedSubOrders =  List.map2 (fun (x:Db.SubOrder) y -> DbObjectWrapper.WrapSubOrder x y) subOrders colors
            let subOdersColorsMap = wrappedSubOrders |> List.map (fun (x:SubOrderWrapped) -> (x.Suborderid,x.Csscolor)) |> Map.ofList
            let wrappedOrderItemDetails = 
                List.map (fun (x:Db.OrderItemDetails) -> DbObjectWrapper.WrapOrderItemDetails x (if (Db.Orders.orderItemIsInASubOrder x.Orderitemid ctx) then subOdersColorsMap.[x.Suborderid] else "#dee7ed")) orderItemsdetailsOfOrder 
            let parametersNames = 
                wrappedOrderItemDetails |> 
                List.map (fun (x:OrderItemDetailsWrapped) -> "orderitem"+(x.Orderitemid|> string))
            let parametersFromRequest = 
                parametersNames |> 
                List.filter (fun z -> match (x.request.queryParam(z)) with | Choice1Of2 _ -> true | _ -> false) |>
                    List.map (fun z -> z.Substring("orderitem".Length)) |> List.map (fun z -> (int) z)
            let abstractCurseId = match x.request.queryParam("piatto") with | Choice1Of2 x -> x | _ -> "99"
            log.Debug(sprintf "%s %s" "piatto" abstractCurseId)
            if (parametersFromRequest.Length>0) then (
                let subGroup = Db.unBoundDifferentSubGroupsOfOrderItemsByIs parametersFromRequest ctx
                parametersFromRequest |> List.iter (fun z -> Db.safeRemoveOrderItem z ctx)
                Db.createPlainUnitaryOrderItemById id (abstractCurseId|>int) subGroup ctx
                Redirection.found (sprintf Path.Orders.colapseDoneOrder id)
            ) else 
                let liquidItem = {orderitemdetailswrapped = wrappedOrderItemDetails; suborderwrapped = wrappedSubOrders; orderid=id; encodedbackurl = WebUtility.UrlEncode (sprintf Path.Orders.subdivideDoneOrder id); coursewrappedlist= abstractCoursesWrapped }
                DotLiquid.page("colapseDoneOrder.html") liquidItem
        )
    ]

let subdivideDoneOrderRef id (user: UserLoggedOnSession)  = 
    log.Debug(sprintf "%s %d" "subdivideDoneOrderRef" id)
    let ctx = Db.getContext()
    choose [
        GET >=> warbler ( fun (x:HttpContext) ->
            let idsOfNonUnitaryOrderItems = Db.getIdsOfNonUnitaryOrderItemsOfOrder id ctx
            let _ = idsOfNonUnitaryOrderItems |> Seq.iter (fun x -> Db.splitOrderItemInToUnitaryOrderItems x ctx)
            let order = Db.getOrderDetail id ctx
            let orderItemsdetailsOfOrder = Db.getOrderItemDetailOfOrderDetailThatAreNotInInitState order ctx
            let dbTenderCodes = Db.getAllTenderCodes ctx
            let wrappedTenderCodes = dbTenderCodes |> List.map (fun (x:Db.TenderCode) -> DbObjectWrapper.WrapTenderCode x)
            let subOrders = 
                Db.Orders.getSubOrdersOfOrderById id ctx
            log.Debug("subtotals:") 
            let _ = subOrders |> List.iter (fun x -> log.Debug(x.Subtotal))
            let _ = subOrders |> List.iter (fun (x:Db.SubOrder) -> recomputeSubOrderTotal x )
            let colors = Globals.getFirstNColorValues (List.length subOrders)
            let wrappedSubOrders =  List.map2 (fun (x:Db.SubOrder) y -> DbObjectWrapper.WrapSubOrder x y) subOrders colors
            let subOdersColorsMap = wrappedSubOrders |> List.map (fun (x:SubOrderWrapped) -> (x.Suborderid,x.Csscolor)) |> Map.ofList
            let wrappedOrderItemDetails = 
                List.map (fun (x:Db.OrderItemDetails) -> DbObjectWrapper.WrapOrderItemDetails x (if (Db.Orders.orderItemIsInASubOrder x.Orderitemid ctx) then subOdersColorsMap.[x.Suborderid] else "#dee7ed")) orderItemsdetailsOfOrder 
            let parametersNames = 
                wrappedOrderItemDetails |> 
                List.map (fun (x:OrderItemDetailsWrapped) -> "orderitem"+(x.Orderitemid|> string))
            let parametersFromRequest = 
                parametersNames |> 
                List.filter (fun z -> match (x.request.queryParam(z)) with | Choice1Of2 _ -> true | _ -> false) |>
                    List.map (fun z -> z.Substring("orderitem".Length)) |> List.map (fun z -> (int) z)
            try
                if (parametersFromRequest.Length>0) then 
                        let subOrder = Db.Orders.createSubOrderOfOrder id ctx
                        parametersFromRequest |> List.iter (fun z ->  Db.bindOrderItemToSubOrder z subOrder.Suborderid ctx)
                        Redirection.found (sprintf Path.Orders.subdivideDoneOrder id
                ) else 
                    let liquidItem = {orderitemdetailswrapped = wrappedOrderItemDetails; suborderwrapped = wrappedSubOrders; orderid=id; table=order.Table; encodedbackurl = WebUtility.UrlEncode (sprintf Path.Orders.subdivideDoneOrder id);tendercodes =  wrappedTenderCodes}
                    DotLiquid.page("subdivideDoneOrder.html") liquidItem
            with
            | ex -> 
                log.Error(sprintf "%A" ex)
                Redirection.found Path.Errors.unableToCompleteOperation 
        )
    ]

let splitOrderItemInToUnitaryOrderItems id (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d " "splitOrderItemInToUnitaryOrderItems X" id)
    let ctx = Db.getContext()
    let theOrderItem = Db.Orders.getTheOrderItemById id ctx
    let connectedOrderItemStates = theOrderItem.``public.orderitemstates by orderitemid`` |> Seq.toList
    connectedOrderItemStates |> List.iter (fun x -> log.Debug(sprintf "X - orderitemstate %A\n" x))

    let ingredientdecrements = theOrderItem.``public.ingredientdecrement by orderitemid`` |> Seq.toList
    let rejectOrderItems = theOrderItem.``public.rejectedorderitems by orderitemid`` |> Seq.toList
    let variations = theOrderItem.``public.variations by orderitemid`` |> Seq.toList
    let courseId = theOrderItem.Courseid
    let orderId = theOrderItem.Orderid
    let comment = theOrderItem.Comment
    let outGroupId = theOrderItem.Ordergroupid
    let clonedOrderItems = 
        [1 .. theOrderItem.Quantity] 
        |> List.map (fun _ -> Db.createUnitaryNakedOrderItemByOrderId courseId orderId comment theOrderItem.Price outGroupId ctx) 
    let clonedIngredientDecrements = 
        clonedOrderItems  
        |> List.map (fun (x:Db.OrderItem) -> 
            ingredientdecrements 
            |> List.map (fun (y:Db.IngredientDecrement) -> 
                Db.createClonedIngredientDecrement x.Orderitemid ((decimal) theOrderItem.Quantity)  y ctx)) 
                |> List.fold (@) []
    let clonedVariations = 
        clonedOrderItems 
        |> List.map (fun (x:Db.OrderItem) -> 
            variations 
            |> List.map  (fun (y:Db.Variation) -> 
                Db.createClonedVariationOfOrderItem x.Orderitemid y.Ingredientid y.Tipovariazione ctx)) 
                |> List.fold (@) []
    let clonedRejectedOrderItems = 
        clonedOrderItems 
        |> List.map (fun (x:Db.OrderItem) -> 
            rejectOrderItems 
            |> List.map (fun (y:Db.RejectedOrderItems) ->
                Db.createClonedRejectedOrderItem x.Orderitemid y ctx )) 
                |> List.fold (@) []
    let clonedOrderItemStates = 
        clonedOrderItems 
        |> List.map (fun (x:Db.OrderItem) -> 
            connectedOrderItemStates 
            |> List.map (fun (y:Db.OrderItemState) -> Db.createClonedOrderItemState x.Orderitemid y ctx)) 
            |> List.fold (@) []
    
    let _ = Db.safeRemoveOrderItem theOrderItem.Orderitemid ctx
    Redirection.FOUND (sprintf Path.Orders.subdivideDoneOrder theOrderItem.Orderid)
            
let editDoneOrder id (user:UserLoggedOnSession) =
    let thisUrl = sprintf Path.Orders.editDoneOrder id
    log.Debug(sprintf "%s %d" "editDoneOrder" id )
    let ctx = Db.getContext()
    let dbUser = Db.getUserById user.UserId ctx
    match (dbUser.Canmanageallorders || user.Role = "admin") with
    | true ->
        choose [
            GET >=> warbler (fun _ ->
                let order = Db.Orders.getOrder id ctx
                let orderItemsdetailsOfOrder = Db.getOrderItemDetailOfOrder order ctx
                html (View.seeOrder order orderItemsdetailsOfOrder thisUrl) 
            )

            POST >=> bindToForm Form.priceAdjustment (fun form -> 
                try
                    let order = Db.Orders.getOrder id ctx

                    let setPercentageOrPlainPriceVariation =
                        match (form.PercentOrValue,form.Value) with 
                        | (_,X)  when (Decimal.Parse(X)  = (decimal)0) ->
                            order.Adjustispercentage <- false
                            order.Adjustisplain <- false
                            order.Percentagevariataion <- (decimal)0
                            order.Plaintotalvariation <- (decimal)0
                            order.Adjustedtotal <- order.Total
                        | ("PERCENTUALE",_) -> 
                            order.Adjustispercentage <- true
                            order.Adjustisplain<- false 
                            order.Plaintotalvariation <- (decimal) 0 
                            order.Percentagevariataion <- Decimal.Parse(form.Value)
                            order.Adjustedtotal <- order.Total + order.Total * order.Percentagevariataion/(decimal)100
                        | ("VALORE",_) -> 
                            order.Adjustispercentage <- false
                            order.Adjustisplain <- true
                            order.Plaintotalvariation <-  Decimal.Parse(form.Value)
                            order.Percentagevariataion <- (decimal)0
                            order.Adjustedtotal <- order.Total + (Decimal.Parse(form.Value))
                        | _ -> 
                            order.Adjustispercentage <- false
                            order.Adjustisplain <- false
                    ctx.SubmitUpdates()
                    let redirTo = Path.Orders.editDoneOrder.Value
                    let redirToTrimmed = redirTo.Substring(0,redirTo.IndexOf("%"))
                    Redirection.FOUND (redirToTrimmed+(id |> string))
                with
                | ex -> 
                    log.Error(sprintf "%A" ex)
                    Redirection.found Path.Errors.unableToCompleteOperation 
            )
        ]
    | _ -> Redirection.FOUND Path.home

let optimizeVoided = warbler (fun _ ->
    let ctx = Db.getContext()
    let voidedOrders = Db.Orders.getVoidedOrders ctx
    let _ = List.iter (fun (x:Db.Order) -> Db.safeRemoveOrder x.Orderid ctx ) voidedOrders
    ctx.SubmitUpdates()
    Redirection.FOUND Path.home
)

let deArchiveLatestOrder (user:UserLoggedOnSession) =
    log.Debug("deArchiveLatestOrder")
    let ctx = Db.getContext()
    let dbUser = Db.getUserById user.UserId ctx
    let _ = 
        try
            if (user.Role = "admin " || dbUser.Canmanageallorders) then
                let latestLogOrder = Db.getLatestLogOrder ctx
                match latestLogOrder with
                | Some theLatestOrder ->
                    log.Debug("got an order");
                    let order = Db.Orders.getOrder theLatestOrder.Orderid  ctx
                    order.Archived <- false
                    theLatestOrder.Delete()
                    ctx.SubmitUpdates()
                | _ -> 
                    log.Debug("got nothing")
                    ()
        with
        | ex -> 
            log.Error(sprintf "%A" ex)
    Redirection.found Path.Orders.seeDoneOrders

let switchCourseCategoryVisibility categoryId  =
    log.Debug(sprintf "%s %d\n" "switchCourseCategoryVisibility" categoryId )
    let ctx = Db.getContext()
    let courseCategory = Db.Courses.tryGetCourseCategory categoryId ctx
    let _ = 
        match courseCategory with
        | Some X -> X.Visibile <- not X.Visibile; ctx.SubmitUpdates()
        | None -> ()
    Redirection.found Path.Courses.adminCategories

let rejectOrderItem orderItemId (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d \n" "rejectOrderItem" orderItemId )
    let ctx = Db.getContext()
    let theOrderItemDetail = Db.Orders.getOrderItemDetail orderItemId ctx
    let roleId = user.RoleId
    let stateId = theOrderItemDetail.Stateid
    let categoryId = theOrderItemDetail.Categoryid

    match Db.isEnablerRoleCatState roleId categoryId stateId ctx with
    | true ->
        choose [
            GET >=> warbler (fun x ->
                View.rejectOrderItem theOrderItemDetail |> html
            )
            POST >=> bindToForm Form.orderItemRejection (fun form ->
                try
                    let _ = Db.createRejectedOrderItem theOrderItemDetail.Orderitemid theOrderItemDetail.Courseid form.Motivation ctx
                    let _ = Db.setOrderItemAsRejected theOrderItemDetail.Orderitemid ctx
                    let _ = Db.reinitializeOrderItemState theOrderItemDetail.Orderitemid ctx
                    Redirection.FOUND Path.Orders.orderItemsProgress
                with
                | ex -> 
                    log.Error(sprintf "%A" ex)
                    Redirection.FOUND Path.Errors.unableToCompleteOperation
            )
        ]
    | false -> Redirection.FOUND Path.Orders.orderItemsProgress

let standardComments = 
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ ->
            let comments = Db.getAllStandardComments ctx
            View.standardComments comments |> html
        )
        POST >=> bindToForm Form.comment (fun form ->
            try
                let _ = Db.addStandardComment form.Comment ctx
                Redirection.FOUND Path.Admin.standardComments
            with
            | ex -> 
                log.Error(sprintf "%A" ex)
                Redirection.FOUND Path.Errors.unableToCompleteOperation
        )
    ]

let removeStandardComment id =
    let ctx = Db.getContext()
    let _ = Db.removeStandardComment id ctx
    Redirection.FOUND Path.Admin.standardComments

let resetDiscount orderId = 
    log.Debug(sprintf "%s %d\n" "resetDiscount" orderId)
    let ctx = Db.getContext()
    let order = Db.Orders.getOrder orderId ctx
    order.Adjustispercentage <- false
    order.Adjustisplain <- false
    order.Percentagevariataion <- 0.0M
    order.Plaintotalvariation <- 0.0M
    let orderItems = order.``public.orderitems by orderid``
    let recoveredTotal = orderItems  |> Seq.map (fun (x:Db.OrderItem) -> x.Price) |> Seq.fold (+) (decimal 0.0)
    order.Adjustedtotal <- recoveredTotal
    ctx.SubmitUpdates()
    Redirection.FOUND Path.Orders.seeDoneOrders

let unVoidLatestVoidedRef backUrl   =
    loggedOn (session (function 
        | UserLoggedOn user ->
            log.Debug(sprintf "%s\n" "unVoidLatestVoided")
            let ctx = Db.getContext()
            let myLatestVoidedOrder = Db.getLatestVoidedOrder user.UserId ctx
            try
                match myLatestVoidedOrder with
                | Some theLatestVoideOrderByMe ->
                    let order = Db.Orders.getOrder theLatestVoideOrderByMe.Orderid ctx
                    order.Voided <- false
                    theLatestVoideOrderByMe.Delete()
                    ctx.SubmitUpdates()
                | _ -> ()
                Redirection.found (WebUtility.UrlDecode backUrl)
            with
            | ex -> 
                log.Error(sprintf "%A" ex)
                Redirection.FOUND Path.Errors.unableToCompleteOperation
        | _ -> UNAUTHORIZED "NOT logged on"
    ))

type Mymodel = {content: string; table: string}


let qrUserImageGenRef  = 
    log.Debug("qrUserImageGenRef")
    warbler (fun x -> 
        let urlToCode = match x.request.queryParam("qrUserLoginUrl") with | Choice1Of2 par -> System.Net.WebUtility.UrlDecode par | _ -> "error"
        let table = match x.request.queryParam("table") with | Choice1Of2 par -> par | _  -> "" 
        let qrGenerator = new QRCoder.QRCodeGenerator();
        let qrCodeData  = qrGenerator.CreateQrCode(urlToCode,QRCodeGenerator.ECCLevel.Q)

        let qrCode = new PngByteQRCode(qrCodeData);

        let qrCodeImage = qrCode.GetGraphic(20)

        let encoded = System.Convert.ToBase64String (qrCodeImage)
        let o = {content=encoded; table=table}
        DotLiquid.page("qrCode.html") o
    ) 

let removeStandardCommentForCourse commentForCourseId =
    let ctx = Db.getContext()
    let standardComment = Db.getStandardCommentForCourse commentForCourseId ctx
    standardComment.Delete()
    ctx.SubmitUpdates()
    let courseId = standardComment.Courseid
    Redirection.FOUND (sprintf Path.Admin.standardCommentsForCourse courseId)

let standardCommentsForCourse courseId =
    let ctx = Db.getContext()
    let course = Db.Courses.getCourse courseId ctx
    choose [
        GET >=> warbler (fun _ -> 
                let commentsForCourseDetails = Db.getCommentsForCourseDetails courseId ctx
                let allStandardComments = Db.getAllStandardComments ctx
                let existingCommentsForCourseIds = commentsForCourseDetails |> List. map (fun x -> x.Standardcommentid)
                let selectableStandardComments = allStandardComments  |> (List.filter (fun x -> (not (List.contains x.Standardcommentid existingCommentsForCourseIds))))
                View.standardCommentsForCourse course commentsForCourseDetails selectableStandardComments  |> html
            )
        POST >=> bindToForm Form.commentForCourse (fun form ->
            try
                let _ = Db.addCommentForCourse ((int)form.CommentForCourse) courseId ctx

                Redirection.FOUND (sprintf Path.Admin.standardCommentsForCourse courseId)
            with
            | ex -> 
                log.Error(sprintf "%A" ex)
                Redirection.FOUND Path.Errors.unableToCompleteOperation
        )
    ]

let standardVariationsForCourse courseId =
    let ctx = Db.getContext()
    let course = Db.Courses.getCourse courseId ctx
    choose [
        GET >=> warbler (fun _ ->
            let standardVariationsForCourseDetails = Db.StandardVariations.getStandardVariationsForCourseDetails courseId ctx
            let allStandardVariations = Db.StandardVariations.getAllStandardVariations ctx
            let existingVariationForCourseIds = standardVariationsForCourseDetails |> List.map (fun x -> x.Standardvariationid)
            let selectableStandardVariations = allStandardVariations |> (List.filter (fun x -> (not (List.contains x.Standardvariationid existingVariationForCourseIds) )))
            View.standardVariationsForCourse course standardVariationsForCourseDetails selectableStandardVariations |> html
        )
        POST >=> bindToForm Form.variationForCourse (fun form ->
            try
                let _ = Db.StandardVariations.addStandardVariationForCourse ((int )form.VariationForCourse) courseId ctx
                Redirection.FOUND (sprintf Path.Admin.standardVariationsForCourse courseId)
            with
            | ex -> 
                log.Error(sprintf "%A" ex)
                Redirection.FOUND Path.Errors.unableToCompleteOperation
        )
    ]

let stateGroupIdentifierMappingForImportedOrderItems (orderItems: OrderItemDetailsWrapped list) =
    let ctx = Db.getContext()
    let differentStates  = orderItems |> List.map (fun (x:OrderItemDetailsWrapped) -> x.Stateid)
    let setOfDifferentStates = differentStates |> Set.ofList
    let n = Set.count setOfDifferentStates 
    let availableGroupIdentifiers = Db.getFirstNAvailableOutGroupIdentifier n ctx
    let stateGroupMapping = List.zip (setOfDifferentStates |> Set.toList) availableGroupIdentifiers |> Map.ofList
    stateGroupMapping

let selectStandardCommentsForOrderItem orderItemId =
    let ctx = Db.getContext()
    let orderItemDetail = Db.Orders.getOrderItemDetail orderItemId ctx
    let standardCommentsForThisCourse = Db.getCommentsForCourseDetails orderItemDetail.Courseid ctx
    let standardVariationsForThisCourse = Db.StandardVariations.getStandardVariationsForCourseDetails orderItemDetail.Courseid ctx
    match (List.length standardCommentsForThisCourse,List.length standardVariationsForThisCourse) with 
        | (0,0) -> Redirection.FOUND (sprintf Path.Orders.viewOrder orderItemDetail.Orderid)
        | (X,_) when X>0 ->  View.selectStandardCommentsAndVariationsForOrderItem orderItemDetail standardCommentsForThisCourse standardVariationsForThisCourse |> html
        | _ -> Redirection.FOUND (sprintf Path.Orders.editOrderItemVariation orderItemId)

let addStandardCommentToOrderItem commentId orderItemId  =
    let ctx = Db.getContext()
    let comment = Db.getStandardComment commentId ctx
    Db.addCommentToOrderItemById comment.Comment orderItemId ctx
    Redirection.FOUND (sprintf Path.Orders.selectStandardCommentsAndVariationsForOrderItem orderItemId)

let addStandardVariationToOrderItem standardVariationId orderItemId =
    let ctx = Db.getContext()
    Db.StandardVariations.setStandardVariationToOrderItem standardVariationId orderItemId ctx
    let _ = adjustPriceOfOrderItemByVariations orderItemId
    let viewOrderUrl = WebUtility.UrlEncode (sprintf Path.Orders.viewOrder orderItemId) 
    Redirection.FOUND (sprintf Path.Orders.editOrderItemVariation orderItemId )

let removeExistingCommentToOrderItem id =
    let ctx = Db.getContext()
    let _ = Db.removeExistingCommentToOrderItem id ctx
    Redirection.FOUND (sprintf Path.Orders.selectStandardCommentsAndVariationsForOrderItem id)

let makeSubCourseCategory id =
    let ctx = Db.getContext()
    let courseCategory = Db.Courses.getCourseCategory id ctx
    choose [
        GET >=> warbler (fun _ ->
            View.makeSubCourseCategory courseCategory "" |> html
        )
        POST >=> bindToForm Form.subCourseCategory (fun form ->
            let visibility = (form.Visibility = Form.VISIBLE)
            match (Db.Courses.tryFindCategoryByName form.Name ctx) with
                | Some X when X.Categoryid <> courseCategory.Categoryid -> View.makeSubCourseCategory courseCategory "esiste gia'" |> html
                | _ -> 
                    try
                        let sonCategory = Db.createSubCourseCategory form.Name  visibility id ctx
                        Redirection.FOUND (sprintf Path.Courses.manageAllCoursesOfACategoryPaginated sonCategory.Categoryid 0)
                    with
                    | ex -> 
                        log.Error(sprintf "%A" ex)
                        Redirection.FOUND Path.Errors.unableToCompleteOperation
        )
    ]

let mergeSubCourseCategoryToFather categoryId =
    log.Debug("mergeSubCourseCategoryToFather")
    let ctx = Db.getContext()
    let father = Db.getCourseCategoryFather categoryId ctx
    let _ = Db.Courses.moveCoursesCategories categoryId father.Categoryid ctx
    let _ = Db.unlinkSonFromFather categoryId father.Categoryid ctx
    let _ = Db.safeDeleteCourseCategory categoryId ctx
    Redirection.FOUND (sprintf Path.Courses.manageAllCoursesOfACategoryPaginated father.Categoryid 0)
    
let manageStandardVariation id =
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ ->
            let standardVariation = Db.getStandardVariation id ctx
            let ingredientCategories = Db.getAllIngredientCategories ctx
            let variationItemDetails = Db.StandardVariations.getStandardVariationItemDetails id ctx
            let (allIngredients:Db.Ingredient list) = Db.getAllIngredients ctx

            let specificCustomAddQuantitiesForIngredients = allIngredients |> List.map (fun (x:Db.Ingredient) -> (x.Ingredientid,Db.getIngredientPrices x.Ingredientid ctx)) |> Map.ofList
            View.manageStandardVariation standardVariation variationItemDetails ingredientCategories allIngredients specificCustomAddQuantitiesForIngredients  |> html
        )

        POST >=> bindToForm Form.ingredientVariation (fun form -> 
            try
                match form.Quantity with
                | Globals.SENZA -> Db.StandardVariations.addRemoveIngredientStandardVariationItem id ((int) form.IngredientBySelect) ctx
                | _ -> Db.StandardVariations.addAddIngredientStandardVariationItem id ((int)form.IngredientBySelect) form.Quantity ctx
                Redirection.FOUND (sprintf Path.Admin.manageStandardVariation id )
            with
            | ex -> 
                log.Error(sprintf "%A" ex)
                Redirection.FOUND Path.Errors.unableToCompleteOperation
        )
    ]

let manageStandardVariationByIngredientCategory standardVariationId ingredientCategoryId =
    choose [
        GET >=> warbler (fun _ ->
            let ctx = Db.getContext()
            let standardVariation = Db.getStandardVariation standardVariationId ctx
            let ingredientCategories = Db.getAllIngredientCategories ctx
            let variationItemDetails = Db.StandardVariations.getStandardVariationItemDetails standardVariationId ctx
            let (ingredients:Db.Ingredient list) = Db.getIngredientsByCategory ingredientCategoryId ctx 
            let specificCustomAddQuantitiesForIngredients = ingredients |> List.map (fun (x:Db.Ingredient) -> (x.Ingredientid,Db.getIngredientPrices x.Ingredientid ctx)) |> Map.ofList
            View.manageStandardVariation standardVariation variationItemDetails ingredientCategories ingredients  specificCustomAddQuantitiesForIngredients |> html
        )
        POST >=> Redirection.FOUND Path.home
    ]

let manageStandardVariations =
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ -> 
            let allStandardVariations = Db.getAllStandardVariations ctx
            View.manageStandardVariations allStandardVariations "" |> html
        )
        POST >=> bindToForm Form.standardVariation  (fun form ->
            try
                let existing = Db.tryGetStandardVariationByName form.Name ctx
                match existing with 
                    | Some X -> 
                        let allStandardVariations = Db.getAllStandardVariations ctx
                        View.manageStandardVariations allStandardVariations "esiste gia'" |> html
                    | None ->
                        let _ = Db.createStandardVariation form.Name ctx 
                        Redirection.FOUND Path.Admin.manageStandardVariations
            with
            | ex -> 
                log.Error(sprintf "%A" ex)
                Redirection.FOUND Path.Errors.unableToCompleteOperation
        )
    ]

let removeStandardVariation variationId =
    log.Debug("removeStandardVariation")
    let ctx = Db.getContext()
    let _ = Db.removeStandardVariation variationId ctx
    Redirection.FOUND Path.Admin.manageStandardVariations

let removeStandardVariationItem id =
    try
        log.Debug(sprintf "removeStandardVariationItem %d " id)
        let ctx = Db.getContext()
        let variationItem = Db.StandardVariations.getStandardVariationItem id ctx
        let standardVariationId = variationItem.Standardvariationid
        variationItem.Delete()
        ctx.SubmitUpdates()
        Redirection.FOUND (sprintf Path.Admin.manageStandardVariation standardVariationId)
    with
    | ex -> 
        log.Error(sprintf "%A" ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

let removeStandardVariationForCourse variationId courseId =
    log.Debug(sprintf "removeStandardVariationForCourse %d %d" variationId courseId)
    try
        let ctx= Db.getContext()
        let _ = Db.removeStandardVariationForCourse variationId ctx
        Redirection.FOUND (sprintf Path.Admin.standardVariationsForCourse courseId)
    with
    | ex -> 
        log.Error(sprintf "%A" ex)
        Redirection.FOUND Path.Errors.unableToCompleteOperation

let selectOrderFromWhichMoveOrderItemsRef targetOrderId  =
    log.Debug("selectOrderFromWhichMoveOrderItemsRef")
    let ctx = Db.getContext() 
    let orders = Db.Orders.getAllUnarchivedOrders ctx
    let wrappedOrders = orders |> List.filter (fun (x:Db.Order) -> x.Orderid <> targetOrderId) |> List.map (fun (x:Db.Order) -> DbWrappedEntities.DbObjectWrapper.WrapOrder x) 
    let targetOrder = Db.Orders.getOrder targetOrderId ctx
    let wrappedTargetOrder = DbWrappedEntities.DbObjectWrapper.WrapOrder targetOrder
    let liquidModel = {orders = wrappedOrders ;  targetOrder = wrappedTargetOrder}

    choose [
        GET >=> warbler (fun (req:HttpContext) ->
            try
                let expectedOrderParameterNames = wrappedOrders |> List.map (fun (x:OrderWrapped) -> "order" + (x.OrderId|> string)) // form: order999 in querystring
                let orderIdsTomerge = expectedOrderParameterNames |> List.filter (fun z -> match (req.request.queryParam(z)) with | Choice1Of2 _ -> true | _ -> false ) |> List.map (fun y -> (y.Substring("order".Length) |> int)) // list of ids of ordersI

                let _ =
                    let idOfOrderItemsToSplit = 
                        orderIdsTomerge 
                        |> List.map 
                            (fun x -> Db.getIdsOfNonUnitaryOrderItemsOfOrder x ctx |> Seq.toList) 
                            |> Seq.fold (@) []
                    idOfOrderItemsToSplit 
                    |> Seq.iter 
                        (fun (x: int) -> Db.splitOrderItemInToUnitaryOrderItems x ctx)
                    ()

                let wrapperOrdersToMerge = wrappedOrders |> List.filter (fun x -> List.contains x.OrderId orderIdsTomerge ) // a wrappedorder format for orders to merge
                let orderAndOrderItems = 
                    wrapperOrdersToMerge |> 
                    List.map (fun (x:OrderWrapped) -> {order=x; orderitems =  (Db.getUnpaidOrderItemDetailOfOrderById x.OrderId ctx) |> List.map (fun (x:Db.OrderItemDetails) -> (DbWrappedEntities.DbObjectWrapper.WrapOrderItemDetailsIncldingVariations x (Db.getVariationDetailsOfOrderItem (x.Orderitemid) ctx) "0000"))})
                let liquidTableMergeModel = { orderandorderitems=orderAndOrderItems; targetorder = wrappedTargetOrder  }
                let expectedOrderItems = orderAndOrderItems |> List.map (fun (x:OrderAndOrderitemslist) -> x.orderitems) |> List.fold (@) []    // |> List.map (fun (y:OrderItemDetailsWrapped) -> y.Orderitemid))) |> List.fold(@) []   I
                let expectedOrderItemsParameterNames = expectedOrderItems |> List.map (fun (x:OrderItemDetailsWrapped) -> "orderitem"+(x.Orderitemid|> string) )
                let orderItemsIdsToMerge = expectedOrderItemsParameterNames |> List.filter (fun z -> match (req.request.queryParam(z)) with | Choice1Of2 _ -> true| _ -> false  ) |> List.map (fun y -> (y.Substring("orderitem".Length) |> int ))

                let wrappedOrderItemsToMerge = expectedOrderItems |> List.filter (fun x -> List.contains x.Orderitemid orderItemsIdsToMerge )
                let stateNewTargetGroupsMapping = stateGroupIdentifierMappingForImportedOrderItems wrappedOrderItemsToMerge 
                match ((List.length wrapperOrdersToMerge),wrappedOrderItemsToMerge) with 
                    | (_,Y) when (List.length Y > 0) -> 
                        let _ = Y |> List.iter (fun (x:OrderItemDetailsWrapped) -> 
                            let outGroup = Db.createOrGetOutGroup targetOrderId (stateNewTargetGroupsMapping.[x.Stateid]) ctx
                            let _ = if (not (Db.isInitialState x.Stateid ctx)) then outGroup.Printcount <- outGroup.Printcount + 1
                            Db.tryMoveOrderItemToAnOutGroupOfAnotherOrder x.Orderitemid outGroup.Ordergroupid ctx )
                        Redirection.found (sprintf Path.Orders.viewOrder targetOrderId)
                    | (X,_) when (X > 0) -> DotLiquid.page("selectOrderItemsToMerge.html") liquidTableMergeModel
                    | _ ->  DotLiquid.page("selectOrderToMerge.html") liquidModel
            with
            | ex -> 
                log.Error(sprintf "%A" ex)
                Redirection.FOUND Path.Errors.unableToCompleteOperation
        )
    ]

let noCache = 
    setHeader "Cache-Control" "no-cache, no-store, must-revalidate"
    >=> setHeader "Pragma" "no-cache"
    >=> setHeader "Expires" "0"

let webPart =
    choose [
        pathScan Path.Admin.standardVariationsForCourse (fun id -> (admin (standardVariationsForCourse id )))
        pathScan Path.Admin.removeStandardVariationItem (fun id -> (admin (removeStandardVariationItem id)))
        pathScan Path.Admin.removeStandardVariationForCourse (fun (variationId,courseId) -> (loggedOn (removeStandardVariationForCourse variationId courseId)))

        pathScan Path.Admin.manageStandardVariationByIngredientCategory (fun (variationId,categoryId) -> (admin (manageStandardVariationByIngredientCategory variationId categoryId)))
        pathScan Path.Admin.manageStandardVariation (fun id -> (admin (manageStandardVariation id)))
        pathScan Path.Admin.removeStandardVariation (fun id -> (admin (removeStandardVariation id)))
        path Path.Admin.manageStandardVariations >=> admin manageStandardVariations
        pathScan Path.Courses.mergeSubCourseCategoryToFather (fun id -> admin (mergeSubCourseCategoryToFather id ))
        pathScan Path.Courses.makeSubCourseCategory (fun id -> admin (makeSubCourseCategory id))
        pathScan Path.Orders.removeExistingCommentToOrderItem (fun id -> loggedOn (removeExistingCommentToOrderItem id))
        pathScan Path.Orders.addStandardCommentToOrderItem (fun (commentId,orderItemId) -> loggedOn (addStandardCommentToOrderItem commentId orderItemId))
        pathScan Path.Orders.addStandardVariationToOrderItem (fun (variationId,orderItemId) -> loggedOn (addStandardVariationToOrderItem variationId orderItemId))
        pathScan Path.Orders.selectStandardCommentsAndVariationsForOrderItem (fun id -> loggedOn (selectStandardCommentsForOrderItem id))
        pathScan Path.Admin.standardCommentsForCourse (fun id -> admin (standardCommentsForCourse id))
        pathScan Path.Admin.removeStandardCommentForCourse (fun id -> admin (removeStandardCommentForCourse id))
        pathScan Path.Admin.removeStandardComment (fun id -> admin  (removeStandardComment id))
        path Path.Admin.standardComments >=> standardComments
        pathScan Path.Orders.removeAllDiscountOfSubOrder (fun id -> admin (removeAllDiscountOfSubOrder id))
        pathScan Path.Orders.removePaymentItemOfOrder (fun (paymentId,orderId) -> admin (removePaymentItemOfOrder paymentId orderId) )
        pathScan Path.Orders.createEcrReceiptInstructionForSubOrder (fun (suborderid,orderid) -> admin (createEcrReceiptInstructionForSubOrder suborderid orderid))
        pathScan Path.Orders.createEcrReceiptInstructionForOrder (fun orderid -> admin (createEcrReceiptInstructionForOrder orderid))
        pathScan Path.Orders.removePaymentItemOfSubOrder (fun (paymentId,subOrderId,orerId) -> admin (removePaymentItemOfSubOrder paymentId subOrderId orerId))

        pathScan Path.Orders.selectOrderFromWhichMoveOrderItems  (fun targetOrerId -> anyUserExceptTemporaryRef   (selectOrderFromWhichMoveOrderItemsRef targetOrerId))
        pathScan Path.Orders.wholeOrderPaymentItems (fun orderId -> admin (wholeOrderPaymentItems orderId))
        pathScan Path.Orders.subOrderPaymentItems (fun (subOrderId,orderId) -> admin (subOrderPaymentItems subOrderId orderId))
        pathScan Path.Orders.viewOrder (fun id -> loggedOn (viewSingleOrderRef id))

        path Path.Extension.qrUserImageGen >=> admin qrUserImageGenRef
        path Path.Extension.qrCodeLogin >=> logonViaQrCode
        path Path.Extension.addQruser >=> admin addQrUser
        pathScan Path.Extension.regenTempUser  (fun id -> admin (regenTempUser id))

        pathScan Path.Orders.removeAllAllergenicFromOrderItem (fun (orderItemId,encodedBackUrl) -> anyUserPassingUserLoggedOn (removeAllAllergenic orderItemId encodedBackUrl))

        pathScan Path.Orders.removeAllUnavailableIngredientsFromOrderItem (fun (orderItemId,encodedBackUrl) -> anyUserPassingUserLoggedOn (removeAllInvisibleIngredients orderItemId encodedBackUrl))
        pathScan Path.Orders.rejectOrderItem (fun orderItemId  -> anyUserPassingUserLoggedOn (rejectOrderItem orderItemId))
        pathScan Path.Orders.printWholeOrderReceipt (fun orderId -> admin (printWholeOrderReceipt orderId))
        pathScan Path.Orders.printWholeOrderInvoice (fun orderId -> admin (printWholeOrderInvoice orderId))

        pathScan Path.Orders.unVoidLatestVoidedOrderOfUser (fun (userId,encodedBackUrl) ->  (unVoidLatestVoidedRef encodedBackUrl))

        path Path.Orders.dearchiveLatestOrder >=> anyUserPassingUserLoggedOn deArchiveLatestOrder
        pathScan Path.Admin.removePrinter (fun id -> admin (deletePrinter id))
        pathScan Path.Admin.deleteIngredientPrice (fun id -> admin (deleteIngredientPrice id))
        pathScan Path.Admin.deleteIngredient (fun id -> admin (deleteIngredient id))
        path Path.Admin.deleteIngredients >=> canManageIngredients deleteIngredientsBySelection
        pathScan Path.Admin.deleteRole (fun id -> admin (deleteUserRole id))
        path Path.Admin.deleteUserRoles >=> admin deleteUserRoles
        pathScan Path.Admin.deleteCourseCategory (fun id -> admin (deleteCourseCategory id))
        path Path.Admin.deleteCourseCategories >=> canManageCourses deleteCourseCategories
        path Path.Admin.deleteIngredientCategories >=> canManageIngredients deleteIngredientCategories
        path Path.Admin.deleteCourses >=> canManageCourses deleteCourses
        pathScan Path.Admin.deleteIngredientCategory (fun id -> admin (deleteIngredientCategory id))
        pathScan Path.Admin.deleteUser (fun id -> admin (deleteUser id))
        path Path.Admin.deleteUsers >=> admin deleteUsers
        path Path.Admin.deleteTemporaryUsers >=> admin deleteTemporaryUsers
        path Path.Admin.deleteObjects >=> admin deleteObjects
        path Path.Admin.optimizeVoided >=> admin  optimizeVoided
        //pathScan Path.Orders.editOrderItemVariation ( fun (orderItemId,encodedBackUrl) -> anyUserPassingUserLoggedOn (editOrderItemVariationPassingUserLoggedOn orderItemId encodedBackUrl))
        pathScan Path.Orders.editOrderItemVariation ( fun orderItemId -> anyUserPassingUserLoggedOn (editOrderItemVariationPassingUserLoggedOn orderItemId ))
        //pathScan Path.Orders.editOrderItemVariationByIngredientCategory ( fun (orderItemId,categoryId,encodedBackUrl) -> anyUserPassingUserLoggedOn (editOrderItemVariationByIngredientCategoryPasssingUserLoggedOn orderItemId categoryId encodedBackUrl))
        pathScan Path.Orders.editOrderItemVariationByIngredientCategory ( fun (orderItemId,categoryId) -> anyUserPassingUserLoggedOn (editOrderItemVariationByIngredientCategoryPasssingUserLoggedOn orderItemId categoryId ))
        pathScan Path.Orders.removeIngredientVariation (fun (variationId, orderItemId,encodedBackUrl) ->  anyUserPassingUserLoggedOn (removeIngredientVariation variationId orderItemId encodedBackUrl)  )
        pathScan Path.Orders.increaseUnitaryIngredientVariation (fun (variationId,encodedBackUrl) -> anyUserPassingUserLoggedOn (increaseUnitaryIngredientVariation variationId encodedBackUrl))
        pathScan Path.Orders.decreaseUnitaryIngredientVariation (fun (variationId,encodedBackUrl) -> anyUserPassingUserLoggedOn (decreaseUnitaryIngredientVariation variationId encodedBackUrl))
        pathScan Path.Orders.addIncreaseIngredientVariation (fun (orderItemId,ingredientId,encodedBackUrl) -> anyUserPassingUserLoggedOn (addIncreaseIngredientVariation orderItemId ingredientId encodedBackUrl))
        pathScan Path.Orders.addDiminuishIngredientVariation (fun (orderItemId,ingredientId,encodedBackUrl) -> anyUserPassingUserLoggedOn (addDiminuishIngredientVariation orderItemId ingredientId encodedBackUrl ))
        pathScan Path.Orders.addWithougIngredientVariation (fun (orderItemId,ingredientId,ingredientcourseId, encodedBackUrl)-> anyUserPassingUserLoggedOn (addWithoutIngredientVariation orderItemId ingredientId encodedBackUrl))
        pathScan Path.Courses.deleteIngredientToCourse (fun (courseId, ingredientId) -> canManageCourses (deleteIngredientToCourse courseId ingredientId))
        pathScan Path.Courses.selectAllIngredientsForCourseEdit (fun courseId -> canManageCourses (selectIAllngredientCatForCourse courseId))
        pathScan Path.Courses.selectIngredientCategoryForCourseEdit (fun (courseId,categoryId) -> canManageCourses (selectIngredientCatForCourse courseId categoryId "" ) )
        pathScan Path.Admin.editIngredient (fun (id,pageNumberBack) -> admin (editIngredient id pageNumberBack))
        pathScan Path.Admin.viewIngredientUsage (fun (ingredientId,pageNumberBack) -> canManageCourses (viewIngredientUsage ingredientId pageNumberBack))
        pathScan Path.Admin.fillIngredient (fun (id,pageNumberBack) -> canManageIngredientsPassingUserLoggedOn (fillIngredient id pageNumberBack))
        pathScan Path.Admin.switchVisibilityOfIngredientCategory (fun (id,encodedBackUrl) -> canManageIngredients (switchVisbilityOfIngredientCategory id encodedBackUrl))
        pathScan Path.Admin.switchVisibilityOfIngredient (fun (idCategory,idIngredient) -> admin (switchVisbilityOfIngredient idCategory idIngredient))
        path Path.home >=> anyUserExceptTemporary controlPanel
        path Path.Orders.allOrders >=> adminPassingUserLoggedOn allOrders // >=> noCache
        path Path.Extension.qrUserOrder >=> temporaryUserPassingUserLoggedOn qrOrder
        path Path.Orders.myOrders  >=>  anyUserPassingUserLoggedOn myOrders // >=> noCache
        path Path.Orders.myOrdersSingles  >=>  anyUserPassingUserLoggedOn myOrdersSingles // >=> noCache
        path Path.Orders.addOrder >=> anyUserPassingUserLoggedOn createOrderByUserLoggedOn
        path Path.Orders.addSingleOrder >=> anyUserPassingUserLoggedOn createSingleOrderByUserLoggedOn
        pathScan Path.Orders.addOrderItemByCategory  (fun (idOrder,idCategory, urlEncodedBackUrl) -> anyUserPassingUserLoggedOn (addOrderItemByCategoryPassingUserLoggedOn idOrder idCategory urlEncodedBackUrl))
        pathScan Path.Orders.addOrderItem  (fun (idOrder, urlEncodedBackUrl) -> anyUserPassingUserLoggedOn (addOrderItemPassingUserLoggedOn idOrder urlEncodedBackUrl))
        
        // pathScan Path.Orders.resetVariationsAndEditOrderItemByCategory (fun (idOrder,idCategory,urlEncodedBackUrl) ->    anyUserPassingUserLoggedOn (resetVariationThenEditOrderItemByCat idOrder idCategory Path.Orders.myOrders urlEncodedBackUrl) )
        pathScan Path.Orders.resetVariationsAndEditOrderItemByCategory (fun (idOrder,idCategory,urlEncodedBackUrl) ->    loggedOn (resetVariationThenEditOrderItemByCatRef idOrder idCategory Path.Orders.myOrders urlEncodedBackUrl) )

        pathScan Path.Orders.editOrderItemByCategory (fun (idOrder,idCategory,encodedBackUrl) -> anyUserPassingUserLoggedOn (editOrderItemByCategoryPassingUserLoggedOn idOrder idCategory Path.Orders.myOrders encodedBackUrl))
        pathScan Path.Orders.editDoneOrderitem (fun (idOrder,idCategory) -> anyUserPassingUserLoggedOn (editOrderItemByCategoryPassingUserLoggedOn idOrder idCategory Path.Orders.seeDoneOrders Path.Orders.seeDoneOrders))
        pathScan Path.Orders.resetDiscount (fun idOrder -> admin (resetDiscount idOrder))
        pathScan Path.Orders.editDoneOrderItemFromSubSplitting (fun (orderItemId,idCategory,orderId) -> anyUserPassingUserLoggedOn (editOrderItemByCategoryPassingUserLoggedOn orderItemId idCategory (sprintf Path.Orders.subdivideDoneOrder orderId) (sprintf Path.Orders.subdivideDoneOrder orderId)))
        pathScan Path.Orders.achiveOrder  (fun id -> anyUserPassingUserLoggedOn (archiveOrder id))
        path Path.Orders.seeDoneOrders >=> anyUserPassingUserLoggedOn seeDoneOrders
        pathScan Path.Orders.moveAllInitialStateOrderItems (fun (orderId) -> anyUserPassingUserLoggedOn (moveAllOrderItemsInBlockFromInitialState orderId))
        pathScan Path.Orders.moveInitialStateOrderItemsByOutGroup (fun (orderId,groupId,encodedBackUrl) -> anyUserPassingUserLoggedOn (moveInitialStateOrderItemsByOrderOutGroup orderId groupId encodedBackUrl))
        pathScan Path.Orders.reprintOrderItemsGroup (fun (orderId,groupId,encodedBackUrl) -> anyUserPassingUserLoggedOn (rePrintOrderOutGroup orderId groupId encodedBackUrl))
        pathScan Path.Orders.moveOrderItemToTheNextStateAndGoMyOrders (fun (itemId,encodedBackUrl) -> anyUserPassingUserLoggedOn (moveOrderItemNextStepAndBacktoOrder itemId encodedBackUrl ))
        pathScan Path.Orders.removeOrderItemThenGoBackToUrl (fun (itemId,encodedBackUrl) -> adminPassingUserLoggedOn (removeOrderItemThenGoBackToUrl itemId encodedBackUrl ))
        pathScan Path.Orders.moveOrderItemToTheNextStateAndGoOrdersProgress (fun itemId -> anyUserPassingUserLoggedOn (moveOrderItemNextStep itemId Path.Orders.orderItemsProgress ))
        pathScan Path.Orders.confirmVoidOrderFromMyOrders (fun (id,encodedBackurl) -> anyUserPassingUserLoggedOn (voidOrderByUserLoggedOn id encodedBackurl ))
        pathScan Path.Orders.voidOrderFromMyOrders (fun (id,encodedBackurl) -> anyUserPassingUserLoggedOn (askConfirmationVoidOrderByUserLoggedOn id encodedBackurl ))
        pathScan Path.Courses.editCategory  (fun id -> canManageCourses (editCourseCategory id))
        pathScan Path.Orders.deleteOrderItem (fun (id,encodedBackUrl) -> anyUserPassingUserLoggedOn (deleteOrderItemByUser id encodedBackUrl))
        pathScan Path.Admin.editUser (fun id -> admin (editUser id))
        pathScan Path.Admin.editTemporaryUser (fun id -> admin (editTemporaryUser id))
        path Path.Account.logoff >=> reset
        path Path.Account.logon >=> logon
        path Path.Errors.unableToCompleteOperation >=> unableToCompleteOperation local.AnErrorOccurred
        path Path.Courses.manageAllCourses >=> admin manageCourses
        path Path.Courses.addCategory >=> canManageCourses createCategory
        pathScan Path.Courses.manageVisibleCoursesOfACategory  (fun id -> admin (manageVisibleCoursesOfACategory id))
        pathScan Path.Courses.manageAllCoursesOfACategory  (fun id -> admin (manageAllCoursesOfACategory id))
        pathScan Path.Courses.manageAllCoursesOfACategoryPaginated (fun (categoryId,pageNumber) -> canManageCourses (manageAllCoursesOfACategoryPaginated categoryId pageNumber ))
        pathScan Path.Courses.manageVisibleCoursesOfACategoryPaginated (fun (categoryId,pageNumber) -> canManageCourses (manageVisibleCoursesOfACategoryPaginated categoryId pageNumber ))
        pathScan Path.Courses.switchCourseCategoryVisibility (fun categoryId -> canManageCourses (switchCourseCategoryVisibility categoryId ))
        path Path.Courses.manageAllCategories >=> canManageCourses manageCategories
        pathScan Path.Courses.editCourse (fun id -> canManageCourses (editCourse id))
        pathScan Path.Courses.addCourseByCategory (fun id -> canManageCourses (createCourseByCatgory ((decimal)id)))
        path Path.Courses.adminCategories >=> canManageCourses adminCategories 
        path Path.Admin.allUsers >=> admin ordinaryUsers
        path Path.Admin.temporaryUsers >=> admin temporaryUsers
        path Path.Admin.addUser >=> admin addUser
        path Path.Admin.roles  >=> admin adminRoles
        path Path.Admin.allIngredientCategories >=> canManageIngredients adminIngredientCategories
        path Path.Admin.recognizePrinters >=> admin  recognizePrinters
        pathScan Path.Admin.managePrinter (fun (printerId,categoryId) -> admin (managePrinter printerId categoryId))
        path Path.Admin.resetPrinters >=> admin resetPrinters
        path Path.Admin.info >=>  about
        path Path.Admin.printers >=> admin adminPrinters
        path Path.Admin.visibleIngredientCategories >=> canManageIngredients visibleingredientCategories
        pathScan Path.Admin.editIngredientCategory (fun id -> (canManageIngredientsPassingUserLoggedOn (editIngredientCategory id)))
        pathScan Path.Admin.editIngredientPrices  (fun id -> admin  (editIngredientPrices id))
        pathScan Path.Admin.editIngredientCategoryPaginated (fun (categoryid, pageNumber) -> (canManageIngredients (editIngredientCategoryPaginated categoryid pageNumber)))
        path Path.Admin.addRole >=> adminPassingUserLoggedOn createRole
        path Path.Admin.roleEnablerObserverCategoriesByCheckBoxes >=> admin roleEnablerObserverCategoriesByCheckBoxes
        pathScan Path.Admin.roleEnablerObserverCategoriesByCheckBoxesByRoleAndCat (fun (roleId,catId) -> admin (roleEnablerObserverCategoriesByCheckBoxesWithRoleAndCat roleId catId))
        pathScan Path.Admin.setStateAsActionableForSpecificWaiter  (fun (userId, stateId) -> admin (setStateAsActionableForWaiter userId stateId))
        pathScan Path.Admin.unSetStateAsActionableForSpecificWaiter  (fun (userId, stateId) -> admin (unSetStateAsAcionableForWaiter userId stateId))
        path Path.Admin.defaultActionableStatesForOrderOwner >=> admin defaultActionableStatesForOrderOwner
        path Path.Admin.tempUserDefaultActionableStates >=> admin tempUserDefaultActionableStates
        pathScan Path.Admin.actionableStatesForSpecificOrderOwner  (fun id -> admin (specificActionableStateForOrderOwner id))
        pathScan Path.Admin.setStateAsActionableForWaiterByDefault (fun id -> admin (setStateAsActiobableByDefault id))
        pathScan Path.Admin.setStateAsActionableForTempUserByDefault (fun id -> admin (setStateAsActionableForTempUserByDefault id))
        pathScan Path.Admin.unSetStateAsActionableForWaiterByDefault (fun id -> admin (unSetStateAsActiobableByDefault id))
        pathScan Path.Admin.unSetStateAsActionableForTempUserByDefault (fun id -> admin (unSetStateAsActionableForTempUserByDefault id))
        pathScan Path.Admin.removeObserverMapping (fun id -> admin (deleteObserverMapping id))
        pathScan Path.Admin.removeEnablerMapping (fun id -> admin (deleteEnablerMapping id))
        path Path.Orders.orderItemsProgress >=> anyUserPassingUserLoggedOn orderItemProgress 
        path Path.Account.changePassword >=> anyUserPassingUserLoggedOn changePassword
        pathScan Path.Orders.editDoneOrder (fun id -> adminPassingUserLoggedOn (editDoneOrder id))
        //pathScan Path.Orders.subdivideDoneOrder  (fun id -> adminPassingUserLoggedOn (subdivideDoneOrder id))
        pathScan Path.Orders.subdivideDoneOrder  (fun id -> adminPassingUserLoggedOn (subdivideDoneOrderRef id))
        pathScan Path.Orders.colapseDoneOrder  (fun id -> adminPassingUserLoggedOn (colapseDoneOrder id))
        pathScan Path.Orders.splitOrderItemInToUnitaryOrderItems (fun id -> adminPassingUserLoggedOn (splitOrderItemInToUnitaryOrderItems id))
        pathScan Path.Orders.deleteSubOrder (fun (subOrderId,orderId) -> anyUserPassingUserLoggedOn (deleteSubOrder subOrderId orderId))
        pathScan Path.Orders.printReceipt (fun (subOrderId,orderId) -> admin (printReceipt subOrderId orderId))
        pathScan Path.Orders.printSubOrderInvoice (fun subOrderId -> admin (printSubOrderInvoice subOrderId))
        pathScan Path.Orders.setSubOrderAsPaid (fun (subOrderId,orderId) -> adminPassingUserLoggedOn (setSubOrderAsPaid subOrderId orderId))
        pathScan Path.Orders.setSubOrderAsNotPaid (fun (subOrderId,orderId) -> adminPassingUserLoggedOn (setSubOrderAsNotPaid subOrderId orderId))
        pathRegex "(.*)\.(css|png|gif|js|html)" >=> Files.browseHome
        html View.notFound
    ]

// let cert = new X509Certificate2("certificate.p12","secret")

let cfg =
    { defaultConfig with
        bindings = [ HttpBinding.createSimple HTTP "0.0.0.0" 8083  ] 

      //   homeFolder= Some "/Users/Tonyx/Projects/orderssystem_core"

      //   homeFolder= Some @"C:\Users\Toni\gitprojects\toni\orderssystem_core"
        homeFolder= Some Settings.HomeFolder

      //    bindings = [ HttpBinding.createSimple (HTTPS cert) "0.0.0.0" 8443  ] 
    }

DotLiquid.setTemplatesDir ("templates-"+Settings.Localization)

startWebServer cfg webPart
