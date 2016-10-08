hub.client.get = function (casssetesList) {
    viewModel.list = casssetesList;
    //$.each(casssetesList, function(i, item) {
    //    viewModel.list.arrayPushAll(ko.mapping.fromJS(item));
    //});
    if (!binded) {
        ko.applyBindings(viewModel);
        binded = true;
    }
};
hub.client.getCasssetteContent = function (resultJson) {
}

hub.client.updateCassetteStatus = function (cassette) {
    $.find(viewModel.list, function (cas) {
        return cas.Path === cassette.Path;
    }).forEach(function (numb, item) {
        item.Status = cassette.Status;
        alert(item.Status);
    });
}
function Count() {
    viewModel.notification();
    return ko.pureComputed(function () {
        viewModel.notification();
        return viewModel.mydata.length;
    });
}
function selectCassette(cassetteModel) {
    //hub.server.CallSparql(cassetteModel,
    //    "Select ?id ?name ?uri "
    //    + "{ ?id a ?type ;" +
    //    //"<uri>  ?uri;" +
    //    "!^<collection-item> ?anyCollection ." +
    //    "}");
    //    hub.server.getItem(cassetteModel, viewModel.id);
}