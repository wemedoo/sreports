db.thesaurusentry.find({}).forEach(function (t) {
	if (t.Translations != null) {
		t.Translations.forEach(function (translation) {
			let list = [];
			if (translation.SimilarTerms != null) {
				translation.SimilarTerms.forEach(function (similarTerm) {
					if (similarTerm != null) {
						let sim = {};
						sim['Name'] = similarTerm;
						sim['Definition'] = null;
						sim['Source'] = 0;
						sim['EntryDateTime'] = null;
						sim['_id'] = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
							var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
							return v.toString(16);
						});
						list.push(sim);
					}
				});
			}
			translation.SimilarTerms = list;
		});
	}
	db.thesaurusentry.updateOne({ '_id': t._id }, { $set: { 'Translations': t.Translations } });
});