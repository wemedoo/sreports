db.form.find({}).forEach(function(f){
	f.Chapters.forEach(function(chapter){
		chapter.Pages.forEach(function(page){
			page.FieldSets.forEach(function(fs){
				fs.Fields.forEach(function(field){
					field['_t'] = field.Type
				})
			})
		})
	})
	db.form.replaceOne(
	{_id: f._id},
	f
	)
})