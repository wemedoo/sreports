
db.form.updateMany(
    {},
    { $set: { "Version": { Major: 1, Minor: 1, _id: "1" } } })


db.forminstance.updateMany(
    {},
    { $set: { "Version": { Major: 1, Minor: 1, _id: "1" } } })