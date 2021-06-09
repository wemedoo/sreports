

db.forminstance.find().forEach(function (entity) {
	var ticks;
	var ticksLastUpdate;
	try {
		ticks = ((entity.EntryDatetime.getTime() * 10000) + 621355968000000000);
		ticksLastUpdate = ((entity.LastUpdate.getTime() * 10000) + 621355968000000000);
		print(ticks);
		print(ticksLastUpdate);
		db.forminstance.updateOne({ _id: entity._id }, { $set: { "EntryDatetime": { "DateTime": ISODate(entity.EntryDatetime.toISOString()), "Ticks": NumberLong(ticks) } } });
		db.forminstance.updateOne({ _id: entity._id }, { $set: { "LastUpdate": { "DateTime": ISODate(entity.LastUpdate.toISOString()), "Ticks": NumberLong(ticksLastUpdate) } } });
	}
	catch{
		var date = new Date();
		ticks = ((date.getTime() * 10000) + 621355968000000000);
		ticksLastUpdate = ((date.getTime() * 10000) + 621355968000000000);
		print(ticks);
		print(ticksLastUpdate);
		db.forminstance.updateOne({ _id: entity._id }, { $set: { "EntryDatetime": { "DateTime": ISODate(date.toISOString()), "Ticks": NumberLong(ticks) } } });
		db.forminstance.updateOne({ _id: entity._id }, { $set: { "LastUpdate": { "DateTime": ISODate(date.toISOString()), "Ticks": NumberLong(ticksLastUpdate) } } });
	}
});

db.dfdforminfo.find().forEach(function (entity) {
	var ticks;
	var ticksLastUpdate;
	try {
		ticks = ((entity.EntryDatetime.getTime() * 10000) + 621355968000000000);
		ticksLastUpdate = ((entity.LastUpdate.getTime() * 10000) + 621355968000000000);
		print(ticks);
		print(ticksLastUpdate);
		db.dfdforminfo.updateOne({ _id: entity._id }, { $set: { "EntryDatetime": { "DateTime": ISODate(entity.EntryDatetime.toISOString()), "Ticks": NumberLong(ticks) } } });
		db.dfdforminfo.updateOne({ _id: entity._id }, { $set: { "LastUpdate": { "DateTime": ISODate(entity.LastUpdate.toISOString()), "Ticks": NumberLong(ticksLastUpdate) } } });
	}
	catch{
		var date = new Date();
		ticks = ((date.getTime() * 10000) + 621355968000000000);
		ticksLastUpdate = ((date.getTime() * 10000) + 621355968000000000);
		print(ticks);
		print(ticksLastUpdate);
		db.dfdforminfo.updateOne({ _id: entity._id }, { $set: { "EntryDatetime": { "DateTime": ISODate(date.toISOString()), "Ticks": NumberLong(ticks) } } });
		db.dfdforminfo.updateOne({ _id: entity._id }, { $set: { "LastUpdate": { "DateTime": ISODate(date.toISOString()), "Ticks": NumberLong(ticksLastUpdate) } } });
	}
});

db.encounterentity.find().forEach(function (entity) {
	var ticks;
	var ticksLastUpdate;
	try {
		ticks = ((entity.EntryDatetime.getTime() * 10000) + 621355968000000000);
		ticksLastUpdate = ((entity.LastUpdate.getTime() * 10000) + 621355968000000000);
		print(ticks);
		print(ticksLastUpdate);
		db.encounterentity.updateOne({ _id: entity._id }, { $set: { "EntryDatetime": { "DateTime": ISODate(entity.EntryDatetime.toISOString()), "Ticks": NumberLong(ticks) } } });
		db.encounterentity.updateOne({ _id: entity._id }, { $set: { "LastUpdate": { "DateTime": ISODate(entity.LastUpdate.toISOString()), "Ticks": NumberLong(ticksLastUpdate) } } });
	}
	catch{
		var date = new Date();
		ticks = ((date.getTime() * 10000) + 621355968000000000);
		ticksLastUpdate = ((date.getTime() * 10000) + 621355968000000000);
		print(ticks);
		print(ticksLastUpdate);
		db.encounterentity.updateOne({ _id: entity._id }, { $set: { "EntryDatetime": { "DateTime": ISODate(date.toISOString()), "Ticks": NumberLong(ticks) } } });
		db.encounterentity.updateOne({ _id: entity._id }, { $set: { "LastUpdate": { "DateTime": ISODate(date.toISOString()), "Ticks": NumberLong(ticksLastUpdate) } } });
	}
});


db.episodeofcareentity.find().forEach(function (entity) {
	var ticks;
	var ticksLastUpdate;
	try {
		ticks = ((entity.EntryDatetime.getTime() * 10000) + 621355968000000000);
		ticksLastUpdate = ((entity.LastUpdate.getTime() * 10000) + 621355968000000000);
		print(ticks);
		print(ticksLastUpdate);
		db.episodeofcareentity.updateOne({ _id: entity._id }, { $set: { "EntryDatetime": { "DateTime": ISODate(entity.EntryDatetime.toISOString()), "Ticks": NumberLong(ticks) } } });
		db.episodeofcareentity.updateOne({ _id: entity._id }, { $set: { "LastUpdate": { "DateTime": ISODate(entity.LastUpdate.toISOString()), "Ticks": NumberLong(ticksLastUpdate) } } });
	}
	catch{
		var date = new Date();
		ticks = ((date.getTime() * 10000) + 621355968000000000);
		ticksLastUpdate = ((date.getTime() * 10000) + 621355968000000000);
		print(ticks);
		print(ticksLastUpdate);
		db.episodeofcareentity.updateOne({ _id: entity._id }, { $set: { "EntryDatetime": { "DateTime": ISODate(date.toISOString()), "Ticks": NumberLong(ticks) } } });
		db.episodeofcareentity.updateOne({ _id: entity._id }, { $set: { "LastUpdate": { "DateTime": ISODate(date.toISOString()), "Ticks": NumberLong(ticksLastUpdate) } } });
	}
});

db.form.find().forEach(function (entity) {
	var ticks;
	var ticksLastUpdate;
	try {
		ticks = ((entity.EntryDatetime.getTime() * 10000) + 621355968000000000);
		ticksLastUpdate = ((entity.LastUpdate.getTime() * 10000) + 621355968000000000);
		print(ticks);
		print(ticksLastUpdate);
		db.form.updateOne({ _id: entity._id }, { $set: { "EntryDatetime": { "DateTime": ISODate(entity.EntryDatetime.toISOString()), "Ticks": NumberLong(ticks) } } });
		db.form.updateOne({ _id: entity._id }, { $set: { "LastUpdate": { "DateTime": ISODate(entity.LastUpdate.toISOString()), "Ticks": NumberLong(ticksLastUpdate) } } });
	}
	catch{
		var date = new Date();
		ticks = ((date.getTime() * 10000) + 621355968000000000);
		ticksLastUpdate = ((date.getTime() * 10000) + 621355968000000000);
		print(ticks);
		print(ticksLastUpdate);
		db.form.updateOne({ _id: entity._id }, { $set: { "EntryDatetime": { "DateTime": ISODate(date.toISOString()), "Ticks": NumberLong(ticks) } } });
		db.form.updateOne({ _id: entity._id }, { $set: { "LastUpdate": { "DateTime": ISODate(date.toISOString()), "Ticks": NumberLong(ticksLastUpdate) } } });
	}
});


db.formdistribution.find().forEach(function (entity) {
	var ticks;
	var ticksLastUpdate;
	try {
		ticks = ((entity.EntryDatetime.getTime() * 10000) + 621355968000000000);
		ticksLastUpdate = ((entity.LastUpdate.getTime() * 10000) + 621355968000000000);
		print(ticks);
		print(ticksLastUpdate);
		db.formdistribution.updateOne({ _id: entity._id }, { $set: { "EntryDatetime": { "DateTime": ISODate(entity.EntryDatetime.toISOString()), "Ticks": NumberLong(ticks) } } });
		db.formdistribution.updateOne({ _id: entity._id }, { $set: { "LastUpdate": { "DateTime": ISODate(entity.LastUpdate.toISOString()), "Ticks": NumberLong(ticksLastUpdate) } } });
	}
	catch{
		var date = new Date();
		ticks = ((date.getTime() * 10000) + 621355968000000000);
		ticksLastUpdate = ((date.getTime() * 10000) + 621355968000000000);
		print(ticks);
		print(ticksLastUpdate);
		db.formdistribution.updateOne({ _id: entity._id }, { $set: { "EntryDatetime": { "DateTime": ISODate(date.toISOString()), "Ticks": NumberLong(ticks) } } });
		db.formdistribution.updateOne({ _id: entity._id }, { $set: { "LastUpdate": { "DateTime": ISODate(date.toISOString()), "Ticks": NumberLong(ticksLastUpdate) } } });
	}
});

db.organizationentity.find().forEach(function (entity) {
	var ticks;
	var ticksLastUpdate;
	try {
		ticks = ((entity.EntryDatetime.getTime() * 10000) + 621355968000000000);
		ticksLastUpdate = ((entity.LastUpdate.getTime() * 10000) + 621355968000000000);
		print(ticks);
		print(ticksLastUpdate);
		db.organizationentity.updateOne({ _id: entity._id }, { $set: { "EntryDatetime": { "DateTime": ISODate(entity.EntryDatetime.toISOString()), "Ticks": NumberLong(ticks) } } });
		db.organizationentity.updateOne({ _id: entity._id }, { $set: { "LastUpdate": { "DateTime": ISODate(entity.LastUpdate.toISOString()), "Ticks": NumberLong(ticksLastUpdate) } } });
	}
	catch{
		var date = new Date();
		ticks = ((date.getTime() * 10000) + 621355968000000000);
		ticksLastUpdate = ((date.getTime() * 10000) + 621355968000000000);
		print(ticks);
		print(ticksLastUpdate);
		db.organizationentity.updateOne({ _id: entity._id }, { $set: { "EntryDatetime": { "DateTime": ISODate(date.toISOString()), "Ticks": NumberLong(ticks) } } });
		db.organizationentity.updateOne({ _id: entity._id }, { $set: { "LastUpdate": { "DateTime": ISODate(date.toISOString()), "Ticks": NumberLong(ticksLastUpdate) } } });
	}
});


db.patiententity.find().forEach(function (entity) {
	var ticks;
	var ticksLastUpdate;
	try {
		ticks = ((entity.EntryDatetime.getTime() * 10000) + 621355968000000000);
		ticksLastUpdate = ((entity.LastUpdate.getTime() * 10000) + 621355968000000000);
		print(ticks);
		print(ticksLastUpdate);
		db.patiententity.updateOne({ _id: entity._id }, { $set: { "EntryDatetime": { "DateTime": ISODate(entity.EntryDatetime.toISOString()), "Ticks": NumberLong(ticks) } } });
		db.patiententity.updateOne({ _id: entity._id }, { $set: { "LastUpdate": { "DateTime": ISODate(entity.LastUpdate.toISOString()), "Ticks": NumberLong(ticksLastUpdate) } } });
	}
	catch{
		var date = new Date();
		ticks = ((date.getTime() * 10000) + 621355968000000000);
		ticksLastUpdate = ((date.getTime() * 10000) + 621355968000000000);
		print(ticks);
		print(ticksLastUpdate);
		db.patiententity.updateOne({ _id: entity._id }, { $set: { "EntryDatetime": { "DateTime": ISODate(date.toISOString()), "Ticks": NumberLong(ticks) } } });
		db.patiententity.updateOne({ _id: entity._id }, { $set: { "LastUpdate": { "DateTime": ISODate(date.toISOString()), "Ticks": NumberLong(ticksLastUpdate) } } });
	}
});

db.thesaurusentry.find().forEach(function (entity) {
	var ticks;
	var ticksLastUpdate;
	try {
		ticks = ((entity.EntryDatetime.getTime() * 10000) + 621355968000000000);
		ticksLastUpdate = ((entity.LastUpdate.getTime() * 10000) + 621355968000000000);
		print(ticks);
		print(ticksLastUpdate);
		db.thesaurusentry.updateOne({ _id: entity._id }, { $set: { "EntryDatetime": { "DateTime": ISODate(entity.EntryDatetime.toISOString()), "Ticks": NumberLong(ticks) } } });
		db.thesaurusentry.updateOne({ _id: entity._id }, { $set: { "LastUpdate": { "DateTime": ISODate(entity.LastUpdate.toISOString()), "Ticks": NumberLong(ticksLastUpdate) } } });
	}
	catch{
		var date = new Date();
		ticks = ((date.getTime() * 10000) + 621355968000000000);
		ticksLastUpdate = ((date.getTime() * 10000) + 621355968000000000);
		print(ticks);
		print(ticksLastUpdate);
		db.thesaurusentry.updateOne({ _id: entity._id }, { $set: { "EntryDatetime": { "DateTime": ISODate(date.toISOString()), "Ticks": NumberLong(ticks) } } });
		db.thesaurusentry.updateOne({ _id: entity._id }, { $set: { "LastUpdate": { "DateTime": ISODate(date.toISOString()), "Ticks": NumberLong(ticksLastUpdate) } } });
	}
});

db.thesaurusmerge.find().forEach(function (entity) {
	var ticks;
	var ticksLastUpdate;
	try {
		ticks = ((entity.EntryDatetime.getTime() * 10000) + 621355968000000000);
		ticksLastUpdate = ((entity.LastUpdate.getTime() * 10000) + 621355968000000000);
		print(ticks);
		print(ticksLastUpdate);
		db.thesaurusmerge.updateOne({ _id: entity._id }, { $set: { "EntryDatetime": { "DateTime": ISODate(entity.EntryDatetime.toISOString()), "Ticks": NumberLong(ticks) } } });
		db.thesaurusmerge.updateOne({ _id: entity._id }, { $set: { "LastUpdate": { "DateTime": ISODate(entity.LastUpdate.toISOString()), "Ticks": NumberLong(ticksLastUpdate) } } });
	}
	catch{
		var date = new Date();
		ticks = ((date.getTime() * 10000) + 621355968000000000);
		ticksLastUpdate = ((date.getTime() * 10000) + 621355968000000000);
		print(ticks);
		print(ticksLastUpdate);
		db.thesaurusmerge.updateOne({ _id: entity._id }, { $set: { "EntryDatetime": { "DateTime": ISODate(date.toISOString()), "Ticks": NumberLong(ticks) } } });
		db.thesaurusmerge.updateOne({ _id: entity._id }, { $set: { "LastUpdate": { "DateTime": ISODate(date.toISOString()), "Ticks": NumberLong(ticksLastUpdate) } } });
	}
});



