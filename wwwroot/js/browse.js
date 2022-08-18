var loadingMore = false;


$("document").ready(function () {
    if ($('#js-spotify-widget-frame').length) {
        let height = $('#js-artist-blurb-frame').outerHeight() + 'px';
        $('#js-spotify-widget-frame').css('height', height);
    }

    $(window).scroll(function () {
        if ($(".js-artist-row").length) {
            let relatedArtists = $('.js-related-artists-container').is(':visible');
            let secondToLastElement = relatedArtists ? $('.js-related-artists-container').find('.js-artist-row').eq('-2') : $('.js-artists-container').find('.js-artist-row').eq('-2');

            let topOfElement = secondToLastElement.offset().top;
            let bottomOfElement = secondToLastElement.offset().top + secondToLastElement.outerHeight();
            let bottomOfScreen = $(window).scrollTop() + $(window).innerHeight();
            let topOfScreen = $(window).scrollTop();

            if ((bottomOfScreen > topOfElement) && (topOfScreen < bottomOfElement)) {
                if (!loadingMore) {
                    loadingMore = true;
                    let cursor = $('#Cursor').val();
                    let pageFrom = parseInt($('#PageFrom').val()) + 1;
                    let searchTerm = parseHiddenValue($('#SearchTerm').val());
                    let genres = parseHiddenValue($('#Genres').val());
                    let sortValue = parseHiddenValue($('#SortValue').val());
                    let searchArtist = parseHiddenValue($('#SearchArtist').val());
                    let searchAlbum = parseHiddenValue($('#SearchAlbum').val());

                    doPost("Browse", "LoadMoreArtists", {
                        "cursor": cursor,
                        "itemsPerPage": 10,
                        "sortValue": sortValue,
                        "genres": genres,
                        "searchTerm": searchTerm,
                        "searchArtist": searchArtist,
                        "searchAlbum": searchAlbum,
                        "pageFrom": pageFrom,
                        "relatedArtists": relatedArtists
                    }, "json", function (result) {
                        if (relatedArtists)
                            $('.js-related-artists-container').append(result.html);
                        else
                            $('.js-artists-container').append(result.html);

                        $('#Cursor').val(result.cursor);
                        $('#PageFrom').val(result.pageFrom);
                        loadingMore = false;
                    });
                }
            }
        }
    });
});

var parseHiddenValue = function (value) {
    if (value != undefined && value != null && value.length > 0)
        return value;
    else
        return null;
}

