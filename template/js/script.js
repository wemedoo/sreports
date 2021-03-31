$(function() {
    $('#closeSidebar, #showSidebar').on('click', function() {
        $('body').toggleClass('sidebar-on');
    });

    $('#menuShrinkBtn').on('click', function(){
        $('body').toggleClass('sidebar-shrink');
    });
});