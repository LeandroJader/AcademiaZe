using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;
using AcademiaDoZe.Infrastructure.Tests;

namespace AcademiaDoZe.Infrastructure.Tests;

public class ColaboradorInfrastructureTests : TestBase
{
    [Fact]
    public async Task Colaborador_LogradouroPorId_CpfJaExiste_Adicionar()
    {
        var logradouroId = 4;
        var repoLogradouro = new LogradouroRepository(ConnectionString, DatabaseType);
        Logradouro? logradouro = await repoLogradouro.ObterPorId(logradouroId);

        Arquivo arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });

        // CPF aleatório para evitar duplicidade
        var random = new Random();
        var cpfAleatorio = string.Concat(Enumerable.Range(0, 11).Select(_ => random.Next(0, 10)));

        var repoColaborador = new ColaboradorRepository(ConnectionString, DatabaseType);
        var cpfExistente = await repoColaborador.CpfJaExiste(cpfAleatorio);
        Assert.False(cpfExistente, "CPF já existe no banco de dados.");

        var colaborador = Colaborador.Criar(
            1,
            "zé",
            cpfAleatorio,
            new DateOnly(2010, 10, 09),
            "49999999999",
            "ze@com.br",
            logradouro!,
            "123",
            "complemento casa",
            "abcBolinhas",
            arquivo,
            new DateOnly(2024, 05, 04),
            EColaboradorTipo.Administrador,
            EColaboradorVinculo.CLT
        );

        var colaboradorInserido = await repoColaborador.Adicionar(colaborador);
        Assert.NotNull(colaboradorInserido);
        Assert.True(colaboradorInserido.Id > 0);
    }

    [Fact]
    public async Task Colaborador_ObterPorCpf_Atualizar()
    {
        var repoColaborador = new ColaboradorRepository(ConnectionString, DatabaseType);

        // Obter o colaborador mais recente para evitar conflitos
        var colaboradorExistente = (await repoColaborador.ObterTodos()).LastOrDefault();
        Assert.NotNull(colaboradorExistente);

        Arquivo arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });

        var colaboradorAtualizado = Colaborador.Criar(
            colaboradorExistente.Id,
            "zé dos testes 123",
            colaboradorExistente.Cpf,
            colaboradorExistente.DataNascimento,
            colaboradorExistente.Telefone,
            colaboradorExistente.Email,
            colaboradorExistente.Endereco,
            colaboradorExistente.Numero,
            colaboradorExistente.Complemento,
            colaboradorExistente.Senha,
            arquivo,
            colaboradorExistente.DataAdmissao,
            colaboradorExistente.Tipo,
            colaboradorExistente.Vinculo
        );

        typeof(Entity).GetProperty("Id")?.SetValue(colaboradorAtualizado, colaboradorExistente.Id);

        var resultadoAtualizacao = await repoColaborador.Atualizar(colaboradorAtualizado);
        Assert.NotNull(resultadoAtualizacao);
        Assert.Equal("zé dos testes 123", resultadoAtualizacao.Nome);
    }

    [Fact]
    public async Task Colaborador_ObterPorCpf_TrocarSenha()
    {
        var repoColaborador = new ColaboradorRepository(ConnectionString, DatabaseType);
        var colaboradorExistente = (await repoColaborador.ObterTodos()).LastOrDefault();
        Assert.NotNull(colaboradorExistente);

        var novaSenha = "novaSenha123";
        var resultadoTrocaSenha = await repoColaborador.TrocarSenha(colaboradorExistente.Id, novaSenha);
        Assert.True(resultadoTrocaSenha);

        var colaboradorAtualizado = await repoColaborador.ObterPorId(colaboradorExistente.Id);
        Assert.NotNull(colaboradorAtualizado);
        Assert.Equal(novaSenha, colaboradorAtualizado.Senha);
    }

    [Fact]
    public async Task Colaborador_ObterPorCpf_Remover_ObterPorId()
    {
        var repoColaborador = new ColaboradorRepository(ConnectionString, DatabaseType);
        var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);

        var colaboradorExistente = (await repoColaborador.ObterTodos()).LastOrDefault();
        Assert.NotNull(colaboradorExistente);

        // Remover todas as matrículas associadas antes de remover colaborador
        var matriculas = await repoMatricula.ObterPorAluno(colaboradorExistente.Id);
        if (matriculas != null)
        {
            foreach (var m in matriculas)
            {
                await repoMatricula.Remover(m.Id);
            }
        }

        var resultadoRemover = await repoColaborador.Remover(colaboradorExistente.Id);
        Assert.True(resultadoRemover);

        var resultadoRemovido = await repoColaborador.ObterPorId(colaboradorExistente.Id);
        Assert.Null(resultadoRemovido);
    }

    [Fact]
    public async Task Colaborador_ObterTodos()
    {
        var repoColaborador = new ColaboradorRepository(ConnectionString, DatabaseType);
        var resultado = await repoColaborador.ObterTodos();
        Assert.NotNull(resultado);
    }
}
//leandro jader