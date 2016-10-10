

var hub = $.connection.cassettesHub;
hub.client.addtriple = Addtriple;
hub.client.addbuffer = AddBuffer;
$.connection.hub.start().done(function () {
    //hub.server.getCassetes();
    //hub.server.getItem(viewModel.id);
    //loadingStatus[viewModel.id] = false;

    ko.applyBindings(viewModel);
    hub.server.getAllDirect(directsRequests);
});
