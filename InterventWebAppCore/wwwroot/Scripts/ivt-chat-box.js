(function ($) {
  var maxBoxes = 4;

  $.fn.chatBox = function() {
    var totalBoxes = 0;
    var template = '' +
      '<div class="chat-box {{position}} {{color}}">' +
      '  <div class="header">' +
      '    <div class="title two">{{header}}</div>' +
      '    <div class="control minimize {{minimizeModifier}}"><i class="fa fa-minus"></i></div>' +
      '    <div class="control close {{closeModifier}}"><i class="fa fa-close"></i></div>' +
      '  </div>' +
      '  <div class="content">{{content}}</div>' +
      '</div>';

    $('ivt-chat-box').each(function() {
      totalBoxes++;
      var currentObject = $(this);
      var currentContent = currentObject.html();
      var title = currentObject.attr('title');
      var color = currentObject.attr('color');
      var minimizeButton = currentObject.attr('minimize-button') || false;
      var closeButton = currentObject.attr('close-button') || false;

      var parsedTemplate = template;
      parsedTemplate = parsedTemplate.split('{{position}}').join('position-' + totalBoxes);
      parsedTemplate = parsedTemplate.split('{{header}}').join(title);
      parsedTemplate = parsedTemplate.split('{{color}}').join(color);
      parsedTemplate = parsedTemplate.split('{{content}}').join(currentContent);
      if(minimizeButton) {
        parsedTemplate = parsedTemplate.split('{{minimizeModifier}}').join('');
      }
      if(closeButton) {
        parsedTemplate = parsedTemplate.split('{{closeModifier}}').join('');
      }

      currentObject.html(parsedTemplate);

      if(!minimizeButton && !closeButton) {
        currentObject.find('.title').removeClass('two');
      }
      if(!minimizeButton) {
        currentObject.find('.minimize').remove();
        currentObject.find('.title').removeClass('two').addClass('one');
      }
      if(!closeButton) {
        currentObject.find('.close').remove();
        currentObject.find('.title').removeClass('two').addClass('one');
      }
    });

  };

  $(document).on('click', '.chat-box .header .minimize', function() {
    var chatBox = $(this).parent().parent();
    chatBox.hasClass('collapsed') ? chatBox.removeClass('collapsed') : chatBox.addClass('collapsed');
  });

  $(document).on('click', '.chat-box .header .close', function() {
    var chatBox = $(this).parent().parent();
    var className = chatBox.attr('class');
    var removedPosition = parseInt(className.substr(-1));

    for(var i = removedPosition; i <= maxBoxes; i++) {
      $('.chat-box.position-' + i).removeClass('position-' + i).addClass('position-' + (i - 1));
    }
    chatBox.remove();
  });

}(jQuery));