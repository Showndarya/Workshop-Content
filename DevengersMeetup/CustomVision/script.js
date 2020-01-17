$(function() {
  // preventing page from redirecting
  $('html').on('dragover', function(e) {
    e.preventDefault();
    e.stopPropagation();
    $('h2').text('Drag here');
  });

  $('html').on('drop', function(e) {
    e.preventDefault();
    e.stopPropagation();
  });

  // Drag enter
  $('.upload-area').on('dragenter', function(e) {
    e.stopPropagation();
    e.preventDefault();
    $('h2').text('Drop');
  });

  // Drag over
  $('.upload-area').on('dragover', function(e) {
    e.stopPropagation();
    e.preventDefault();
    $('h2').text('Drop');
  });

  // Drop
  $('.upload-area').on('drop', function(e) {
    e.stopPropagation();
    e.preventDefault();

    $('h2').text('Uploading');

    var file = e.originalEvent.dataTransfer.files[0];
    readURLd(file);
    //var fd = new FormData();

    //fd.append('file', file[0]);

    uploadData(file);
  });

  // Open file selector on div click
  $('#uploadfile').click(function() {
    $('#file').click();
  });

  // file selected
  $('#file').change(function() {
    //var fd = new FormData();
    $('h2').text('Uploading');
    var files = document.getElementById('file').files[0];

    //fd.append('file',files);

    uploadData(files);
  });
});
function readURLd(input) {
  var reader = new FileReader();

  reader.onload = function(e) {
    $('#disp')
      .attr('src', e.target.result)
      .width(425)
      .height(250);
  };

  reader.readAsDataURL(input);
}

function readURL(input) {
  if (input.files && input.files[0]) {
    var reader = new FileReader();

    reader.onload = function(e) {
      $('#disp')
        .attr('src', e.target.result)
        .width(425)
        .height(250);
    };

    reader.readAsDataURL(input.files[0]);
  }
}
// Sending AJAX request and upload file
function uploadData(data) {
  $('h2').text('');

  $.ajax({
    url: '',
    method: 'POST',
    beforeSend: function(xhrObj) {
      xhrObj.setRequestHeader('Content-Type', 'application/octet-stream');
      xhrObj.setRequestHeader('Prediction-key', '');
    },
    async: true,
    crossDomain: true,
    processData: false,
    contentType: false,
    //"mimeType": "multipart/form-data",
    data: data
  })
    .done(function(data) {
      var trHTML = '';
      trHTML = '<tr><th style="text-align:center">Results</th></tr>';
      //trHTML+= '<tr><thstyle="text-align:justified">Tag</th><thstyle="text-align:justified">Percentage</th></tr>';
      $.each(data.predictions, function(i, item) {
        trHTML +=
          '<tr><td>' +
          data.predictions[i].tagName +
          '</td><td>' +
          data.predictions[i].probability * 100 +
          '</td></tr>';
      });

      $('#location').append(trHTML);
    })
    .fail(function() {
      alert('Error');
    });
}
