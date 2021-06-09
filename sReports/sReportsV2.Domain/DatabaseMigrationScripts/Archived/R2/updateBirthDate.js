//Update patient birth date
db.patiententity.find().forEach(function (entity) {
    if (entity.BirthDate) {
        db.patiententity.update({ _id: entity._id },
            [{
                $set: {
                    BirthDate: {
                        "$dateToString": {
                            "date": "$BirthDate",
                            "format": "%m-%d-%Y"
                        }
                    }
                }
            }]
        )

    }
});

db.patiententity.find().forEach(function (entity) {
    if (entity.BirthDate) {
        print(entity.BirthDate);
        db.patiententity.update({ _id: entity._id },
            [
                {
                    $set: {
                        BirthDate: {
                            "$dateFromString": {
                                "dateString": "$BirthDate",
                                "format": "%m-%d-%Y"
                            }
                        }
                    }
                }
            ])
    }
});

//Update encounter start
db.encounterentity.find().forEach(function (entity) {
    if (entity.Period != null && entity.Period.Start != null) {
        db.encounterentity.update({ _id: entity._id },
            [{
                $set: {
                    "Period.Start": {
                        "$dateToString": {
                            "date": "$Period.Start",
                            "format": "%m-%d-%Y"
                        }
                    }
                }
            }]
        )

    }
});

db.encounterentity.find().forEach(function (entity) {
    if (entity.Period != null && entity.Period.Start != null) {
        db.encounterentity.update({ _id: entity._id },
            [
                {
                    $set: {
                        "Period.Start": {
                            "$dateFromString": {
                                "dateString": "$Period.Start",
                                "format": "%m-%d-%Y"
                            }
                        }
                    }
                }
            ])
    }
});

//update encounter end
db.encounterentity.find().forEach(function (entity) {
    if (entity.Period != null && entity.Period.End != null) {
        db.encounterentity.update({ _id: entity._id },
            [{
                $set: {
                    "Period.End": {
                        "$dateToString": {
                            "date": "$Period.End",
                            "format": "%m-%d-%Y"
                        }
                    }
                }
            }]
        )

    }
});

db.encounterentity.find().forEach(function (entity) {
    if (entity.Period != null && entity.Period.End != null) {
        db.encounterentity.update({ _id: entity._id },
            [
                {
                    $set: {
                        "Period.End": {
                            "$dateFromString": {
                                "dateString": "$Period.End",
                                "format": "%m-%d-%Y"
                            }
                        }
                    }
                }
            ])
    }
});
//Update episode of care end
db.episodeofcareentity.find().forEach(function (entity) {
    if (entity.Period != null && entity.Period.End != null) {
        db.episodeofcareentity.update({ _id: entity._id },
            [{
                $set: {
                    "Period.End": {
                        "$dateToString": {
                            "date": "$Period.End",
                            "format": "%m-%d-%Y"
                        }
                    }
                }
            }]
        )

    }
});

db.episodeofcareentity.find().forEach(function (entity) {
    if (entity.Period != null && entity.Period.End != null) {
        db.episodeofcareentity.update({ _id: entity._id },
            [
                {
                    $set: {
                        "Period.End": {
                            "$dateFromString": {
                                "dateString": "$Period.End",
                                "format": "%m-%d-%Y"
                            }
                        }
                    }
                }
            ])
    }
});

//Update episode of care start
db.episodeofcareentity.find().forEach(function (entity) {
    if (entity.Period != null && entity.Period.Start != null) {
        db.episodeofcareentity.update({ _id: entity._id },
            [{
                $set: {
                    "Period.Start": {
                        "$dateToString": {
                            "date": "$Period.Start",
                            "format": "%m-%d-%Y"
                        }
                    }
                }
            }]
        )

    }
});

db.episodeofcareentity.find().forEach(function (entity) {
    if (entity.Period != null && entity.Period.Start != null) {
        db.episodeofcareentity.update({ _id: entity._id },
            [
                {
                    $set: {
                        "Period.Start": {
                            "$dateFromString": {
                                "dateString": "$Period.Start",
                                "format": "%m-%d-%Y"
                            }
                        }
                    }
                }
            ])
    }
});