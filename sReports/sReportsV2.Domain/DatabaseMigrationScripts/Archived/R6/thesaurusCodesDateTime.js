db.thesaurusentry.find().forEach(function (entity) {
	if (entity.Codes) {
		let listCodes = [];
		entity.Codes.forEach(function (code) {
			print("test");
			let c = {};
			if (!code.VersionPublishDate) {
				code["VersionPublishDate"] = new Date(new Date().setHours(2, 0, 0, 0));
				let c = code;
				listCodes.push(c);
				entity.Codes = listCodes;
				db.thesaurusentry.updateOne({ _id: entity._id }, { $set: { Codes: entity.Codes } })
			}
		});
	}
}
);





db.thesaurusentry.find().forEach(function (entity) {
	if (entity.Codes) {
		let listCodes = [];
		entity.Codes.forEach(function (code) {
			let c = {};
			if (code.VersionPublishDate != null && !code.VersionPublishDate.getTime().toString().endsWith("00000")) {
				print(code.VersionPublishDate);
				code.VersionPublishDate = new Date(code.VersionPublishDate.setHours(1, 0, 0, 0));
				let c = code;
				listCodes.push(c);
				entity.Codes = listCodes;
				db.thesaurusentry.updateOne({ _id: entity._id }, { $set: { Codes: entity.Codes } })
			}
		});
	}
}
);