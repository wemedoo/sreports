db.getCollection("servicetype").aggregate([{ $out: 'predefinedtypes' }])

db.predefinedtypes.updateMany(
	{},
	{
		$set: {
			"Type": "ServiceType"
		}
	}
)

db.predefinedtypes.insertMany([
	{ ThesaurusId: "12208", IsDeleted: false, Type: "EncounterType" },
	{ ThesaurusId: "12209", IsDeleted: false, Type: "EncounterType" },
	{ ThesaurusId: "12210", IsDeleted: false, Type: "EncounterType" },
	{ ThesaurusId: "12211", IsDeleted: false, Type: "EncounterType" },
	{ ThesaurusId: "12212", IsDeleted: false, Type: "EncounterType" }
]);

db.predefinedtypes.insertMany([
	{ ThesaurusId: "12227", IsDeleted: false, Type: "DiagnosisRole" },
	{ ThesaurusId: "12226", IsDeleted: false, Type: "DiagnosisRole" },
	{ ThesaurusId: "12225", IsDeleted: false, Type: "DiagnosisRole" },
	{ ThesaurusId: "12224", IsDeleted: false, Type: "DiagnosisRole" },
	{ ThesaurusId: "12223", IsDeleted: false, Type: "DiagnosisRole" },
	{ ThesaurusId: "12222", IsDeleted: false, Type: "DiagnosisRole" },
	{ ThesaurusId: "12221", IsDeleted: false, Type: "DiagnosisRole" },
	{ ThesaurusId: "12220", IsDeleted: false, Type: "DiagnosisRole" }
]);

db.predefinedtypes.insertMany([
	{ ThesaurusId: "12236", IsDeleted: false, Type: "EncounterClassification" },
	{ ThesaurusId: "12237", IsDeleted: false, Type: "EncounterClassification" },
	{ ThesaurusId: "12238", IsDeleted: false, Type: "EncounterClassification" },
	{ ThesaurusId: "12239", IsDeleted: false, Type: "EncounterClassification" },
	{ ThesaurusId: "12240", IsDeleted: false, Type: "EncounterClassification" },
	{ ThesaurusId: "12241", IsDeleted: false, Type: "EncounterClassification" },
	{ ThesaurusId: "12242", IsDeleted: false, Type: "EncounterClassification" },
	{ ThesaurusId: "12243", IsDeleted: false, Type: "EncounterClassification" },
	{ ThesaurusId: "12244", IsDeleted: false, Type: "EncounterClassification" },
	{ ThesaurusId: "12245", IsDeleted: false, Type: "EncounterClassification" },
	{ ThesaurusId: "12246", IsDeleted: false, Type: "EncounterClassification" },
	{ ThesaurusId: "12247", IsDeleted: false, Type: "EncounterClassification" }

]);

db.predefinedtypes.insertMany([
	{ ThesaurusId: "12213", IsDeleted: false, Type: "EncounterStatus" },
	{ ThesaurusId: "12214", IsDeleted: false, Type: "EncounterStatus" },
	{ ThesaurusId: "12215", IsDeleted: false, Type: "EncounterStatus" },
	{ ThesaurusId: "12216", IsDeleted: false, Type: "EncounterStatus" },
	{ ThesaurusId: "12217", IsDeleted: false, Type: "EncounterStatus" },
	{ ThesaurusId: "12218", IsDeleted: false, Type: "EncounterStatus" },
	{ ThesaurusId: "12219", IsDeleted: false, Type: "EncounterStatus" }

]);

db.predefinedtypes.insertMany([
	{ ThesaurusId: "12248", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12249", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12250", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12251", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12252", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12253", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12254", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12255", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12256", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12257", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12258", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12259", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12260", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12261", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12262", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12263", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12264", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12265", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12266", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12267", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12268", IsDeleted: false, Type: "OrganizationType" },
	{ ThesaurusId: "12269", IsDeleted: false, Type: "OrganizationType" }
]);

db.predefinedtypes.insertMany([
	{ ThesaurusId: "10489", IsDeleted: false, Type: "EpisodeOfCareType" },
	{ ThesaurusId: "10490", IsDeleted: false, Type: "EpisodeOfCareType" },
	{ ThesaurusId: "10491", IsDeleted: false, Type: "EpisodeOfCareType" },
	{ ThesaurusId: "10492", IsDeleted: false, Type: "EpisodeOfCareType" },
	{ ThesaurusId: "10493", IsDeleted: false, Type: "EpisodeOfCareType" },
	{ ThesaurusId: "10494", IsDeleted: false, Type: "EpisodeOfCareType" },
	{ ThesaurusId: "10495", IsDeleted: false, Type: "EpisodeOfCareType" },
	{ ThesaurusId: "10496", IsDeleted: false, Type: "EpisodeOfCareType" },
	{ ThesaurusId: "10497", IsDeleted: false, Type: "EpisodeOfCareType" },
	{ ThesaurusId: "10498", IsDeleted: false, Type: "EpisodeOfCareType" },
	{ ThesaurusId: "10499", IsDeleted: false, Type: "EpisodeOfCareType" },
	{ ThesaurusId: "10500", IsDeleted: false, Type: "EpisodeOfCareType" },
	{ ThesaurusId: "10501", IsDeleted: false, Type: "EpisodeOfCareType" }
]);
