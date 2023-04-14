function getSelectedText() {
    return window.getSelection().toString();
}

window.dotNetToJsSamples = {
    getValue: function (node, txt) {
        var curPos = node.selectionStart;
        var newtxt = node.value.substr(0, curPos) + txt + node.value.substr(curPos);;
        console.log(newtxt);
        return newtxt;
    }
};

