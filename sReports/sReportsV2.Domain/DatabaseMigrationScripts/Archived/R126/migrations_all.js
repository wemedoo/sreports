************************************************************************** MIGRATE TYPE FIELD **************************************************************************
	db.form.find({}).forEach(function (f) {
		f.Chapters.forEach(function (chapter) {
			chapter.Pages.forEach(function (page) {
				page.FieldSets.forEach(function (fs) {
					fs.Fields.forEach(function (field) {
						field['_t'] = field.Type
					})
				})
			})
		})
		db.form.replaceOne(
			{ _id: f._id },
			f
		)
	})

db.forminstance.find({}).forEach(function (f) {
	f.Chapters.forEach(function (chapter) {
		chapter.Pages.forEach(function (page) {
			page.FieldSets.forEach(function (fs) {
				fs.Fields.forEach(function (field) {
					field['_t'] = field.Type
				})
			})
		})
	})
	db.forminstance.replaceOne(
		{ _id: f._id },
		f
	)
})
************************************************************************** END MIGRATE TYPE FIELD **************************************************************************

************************************************************************** MIGRATE VERSION FIELDS **************************************************************************


	db.form.updateMany(
		{},
		{ $set: { "Version": { Major: 1, Minor: 1, _id: new ObjectId().str } } })


db.forminstance.updateMany(
	{},
	{ $set: { "Version": { Major: 1, Minor: 1, _id: new ObjectId().str } } })

db.form.updateMany(
	{},
	{
		$set: { "State": 2 }
	}
)

************************************************************************** END MIGRATE VERSION FIELDS **************************************************************************

************************************************************************** MIGRATE REPETITIVE FIELDS **************************************************************************

	db.formvalues.find({}).forEach(function (formValue) {
		formValue.Values.forEach(function (field) {
			field.Value = [field.Value];
		})
		db.formvalues.updateOne({ _id: formValue._id }, { $set: { Values: formValue.Values } });
	});

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
					});
				});
			});
		});
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
			});
		});
	});
	db.form.updateOne({ _id: myDoc._id }, { $set: { Chapters: myDoc.Chapters } });
});

///////////////////////////////////////////

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
				});
			});
		});
	});
	db.forminstance.updateOne({ _id: myDoc._id }, { $set: { Chapters: myDoc.Chapters } });
});

db.forminstance.find({}).forEach(function (myDoc) {
	myDoc.Chapters.forEach(function (chapter) {
		chapter.Pages.forEach(function (page) {
			page['ListOfFieldSets'] = [];
			page.FieldSets.forEach(function (fieldSet) {
				let list = [fieldSet];
				page['ListOfFieldSets'].push(list);
			});
		});
	});
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
				});
			});
		});
	});
});
************************************************************************** END MIGRATE REPETITIVE FIELDS **************************************************************************
************************************************************************** START MIGRATE CTCAE DYNAMIC FORM **************************************************************************
    //FIRST STEP - Remove existing chapter
    db.form.find({ Title: "CTCAE Dynamic" }).forEach(function (form) {
        db.form.update(
            {},
            { $pull: { "Chapters": { Title: "Chapter 1" } } },
            false,
            true
        );
    })

//SECOND STEP - Add new chapter
var ctcaeChapter =
{
    "_id": "c1",
    "Title": "Chapter 1",
    "Description": null,
    "ThesaurusId": "15121",
    "IsReadonly": false,
    "Pages": [
        {
            "_id": "c1p1",
            "Title": "Page 1",
            "IsVisible": false,
            "Description": null,
            "ThesaurusId": "15122",
            "ImageMap": null,
            "ListOfFieldSets": [
                [
                    {
                        "FhirType": null,
                        "_id": "fs_1",
                        "Label": null,
                        "Description": null,
                        "ThesaurusId": "15123",
                        "Fields": [
                            {
                                "MinLength": null,
                                "MaxLength": null,
                                "IsRepetitive": false,
                                "RepetitiveValue": null,
                                "NumberOfRepetitions": 0,
                                "InstanceId": null,
                                "FhirType": "Observation",
                                "_id": "1",
                                "Value": [],
                                "_t": "text",
                                "Label": "MedDRA Code",
                                "Description": null,
                                "Unit": null,
                                "ThesaurusId": "15124",
                                "IsVisible": true,
                                "IsReadonly": false,
                                "IsRequired": false,
                                "IsBold": false,
                                "Help": null,
                                "IsHiddenOnPdf": false
                            },
                            {
                                "MinLength": null,
                                "MaxLength": null,
                                "IsRepetitive": false,
                                "RepetitiveValue": null,
                                "NumberOfRepetitions": 0,
                                "InstanceId": null,
                                "FhirType": "Observation",
                                "_id": "2",
                                "Value": [],
                                "_t": "text",
                                "Label": "CTCAE term",
                                "Description": null,
                                "Unit": null,
                                "ThesaurusId": "15125",
                                "IsVisible": true,
                                "IsReadonly": false,
                                "IsRequired": false,
                                "IsBold": false,
                                "Help": null,
                                "IsHiddenOnPdf": false
                            },
                            {
                                "Values": [
                                    {
                                        "Label": "Grade 1",
                                        "Value": "Grade 1",
                                        "ThesaurusId": "15127"
                                    },
                                    {
                                        "Label": "Grade 2",
                                        "Value": "Grade 2",
                                        "ThesaurusId": "15128"
                                    },
                                    {
                                        "Label": "Grade 3",
                                        "Value": "Grade 3",
                                        "ThesaurusId": "15129"
                                    },
                                    {
                                        "Label": "Grade 4",
                                        "Value": "Grade 4",
                                        "ThesaurusId": "15130"
                                    }
                                ],
                                "Dependables": [],
                                "InstanceId": null,
                                "FhirType": "Observation",
                                "_id": "3",
                                "Value": [],
                                "_t": "radio",
                                "Label": "Grade",
                                "Description": null,
                                "Unit": null,
                                "ThesaurusId": "15126",
                                "IsVisible": true,
                                "IsReadonly": false,
                                "IsRequired": false,
                                "IsBold": false,
                                "Help": null,
                                "IsHiddenOnPdf": false
                            },
                            {
                                "MinLength": null,
                                "MaxLength": null,
                                "IsRepetitive": false,
                                "RepetitiveValue": null,
                                "NumberOfRepetitions": 0,
                                "InstanceId": null,
                                "FhirType": "Observation",
                                "_id": "7",
                                "Value": [],
                                "_t": "text",
                                "Label": "Grade description",
                                "Description": null,
                                "Unit": null,
                                "ThesaurusId": "15135",
                                "IsVisible": true,
                                "IsReadonly": false,
                                "IsRequired": false,
                                "IsBold": false,
                                "Help": null,
                                "IsHiddenOnPdf": false
                            }
                        ],
                        "LayoutStyle": null,
                        "IsBold": true,
                        "MapAreaId": null,
                        "Help": null,
                        "IsRepetitive": true,
                        "NumberOfRepetitions": 0,
                        "InstanceId": null
                    }
                ],
                [
                    {
                        "FhirType": null,
                        "_id": "fs_2",
                        "Label": null,
                        "Description": null,
                        "ThesaurusId": "15131",
                        "Fields": [
                            {
                                "MinLength": null,
                                "MaxLength": null,
                                "IsRepetitive": false,
                                "RepetitiveValue": null,
                                "NumberOfRepetitions": 0,
                                "InstanceId": null,
                                "FhirType": "Observation",
                                "_id": "4",
                                "Value": [],
                                "_t": "text",
                                "Label": "Patient ID",
                                "Description": null,
                                "Unit": null,
                                "ThesaurusId": "15132",
                                "IsVisible": true,
                                "IsReadonly": false,
                                "IsRequired": false,
                                "IsBold": false,
                                "Help": null,
                                "IsHiddenOnPdf": false
                            },
                            {
                                "MinLength": null,
                                "MaxLength": null,
                                "IsRepetitive": false,
                                "RepetitiveValue": null,
                                "NumberOfRepetitions": 0,
                                "InstanceId": null,
                                "FhirType": "Observation",
                                "_id": "5",
                                "Value": [],
                                "_t": "text",
                                "Label": "Visit No",
                                "Description": null,
                                "Unit": null,
                                "ThesaurusId": "15133",
                                "IsVisible": true,
                                "IsReadonly": false,
                                "IsRequired": false,
                                "IsBold": false,
                                "Help": null,
                                "IsHiddenOnPdf": false
                            },
                            {
                                "IsRepetitive": false,
                                "RepetitiveValue": null,
                                "NumberOfRepetitions": 0,
                                "InstanceId": null,
                                "FhirType": "Observation",
                                "_id": "6",
                                "Value": [],
                                "_t": "date",
                                "Label": "Date",
                                "Description": null,
                                "Unit": null,
                                "ThesaurusId": "15134",
                                "IsVisible": true,
                                "IsReadonly": false,
                                "IsRequired": false,
                                "IsBold": false,
                                "Help": null,
                                "IsHiddenOnPdf": false
                            },
                            {
                                "MinLength": null,
                                "MaxLength": null,
                                "IsRepetitive": false,
                                "RepetitiveValue": null,
                                "NumberOfRepetitions": 0,
                                "InstanceId": null,
                                "FhirType": "Observation",
                                "_id": "8",
                                "Value": [],
                                "_t": "text",
                                "Label": "Selected value",
                                "Description": null,
                                "Unit": null,
                                "ThesaurusId": "15136",
                                "IsVisible": true,
                                "IsReadonly": false,
                                "IsRequired": false,
                                "IsBold": false,
                                "Help": null,
                                "IsHiddenOnPdf": false
                            }
                        ],
                        "LayoutStyle": null,
                        "IsBold": true,
                        "MapAreaId": null,
                        "Help": null,
                        "IsRepetitive": false,
                        "NumberOfRepetitions": 0,
                        "InstanceId": null
                    }
                ]
            ],
            "LayoutStyle": null
        }
    ]
}

//LAST STEP - Add new chapter
db.form.find({ Title: "CTCAE Dynamic" }).forEach(function (form) {
    form.Chapters.push(ctcaeChapter)
    db.form.updateOne({ _id: form._id }, { $set: { Chapters: form.Chapters } })
})

************************************************************************** END MIGRATE CTCAE DYNAMIC FORM **************************************************************************

