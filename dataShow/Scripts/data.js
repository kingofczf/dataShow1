$.ajax({
    type: 'GET',
    url: AJAXurl,
    dataType: 'json',
    success: function (data) {
        build_dropdown(data, $('#SourceName'), '请选择...');//填充表单
    }
});
var build_dropdown = function( data, element, defaultText ){
    element.empty().append( '<option value="">' + defaultText + '</option>' );
    if( data ){
        $.each( data, function( key, value ){
            element.append( '<option value="' + key + '">' + value + '</option>' );
        } );
    }
