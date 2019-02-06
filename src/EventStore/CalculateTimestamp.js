function calculateTimestamp() {
    var context = getContext();
    var request = context.getRequest();

    // document to be created in the current operation
    var documentToCreate = request.getBody();

    // validate properties
    if (!("Timestamp" in documentToCreate)) {
        var ts = new Date();
        documentToCreate["Timestamp"] = ts.getTime();
    }

    // update the document that will be created
    request.setBody(documentToCreate);
}