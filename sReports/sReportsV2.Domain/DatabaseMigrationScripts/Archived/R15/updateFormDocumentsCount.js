db.form.find({}).forEach(function (f) {
    var count = db.forminstance.count({ FormDefinitionId: f._id.str });
    db.form.updateOne({ '_id': f._id }, { $set: { 'DocumentsCount': count } });
});