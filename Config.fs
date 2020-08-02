module OrdersSystem.Settings

    let HostAddress="192.168.1.189:8083" 
    let Print = true
    //let Printcommand ="/usr/bin/lpr"  // unix
    let Printcommand ="PRINT"  // windows
    let InAddIngredientAdjustPrice =true
    let InRemoveIngredientAdjustPrice ="true" 
    //let PrinterSelector = "-P " // unix
    let PrinterSelector = "/D:" // windows
    let Localization = "it" 
    // let EcrFilePath = @"C:\Users\pc\programmi_tony\orderssystem_core\ecr.txt"
    let EcrFilePath = "ecr.txt"
    let HomeFolder = @"C:\Users\Toni\gitprojects\toni\orderssystem_core"
    let [<Literal>] NpgSqlResPath = "C:/Users/Toni/gitprojects/toni/orderssystem_core/packages/Npgsql.2.2.1/lib/net45/"  // location of a valid Npgsql.dll file used at compile time by sql data provider
