using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests;

public class MatriculaInfrastructureTests : TestBase
{
    [Fact]
    public async Task Matricula_Adicionar()
    {
        var repoLogradouro = new LogradouroRepository(ConnectionString, DatabaseType);
        var logradouro = await repoLogradouro.ObterPorId(4);
        Assert.NotNull(logradouro);

        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });


        var random = new Random();
        var _cpf = string.Concat(Enumerable.Range(0, 11).Select(_ => random.Next(0, 10)));

        var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);
        Assert.False(await repoAluno.CpfJaExiste(_cpf), "CPF já existe no banco.");

        var aluno = Aluno.Criar(
            1,
            "Aluno Teste",
            _cpf,
            new DateOnly(2010, 10, 09),
            "49999999999",
            "aluno@teste.com",
            logradouro!,
            "123",
            "Complemento casa",
            "Senha@123",
            arquivo
        );

        await repoAluno.Adicionar(aluno);

        var matricula = Matricula.Criar(
            1,
            aluno,
            EMatriculaPlano.Semestral,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddMonths(6)),
            "Emagrecer",
            EMatriculaRestricoes.None,
            arquivo,
            "Sem observações"
        );

        var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
        var matriculaInserida = await repoMatricula.Adicionar(matricula);

        Assert.NotNull(matriculaInserida);
        Assert.True(matriculaInserida.Id > 0);

 
        var removerMatricula = await repoMatricula.Remover(matriculaInserida.Id);
        Assert.True(removerMatricula);

        var removerAluno = await repoAluno.Remover(aluno.Id);
        Assert.True(removerAluno);
    }

    [Fact]
    public async Task Matricula_ObterPorAluno_Atualizar()
    {
        var repoLogradouro = new LogradouroRepository(ConnectionString, DatabaseType);
        var logradouro = await repoLogradouro.ObterPorId(4);
        Assert.NotNull(logradouro);

        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });
        var random = new Random();
        var _cpf = string.Concat(Enumerable.Range(0, 11).Select(_ => random.Next(0, 10)));

        var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);
        var aluno = Aluno.Criar(
            1,
            "Aluno Teste",
            _cpf,
            new DateOnly(2010, 10, 09),
            "49999999999",
            "aluno@teste.com",
            logradouro!,
            "123",
            "Complemento casa",
            "Senha@123",
            arquivo
        );

        await repoAluno.Adicionar(aluno);

        var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
        var matricula = Matricula.Criar(
            1,
            aluno,
            EMatriculaPlano.Semestral,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddMonths(6)),
            "Emagrecer",
            EMatriculaRestricoes.None,
            arquivo,
            "Sem observações"
        );

        await repoMatricula.Adicionar(matricula);

        var matriculaAtualizada = Matricula.Criar(
            1,
            aluno,
            EMatriculaPlano.Anual,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddYears(1)),
            "Hipertrofia",
            EMatriculaRestricoes.None,
            arquivo,
            "Observação atualizada"
        );

        typeof(Entity).GetProperty("Id")?.SetValue(matriculaAtualizada, matricula.Id);

        var resultado = await repoMatricula.Atualizar(matriculaAtualizada);
        Assert.NotNull(resultado);
        Assert.Equal("Hipertrofia", resultado.Objetivo);
        Assert.Equal("Observação atualizada", resultado.ObservacoesRestricoes);
        Assert.Equal(EMatriculaPlano.Anual, resultado.Plano);

        // Limpeza
        await repoMatricula.Remover(resultado.Id);
        await repoAluno.Remover(aluno.Id);
    }

    [Fact]
    public async Task Matricula_ObterPorAluno_Remover_ObterPorId()
    {
        var repoLogradouro = new LogradouroRepository(ConnectionString, DatabaseType);
        var logradouro = await repoLogradouro.ObterPorId(4);
        Assert.NotNull(logradouro);

        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });
        var random = new Random();
        var _cpf = string.Concat(Enumerable.Range(0, 11).Select(_ => random.Next(0, 10)));

        var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);
        var aluno = Aluno.Criar(
            1,
            "Aluno Teste",
            _cpf,
            new DateOnly(2010, 10, 09),
            "49999999999",
            "aluno@teste.com",
            logradouro!,
            "123",
            "Complemento casa",
            "Senha@123",
            arquivo
        );

        await repoAluno.Adicionar(aluno);

        var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
        var matricula = Matricula.Criar(
            1,
            aluno,
            EMatriculaPlano.Semestral,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddMonths(6)),
            "Emagrecer",
            EMatriculaRestricoes.None,
            arquivo,
            "Sem observações"
        );

        await repoMatricula.Adicionar(matricula);


        var resultadoRemocao = await repoMatricula.Remover(matricula.Id);
        Assert.True(resultadoRemocao);


        var matriculaRemovida = await repoMatricula.ObterPorId(matricula.Id);
        Assert.Null(matriculaRemovida);


        await repoAluno.Remover(aluno.Id);
    }

    [Fact]
    public async Task Matricula_ObterTodos()
    {
        var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
        var resultado = await repoMatricula.ObterTodos();
        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task Matricula_ObterPorId()
    {
        var repoLogradouro = new LogradouroRepository(ConnectionString, DatabaseType);
        var logradouro = await repoLogradouro.ObterPorId(4);
        Assert.NotNull(logradouro);

        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });
        var random = new Random();
        var _cpf = string.Concat(Enumerable.Range(0, 11).Select(_ => random.Next(0, 10)));

        var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);
        var aluno = Aluno.Criar(
            1,
            "Aluno Teste",
            _cpf,
            new DateOnly(2010, 10, 09),
            "49999999999",
            "aluno@teste.com",
            logradouro!,
            "123",
            "Complemento casa",
            "Senha@123",
            arquivo
        );

        await repoAluno.Adicionar(aluno);

        var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
        var matricula = Matricula.Criar(
            1,
            aluno,
            EMatriculaPlano.Semestral,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddMonths(6)),
            "Emagrecer",
            EMatriculaRestricoes.None,
            arquivo,
            "Sem observações"
        );

        await repoMatricula.Adicionar(matricula);

        var matriculaPorId = await repoMatricula.ObterPorId(matricula.Id);
        Assert.NotNull(matriculaPorId);
        Assert.Equal(matricula.Id, matriculaPorId.Id);

        // Limpeza
        await repoMatricula.Remover(matricula.Id);
        await repoAluno.Remover(aluno.Id);
    }
}
//Leandro Jader