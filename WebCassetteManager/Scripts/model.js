var loadingStatus = {};
var viewModel;

function get(id, predicate) {
    if (typeof id === "function")
        id = id();
    var key = id + '.' + predicate;
    var value = viewModel.data()[key];
    if (value == null) {
        if (loadingStatus[key] == null) {
            if (predicate[0] === '^') {
            } //hub.server.getInverseValue(id, predicate.substring(1));
            else hub.server.getDirectValue(id, predicate);
            loadingStatus[key] = false;
        }
        return null;
    } else {
        return value;
    }
}

function NameorId(id) {
    //viewModel.notification();
     var name = get(id, 'name');
        return name === null ? id : name;
}
function ChangeMainId(id) {
    return function() {
        viewModel.id(id);
    }
}

function IsImageTypeUri(uri) {
   // viewModel.notification();
        if (uri === null) return false;
        if (typeof uri === "function")
            uri = uri();
        var parts = uri.split('.');
        var ext = parts[parts.length - 1];
        if (ext === "jpg" || ext === "tif" || ext==="bmp")
            return true;
        return false;
}

function appendTriple(id, property, objvalue) {
    var newkey = id + "." + property;
    //  alert(newkey+' '+objvalue);
    var data = viewModel.data();
    var array = data[newkey]; //GetItemById(id, property);
    if (array == null) {
        array = ko.observableArray([objvalue]);
        data[newkey] = array;
       // ko.utils.arrayPushAll(viewModel.mydata, [{ key: newkey, value: array }]);
    } else if (array.indexOf(objvalue) === -1) ko.utils.arrayPushAll(array, [objvalue]);

   
    viewModel.data.notifySubscribers();
    
}

function Addtriple(id, property, objvalue, isObjectIri) {
    appendTriple(id, property, objvalue);
    if (isObjectIri)
        appendTriple(objvalue, "^" + property, id);
}

function CallGetTriples(path) {
    hub.server.getTriplesFromPath(viewModel.id(), path);
        return true;
}

var hub;
$(function () {
    //  ko.punches.enableAll();
    hub = $.connection.cassettesHub;
    hub.client.addtriple = Addtriple;
    viewModel = {
        id: ko.observable("cassetterootcollection"),
        data: ko.observable({})
    }
    $.connection.hub.start().done(function () {
        Import();
        ko.punches.enableAll();
        ko.applyBindings(viewModel);
      
    });
});