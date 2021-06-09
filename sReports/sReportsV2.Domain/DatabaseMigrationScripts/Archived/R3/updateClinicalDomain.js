db.form.find({}).forEach(function (f) {
	if (f.DocumentProperties != null && f.DocumentProperties.ClinicalDomain != null && !Array.isArray(f.DocumentProperties.ClinicalDomain)) {
		let value = f.DocumentProperties.ClinicalDomain;
		f.DocumentProperties.ClinicalDomain = [];
		f.DocumentProperties.ClinicalDomain.push(value);
	}

	db.form.updateOne({ '_id': f._id }, { $set: { 'DocumentProperties': f.DocumentProperties } });
});

db.forminstance.find({}).forEach(function (f) {
	if (f.DocumentProperties != null && f.DocumentProperties.ClinicalDomain != null && !Array.isArray(f.DocumentProperties.ClinicalDomain)) {
		let value = f.DocumentProperties.ClinicalDomain;
		f.DocumentProperties.ClinicalDomain = [];
		f.DocumentProperties.ClinicalDomain.push(value);
	}

	db.forminstance.updateOne({ '_id': f._id }, { $set: { 'DocumentProperties': f.DocumentProperties } });
});