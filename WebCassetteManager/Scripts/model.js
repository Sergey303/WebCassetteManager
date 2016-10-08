var loadingStatus = {};
var viewModel;
ko.punches.enableAll();

function GetItemById(id, predicate) {
    var key = id + '.' + predicate;
    var first = ko.utils.arrayFirst(viewModel.mydata(), function (item) {
        return item.key === key;
    });
    if (first == null) {
        if (loadingStatus[id + predicate] == null) {
     //   alert(key);
            if (predicate[0] === '^')
                hub.server.getInverseValue(id, predicate.substring(1));
            else hub.server.getDirectValue(id, predicate);
            loadingStatus[id + predicate] = false;
        }
        return null;
    }
    return first.value;
}

function get(idobservable, predicate) {
    var id;
    if (typeof idobservable === "function")
        id = idobservable();
    else id = idobservable;

    viewModel.notification();
    return ko.pureComputed(function () {
        viewModel.notification();
        return GetItemById(id, predicate);
    }, this);
}
function NameorId(id) {
    viewModel.notification();
    return ko.pureComputed(function () {
        var name = get(id, 'name')();
        viewModel.notification();
        return name === null ? id : name;
    });
}
function ChangeMainId(id) {
    return function () {
        viewModel.id = id;
      //  ko.utils.arrayPushAll(viewModel.mydata, []);
      //  viewModel.notification.notifySubscribers();
        viewModel.mydata.notifySubscribers();
    };
}

function IsImageTypeUri(uri) {
    var parts = uri.split('.');
    var ext = parts[parts.length - 1];
    if (ext === "jpg" || ext === "tif")
        return true;
    return false;
}
function appendTriple(id, property, objvalue) {
    var newkey = id + "." + property;
    //   alert(newkey+' '+objvalue);
    var array = GetItemById(id, property);
    if (array === null) {
        array = ko.observableArray([objvalue]);
        ko.utils.arrayPushAll(viewModel.mydata, [{ key: newkey, value: array }]);
    } else if (array.indexOf(objvalue) === -1) ko.utils.arrayPushAll(array, [objvalue]);
}

function Addtriple(id, property, objvalue, isObjectIri) {
    //   alert(id+" "+ property +" "+ objvalue);
    appendTriple(id, property, objvalue);
    if (isObjectIri)
        appendTriple(objvalue, "^" + property, id);
    //viewModel.notifySubscribers();
   // viewModel.notification.notifySubscribers();
}
$(function () {
    //  ko.punches.enableAll();

    viewModel = {
        notification: ko.observable(),

        id: ko.observable("cassetterootcollection"),
        mydata: ko.observableArray()
    }
});