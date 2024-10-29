using System.Text.RegularExpressions;

namespace FI.WebAtividadeEntrevista.Auxiliar.Extensoes
{
    public static class Documentos
    {
        public static bool CpfValido(this string cpf)
        {
            if (string.IsNullOrEmpty(cpf)) 
                return false;

            // Remove non-digit characters
            cpf = Regex.Replace(cpf, @"[^\d]", "");

            // Check if CPF has 11 digits and is not a sequence of the same digit
            if (cpf.Length != 11 || Regex.IsMatch(cpf, @"^(\d)\1{10}$"))
            {
                return false;
            }

            int soma = 0;
            int resto;

            // First Check Digit Calculation
            for (int i = 1; i <= 9; i++)
            {
                soma += int.Parse(cpf[i - 1].ToString()) * (11 - i);
            }

            resto = (soma * 10) % 11;
            if (resto == 10 || resto == 11) resto = 0;
            if (resto != int.Parse(cpf[9].ToString())) return false;

            // Second Check Digit Calculation
            soma = 0;
            for (int i = 1; i <= 10; i++)
            {
                soma += int.Parse(cpf[i - 1].ToString()) * (12 - i);
            }

            resto = (soma * 10) % 11;
            if (resto == 10 || resto == 11) resto = 0;
            if (resto != int.Parse(cpf[10].ToString())) return false;

            return true;
        }

        public static string ApenasNumeros(this string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            return Regex.Replace(texto, "[^0-9]", "");
        }

        public static string CPFformatado(this string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            texto = Regex.Replace(texto, "[^0-9]", "");

            if (texto.Length != 11)
                return texto;

            return $"{texto.Substring(0, 3)}.{texto.Substring(3, 3)}.{texto.Substring(6, 3)}-{texto.Substring(9, 2)}";
        }
    }
}
