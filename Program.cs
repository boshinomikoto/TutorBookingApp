var tasks = new List<string>();

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1 - додати завдання");
    Console.WriteLine("2 - показати список");
    Console.WriteLine("3 - вийти");
    Console.Write("> ");

    var choice = Console.ReadLine();

    if (choice == "1")
    {
        Console.Write("Текст завдання: ");
        var text = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(text))
        {
            tasks.Add(text);
            Console.WriteLine("Додано.");
        }
    }
    else if (choice == "2")
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("Список порожній.");
        }
        else
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tasks[i]}");
            }
        }
    }
    else if (choice == "3")
    {
        break;
    }
    else
    {
        Console.WriteLine("Не зрозумів, спробуй ще раз.");
    }
}
