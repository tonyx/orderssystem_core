
// alert ('ok1');

// alert (ingrAdds);
// alert ('ok1');

var idNamesDropList = document.getElementsByName("IngredientBySelect")[0];
alert (idNamesDropList[0].value);

var mapIdName = new Map();

// alert ('okx1');
var values = []
for (i=0;i<idNamesDropList.length;i++) {
    mapIdName.set(idNamesDropList.options[i].value,idNamesDropList.options[i].text);
    values.push (idNamesDropList.options[i].text);
}

var categoryLabel = document.getElementsByClassName("editor-label")[0];
var mapNamePrices = new Map();

// for (i=0;i<idNamesDropList.length;i++) {
//     mapNamePrices.set(idNamesDropList.options[i].text,ingrAdds.get(idNamesDropList.options[i].value));
// }


document.getElementsByName("IngredientBySelect")[0].addEventListener("change",function(){
        // alert('updating');
        
        alert (document.getElementsByName("IngredientBySelect")[0].value); 
        
        // let elementsToAdd = ingrAdds.get(parseInt(document.getElementsByName("IngredientBySelect")[0].value));
        // let elementsToAddKeySet = elementsToAdd.keys();
        // let singleElementToAdd = elementsToAdd.get(document.getElementsByName("IngredientBySelect")[0].value);
        
        // let elementKeys = singleElementToAdd.keys()
        // let elementPairs = singleElementToAdd.entries();
        // alert (elementPairs);

        alert('updating- 1');
    
        let quantityDropList = document.getElementsByName("Quantity")[0]; 
        quantityDropList.options.length = 0;
        
        alert('updating- 2');

        const newOption = new Option('New Option', 'newValue');
        quantityDropList.add(newOption);
        
        alert('updating- 3');
        
        console.log ('Keys' + ingrAdds)
        
        let specificIngrAdds = ingrAdds.get(document.getElementsByName("IngredientBySelect")[0].value);
        let specificIngrAddsKeySet = specificIngrAdds.keys();
        
        console.log ('specificIngrAdds' + specificIngrAdds);
        console.log ('specificIngrAddsKeys' + specificIngrAddsKeySet);
        console.log ('specificIngrAddsKeys' + specificIngrAddsKeySet.next().value);
        
        alert('updating- 4');

        alert('updating');
    }
);

alert ('okx6');

// function updatePrice(name) {
//     document.getElementsByName("Price")[0].value = mapNamePrices.get(name);
// }

// if uncomment will reset the price even if overwritten

// if (pricesForCourses.has("-1")) {
// } else {
//     document.getElementsByName("Price")[0].value = pricesForCourses.get(document.getElementsByName("CourseId")[0].value);
// }

// document.getElementsByName("CourseByName")[0].value = mapIdName.get(document.getElementsByName("CourseId")[0].value);
// autocomplete(document.getElementsByName("CourseByName")[0], values);
//alert('ok3');

