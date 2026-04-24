namespace CommunityApp;

using Spectre.Console;

public class EventView
{
    private readonly EventService _eventService = new();

    public void Add(Resident currentUser)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Panel("[bold blue]Add Event[/]").BorderColor(Color.Blue));

        var title = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter [green]title[/]:")
                .Validate(input => string.IsNullOrWhiteSpace(input)
                    ? ValidationResult.Error("[red]Title is required[/]")
                    : ValidationResult.Success()));

        var description = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter [green]description[/]:")
                .Validate(input => string.IsNullOrWhiteSpace(input)
                    ? ValidationResult.Error("[red]Description is required[/]")
                    : ValidationResult.Success()));

        var location = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter [green]location[/]:")
                .Validate(input => string.IsNullOrWhiteSpace(input)
                    ? ValidationResult.Error("[red]Location is required[/]")
                    : ValidationResult.Success()));

        DateTime start;
        DateTime end;

        while (true)
        {
            var startStr = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter [green]start date/time[/] (yyyy-MM-dd HH:mm):"));

            if (!DateTime.TryParse(startStr, out start))
            {
                AnsiConsole.MarkupLine("[red]Invalid date format. Try again.[/]");
                continue;
            }

            var endStr = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter [green]end date/time[/] (yyyy-MM-dd HH:mm):"));

            if (!DateTime.TryParse(endStr, out end))
            {
                AnsiConsole.MarkupLine("[red]Invalid date format. Try again.[/]");
                continue;
            }

            if (end <= start)
            {
                AnsiConsole.MarkupLine("[red]End time must be after start time. Try again.[/]");
                continue;
            }

            break;
        }

        bool hasConflict = _eventService.HasConflict(start, end);
        EventType eventType;

        if (hasConflict)
        {
            AnsiConsole.MarkupLine("[yellow]⚠ Warning: This time slot conflicts with an existing event.[/]");
            if (!AnsiConsole.Confirm("Continue anyway?"))
            {
                AnsiConsole.MarkupLine("[red]Cancelled.[/]");
                AnsiConsole.MarkupLine("[grey]Press any key to return...[/]");
                Console.ReadKey();
                return;
            }
            eventType = EventType.Standard;
        }
        else
        {
            eventType = EventType.Standard;
            AnsiConsole.MarkupLine("[grey]Event type: Standard[/]");
        }

        if (!AnsiConsole.Confirm("Save event?"))
        {
            AnsiConsole.MarkupLine("[red]Cancelled.[/]");
            AnsiConsole.MarkupLine("[grey]Press any key to return...[/]");
            Console.ReadKey();
            return;
        }

        var creator = new Resident
        {
            Id = currentUser.Id,
            Name = currentUser.Name,
            Address = currentUser.Address,
            Role = currentUser.Role
        };

        var ev = new Event
        {
            Title = title,
            Description = description,
            Location = location,
            Start = start,
            End = end,
            EventType = eventType,
            CreatedBy = creator
        };

        _eventService.Save(ev);

        AnsiConsole.MarkupLine("[green]✔ Event saved successfully![/]");
        AnsiConsole.MarkupLine("[grey]Press any key to return...[/]");
        Console.ReadKey();
    }

    public void ViewAll()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Panel("[bold blue]Upcoming Events[/]").BorderColor(Color.Blue));

        var events = _eventService.GetAll();

        if (!events.Any())
        {
            AnsiConsole.MarkupLine("[grey]No events found.[/]");
            AnsiConsole.MarkupLine("[grey]Press any key to return...[/]");
            Console.ReadKey();
            return;
        }

        var choices = events.Select(e => $"{e.Start:MMM dd, yyyy h:mm tt} — {e.Title}").ToList();
        choices.Add("← Back");

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select an event to view details:")
                .PageSize(10)
                .AddChoices(choices));

        if (selected == "← Back") return;

        var index = choices.IndexOf(selected);
        DisplayEventDetails(events[index]);

        AnsiConsole.MarkupLine("[grey]Press any key to return...[/]");
        Console.ReadKey();
    }

    public void DisplayEventDetails(Event ev)
    {
        AnsiConsole.Clear();

        AnsiConsole.Write(
            new Panel($"[bold blue]{ev.Title}[/]")
                .BorderColor(Color.Blue));

        AnsiConsole.MarkupLine($"[bold]Description:[/] {ev.Description}");
        AnsiConsole.MarkupLine($"[bold]Date & Time:[/] {ev.Start:MMM dd, yyyy h:mm tt} - {ev.End:MMM dd, yyyy h:mm tt}");
        AnsiConsole.MarkupLine($"[bold]Location:[/] {ev.Location}");
        AnsiConsole.MarkupLine($"[bold]Type:[/] {ev.EventType}");
        AnsiConsole.MarkupLine($"[bold]Created By:[/] {ev.CreatedBy.Name}");
    }
}