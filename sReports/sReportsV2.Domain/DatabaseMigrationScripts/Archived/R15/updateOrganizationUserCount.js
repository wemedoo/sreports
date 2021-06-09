db.organizationentity.find({}).forEach(function (f) {
	var count = 0;
	db.users.find({}).forEach(function (u) {
		u.OrganizationRefs.forEach(function (o) {
			if (f._id.str == o) {
				count++;
			}
		})
	})
	db.organizationentity.updateOne({ '_id': f._id }, { $set: { 'NumOfUsers': count } });
});