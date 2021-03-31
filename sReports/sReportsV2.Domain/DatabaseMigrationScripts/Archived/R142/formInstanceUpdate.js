/************************************************************************** START MIGRATE FORM INSTANCE FIELDS ***************************************************************************/
db.forminstance.find({}).forEach(function (formInstance) {
	var fields = [];
	formInstance.Chapters.forEach(function (chapter) {
		chapter.Pages.forEach(function (page) {
			page.ListOfFieldSets.forEach(function (list) {
				list.forEach(function (fieldSet) {
					var count = 0;
					fieldSet.Fields.forEach(function (field) {
						if (field.InstanceId != null) {
							var f = {
								_id: field._id,
								ThesaurusId: field.ThesaurusId,
								Value: field.Value,
								InstanceId: field.InstanceId,
								Type: ""
							};
							fields.push(f);
						} else {
							var instanceId = `${fieldSet._id}-${count}-${field._id}-1`;
							var ff = {
								_id: field._id,
								ThesaurusId: field.ThesaurusId,
								Value: field.Value,
								InstanceId: instanceId,
								Type: ""
							};
							fields.push(ff);
						}
					});
					count++;
				});
			});
		});
	});
	db.forminstance.updateOne({ _id: formInstance._id }, { $set: { Fields: fields } });
});

db.forminstance.update({}, { $unset: { Chapters: 1 } }, false, true);


/************************************************************************** END MIGRATE FORM INSTANCE FIELDS *******************************************************************************************/

/************************************************************************** MIGRATE FORM VALUE ID INTO FORM INSTANCE FOR DFD **************************************************************************/
db.formvalues.find({}).forEach(function (fv) {
	var formInstance = db.forminstance.findOne({ _id: new ObjectId(fv.FormInstanceId) });
	if (formInstance) {
		fv.Values.forEach(function (field) {
			let formInstanceField = formInstance.Fields.find(x => x._id == field._id);
			if (formInstanceField) {
				formInstanceField.Value = field.Value;
			} else {
				print("NOT FOUND!!!!!!!!!!!!!!!!!!");
			}
		});

		print(formInstance);
		db.forminstance.remove({ _id: formInstance._id });
		formInstance._id = fv._id;

		db.forminstance.insert(formInstance);
	}
});

/************************************************************************** MIGRATE FORM VALUE ID INTO FORM INSTANCE FOR DFD **************************************************************************/
