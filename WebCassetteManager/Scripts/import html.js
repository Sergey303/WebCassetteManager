function Import() {
    var links = document.querySelectorAll('link[rel=import]');
    for (var i = 0; i < links.length; i++) {
        var link = links[i];
        var content = link.import.querySelector('#intro-dm');
        document.body.appendChild(content.cloneNode(true));
    }
    
}