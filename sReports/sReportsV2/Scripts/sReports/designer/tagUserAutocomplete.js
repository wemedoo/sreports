$(document).on('keyup', '.comment-text', function (e) {
    let $this = $(this);
    var commentId = $this.attr('data-id');
    let tagIndex = tagIndexes[commentId];
    if (e.which !== downArrow && e.which !== upArrow && e.which !== enter) {
        let commentText = $this.html();
        let lastCharacterIndex = commentText.length - 1;
        let lastCharacter = commentText.charAt(lastCharacterIndex);
        if (lastCharacter === '@') {
            searchWords[commentId] = "";
            tagIndexes[commentId] = lastCharacterIndex;
        } else if (tagIndex !== -1) {
            let searchWord = commentText.substring(tagIndex + 1, lastCharacterIndex + 1);
            searchWords[commentId] = searchWord;
            if (searchWord.length >= 2) {
                $.ajax({
                    method: 'get',
                    url: `/Form/RetrieveUser?searchWord=${searchWord}&commentId=${commentId}`,
                    contentType: 'application/json',
                    success: function (data) {
                        let userOptions = $this.closest('div#search-user').siblings(':first');
                        $(userOptions).html(data);
                        $(userOptions).show();
                    },
                    error: function (xhr, textStatus, thrownError) {
                        handleResponseError(xhr, thrownError);
                    }
                });
            }
        }
    }

    if (e.which === enter) {
        let $emptyElementsAtBeggining = $this.children('div').has('br');
        if ($emptyElementsAtBeggining.length > 0) {
            $emptyElementsAtBeggining.remove();
            var el = document.getElementById(`commentText_${commentId}`);
            var sel = window.getSelection();
            sel.setPosition(el.lastChild, 1);
        }
    }

    if (e.which === backSpace) {
        let $commentTextInput = $this;
        let commentText = $commentTextInput.html();
        let lastCharacterIndex = commentText.length - 1;

        if (tagIndex - lastCharacterIndex >= 1) {
            let userOptions = $this.closest('div#search-user').siblings(':first');
            $(userOptions).hide();
        }

        let taggedUsers = $commentTextInput.data();
        delete taggedUsers['text'];
        let shouldUntagUser = false;
        for (let key in taggedUsers) {
            if (parseInt(key) + closingAOffset == lastCharacterIndex) shouldUntagUser = true;
        }
        if (shouldUntagUser) {
            $commentTextInput.children().last().remove();
            searchWords[commentId] = "";
            tagIndexes[commentId] = -1;
            $commentTextInput.data((lastCharacterIndex - closingAOffset).toString(), "");
        }
    }
});

$(document).on('focus', '.comment-text', function (e) {
    let $this = $(this);
    var commentId = $this.attr('data-id');
    let commentText = $this.html();
    let lastCharacterIndex = commentText.length - 1;
    let lastCharacter = commentText.charAt(lastCharacterIndex);
    let tagIndex = tagIndexes[commentId];
    if (tagIndex != undefined) {
        console.log(` FOCUS EVNT: comment: ${commentText}, lastCharIndex: ${lastCharacterIndex}, lastChar: ${lastCharacter}, tagIndex: ${tagIndex}`);
        if (lastCharacter === '@') {
            searchWords[commentId] = "";
            tagIndexes[commentId] = lastCharacterIndex;
        } else if (tagIndex !== -1) {
            let searchWord = commentText.substring(tagIndex + 1, lastCharacterIndex + 1);
            searchWords[commentId] = searchWord;
            if (searchWord.length >= 2) {
                $.ajax({
                    method: 'get',
                    url: `/Form/RetrieveUser?searchWord=${searchWord}&commentId=${commentId}`,
                    contentType: 'application/json',
                    success: function (data) {
                        let userOptions = $this.closest('div#search-user').siblings(':first');
                        $(userOptions).html(data);
                        $(userOptions).show();
                    },
                    error: function (xhr, textStatus, thrownError) {
                        handleResponseError(xhr, thrownError);
                    }
                });
            }
        }
    }
});

$(document).on('keydown', '#search-user', function (e) {
    let next;
    if (e.which === downArrow) {
        if (liSelected) {
            $(liSelected).removeClass('selected');
            next = $(liSelected).next();
            if (next.length > 0) {
                liSelected = $(next).addClass('selected');
            } else {
                liSelected = $('.option').eq(0).addClass('selected');
            }
        } else {
            liSelected = $('.option').eq(0).addClass('selected');
        }
    } else if (e.which === upArrow) {
        if (liSelected) {
            $(liSelected).removeClass('selected');
            next = $(liSelected).prev();
            if (next.length > 0) {
                liSelected = $(next).addClass('selected');
            } else {
                liSelected = $('.option').last().addClass('selected');
            }
        } else {
            liSelected = $('.option').last().addClass('selected');
        }
    }
    else if (e.which === enter) {
        $(liSelected).click();
    }

    e.stopImmediatePropagation();
});

function addTaggedUser(userIdentifier, name, commentId) {
    let $commentTextField = $(`#commentText_${commentId}`)
    let comment = $commentTextField.html();
    let tagIndex = tagIndexes[commentId];

    let host = $(location).prop('origin');
    let hrefUserProfile = `${host}/UserAdministration/DisplayUser?userId=${userIdentifier}`;

    let tagUserElement = $('<a></a>')
        .attr('href', hrefUserProfile)
        .attr('target', '_blank')
        .attr('rel', 'noopener noreferrer')
        .addClass('tagged-user')
        .text(`@${name}`);
    let contentBeforeTagging = comment.slice(0, tagIndex);

    $commentTextField.html(contentBeforeTagging);
    $commentTextField.append(tagUserElement);
    $commentTextField.append("&nbsp;");

    var taggedUserTagIndex = tagIndex + startingTagIndexOffset;
    $commentTextField.data(taggedUserTagIndex.toString(), userIdentifier);

    let userOptions = $commentTextField.closest('div#search-user').siblings(':first');
    $(userOptions).hide();
    searchWords[commentId] = "";
    tagIndexes[commentId] = -1;
}

function userOptionClicked(e, userIdentifier, name, commentId) {
    addTaggedUser(userIdentifier, decodeLocalizedString(name), commentId);
}

$(document).on("click", '.sidebar-shrink', function (e) {
    let displayCommentSectionProp = $("#commentSection").css('display');
    if (displayCommentSectionProp !== 'none') {
        $('.user-options').hide();
        let target = e.target;
        if ($(target).hasClass('option')) {
            let currentUserOptions = $(target).closest('.user-options');
            showActiveUserOptionsAutocomplete(currentUserOptions);
        }
        if ($(target).hasClass('comment-text')) {
            let currentUserOptions = $(target).closest('div#search-user').siblings(':first');
            showActiveUserOptionsAutocomplete(currentUserOptions);
        }
    }
});

function showActiveUserOptionsAutocomplete(currentUserOptions) {
    let commentId = currentUserOptions.attr('data-id');
    let tagIndex = tagIndexes[commentId];
    if (tagIndex != undefined && tagIndex != -1) {
        $(currentUserOptions).show();
    }
}

var enter = 13;
var downArrow = 40;
var upArrow = 38;
var backSpace = 8;
var startingTagIndexOffset = 136;
var closingAOffset = 4;

var li = $('.option');
var liSelected = null;
var tagIndexes = {};
var searchWords = {};
