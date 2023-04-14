window.TextFormatting = (txt) => {
    var parag = document.getElementById('cbtquestion');
    var x = txt;
    parag.innerHTML = x;
    /* document.getElementById('cbtquestion').innerHTML = x;*/
}

function GetNumber(noIndex) {
    var result = DotNet.invokeMethodAsync("SchMagnetApp.Academics", 'ReturnArrayAsync', parseInt(noIndex));
    result.then(function (val) {
        document.getElementById("noIndex").innerHTML = val;
    });
}

var myObject = { QID: 1, CurrentQuestion: "Test Question", Equation: "Test Equation"};

window.getMyObject = () => {

    return myObject;
};

window.GetQuestions = (questions) => {
    var lst = questions;
    for (var item in lst) {
        console.log(item, ":", lst[item]);
    }
    
    //$.each(questions, function () {
    //    /*alert('FirstName: ' + this.FirstName + ' LastName:' + this.LastName);*/
    //    console.log('No.: ' + this.QNo + ' Question: ' + this.Question);
    //});
    //console.log(model.CurrentQuestion);
}


