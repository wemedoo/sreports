db.encounterentity.find({}).forEach(function(encounter){
	let eoc = db.episodeofcareentity.findOne({_id: encounter.EpisodeOfCareId});
	if(eoc && eoc.PatientId){
		print(eoc.PatientId);
		db.encounterentity.updateOne({_id: encounter._id}, {$set:{ "PatientId" : eoc.PatientId}})
	}
})