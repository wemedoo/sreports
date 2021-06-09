db.organizationentity.find({}).forEach(function (o) {
	if (o.ClinicalDomain) {
		o.ClinicalDomain = [o.ClinicalDomain]
		db.organizationentity.replaceOne({ _id: o._id }, o)
	}
})