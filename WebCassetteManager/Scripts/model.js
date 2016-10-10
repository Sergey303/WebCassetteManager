var loadingStatus = {};
var viewModel;
ko.punches.enableAll();

function GetItemById(id, predicate) {
    var key = id + '.' + predicate;
    
    var first = ko.utils.arrayFirst(viewModel.mydata(), function (item) {
        return item.key === key;
    });
    if (first === null) {
        if (loadingStatus[id + predicate] == null) {
            if (predicate[0] === '^')
                hub.server.getInverseValuesBuffer(id, predicate.substring(1));
            else hub.server.getDirectValue(id, predicate);
            loadingStatus[id + predicate] = false;
        }
        //if (key === 'img1.uri') {
        //    alert('null');
        //}
        return null;
    } else {
        //alert(key + " " + first.value());
        //if (key === 'img1.uri') {
        //    alert(first.value());
        //}
        return first.value;
    }
}

function get(idobservable, predicate) {
    viewModel.notification();
    return ko.pureComputed(function () {
      //  viewModel.notification();
       var id;
    if (typeof idobservable === "function")
        id = idobservable();
    else id = idobservable;
        return GetItemById(id, predicate);
    }, this);
}
function NameorId(id) {
    viewModel.notification();
  return ko.pureComputed(function () {
        var name = get(id, 'name')();
   //     viewModel.notification();
        return name === null ? id : name;
    });
}
function ChangeMainId(id) {
    return function () {
        viewModel.id = ko.observable(id);
      //  ko.utils.arrayPushAll(viewModel.mydata, []);
        viewModel.notification.notifySubscribers();
      //  viewModel.mydata.notifySubscribers();
    };
}

function IsImageTypeUri(uri) {
    viewModel.notification();
    return ko.pureComputed(function () {
       // viewModel.notification();
       // alert("uri"+uri);
        if (uri === null) return false;

        if (typeof uri === "function")
            uri = uri();
      //  alert("uri "+uri);
        var parts = uri.split('.');
       // alert(parts.length);
        var ext = parts[parts.length - 1];
      //  alert(ext);

        if (ext === "jpg" || ext === "tif" || ext==="bmp")
            return true;
        return false;
    });
}

function appendTriple(id, property, objvalue) {
    var newkey = id + "." + property;
     //  alert(newkey+' '+objvalue);
    var array = GetItemById(id, property);
    if (array === null) {
        array = ko.observableArray([objvalue]);
        ko.utils.arrayPushAll(viewModel.mydata, [{ key: newkey, value: array }]);
    } else if (array.indexOf(objvalue) === -1) ko.utils.arrayPushAll(array, [objvalue]);
}

function Addtriple(id, property, objvalue, isObjectIri) {
    appendTriple(id, property, objvalue);
    if (isObjectIri)
        appendTriple(objvalue, "^" + property, id);
}

function AddBuffer(subjetsBuffer, property, objvalue) {
    $.each(subjetsBuffer, function(i, id) {
        appendTriple(id, property, objvalue);
            appendTriple(objvalue, "^" + property, id);
    });
    
}

$(function () {
    //  ko.punches.enableAll();

    viewModel = {
        notification: ko.observable(),

        id: ko.observable("cassetterootcollection"),
        mydata: ko.observableArray()
    }
});