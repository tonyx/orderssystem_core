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
    let EcrFilePath = @"C:\Users\pc\programmi_tony\orderssystem_core\ecr.txt"
