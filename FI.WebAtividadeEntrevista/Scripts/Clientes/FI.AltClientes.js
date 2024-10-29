$(document).ready(function () {
    if (obj) {
        $('#formCadastro #Nome').val(obj.Nome);
        $('#formCadastro #CEP').val(obj.CEP);
        $('#formCadastro #Email').val(obj.Email);
        $('#formCadastro #Sobrenome').val(obj.Sobrenome);
        $('#formCadastro #CPF').val(obj.CPF);
        $('#formCadastro #Nacionalidade').val(obj.Nacionalidade);
        $('#formCadastro #Estado').val(obj.Estado);
        $('#formCadastro #Cidade').val(obj.Cidade);
        $('#formCadastro #Logradouro').val(obj.Logradouro);
        $('#formCadastro #Telefone').val(obj.Telefone);

        carregarBeneficiarios(obj.Beneficiarios);
    }

    $('#formCadastro').submit(function (e) {
        e.preventDefault();

        let beneficiarios = listaBeneficiarios();

        $.ajax({
            url: urlPost,
            method: "POST",
            data: {
                "NOME": $(this).find("#Nome").val(),
                "CEP": $(this).find("#CEP").val(),
                "Email": $(this).find("#Email").val(),
                "Sobrenome": $(this).find("#Sobrenome").val(),
                "CPF": $(this).find("#CPF").val(),
                "Nacionalidade": $(this).find("#Nacionalidade").val(),
                "Estado": $(this).find("#Estado").val(),
                "Cidade": $(this).find("#Cidade").val(),
                "Logradouro": $(this).find("#Logradouro").val(),
                "Telefone": $(this).find("#Telefone").val(),
                "Beneficiarios": beneficiarios
            },
            error:
            function (r) {
                if (r.status == 400)
                    ModalDialog("Ocorreu um erro", r.responseJSON);
                else if (r.status == 500)
                    ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
            },
            success:
            function (r) {
                ModalDialog("Sucesso!", r)
                $("#formCadastro")[0].reset();                                
                window.location.href = urlRetorno;
            }
        });
    })

    document.getElementById('btnBeneficiarios').addEventListener('click', function () {
        $("#modalBeneficiarios").modal('show');
    });

    document.getElementById('btnIncluir').addEventListener('click', function (e) {
        addBeneficiario(e);
    });

    document.getElementById('beneficiariosTableBody').addEventListener('click', function (event) {
        if (event.target.classList.contains('removerBeneficiario')) {
            if (confirm("Quer mesmo excluir o beneficiário?")) {
                removerBeneficiario(event.target);
            } else { }
        } else if (event.target.classList.contains('editBeneficiario')) {
            editarBeneficiario(event.target);
        }
    });

    function listaBeneficiarios() {
        const beneficiarios = [];
        const linhas = document.querySelectorAll('#beneficiariosTableBody tr');

        linhas.forEach(linha => {
            const Id = linha.cells[0].textContent.trim();
            const CPF = linha.cells[1].textContent.trim();
            const Nome = linha.cells[2].textContent.trim();

            beneficiarios.push({ Id, CPF, Nome });
        });

        return beneficiarios;
    }

    let cpfInputs = document.getElementsByClassName("cpf_format");
    Array.from(cpfInputs).forEach(input => {
        input.addEventListener('input', function (e) {
            let cpf = e.target.value.replace(/\D/g, '');
            if (cpf.length > 11) cpf = cpf.slice(0, 11);

            cpf = cpf.replace(/(\d{3})(\d)/, '$1.$2');
            cpf = cpf.replace(/(\d{3})(\d)/, '$1.$2');
            cpf = cpf.replace(/(\d{3})(\d{1,2})$/, '$1-$2');

            e.target.value = cpf;
        });
    });

    function cpfExiste(cpf) {
        let tableBody = document.getElementById("beneficiariosTableBody")
        const linhas = tableBody.getElementsByTagName('tr');
        for (let linha of linhas) {
            const cpfCell = linha.getElementsByTagName('td')[1];
            if (cpfCell && cpfCell.textContent === cpf) {
                return true;
            }
        }
        return false;
    }

    function addBeneficiario(event) {
        event.preventDefault();

        let cpf = document.getElementById("beneficiarioCpfInput").value;
        let name = document.getElementById("beneficiarioNomeInput").value;

        if (cpf && name) {

            if (!cpfValido(cpf)) {
                alert("CPF inválido.");
                return;
            }

            if (name.length > 50) {
                alert("Nome inválido.");
                return;
            }

            if (cpfExiste(cpf)) {
                alert("CPF já adicionado.");
                document.getElementById("beneficiarioCpfInput").value = "";
                document.getElementById("beneficiarioNomeInput").value = "";
                return;
            }

            if (cpf == document.getElementById("CPF").value) {
                alert("Este CPF é seu, tente outro.");
                document.getElementById("beneficiarioCpfInput").value = "";
                document.getElementById("beneficiarioNomeInput").value = "";
                return;
            }

            const tableBody = document.getElementById("beneficiariosTableBody");
            const novaLinha = document.createElement("tr");

            novaLinha.innerHTML = `
            <td style='display: none'>0</td>
            <td>${cpf}</td>
            <td>${name}</td>
            <td>
                <button class="btn btn-primary btn-sm editBeneficiario">Alterar</button> 
                <button class="btn btn-primary btn-sm removerBeneficiario">Excluir</button>
            </td>
            `;

            tableBody.appendChild(novaLinha);

            document.getElementById("beneficiarioCpfInput").value = "";
            document.getElementById("beneficiarioNomeInput").value = "";
        } else {
            alert("CPF e Nome são obrigatórios.");
        }
    }

    function removerBeneficiario(button) {
        const linha = button.closest('tr');
        if (linha) {
            linha.remove();
        }
    }

    function editarBeneficiario(button) {
        const linha = button.closest('tr');
        if (linha) {
            const nomeCell = linha.querySelector('td:nth-child(3)');
            const nomeAtual = nomeCell.textContent;
            const novoName = prompt('Alterar Nome do Beneficiário:', nomeAtual);

            if (novoName) {
                if (novoName.length > 50) {
                    alert("Nome inválido.");
                    return;
                }
                                
                if (novoName) {
                    nomeCell.textContent = novoName;
                }
            } else {
                alert("Campo Nome é obrigatório.");
            }
        }
    }

    function cpfValido(cpf) {
        cpf = cpf.replace(/[^\d]+/g, '');

        if (cpf.length !== 11 || /^(\d)\1{10}$/.test(cpf)) {
            return false;
        }

        let soma = 0;
        let resto;

        for (let i = 1; i <= 9; i++) {
            soma += parseInt(cpf[i - 1]) * (11 - i);
        }

        resto = (soma * 10) % 11;
        if (resto === 10 || resto === 11) resto = 0;
        if (resto !== parseInt(cpf[9])) return false;

        soma = 0;
        for (let i = 1; i <= 10; i++) {
            soma += parseInt(cpf[i - 1]) * (12 - i);
        }
        resto = (soma * 10) % 11;
        if (resto === 10 || resto === 11) resto = 0;
        if (resto !== parseInt(cpf[10])) return false;

        return true;
    }

    function carregarBeneficiarios(beneficiarios) {
        const tableBody = document.getElementById('beneficiariosTableBody');
        tableBody.innerHTML = '';

        beneficiarios.forEach(beneficiary => {
            const linha = document.createElement('tr');

            const idCell = document.createElement('td');
            idCell.textContent = beneficiary.Id;
            idCell.style.display = 'none';
            linha.appendChild(idCell);

            const cpfCell = document.createElement('td');
            cpfCell.textContent = beneficiary.CPF;
            linha.appendChild(cpfCell);

            const nomeCell = document.createElement('td');
            nomeCell.textContent = beneficiary.Nome;
            linha.appendChild(nomeCell);

            const acoesCell = document.createElement('td');

            const editarButton = document.createElement('button');
            editarButton.className = 'btn btn-primary btn-sm editBeneficiario';
            editarButton.textContent = 'Alterar';
            editarButton.style.marginRight = '3px';
            acoesCell.appendChild(editarButton);

            const excluirButton = document.createElement('button');
            excluirButton.className = 'btn btn-primary btn-sm removerBeneficiario';
            excluirButton.textContent = 'Excluir';
            acoesCell.appendChild(excluirButton);

            linha.appendChild(acoesCell);

            tableBody.appendChild(linha);
        });
    }

})

function ModalDialog(titulo, texto) {
    var random = Math.random().toString().replace('.', '');
    var texto = '<div id="' + random + '" class="modal fade">                                                               ' +
        '        <div class="modal-dialog">                                                                                 ' +
        '            <div class="modal-content">                                                                            ' +
        '                <div class="modal-header">                                                                         ' +
        '                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>         ' +
        '                    <h4 class="modal-title">' + titulo + '</h4>                                                    ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-body">                                                                           ' +
        '                    <p>' + texto + '</p>                                                                           ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-footer">                                                                         ' +
        '                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>             ' +
        '                                                                                                                   ' +
        '                </div>                                                                                             ' +
        '            </div><!-- /.modal-content -->                                                                         ' +
        '  </div><!-- /.modal-dialog -->                                                                                    ' +
        '</div> <!-- /.modal -->                                                                                        ';

    $('body').append(texto);
    $('#' + random).modal('show');
}
