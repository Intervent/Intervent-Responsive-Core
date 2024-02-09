(function ($) {
  var MODAL_OPEN = 'modal-open';
  var MODAL_CLOSE = 'modal-close';
  var ANIMATION_TIME = 300;

  $.fn.ivtModal = function() {

    var suffix = '-ivt-modal';
    var template = '' +
      '<div id="{{newId}}" class="ivt-modal">' +
      '  <div class="modal-content">{{modalContent}}</div>' +
      '</div>';

    $('ivt-modal').each(function(idx) {
      var currentObject = $(this);
      var currentId = currentObject.attr('id');
      var newId = currentId + suffix;
      var parsedTemplate = template;
      parsedTemplate = parsedTemplate.split('{{modalContent}}').join(currentObject.html());
      parsedTemplate = parsedTemplate.split('{{newId}}').join(newId);
      currentObject.html('');
      $('body').append(parsedTemplate);

      if(idx === 0) {
        $('body').append('<div class="ivt-modal-bg"></div>');
      }

      $(document).on('click', '#' + currentId + suffix + ' .close-reveal-modal', function(event) {
        var elementId = $(event.currentTarget).parents('.ivt-modal').attr('id');
        event.stopImmediatePropagation();
        $('#' + elementId).trigger(MODAL_CLOSE);
      });

      $('#' + currentId).on(MODAL_OPEN, function(event) {
        $('#' + currentId + suffix).trigger(MODAL_OPEN);
      });

      $('#' + currentId).on(MODAL_CLOSE, function(event) {
        $('#' + currentId + suffix).trigger(MODAL_CLOSE);
      });

      $('#' + currentId + suffix).on(MODAL_OPEN, function(event) {
        $('#' + currentId + suffix).addClass('entry');
        $('.ivt-modal-bg').addClass('entry');
        
        setTimeout(function() {
          $('#' + currentId + suffix).addClass('shown');
          $('.ivt-modal-bg').addClass('shown');          
        }, 0);
      });

      $('#' + currentId + suffix).on(MODAL_CLOSE, function(event) {
        $('#' + currentId + suffix).removeClass('shown');
        $('.ivt-modal-bg').removeClass('shown');

        setTimeout(function() {
          $('#' + currentId + suffix).removeClass('entry');
          $('.ivt-modal-bg').removeClass('entry');          
        }, ANIMATION_TIME);
      });
    });
  };

}(jQuery));