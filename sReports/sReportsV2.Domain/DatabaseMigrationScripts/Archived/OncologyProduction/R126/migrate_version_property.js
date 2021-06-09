db.form.updateMany(
   { },
   { $set: { "Version": {Major : 1, Minor : 1, _id : new ObjectId().str} } })
   

db.forminstance.updateMany(
   { },
   { $set: { "Version": {Major : 1, Minor : 1, _id : new ObjectId().str} } })
   
db.form.updateMany(
	{},
		{
			$set:{"State": 2}
		}
)

