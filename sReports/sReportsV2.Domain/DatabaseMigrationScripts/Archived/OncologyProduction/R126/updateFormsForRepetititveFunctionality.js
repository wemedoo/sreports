db.form.find({}).forEach(function (myDoc) {
	myDoc.Chapters.forEach(function (chapter) {
		chapter._id = chapter._id.replace(/-/g, '_');
		chapter.Pages.forEach(function (page) {
			page._id = page._id.replace(/-/g, '_');
			page.FieldSets.forEach(function (fieldSet) {
				if (fieldSet._id == null) {
					fieldSet._id = "fs_1";
				} else {
					fieldSet._id = fieldSet._id.replace(/-/g, '_');
				}
				fieldSet.Fields.forEach(function (field) {
					field._id = field._id.replace(/-/g, '_');
					field.Dependables.forEach(function (dependable) {
						if (dependable.ActionParams != null) {
							dependable.ActionParams = dependable.ActionParams.replace(/-/g, '_');
						}
					})
					if (field.IsRepetitive) {
						field.Value = field.RepetitiveValue;
					} else {
						if (field.Value != null && field.Value != "") {
							field.Value = [field.Value];
						} else {
							field.Value = null;
						}
					}
				})
			})
		})
	})
	db.form.updateOne({ _id: myDoc._id }, { $set: { Chapters: myDoc.Chapters } });
});

//creating ListOfFieldSets in page and add every field set from fieldSets as list of 1 element

db.form.find({}).forEach(function (myDoc) {
	myDoc.Chapters.forEach(function (chapter) {
		chapter.Pages.forEach(function (page) {
			page['ListOfFieldSets'] = [];
			page.FieldSets.forEach(function (fieldSet) {
				let list = [fieldSet];
				page['ListOfFieldSets'].push(list);
			})
		})
	})
	db.form.updateOne({ _id: myDoc._id }, { $set: { Chapters: myDoc.Chapters } });
});



////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


db.forminstance.find({}).forEach(function (myDoc) {
	myDoc.Chapters.forEach(function (chapter) {
		chapter._id = chapter._id.replace(/-/g, '_');
		chapter.Pages.forEach(function (page) {
			page._id = page._id.replace(/-/g, '_');
			page.FieldSets.forEach(function (fieldSet) {
				if (fieldSet._id == null) {
					fieldSet._id = "fs_1";
				} else {
					fieldSet._id = fieldSet._id.replace(/-/g, '_');
				}
				fieldSet.Fields.forEach(function (field) {
					field._id = field._id.replace(/-/g, '_');
					field.Dependables.forEach(function (dependable) {
						if (dependable.ActionParams != null) {
							dependable.ActionParams = dependable.ActionParams.replace(/-/g, '_');
						}
					})
					if (field.IsRepetitive) {
						field.Value = field.RepetitiveValue;
					} else {
						if (field.Value != null && field.Value != "") {
							field.Value = [field.Value];
						} else {
							field.Value = null;
						}
					}
				})
			})
		})
	})
	db.forminstance.updateOne({ _id: myDoc._id }, { $set: { Chapters: myDoc.Chapters } });
});

db.forminstance.find({}).forEach(function (myDoc) {
	myDoc.Chapters.forEach(function (chapter) {
		chapter.Pages.forEach(function (page) {
			page['ListOfFieldSets'] = [];
			page.FieldSets.forEach(function (fieldSet) {
				let list = [fieldSet];
				page['ListOfFieldSets'].push(list);
			})
		})
	})
	db.forminstance.updateOne({ _id: myDoc._id }, { $set: { Chapters: myDoc.Chapters } });
});



//printing formId who contains fielSets with  id null

db.form.find({}).forEach(function (myDoc) {
	myDoc.Chapters.forEach(function (chapter) {
		chapter.Pages.forEach(function (page) {
			page.ListOfFieldSets.forEach(function (list) {
				list.forEach(function (fieldSet) {
					if (fieldSet._id == null) {
						print(myDoc._id);
					}
				})
			})
		})
	})

});

