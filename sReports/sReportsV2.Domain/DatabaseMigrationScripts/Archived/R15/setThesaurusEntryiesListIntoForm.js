db.form.find({}).forEach(function (form) {
	let allThesauruses = [];
	if (form.ThesaurusId) {
		allThesauruses.push(form.ThesaurusId);
	}
	if (form.Chapters) {
		form.Chapters.forEach(function (chapter) {
			if (chapter.ThesaurusId) {
				allThesauruses.push(chapter.ThesaurusId);
			}

			if (chapter.Pages) {

				chapter.Pages.forEach(function (page) {
					if (page.ThesaurusId) {
						allThesauruses.push(page.ThesaurusId);
					}
					if (page.ListOfFieldSets) {
						page.ListOfFieldSets.forEach(function (listOfFs) {
							listOfFs.forEach(function (fieldSet) {
								if (fieldSet.ThesaurusId) {
									allThesauruses.push(fieldSet.ThesaurusId);
									if (fieldSet.Fields) {
										fieldSet.Fields.forEach(function (field) {
											if (field.ThesaurusId) {
												allThesauruses.push(field.ThesaurusId);
											}
											if (field.Values) {
												field.Values.forEach(function (value) {
													if (value.ThesaurusId) {
														allThesauruses.push(value.ThesaurusId);
													}
												})
											}
										})
									}
								}
							})
						})
					}
				})
			}
		})
	}
	form.ThesaurusList = allThesauruses;
	db.form.update({ _id: form._id }, {
		$set: { ThesaurusEntriesList: allThesauruses }
	});
})