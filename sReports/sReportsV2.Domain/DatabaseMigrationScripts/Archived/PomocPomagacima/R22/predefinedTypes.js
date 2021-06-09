
db.diagnosisrole.updateMany({}, { $set: { OrganizationRef: "5da991e123460818040d8a50" } });
db.encounterclassification.updateMany({}, { $set: { OrganizationRef: "5da991e123460818040d8a50" } });
db.encounterstatus.updateMany({}, { $set: { OrganizationRef: "5da991e123460818040d8a50" } });
db.encountertypes.updateMany({}, { $set: { OrganizationRef: "5da991e123460818040d8a50" } });
db.episodeofcaretypes.updateMany({}, { $set: { OrganizationRef: "5da991e123460818040d8a50" } });
db.organizationtype.updateMany({}, { $set: { OrganizationRef: "5da991e123460818040d8a50" } });
db.servicetype.updateMany({}, { $set: { OrganizationRef: "5da991e123460818040d8a50" } });
db.identifiertype.updateMany({}, { $set: { OrganizationRef: "5da991e123460818040d8a50" } });


db.diagnosisrole.updateMany({}, { $set: { EntryDatetime: new Date("2020-04-08T10:27:08.131+00:00") } });
db.encounterclassification.updateMany({}, { $set: { EntryDatetime: new Date("2020-04-08T10:27:08.131+00:00") } });
db.encounterstatus.updateMany({}, { $set: { EntryDatetime: new Date("2020-04-08T10:27:08.131+00:00") } });
db.encountertypes.updateMany({}, { $set: { EntryDatetime: new Date("2020-04-08T10:27:08.131+00:00") } });
db.episodeofcaretypes.updateMany({}, { $set: { EntryDatetime: new Date("2020-04-08T10:27:08.131+00:00") } });
db.organizationtype.updateMany({}, { $set: { EntryDatetime: new Date("2020-04-08T10:27:08.131+00:00") } });
db.servicetype.updateMany({}, { $set: { EntryDatetime: new Date("2020-04-08T10:27:08.131+00:00") } });
db.identifiertype.updateMany({}, { $set: { EntryDatetime: new Date("2020-04-08T10:27:08.131+00:00") } });

db.identifiertype.updateMany({}, { $set: { IsDeleted: false } });
db.identifiertype.updateMany({}, { $rename: { "O4MtId": "ThesaurusId" } })
db.identifiertype.updateMany({}, { $unset: { Name: 1 } }, false, true);

db.servicetype.update({}, { $unset: { Display: 1 } }, false, true);
db.servicetype.update({}, { $unset: { Definition: 1 } }, false, true);