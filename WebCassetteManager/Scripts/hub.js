

var hub = $.connection.cassettesHub;
hub.client.addtriple = Addtriple;
$.connection.hub.start().done(function () {
    //hub.server.getCassetes();
    //hub.server.getItem(viewModel.id);
    //loadingStatus[viewModel.id] = false;

    ko.applyBindings(viewModel);
});
