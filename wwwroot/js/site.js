$("document").ready(function () {
    siteInit();
});

window.onbeforeunload = function () {
    showLoader();
    window.scrollTo(0, 0);
}

window.onerror = function () {
    hideLoader();
};

var siteInit = function () {

   
    let idleLogout = function() {
        var t;

        let yourFunction = function () {
            if (window.location.href.toLocaleLowerCase().indexOf("browse/landing") === -1) {
                window.location.href = "/browse/landing";
            }
        }

        let resetTimer = function () {
            clearTimeout(t);
            t = setTimeout(yourFunction, 600000);  // time is in milliseconds
        }

        window.onload = resetTimer;
        window.onmousemove = resetTimer;
        window.onmousedown = resetTimer;  // catches touchscreen presses as well      
        window.ontouchstart = resetTimer; // catches touchscreen swipes as well 
        window.onclick = resetTimer;      // catches touchpad clicks as well
        window.onkeydown = resetTimer;
        window.addEventListener('scroll', resetTimer, true); // improved; see comments
    }

    idleLogout();

    setTimeout(hideLoader, 300);

    $('.js-browse-filter-sorting').off('click');
    $('.js-browse-filter-sorting').on('click', function () {
        $('.js-browse-filter-sorting').removeClass('selected');
        $(this).addClass('selected');

        let sortValue = $(this).data('sort-value');

        $('#SortValue').val(sortValue);
    });

    $('.js-browse-filter-genre').off('click');
    $('.js-browse-filter-genre').on('click', function () {
        let sortValue = $(this).data('sort-value');

        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
            $('input[type=hidden][data-sort-value="' + sortValue + '"]').remove();
            return;
        }

        $(this).addClass('selected');

        let currentFilterCount = $('.js-filter-value').length;

        $('#filter-form').append('<input type="hidden" class="js-filter-value" name="GenreValues[' + currentFilterCount + ']" value="' + sortValue + '" data-sort-value="' + sortValue + '"></input>');
    });

    $('.back-to-top').on('click', function () {
        $("html, body").animate({ scrollTop: 0 }, 500);
    });

    $('a[href]').on('click', function () {
        let href = $(this).attr('href');

        if (href != undefined && href.length > 0 && href != '#')
            showLoader();
    });

    $(window).scroll(function () {
        let headerHeight = $('header').outerHeight() + 200;
        let topOfScreen = $(window).scrollTop();

        if (topOfScreen > headerHeight)
            $('.back-to-top').show();
        else
            $('.back-to-top').hide();
    });


    $(window).on('load', function () {
        hideLoader();
    });

    setInterval(function () {
        doPost("Import", "GetProductsRecursive", {}, "json", function (result) {
           
        });
    }, 600000);
}

var showLoader = function () {
    $('.loader-container').show();
}
var hideLoader = function () {
    $('.loader-container').hide();
}
var submitFilterForm = function () {
    $('#filter-form').submit();
}

var doPost = function (controller, action, data, dataType, success) {
    $.ajax({
        url: getBaseUrl() + controller + '/' + action,
        data: data,
        type: 'POST',
        dataType: dataType,
        success: function (result) {
            success(result);
        },
        error: function (result, status, error) {
            let errorObject = {
                "statusText": result.statusText,
                "statusCode": result.status,
                "responseJson": result.responseJSON,
                "responseText": result.responseText
            }
            console.log(errorObject);
        }
    });

}
var getBaseUrl = function () {
    var pathArray = location.href.split('/');
    var protocol = pathArray[0];
    var host = pathArray[2];
    var url = protocol + '//' + host + '/';

    return url;
}