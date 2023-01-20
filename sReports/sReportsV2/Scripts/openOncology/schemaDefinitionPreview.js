function newEntity() {
    window.location.href = `/SmartOncology/CreateNewSchema`;
}

function editEntity(event, id) {
    event.preventDefault();
    window.location.href = `/SmartOncology/EditSchema/${id}`;
}