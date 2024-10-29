using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using FI.AtividadeEntrevista.DML;

namespace FI.AtividadeEntrevista.DAL
{
    /// <summary>
    /// Classe de acesso a dados de Beneficiario
    /// </summary>
    internal class DaoBeneficiario : AcessoDados
    {
        /// <summary>
        /// Retorna Beneficiarios de um cliente
        /// </summary>
        /// <param name="Id">Id do cliente</param>
        /// <returns>Lista de beneficiarios</returns>
        internal List<DML.Beneficiario> Consultar(long Id)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Id", Id));

            DataSet ds = base.Consultar("FI_SP_ConsBeneficiario", parametros);
            List<DML.Beneficiario> beneficiarios = Converter(ds);

            return beneficiarios;
        }

        /// <summary>
        /// Concilia beneficiarios 
        /// </summary>
        /// <param name="id">Id do cliente</param>
        /// <param name="beneficiarios">Lista de beneficiarios</param>
        internal void Conciliar(long id, List<DML.Beneficiario> beneficiarios)
        {
            if (beneficiarios == null || !beneficiarios.Any())
            {
                List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

                parametros.Add(new System.Data.SqlClient.SqlParameter("IDCLIENTE", id));

                base.Executar("FI_SP_DelBenef", parametros);

                return;
            }

            var novosBeneficiarios = beneficiarios.Where(b => b.Id == 0).ToList();
            if (novosBeneficiarios.Any())
            {
                List<System.Data.SqlClient.SqlParameter> parametrosNovos = new List<System.Data.SqlClient.SqlParameter>();
                var dataTable = ConverterDT(novosBeneficiarios);

                parametrosNovos.Add(new System.Data.SqlClient.SqlParameter("Beneficiarios", dataTable));

                base.Executar("FI_SP_GravaBeneficiario", parametrosNovos);
            }

            var beneficiariosAlterar = beneficiarios.Where(b => b.Id > 0).ToList();
            foreach (var beneficiario in beneficiariosAlterar)
            {
                List<System.Data.SqlClient.SqlParameter> parametrosAlterar = new List<System.Data.SqlClient.SqlParameter>();
                parametrosAlterar.Add(new System.Data.SqlClient.SqlParameter("NOME", beneficiario.Nome));
                parametrosAlterar.Add(new System.Data.SqlClient.SqlParameter("CPF", beneficiario.CPF));
                parametrosAlterar.Add(new System.Data.SqlClient.SqlParameter("ID", beneficiario.Id));

                base.Executar("FI_SP_AltBenef", parametrosAlterar);
            }

            var beneficiariosRemover = BeneficiariosRemover(id, beneficiarios);
            var dataTableConciliacao = ConverterDT(beneficiariosRemover);
            List<System.Data.SqlClient.SqlParameter> parametrosConciliacao = new List<System.Data.SqlClient.SqlParameter>();
            parametrosConciliacao.Add(new System.Data.SqlClient.SqlParameter("Beneficiarios", dataTableConciliacao));

            base.Executar("FI_SP_ConciliaBeneficiario", parametrosConciliacao);
        }

        private List<Beneficiario> BeneficiariosRemover(long id, List<Beneficiario> beneficiarios)
        {
            var beneficiariosExistentes = Consultar(id);

            var beneficiariosComuns = beneficiariosExistentes
                .Select(be => be.CPF)
                .Intersect(beneficiarios.Select(b => b.CPF))
                .ToHashSet();

            return beneficiariosExistentes
                .Concat(beneficiarios)
                .Where(c => !beneficiariosComuns.Contains(c.CPF))
                .ToList();
        }

        private DataTable ConverterDT(List<DML.Beneficiario> beneficiarios)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("CPF", typeof(string));
            dt.Columns.Add("NOME", typeof(string));
            dt.Columns.Add("IDCLIENTE", typeof(long));

            if (beneficiarios != null)
                foreach (var beneficiario in beneficiarios)
                    dt.Rows.Add(beneficiario.Id, beneficiario.CPF, beneficiario.Nome, beneficiario.IdCliente);

            return dt;
        }

        private List<DML.Beneficiario> Converter(DataSet ds)
        {
            List<DML.Beneficiario> lista = new List<DML.Beneficiario>();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DML.Beneficiario ben = new DML.Beneficiario();
                    ben.Id = row.Field<long>("ID");
                    ben.CPF = row.Field<string>("CPF");
                    ben.Nome = row.Field<string>("NOME");
                    ben.IdCliente = row.Field<long>("IDCLIENTE");

                    lista.Add(ben);
                }
            }

            return lista;
        }
    }
}
