db.users.find().forEach(function (u) {
	var organization = {};
	organization["OrganizationRef"] = u["ActiveOrganization"];
	organization["State"] = 0;
	db.users.updateOne({ '_id': u._id }, { $set: { 'Organizations': [organization] } });
});