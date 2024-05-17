module OrdersSystem.Settings

    let HostAddress="192.168.1.8:8083" 
    let Print = true
    let Printcommand ="/usr/bin/lpr"  // unix
    // let Printcommand ="PRINT"  // windows
    let InAddIngredientAdjustPrice =true
    let InRemoveIngredientAdjustPrice ="true" 
    //let PrinterSelector = "-P " // unix
    let PrinterSelector = "/D:" // windows
    let Localization = "it" 
    // let EcrFilePath = @"C:\Users\pc\programmi_tony\orderssystem_core\ecr.txt"
    let EcrFilePath = "ecr.txt"
    // let HomeFolder = @"C:\Users\Toni\gitprojects\toni\orderssystem_core\orderssystem_core"
    let HomeFolder = "/Users/antoniolucca/github/orders_system/orderssystem_core/orderssystem_core"
    let [<Literal>] NpgSqlResPath = "" // after version 5.0 there is no need of a specific assembly anymore
