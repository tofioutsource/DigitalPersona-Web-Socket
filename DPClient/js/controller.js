

(function (window, $) {



  WEB_SOCKET_DEBUG = true;

  var sock = {};
  var me = {};

  function Connect() {
    var mServer = $('#server').val();
    var mPort = $('#port').val();
    sock = io(`ws://${mServer}:${mPort}`);

    LogMessage('Connecting...');
    $('#status').removeClass('offline').addClass('pending').text('Connecting...');

    // sock.Connected = function() {
    //   LogMessage('Connection established!');
    //   $('#status').removeClass('pending').addClass('online').text('Online');
    //   $('#connectToServer').hide('fast', function() { 
    //     $('#registerName, #tapUser').show('fast'); 
    //   });

    //   //initiate finger listener by default 
    //   sock.Send({ Type: 1 });
    // };

    // sock.Disconnected = function() {
    //   LogMessage('Connection closed.');
    //   $('#status').removeClass('pending').removeClass('online').addClass('offline').text('Offline');

    //   $('#registerName, #sendMessage').hide('fast', function() { $('#connectToServer').show('fast'); });
    // };

    // sock.MessageReceived = function(event) {
    //   ParseResponse(event.data);
    // };

    // sock.Start();
    sock.on('connect', () => {
      LogMessage('Connection established!');
      $('#status').removeClass('pending').addClass('online').text('Online');
      $('#connectToServer').hide('fast', function () {
        $('#registerName, #tapUser').show('fast');
      });

      sock.emit('reader');
    });

    sock.on('disconnect', () => {
      LogMessage('Connection closed.');
      $('#status').removeClass('pending').removeClass('online').addClass('offline').text('Offline');
      $('#registerName, #sendMessage').hide('fast', function () { $('#connectToServer').show('fast'); });
    });

    sock.on("connect_error", err => {
      console.error(err);
    });

    sock.on('readers', x => {
      console.log(x);
      sock.emit('capture');
      LogMessage(JSON.stringify(x));
    });

    sock.on('message', x => {
      console.log('message', x);
      LogMessage(x);
    });

    sock.on('error', x => {
      console.log('error', x);
      LogMessage('Error:' + x);
    });

    sock.on('fingerData', x => {
      console.log('error', x);
      var p = $('<img />');
      p.attr('src', x);
      $('#results').prepend(p);
    })

    sock.connect();
  };

  function LogMessage(message) {
    var p = $('<p></p>').text(message);
    $('#results').prepend(p);
  }

  function ParseResponse(response) {
    var data = JSON.parse(response);

    // The Chat server demo sends back a responsetype to let us know how to parse the message.
    if (data.Type == 0) {
      var message = data.Data.Name + ' connected!';
      LogMessage(message);
    } else if (data.Type == 1) {
      var message = data.Data.Name + ' disconnected!';
      LogMessage(message);
    } else if (data.Type == 2) {
      // We don't display it if it's from ourselves, because we display our own messages immediately
      // see the jQuery bindings later on for more info)
      //var message = data.Data.Name + ': ' + data.Data.Message;
      LogMessage(data.Data.Message);
    } else if (data.Type == 3) {
      var message = data.Data;
      LogMessage(message);
    } else if (data.Type == 4) {
      // Set the online users, and show the list of users if you hover over the number.
      $('#onlineUsers').text(data.Data.Users.length).attr('title', data.Data.Users.join('\n'));
    } else if (data.Type === 255) {
      LogMessage('Error:' + data.Data.Message);
    }
  }

  function ValidateName(name) {
    if (name.length < 3 || name.length > 25) {
      return false;
    }

    return true;
  }

  // Just some event bindings.
  $(function () {
    $('#sendMessage').bind('submit', function (e) {
      e.preventDefault();

      var message = $('#message').val();
      var data = {};

      if (message.indexOf('/nick ') == 0) {
        var name = message.replace(/\/nick /, '');
        data = { Type: 2, Name: name };

        if (!ValidateName(name)) {
          alert('Please pick a name of length 3 - 25.');
          return;
        }

        if (name == me.Name) {
          return;
        }

        me.Name = name;
      } else {
        // We display our own messages immediately to increase perceived performance.
        data = { Type: 1, Message: message };
        LogMessage(me.Name + ': ' + data.Message);
      }

      sock.Send(data);

      $('#message').val('').focus();
    });

    $('#registerName').bind('submit', function (e) {
      e.preventDefault();
      // var name = $('#name').val();

      // if (!ValidateName(name)) {
      //   alert('Please pick a name of length 3 - 25.');
      //   return;
      // }

      // me.Name = name;
      // var data = { Type: 0, Name: name };

      sock.Send({ Type: 2 });

      $('#registerName, #tapUser').hide('fast', function () {
        $('#registerTap').show('fast');
      });
    });

    $('#connectToServer').bind('submit', function (e) {
      e.preventDefault();
      Connect();
    });

    $('#disconnect').bind('click', function (e) {
      e.preventDefault();
      sock.disconnect();
      $('#registerName, #tapUser').hide('fast', function () {
        $('#connectToServer').show('fast');
      });
    });
  });
})(window, jQuery);
