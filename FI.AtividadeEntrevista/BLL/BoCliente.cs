using System.Collections.Generic;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoCliente
    {
        /// <summary>
        /// Inclui um novo cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        /// <returns>Retorna tupla (long, bool): (Id do Cliente, Se cliente é repetido)</returns>
        public (long, bool) Incluir(DML.Cliente cliente, List<DML.Beneficiario> beneficiarios)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();

            var repetido = cli.VerificarExistencia(cliente.CPF);
            if (repetido)
                return (0, repetido);

            var id = cli.Incluir(cliente);

            if (beneficiarios != null)
            {
                DAL.DaoBeneficiario ben = new DAL.DaoBeneficiario();
                beneficiarios.ForEach(b => b.IdCliente = id);
                ben.Conciliar(id, beneficiarios);
            }

            return (id, repetido);
        }

        /// <summary>
        /// Altera um cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        /// <returns>Retorna bool: Faz validação do CPF e retorna se foi alterado ou não</returns>
        public bool Alterar(DML.Cliente cliente, List<DML.Beneficiario> beneficiarios)
        {
            var alterado = true;

            try
            {
                DAL.DaoCliente cli = new DAL.DaoCliente();
                cli.Alterar(cliente);

                DAL.DaoBeneficiario ben = new DAL.DaoBeneficiario();
                ben.Conciliar(cliente.Id, beneficiarios);
            }
            catch (System.Exception ex)
            {
                if (!ex.Message.ToUpper().Contains("UNIQUE KEY"))
                    throw;

                alterado = false;
            }

            return alterado;
        }

        /// <summary>
        /// Consulta o cliente pelo id
        /// </summary>
        /// <param name="id">id do cliente</param>
        /// <returns></returns>
        public (DML.Cliente, List<DML.Beneficiario>) Consultar(long id)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            var cliente = cli.Consultar(id);

            DAL.DaoBeneficiario ben = new DAL.DaoBeneficiario();
            var beneficiarios = ben.Consultar(id);

            return (cliente, beneficiarios);
        }

        /// <summary>
        /// Excluir o cliente pelo id
        /// </summary>
        /// <param name="id">id do cliente</param>
        /// <returns></returns>
        public void Excluir(long id)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            cli.Excluir(id);
        }

        /// <summary>
        /// Lista os clientes
        /// </summary>
        public List<DML.Cliente> Listar()
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            return cli.Listar();
        }

        /// <summary>
        /// Lista os clientes
        /// </summary>
        public List<DML.Cliente> Pesquisa(int iniciarEm, int quantidade, string campoOrdenacao, bool crescente, out int qtd)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            return cli.Pesquisa(iniciarEm,  quantidade, campoOrdenacao, crescente, out qtd);
        }

        /// <summary>
        /// VerificaExistencia
        /// </summary>
        /// <param name="CPF"></param>
        /// <returns></returns>
        public bool VerificarExistencia(string CPF)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            return cli.VerificarExistencia(CPF);
        }
    }
}
