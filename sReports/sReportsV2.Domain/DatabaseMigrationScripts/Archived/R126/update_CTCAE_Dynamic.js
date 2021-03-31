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