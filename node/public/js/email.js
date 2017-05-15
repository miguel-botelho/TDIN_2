
function submitContactForm() {
	$.ajax({
		type: 'POST',
        url: '/contact-company',
		data: {
			email: $("#email").val(),
			name: $("#name").val(),
			message : $("#message").val()
		},
		dataType : "json",
		success : function (data) {
			if (data == 'success')
				swal("O e-mail foi enviado corretamente.", "", "success");
			else
				swal("O e-mail não foi enviado corretamente.", "Tente outra vez.", "error");
		},
		error : function(jq, stat, err) {
			alert(err);
		}
	});
}