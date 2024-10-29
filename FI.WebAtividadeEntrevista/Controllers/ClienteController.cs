using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using FI.AtividadeEntrevista.DML;
using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using FI.WebAtividadeEntrevista.Models;
using FI.WebAtividadeEntrevista.Auxiliar.Extensoes;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

            if (!model.CPF.CpfValido())
                this.ModelState.AddModelError("CPF", "CPF inválido.");

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                var (id, repetido) = bo.Incluir(new Cliente()
                {                    
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    CPF = model.CPF.ApenasNumeros(),
                    Telefone = model.Telefone
                },
                model?.Beneficiarios?.Select(b => new Beneficiario
                {
                    CPF = b.CPF.ApenasNumeros(),
                    Nome = b.Nome
                }).ToList());

                model.Id = id;

                return Json(
                    repetido ? 
                    "CPF já cadastrado." :
                    "Cadastro efetuado com sucesso.");
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

            if (!model.CPF.CpfValido())
                this.ModelState.AddModelError("CPF", "CPF inválido.");

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                var alterado = bo.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,  
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    CPF = model.CPF.ApenasNumeros(),
                    Telefone = model.Telefone
                },
                model?.Beneficiarios?.Select(b => new Beneficiario
                {
                    Id = b.Id,
                    CPF = b.CPF.ApenasNumeros(),
                    Nome = b.Nome,
                    IdCliente = model.Id
                }).ToList());

                if (!alterado)
                {
                    Response.StatusCode = 400;
                    return Json("CPF já utilizado, tente outro.");
                }

                return Json("Cadastro alterado com sucesso.");
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Models.ClienteModel model = null;
            var (cliente, beneficiarios) = bo.Consultar(id);

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    CPF = cliente.CPF.CPFformatado(),
                    Telefone = cliente.Telefone, 
                    Beneficiarios = beneficiarios?.Select(b => new BeneficiarioModel
                    {
                        Id = b.Id, 
                        CPF = b.CPF.CPFformatado(), 
                        Nome = b.Nome, 
                        IdCliente = b.IdCliente
                    }).ToList()
                };
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}