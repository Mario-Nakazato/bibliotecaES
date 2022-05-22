using System;

using System.IO;//File

using System.Data.SQLite;
using System.Data;

namespace biblioteca
{
    public class SQLite
    {
        private string bancoDados;
        private string conexao;
        private SQLiteConnection conectar;

        public SQLite()
        {
            //bancoDados = Application.StartupPath + @"\biblioteca.db";
            bancoDados = @"C:\Users\Notebook\Documents\Projeto\bibliotecaES\biblioteca\biblioteca\bin\Debug" + @"\biblioteca.db";
            conexao = @"Data Source = " + bancoDados + "; Version = 3";
            conectar = new SQLiteConnection(conexao);
        }

        public void Conectar()//Reconstruir o banco do zero. Descontinuado no momento
        {
            if (!File.Exists(bancoDados))
            {
                SQLiteConnection.CreateFile(bancoDados);
            }

            try
            {
                conectar.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao conectar SQLite \n" + ex);
            }
            finally
            {
                conectar.Close();
            }
        }

        public bool InserirLivro(string titulo, string autor, string editora, string publicado, string quantidade, string descricao, string capa)
        {
            bool inserido;
            try
            {
                conectar.Open();

                SQLiteCommand comando = new SQLiteCommand();
                comando.Connection = conectar;

                comando.CommandText = "INSERT INTO LIVRO (liv_titulo, liv_autor, liv_editora, liv_publicado, liv_quantidade, liv_descricao, liv_capa)" +
                    "VALUES " + "('" + titulo + "', '" + autor + "', '" + editora + "', '" + publicado + "', '" + quantidade + "', '" + descricao + "', '" + capa + "')";

                comando.ExecuteNonQuery();
                comando.Dispose();
                inserido = true;
            }
            catch (Exception ex)
            {
                inserido = false;
                Console.WriteLine("Erro ao inserir registro SQLite " + ex.Message);
            }
            finally
            {
                conectar.Close();
            }
            return inserido;
        }

        public void ExcluirLivro(string id)
        {
            try
            {
                conectar.Open();

                SQLiteCommand comando = new SQLiteCommand();
                comando.Connection = conectar;

                comando.CommandText = "DELETE FROM LIVRO WHERE liv_id = '" + id + "'";

                comando.ExecuteNonQuery();
                comando.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao excluir registro SQLite " + ex.Message);
            }
            finally
            {
                conectar.Close();
            }
        }

        public DataTable ProcurarLivro(string pesquisa, string atributo)
        {
            DataTable dados = new DataTable(); ;
            try
            {
                string query = "SELECT * FROM LIVRO";

                if (atributo == "titulo")
                {
                    if (pesquisa != "*")
                    {
                        query = "SELECT * FROM LIVRO WHERE liv_titulo LIKE '%" + pesquisa + "%'";
                    }
                }
                else if (atributo == "id")
                {
                    query = "SELECT * FROM LIVRO WHERE liv_id = " + pesquisa;
                }

                SQLiteDataAdapter adaptador = new SQLiteDataAdapter(query, conexao);

                conectar.Open();
                adaptador.Fill(dados);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao inserir registro SQLite " + ex.Message);
            }
            finally
            {
                conectar.Close();
            }
            return dados;
        }

        public void InserirEmprestimo(string codigo, string nomeCompleto, string dataEmprestimo, string dataDevolucao, string[] idLivros)
        {
            try
            {
                conectar.Open();

                SQLiteCommand comando = new SQLiteCommand();
                comando.Connection = conectar;

                comando.CommandText = "INSERT INTO EMPRESTIMO (emp_codigo, emp_nomeCompleto, emp_dataEmprestimo, emp_dataDevolucao)" +
                    "VALUES " + "('" + codigo + "', '" + nomeCompleto + "', '" + dataEmprestimo + "', '" + dataDevolucao + "')";

                comando.ExecuteNonQuery();

                for (int i = 0; i < idLivros.Length; i++)
                {
                    comando.CommandText = "INSERT INTO EMPRESTIMOLIVRO (emp_codigo, liv_id)" +
                    "VALUES " + "('" + codigo + "', '" + idLivros[i] + "')";

                    comando.ExecuteNonQuery();

                    comando.CommandText = "UPDATE LIVRO SET liv_quantidade = liv_quantidade -1 WHERE liv_id = '" + idLivros[i] + "'";

                    comando.ExecuteNonQuery();
                }

                comando.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao inserir registro SQLite " + ex.Message);
            }
            finally
            {
                conectar.Close();
            }
        }

        public DataTable ProcurarEmprestimo(string pesquisa, string atributo)
        {
            DataTable dados = new DataTable(); ;
            try
            {
                string query = "SELECT * FROM EMPRESTIMO";

                if (atributo == "codigo")
                {
                    if (pesquisa != "")
                    {
                        query = "SELECT * FROM EMPRESTIMO WHERE emp_codigo = " + pesquisa;
                    }
                }
                else if (atributo == "livro")
                {
                    query = "SELECT * FROM LIVRO WHERE liv_id IN ( SELECT liv_id FROM EMPRESTIMOLIVRO WHERE emp_codigo = '" + pesquisa + "')";
                }

                SQLiteDataAdapter adaptador = new SQLiteDataAdapter(query, conexao);

                conectar.Open();
                adaptador.Fill(dados);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao inserir registro SQLite " + ex.Message);
            }
            finally
            {
                conectar.Close();
            }
            return dados;
        }

        public void InserirDevolucao(string codigo)
        {
            try
            {
                conectar.Open();

                SQLiteCommand comando = new SQLiteCommand();
                comando.Connection = conectar;

                comando.CommandText = "UPDATE LIVRO SET liv_quantidade = liv_quantidade +1 WHERE liv_id IN ( SELECT liv_id FROM EMPRESTIMOLIVRO WHERE emp_codigo = '" + codigo + "')";
                comando.ExecuteNonQuery();

                comando.CommandText = "DELETE FROM EMPRESTIMO WHERE emp_codigo = '" + codigo + "'";
                comando.ExecuteNonQuery();

                comando.CommandText = "DELETE FROM EMPRESTIMOLIVRO WHERE emp_codigo = '" + codigo + "'";
                comando.ExecuteNonQuery();

                comando.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao excluir registro SQLite " + ex.Message);
            }
            finally
            {
                conectar.Close();
            }
        }
    }
}
