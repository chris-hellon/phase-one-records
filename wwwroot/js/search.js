
$(function () {

    try {
        const Keyboard = window.SimpleKeyboard.default;

        var onChange = function(input) {
            document.querySelector(".input").value = input;
            console.log("Input changed", input);

            if (input === "") $('.js-search-input').removeClass('error'); 

            $('.js-text-search').val(input);
        }


        var onKeyPress = function(button) {
            console.log("Button pressed", button);

            if (button === '{enter}') {
                if ($('.js-text-search').val() !== "") {
                    $('.js-search').trigger('click');
                }
                else {
                    $('.js-search-input').addClass('error');
                }
            }
        }

        const myKeyboard = new Keyboard({
            onChange: input => onChange(input),
            onKeyPress: button => onKeyPress(button),
            layout: {
                'default': [
                    '1 2 3 4 5 6 7 8 9 0 {bksp}',
                    'Q W E R T Y U I O P ',
                    'A S D F G H J K L {enter}',
                    '@ Z X C V B N M \, .',
                    '{space}'
                ],
                'shift': [
                    '~ ! @ # $ % ^ & * ( ) _ + {bksp}',
                    '{tab} Q W E R T Y U I O P { } |',
                    '{lock} A S D F G H J K L : " {enter}',
                    '{shift} Z X C V B N M < > ? {shift}',
                    '.com @ {space}'
                ]
            }
        });


        $('.js-search-input').on('change', function () {
            $('.js-text-search').val($(this).val());
        });


        $('.js-search-input').val("");
    }
    catch (err) {
        console.log(err);
    }
  
});
