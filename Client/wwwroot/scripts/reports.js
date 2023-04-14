export function downloadPdf(fileName, byteBase64) {
    var link = document.createElement('a');
    link.download = fileName;
    link.href = "data:application/octet-stream;base64," + byteBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link)
}


export function viewPdf(iframeId, byteBase64) {
    document.getElementById(iframeId).innerHTML = '';
    var ifrm = document.createElement('iframe');
    ifrm.setAttribute("src", "data:application/pdf;base64," + byteBase64);
    ifrm.style.width = "100%";
    ifrm.style.height = "680px";
    document.getElementById(iframeId).appendChild(ifrm);
}

export function openPDFNewTab(data) {
    var arrrayBuffer = base64ToArrayBuffer(data); //data is the base64 encoded string
    function base64ToArrayBuffer(base64) {
        var binaryString = window.atob(base64);
        var binaryLen = binaryString.length;
        var bytes = new Uint8Array(binaryLen);
        for (var i = 0; i < binaryLen; i++) {
            var ascii = binaryString.charCodeAt(i);
            bytes[i] = ascii;
        }
        return bytes;
    }
    var blob = new Blob([arrrayBuffer], { type: "application/pdf" });
    var link = window.URL.createObjectURL(blob);
    window.open(link);
    //window.open(link, '', 'height=650,width=840');
       
}


