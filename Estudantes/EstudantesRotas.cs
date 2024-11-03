using Microsoft.EntityFrameworkCore;

public static class EstudantesRotas

{
    public static void AddRotasEstudantes(this WebApplication app)
    {

        var rotasEstudantes = app.MapGroup(prefix: "estudantes");

        // Post
        rotasEstudantes.MapPost(pattern: "", handler: async (AddEstudanteRequest request, AppDbContext context, CancellationToken ct) =>
        {
            bool estudanteExiste = await context.Estudantes.AnyAsync(estudante => estudante.Name == request.Name);

            if (!estudanteExiste)
            {
                var novoEstudante = new Estudante(request.Name);

                await context.Estudantes.AddAsync(novoEstudante,ct);
                await context.SaveChangesAsync(ct);
                var estudanteRetorno = new EstudanteDto(novoEstudante.Id, novoEstudante.Name);
                return Results.Ok(estudanteRetorno);

            }
            else
            {
                return Results.Conflict(error: "Estudante ja cadastrado");
            }

        });

        // Get

        rotasEstudantes.MapGet(pattern: "", handler: async (AppDbContext context, CancellationToken ct) =>
        {
            var estudantes = await context.Estudantes.Where(estudante => estudante.Ativo == true)
            .Select(estudante => new EstudanteDto(estudante.Id, estudante.Name))
            .ToListAsync(ct);

            return estudantes;
        });

        //Put

        rotasEstudantes.MapPut(pattern: "{id:guid}", handler: async (Guid id, UpdateEstudanteRequest request, AppDbContext context, CancellationToken ct) =>
        {
            var estudante = await context.Estudantes.SingleOrDefaultAsync(estudante => estudante.Id == id,ct);

            if (estudante == null)

                return Results.NotFound();

            estudante.NameUpdate(request.Name);

            await context.SaveChangesAsync(ct);

            return Results.Ok(new EstudanteDto(estudante.Id, estudante.Name));

        });

        // delete (marcar ativos como false)

        rotasEstudantes.MapDelete(pattern: "{id:guid}", handler: async (Guid id, AppDbContext context, CancellationToken ct)
        =>{

            var estudante = await context.Estudantes.SingleOrDefaultAsync(estudante => estudante.Id == id,ct);
           
           if(estudante == null)
            return Results.NotFound();

            estudante.StudentDesactive();
            await context.SaveChangesAsync(ct);
            return Results.Ok();

        });

    }

}
