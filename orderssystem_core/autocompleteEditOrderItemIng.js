
var dropList = document.getElementsByName("IngredientBySelect")[0];
var mapIngredients = new Map();
var mapIngredientsNameId = new Map();

var values = [];

for (i =0;i<dropList.length;i++) {
  mapIngredients.set(dropList.options[i].value,dropList.options[i].text);
  mapIngredientsNameId.set(dropList.options[i].text,dropList.options[i].value);
  values.push (dropList.options[i].text);
}

addNewOption(document.getElementsByName("IngredientBySelect")[0].value);


document.getElementsByName("IngredientBySelect")[0].addEventListener("change", function() {
    addNewOption(document.getElementsByName("IngredientBySelect")[0].value);
});


function cleanSelectQuantity() {
  var selectQuantityList = document.getElementsById("Quantity")[0];
  for (var i =0;i<selectQuantityList.length;i++) {
    if (isNumeric(selectQuantityList.options[i].value))
      selectQuantityList.remove(i);
  }
}

function isNumeric(n) {
  return !isNaN(parseFloat(n)) && isFinite(n);
}

function addNewOption(key) {
  var theSelect = document.getElementsByName("Quantity")[0];
  for (var i=0;i<theSelect.length;i++) {
     if (isNumeric(theSelect.options[i].value)) {
      theSelect.remove(i);
     }
  }


  let specificIngrAdds = ingrAdds.get(parseInt(key));
  let specificIngrAddsKeySet = specificIngrAdds.keys();

  specificIngrAdds.forEach(function(value,key,map) {
    var opt = document.createElement('option');
    opt.value=key;
    opt.innerHTML = value;
    theSelect.appendChild(opt);
  } );
}