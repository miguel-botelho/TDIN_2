
function login() {
	var check = validateNIF();
	if (check) {
		var isChecked;
		if ($('#keep_logged:checkbox:checked').length > 0)
			isChecked = true;
		else isChecked = false;

		$.ajax({
			type: 'POST',
			url: '/log',
			data : {
				email: $("#nif").val(),
				pass: $("#password").val(),
				keep_logged: isChecked
			},
			dataType : "json",
			success : function(data) {
				if (data == 'no user')
					swal("Essa conta não existe.", "Tente outra vez.", "error");
				else if (data == 'wrong password')
					swal("A password inserida está errada.", "Tente outra vez.", "error");
				else if (data == 'error')
					swal("Aconteceu um erro inesperado.", "Tente outra vez.", "error");
				else if (data == 'success') {
					swal({
						title: "Irá agora ser redirecionado para a página principal.",
						text: "",
						type: "success",
						showCancelButton: false,
						confirmButtonColor: "#DD6B55",
						confirmButtonText: "OK",
						closeOnConfirm: false
						},
						function(){
							window.location.href = "/";
						});
					}
			},
			error : function(jq, stat, err) {
				alert(err);
			}
		});
	}
	else {
		return false;
	}
}

function validateNIF() {
	var nif = $("#nif").val();

	// ver se é só números
	if (!($.isNumeric(nif))) {
		$("#nif").css("background-color", "#ff9a9a");
		return false;
	}
	// 9 numeros
	if (nif.length != 9) {
		$("#nif").css("background-color", "#ff9a9a");
		return false;
	}
	// primeiro digito mal
	if (nif[0] == "3" | nif[0] == "4" | nif[0] == "7") {
		$("#nif").css("background-color", "#ff9a9a");
		return false;
	}

	// digito de controlo
	var soma = 0;
	for (var i = 1; i < nif.length; i++) {
		soma += nif[i-1] * (10 - i);
	}
	var remainder = soma % 11;
	var controlo;
	if (remainder == 0 || remainder == 1) {
		controlo = 0;
	}
	else controlo = 11 - remainder;
	
	// verificar último digito
	if (controlo != nif[8]) {
		$("#nif").css("background-color", "#ff9a9a");
		return false;
	}
	else {
		$("#nif").css("background-color", "#f2f2f2");
		return true;
	}

}