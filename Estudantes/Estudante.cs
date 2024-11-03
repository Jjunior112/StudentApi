
public class Estudante
{
    public Guid Id { get; init; }
    public string Name { get; private set; }
    public bool Ativo { get; private set; }


    public Estudante(string name)

    {
        Name = name;
        Id = Guid.NewGuid();
        Ativo = true;
    }

    public void NameUpdate(string name)
    {
        Name = name;
    }
    public void StudentDesactive(){
        Ativo = false;
    }

}
