db.form.find({}).forEach(function (f) {
	if (f.DocumentProperties) {
		if (f.DocumentProperties.Class || f.DocumentProperties.Class == 0) {
			f.DocumentProperties.Class = {
				"Class": f.DocumentProperties.Class,
				"Other": ""
			}
			db.form.updateOne({ _id: f._id }, { $set: { DocumentProperties: f.DocumentProperties } })
		}
	}
})

db.forminstance.find({}).forEach(function (f) {
	if (f.DocumentProperties) {
		if (f.DocumentProperties.Class || f.DocumentProperties.Class == 0) {
			f.DocumentProperties.Class = {
				"Class": f.DocumentProperties.Class,
				"Other": ""
			}
			db.forminstance.updateOne({ _id: f._id }, { $set: { DocumentProperties: f.DocumentProperties } })
		}
	}
})