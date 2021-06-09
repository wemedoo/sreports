db.users.find().forEach(function (u) {
	db.users.updateOne({ '_id': u._id }, { $set: { 'ClinicalTrials': [] } });
});