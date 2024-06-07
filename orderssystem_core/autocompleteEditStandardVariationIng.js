

//
// var idNamesDropList = document.getElementsByName("IngredientBySelect")[0];
// alert (idNamesDropList[0].value);
//
// var mapIdName = new Map();
//
// var values = []
// for (i=0;i<idNamesDropList.length;i++) {
//     mapIdName.set(idNamesDropList.options[i].value,idNamesDropList.options[i].text);
//     values.push (idNamesDropList.options[i].text);
// }
//
// var categoryLabel = document.getElementsByClassName("editor-label")[0];
// var mapNamePrices = new Map();
//
// document.getElementsByName("IngredientBySelect")[0].addEventListener("change",function(){
//         // alert('updating');
//        
//         alert (document.getElementsByName("IngredientBySelect")[0].value); 
//        
//
//         alert('updating- 1');
//    
//         let quantityDropList = document.getElementsByName("Quantity")[0]; 
//         quantityDropList.options.length = 0;
//        
//         alert('updating- 2');
//
//         const newOption = new Option('New Option', 'newValue');
//         quantityDropList.add(newOption);
//        
//         alert('updating- 3');
//        
//         console.log ('Keys' + ingrAdds)
//        
//         let specificIngrAdds = ingrAdds.get(document.getElementsByName("IngredientBySelect")[0].value);
//         let specificIngrAddsKeySet = specificIngrAdds.keys();
//        
//         console.log ('specificIngrAdds' + specificIngrAdds);
//         console.log ('specificIngrAddsKeys' + specificIngrAddsKeySet);
//         console.log ('specificIngrAddsKeys' + specificIngrAddsKeySet.next().value);
//        
//         alert('updating- 4');
//
//         alert('updating');
//     }
// );
//
// alert ('okx6');

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


addNewOption(document.getElementsByName("IngredientBySelect")[0].value);

document.getElementsByName("IngredientBySelect")[0].addEventListener("change", function() {
        addNewOption(document.getElementsByName("IngredientBySelect")[0].value);
});

function addNewOption(key) {
        var theSelect = document.getElementsByName("Quantity")[0];
        theSelect.options.length = 4;
        
        let specificIngrAdds = ingrAdds.get(key);
        // let specificIngrAddsKeySet = specificIngrAdds.keys();
        console.log ('addNewOption 3');
        
        var abudant = document.createElement('option');
        var scarse = document.createElement('option');
        
        specificIngrAdds.forEach(function(value, key, map) {
                var opt = document.createElement('option');
                opt.value=key;
                opt.innerHTML = "Add " + value;
                theSelect.appendChild(opt);
                
                var opt2 = document.createElement('option');
                opt2.value=key;
                opt2.innerHTML = "Remove " + value;
                theSelect.appendChild(opt2);
        } );
        console.log ('addNewOption 6');
}

