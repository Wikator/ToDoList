namespace ToDoList.DataAccess.DbInitializer;

public interface IDbInitializer
{
    Task InitializeAsync();
}