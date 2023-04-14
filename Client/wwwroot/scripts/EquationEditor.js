function display_equation(equationcode, qno) {
    var input = equationcode;
    output = document.getElementById(qno);
    output.innerHTML = '';

    var options = MathJax.getMetricsFor(output);
    MathJax.tex2chtmlPromise(input, options).then(function (node) {
        console.log(node);
        output.appendChild(node);
        MathJax.startup.document.clear();
        MathJax.startup.document.updateDocument();
    });
}


function display_equation_image(equationcode, qno) {
    var input = equationcode;
    /*var display = document.getElementById("display");*/
    output = document.getElementById(qno);
    output.innerHTML = '';

    MathJax.texReset();
    var options = MathJax.getMetricsFor(output);
   /* options.display = display.checked;*/
    MathJax.tex2svgPromise(input, options).then(function (node) {
        console.log(node);
        output.appendChild(node);
        MathJax.startup.document.clear();
        MathJax.startup.document.updateDocument();
    });
}






//function display_equation(equationcode, qno) {
//    tex = equationcode;
//    d = document.getElementById(qno);

//    MathJax.tex2chtmlPromise(tex).then((node) => {
//        d.innerHTML = '';
//        d.append(node);
//        MathJax.startup.document.clear();
//        MathJax.startup.document.updateDocument();
//    });
//}
