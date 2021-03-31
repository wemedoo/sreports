/************************************************************************** ADD ROLES FIELD TO USER **************************************************************************/
db.users.updateMany(
	{},
	{
		$set:{
			"Roles": ["Administrator"]
		}
	}
)
/************************************************************************** ADD ROLES FIELD TO USER **************************************************************************/

/************************************************************************** USER ADMINISTRATION *****************************************************************************/
	db.users.updateMany(
		{},
		{
			$set: {
				"Email": ""
			}
		}
	);

db.users.updateMany(
	{},
	{
		$set: {
			"ContactPhone": ""
		}
	}
);

db.users.updateMany(
	{},
	{
		$set: {
			"LastUpdate": new Date("2020-04-15T10:13:06.262+00:00")
		}
	}
);

db.roles.insertMany([
	{ Role: "Administrator", IsDeleted: false },
	{ Role: "Doctor", IsDeleted: false }
]);

/************************************************************************** USER ADMINISTRATION **************************************************************************/

/************************************************************************** MIGRATE EPISODE OF CARE TYPES ***************************************************************/
db.episodeofcaretypes.find({}).forEach(function (type) {
	db.episodeofcaretypes.updateOne({ _id: type._id }, { $set: { ThesaurusId: type.Code } });
});

db.episodeofcaretypes.find({}).forEach(function (type) {
	db.episodeofcaretypes.updateOne({ _id: type._id }, { $set: { IsDeleted: false } });
});

db.episodeofcaretypes.update({}, { $unset: { Display: 1 } }, false, true);
db.episodeofcaretypes.update({}, { $unset: { Code: 1 } }, false, true);
db.episodeofcaretypes.update({}, { $unset: { CodeSystem: 1 } }, false, true);
db.episodeofcaretypes.update({}, { $unset: { Definition: 1 } }, false, true);
db.episodeofcaretypes.update({}, { $unset: { FHIRResource: 1 } }, false, true);
/**************************************************************************END MIGRATE EPISODE OF CARE TYPES ***************************************************************/