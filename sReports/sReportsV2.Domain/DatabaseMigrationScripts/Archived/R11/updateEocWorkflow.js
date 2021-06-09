db.episodeofcareentity.find().forEach(function (entity) {
	if (entity.ListHistoryStatus) {
		let workflows = [];
		entity.ListHistoryStatus.forEach(function (status) {
			let workflow = {}
			workflow["Status"] = status.StatusValue;
			workflow["DiagnosisCondition"] = entity.DiagnosisCondition;
			workflow["DiagnosisRole"] = entity.DiagnosisRole;
			workflow["User"] = "5c2e7bdf91fa2c1380b3a6eb";
			workflow["Submited"] = entity.EntryDatetime;
			workflows.push(workflow);
		});
		db.episodeofcareentity.updateOne({ _id: entity._id }, { $set: { WorkflowHistory: workflows } });
		db.episodeofcareentity.update({}, { $unset: { ListHistoryStatus: 1 } }, false, true);
	}
});