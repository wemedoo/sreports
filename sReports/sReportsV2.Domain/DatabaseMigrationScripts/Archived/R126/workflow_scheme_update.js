db.form.find({}).forEach(function(f){
	print(f._id);
	f['WorkflowHistory'] = f.Workflow ? f.Workflow.WorkflowHistory : null
	
	delete f.Workflow
	
	db.form.replaceOne(
	{_id: f._id},
	f
	)
})
