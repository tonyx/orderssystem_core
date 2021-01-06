namespace orderssystem_webtest

module Home =
    let home = "http://localhost:8083"
    let infoButton = ".//*[@id='main']/p[1]/a"
    let deleteButton = ".//*[@id='main']/p[12]/a"
    let managePrintersButton =  ".//*[@id='main']/p[2]/a"
    let ingredientsButton = ".//*[@id='main']/p[3]/a"
    let dishesButton = ".//*[@id='main']/p[4]/a"
    let ordersButton = "//*[@id='main']/p[11]/a"

module Orders =
    let newOrder = ".//*[@id='main']/p/a"
    
module NewOrder =
    let tableNumber = ".//*[@id='main']/form/fieldset/div[2]/input"
    let submit = ".//*[@id='main']/form/input"
    let firstCategoryOfDishesInFirstTable = ".//*[@id='item-list']/innerp/table[1]/tbody/tr/td[1]/a"
    
module Deletion =
    let eliminateDishesCategoryButton = ".//*[@id='main']/p[4]/a"
    let eliminateIngredientCategories = ".//*[@id='main']/p[7]/a"

module Dishes =
    let pageTitle = "//*[@id='main']/h1"
    let createNewCategoryButton = "//*[@id='main']/p/a"
    let findField  = ".//*[@id='main']/form/fieldset/div[2]/input"
    let findSubmit = ".//*[@id='main']/form/input"
    let firstResultInSearchWithSingleCat = ".//*[@id='main']/table[2]/tbody/tr/td[2]/a"
    // todo: this should be when categories are two: "//*[@id='main']/table[1]/tbody/tr[2]/td[1]/a". So try to generalize
    
module AddDish =
    let submit = ".//*[@id='main']/form/input"
    let selectDish = ".//*[@id='main']/form/fieldset/div[2]/select"
    let modifyIngredientOfFirstOrderItem = ".//*[@id='item-list']/innerp/table[2]/tbody/tr/td/a[1]"
    let ingredientsOfFirstOrderItemOfFirstOrder =  ".//*[@id='item-list']/innerp/table[2]/tbody/tr/td/a[3]"
   

module EditDish =
    let addIngredientAmongTheFirstCategory = "/html/body/div[3]/ul/p/a"

module OrderItemVariations =
     let priceInfo = "/html/body/div[3]/text()"
     let mainPage = "//*[@id='main']"


module DishesCreationPage =
    let newCategory = "//*[@id='main']/h2"
    let newCategoryField = "//*[@id='main']/form/fieldset/div[2]/input"
    let submit = "//*[@id='main']/form/input"
    let firstCategory = "//*[@id='main']/table[1]/tbody/tr/td[1]/a"
    let addNew = ".//*[@id='main']/p[2]/a"
    let nameField = ".//*[@id='main']/form/fieldset/div[2]/input"
    let priceField = ".//*[@id='main']/form/fieldset/div[4]/input"
    let input = ".//*[@id='main']/form/input"
    
module IngredientCategories =
    let title = ".//*[@id='main']/h1" 
    let nameField = ".//*[@id='main']/form/fieldset/div[2]/input" 
    let submit = ".//*[@id='main']/form/input"
    let firstExistingCategory = ".//*[@id='item-list']/p/a[1]"
    let name = ".//*[@id='main']/form/fieldset/div[2]/input"

module IngredientCategory =
    let addNew = "//*[@id='main']/a[1]"
    let priceButtonOfFirstIngredientItem = ".//*[@id='main']/table/tbody/tr[1]/td[2]/a"
   
module Ingredient =    
    let name = ".//*[@id='main']/form/fieldset/div[2]/input"
    let submit =  ".//*[@id='main']/form/input"
    

module IngredientEdit =
    let name = ".//*[@id='main']/form/fieldset/div[2]/input"
    let submit = ".//*[@id='main']/form/input"

module IngredientPrice =
    let priceAddIngredient = ".//*[@id='main']/form/fieldset/div[2]/input"
    let priceSubtractIngredient = ".//*[@id='main']/form/fieldset/div[4]/input"
    let isDefaultAddPrice = ".//*[@id='main']/form/fieldset/div[8]/select"
    let isDefaultSubtractPrice = ".//*[@id='main']/form/fieldset/div[10]/select"
    let quantity = ".//*[@id='main']/form/fieldset/div[6]/input"
    let submit = ".//*[@id='main']/form/input"
    let existingItem = ".//*[@id='main']/table/tbody/tr"
    let existingItemText = "//*[@id='main']/table/tbody/tr/td[1]"

module AddIngredientToDish =
    let name = ".//*[@id='main']/form/fieldset/div[2]/select"
    let submit = ".//*[@id='main']/form/input"
    
module DishesCategoryDeletion =
    let eliminateTheOnlyExistingCategoryButton = ".//*[@id='item-list']/p/a"
    let nameField = ".//*[@id='main']/form/fieldset/div[2]/input"

module IngredientCategoryDeletion =
    let eliminateTheOnlyExistingIngredientCategoryButton = ".//*[@id='item-list']/p/a" 

module DishEdit =
    let singleEntryExistingIngredient = "//*[@id='main']/table[1]/tbody/tr/td[1]"
    let tableOfIngredients = ".//*[@id='main']/table[1]/tbody"

module IngredientsOfOrderItem =
    let main =  ".//*[@id='main']"
    let firstIngredientsItem = ".//*[@id='main']/fieldset[3]/table/tbody/tr[1]"
    let originalPrice = ".//*[@id='main']/originalprice"
    let updatedPrice = ".//*[@id='main']/updatedprice"

module Auth =
    let username = ".//*[@name='Username']"
    let password = ".//*[@name='Password']"
    let submit = ".//*[@type='submit']"
