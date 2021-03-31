db.form.find({ }).forEach(function (myDoc) {
	myDoc.Chapters.forEach(function (chapter) {
		if (chapter.Title == "Patient Info") {
			chapter.Pages.forEach(function (page) {
				page.ListOfFieldSets.forEach(function (list) {
					list.forEach(function (fieldSet) {
						fieldSet.Fields.forEach(function (field) {
							if (field._id === "343") {
								field.Dependables.forEach(function (dependable) {
									if (dependable.ActionParams == "351") {
										dependable.ActionParams = "801";
									}
									if (dependable.ActionParams == "352") {
										dependable.ActionParams = "372";
									}
								})
							}
						})
					})
				})
			})
		}

	})
	db.form.updateOne({ _id: myDoc._id }, { $set: { Chapters: myDoc.Chapters } });
});
