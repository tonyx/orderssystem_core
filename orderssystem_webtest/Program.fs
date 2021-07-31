open System
open canopy.runner.classic
open canopy.configuration
open canopy.classic
open orderssystem_webtest
open OrdersSystem.View

start chrome

let home = "http://localhost:8083" 

once
    (
        fun _ -> 
            url Home.home
            Auth.username << "administrator"
            Auth.password << "administrator"
            click Auth.submit
    )
before
    (
        fun _ ->
            url "http://localhost:8083"
    )
lastly
    (
        fun _ ->

        //    url Home.home
        //    Home.deleteButton == "cancellazioni"
        //    click Home.deleteButton
        //    Deletion.eliminateDishesCategoryButton == "elimina categorie di piatti"
        //    click Deletion.eliminateDishesCategoryButton
        //    DishesCategoryDeletion.eliminateTheOnlyExistingCategoryButton == "elimina category1"
        //    click DishesCategoryDeletion.eliminateTheOnlyExistingCategoryButton
           
        //    url Home.home
        //    click Home.deleteButton
        //    Deletion.eliminateIngredientCategories == "elimina categorie di ingredienti"
        //    click Deletion.eliminateIngredientCategories
        //    IngredientCategoryDeletion.eliminateTheOnlyExistingIngredientCategoryButton == "elimina ingCategory1"
        //    click IngredientCategoryDeletion.eliminateTheOnlyExistingIngredientCategoryButton

        //    url Home.home
        //    click Home.ordersButton
        //    click Orders.firstOrder
        //    click Orders.voidOrder
        //    click Confirmation.yes
           ()
    )
"first button is information" &&& fun _ ->
    Home.infoButton == local.Info
    
"second button is manage printers" &&& fun _ ->
    Home.managePrintersButton == local.ManagePrinters.Trim()
    
"third button is ingredients" &&& fun _ ->
    Home.ingredientsButton == local.Ingredients.Trim()
    
"forth button is dishes" &&& fun _ ->
    Home.dishesButton == local.Courses.Trim()
    
"create a new dishes category" &&& fun _ ->
    click Home.dishesButton
    Dishes.pageTitle == local.CourseCategoriesManagement.Trim()
    Dishes.createNewCategoryButton |> click
    DishesCreationPage.newCategory == local.NewCategory.Trim()
    DishesCreationPage.newCategoryField << "category1"
    DishesCreationPage.submit |> click
    DishesCreationPage.firstCategory == "category1"
    
"create a new dish in the new category" &&& fun _ ->
    click Home.dishesButton
    DishesCreationPage.firstCategory == "category1"
    DishesCreationPage.firstCategory |> click
    DishesCreationPage.addNew == local.AddNew.Trim()
    DishesCreationPage.addNew |> click
    DishesCreationPage.nameField << "piatto1"
    DishesCreationPage.priceField << "5"
    DishesCreationPage.input |> click
    
"create a second dish in the new category" &&& fun _ ->
    click Home.dishesButton
    DishesCreationPage.firstCategory == "category1"
    DishesCreationPage.firstCategory |> click
    DishesCreationPage.addNew == local.AddNew.Trim()
    DishesCreationPage.addNew |> click
    DishesCreationPage.nameField << "piatto2"
    DishesCreationPage.priceField << "4"
    DishesCreationPage.input |> click

"create a new ingredient category "  &&& fun _ ->
   Home.ingredientsButton == local.Ingredients.Trim()
   Home.ingredientsButton |> click
   IngredientCategories.title == local.AllCategoriesOfIngredients.Trim()
   IngredientCategories.nameField << "ingCategory1"
   IngredientCategories.submit |> click

 
"create a new ingredient" &&& fun _ ->
   Home.ingredientsButton |> click
   IngredientCategories.firstExistingCategory == "ingCategory1"
   IngredientCategories.firstExistingCategory |> click
   IngredientCategory.addNew |> click
   IngredientEdit.name << "ingredient1"
   IngredientEdit.submit |> click
    
"create another ingredient" &&& fun _ ->
   Home.ingredientsButton |> click
   IngredientCategories.firstExistingCategory == "ingCategory1"
   IngredientCategories.firstExistingCategory |> click
   IngredientCategory.addNew |> click
   IngredientEdit.name << "ingredient2"
   IngredientEdit.submit |> click
    
"search and edit first dishes adding two ingredients, no quantity" &&& fun _ ->
   Home.dishesButton |> click
   Dishes.findField << "piat"
   Dishes.findSubmit |> click
   Dishes.firstResultInSearchWithSingleCat == "piatto1"
   Dishes.firstResultInSearchWithSingleCat |> click
   EditDish.addIngredientAmongTheFirstCategory == local.AddAmong.Trim()+" ingCategory1"
   click EditDish.addIngredientAmongTheFirstCategory
   AddIngredientToDish.name << "ingredient1"
   sleep 1
   click AddIngredientToDish.submit
   DishEdit.singleEntryExistingIngredient == "ingredient1"
   click EditDish.addIngredientAmongTheFirstCategory
   AddIngredientToDish.name << "ingredient2"
   sleep 1
   click AddIngredientToDish.submit
   
   read DishEdit.tableOfIngredients |> contains "ingredient2"
   read DishEdit.tableOfIngredients |> contains "ingredient1"
   
"set a default add/subtract price for ingredient1" &&& fun _ ->
   Home.ingredientsButton |> click
   IngredientCategories.firstExistingCategory == "ingCategory1"
   IngredientCategories.firstExistingCategory |> click
   IngredientCategory.priceButtonOfFirstIngredientItem |> click
   IngredientPrice.existingItem |> notDisplayed
   IngredientPrice.priceAddIngredient << "1"
   IngredientPrice.priceSubtractIngredient << "1"
   IngredientPrice.isDefaultAddPrice << "YES"
   IngredientPrice.isDefaultSubtractPrice << "YES"
   IngredientPrice.quantity << "10"
   IngredientPrice.submit |> click
   IngredientPrice.existingItem |> displayed
   read IngredientPrice.existingItem |> contains (local.Quantity.Trim())
   read IngredientPrice.existingItem |> contains ("10")
   
"create a new order adding a dish removing an ingredient, so updating the price automatically" &&& fun _ ->
   Home.ordersButton |> click
   Orders.newOrder |> click
   NewOrder.tableNumber << "1"
   NewOrder.submit |> click
   NewOrder.firstCategoryOfDishesInFirstTable == "+ category1"
   NewOrder.firstCategoryOfDishesInFirstTable |> click
   AddDish.selectDish << "piatto1"
   AddDish.submit |> click
   AddDish.ingredientsOfFirstOrderItemOfFirstOrder == local.Ingredients.Trim()
   AddDish.ingredientsOfFirstOrderItemOfFirstOrder |> click
   IngredientsOfOrderItem.originalPrice.Trim() == "prezzo originario 5.00."
   IngredientsOfOrderItem.updatedPrice.Trim() == "prezzo ricalcolato 5.00."
   IngredientsOfOrderItem.nameOfFirstIngredient == "ingredient1"
   IngredientsOfOrderItem.buttonDeleteFirstIngredient |> click
   IngredientsOfOrderItem.updatedPrice.Trim() == "prezzo ricalcolato 4.00."
  
    
"set a non default add/subtract price for ingredient1" &&& fun _ ->
   Home.ingredientsButton |> click
   IngredientCategories.firstExistingCategory == "ingCategory1"
   IngredientCategories.firstExistingCategory |> click
   IngredientCategory.priceButtonOfFirstIngredientItem |> click
   IngredientPrice.priceAddIngredient << "2"
   IngredientPrice.priceSubtractIngredient << "2"
   IngredientPrice.isDefaultAddPrice << "NO"
   IngredientPrice.isDefaultSubtractPrice << "NO"
   IngredientPrice.quantity << "20"
   IngredientPrice.submit |> click
   IngredientPrice.existingItem |> displayed
   read IngredientPrice.existingItem |> contains (local.Quantity.Trim())
   read IngredientPrice.secondItemText |> contains ("20")
  
run()

printfn "press [enter] to exit"
System.Console.ReadLine() |> ignore

quit()


