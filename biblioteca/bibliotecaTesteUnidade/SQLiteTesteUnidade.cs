using Microsoft.VisualStudio.TestTools.UnitTesting;
using biblioteca;
using System;
using System.Data;

namespace bibliotecaTesteUnidade
{
    [TestClass]
    public class SQLiteTesteUnidade
    {

        string titulo = "TITULO",
                autor = "AUTOR",
                editora = "EDITORA",
                publicado = "2022",
                quantidade = "1",
                descricao = "DESCRICAO",
                capa = "",
                pesquisa = "TITULO",
                atributo = "titulo";

        SQLite banco = new SQLite();
        DataTable busca = new DataTable();

        [TestMethod]
        public void TestInserirLivro()
        {
            bool resultado = banco.InserirLivro(titulo, autor, editora, publicado, quantidade, descricao, capa);
            Assert.IsTrue(resultado);

            if (resultado)
            {
                busca = banco.ProcurarLivro(pesquisa, atributo);
            }

            if (resultado)
            {
                banco.ExcluirLivro(busca.Rows[0][0].ToString());
            }
        }

        [TestMethod]
        public void TestProcurarLivro()
        {
            bool resultado = banco.InserirLivro(titulo, autor, editora, publicado, quantidade, descricao, capa);

            if (resultado)
            {
                busca = banco.ProcurarLivro(pesquisa, atributo);
            }

            Assert.IsTrue(busca.Rows.Count > 0);

            if (resultado)
            {
                banco.ExcluirLivro(busca.Rows[0][0].ToString());
            }

        }

        [TestMethod]
        public void TestExcluirLivro()
        {
            bool resultado = banco.InserirLivro(titulo, autor, editora, publicado, quantidade, descricao, capa);

            if (resultado)
            {
                busca = banco.ProcurarLivro(pesquisa, atributo);
            }

            if (resultado)
            {
                banco.ExcluirLivro(busca.Rows[0][0].ToString());
            }

            if (resultado)
            {
                busca = banco.ProcurarLivro(pesquisa, atributo);
                Assert.IsFalse(busca.Rows.Count > 0);
            }

        }
    }
}
