namespace CommunityApp;

using Spectre.Console;

public class EventView
{
    private readonly EventService _eventService = new();

    private Resident? GetCurrentUser()
    {
        var residents = new JsonDataService().LoadData<Resident>("Data/residents.json");
        return Globals.CurrentUser != null
            ? residents.FirstOrDefault(r => r.Id == Globals.CurrentUser.Id)
            : null;
    }

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

        var eventType = AnsiConsole.Prompt(
            new SelectionPrompt<EventType>()
                .Title("Select [green]event type[/]:")
                .AddChoices(EventType.Standard, EventType.GroupActivity));

        List<TimeSlot> timeSlots = new();

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
        }
        else
        {
            AnsiConsole.MarkupLine($"[grey]Event type: {eventType}[/]");
        }

        if (eventType == EventType.GroupActivity)
        {
            var slotLength = AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                    .Title("Select [yellow]time slot length[/] (minutes):")
                    .AddChoices(30, 60)
            );

            var duration = end - start;
            int totalSlots = (int)(duration.TotalMinutes / slotLength);

            for (int i = 0; i < totalSlots; i++)
            {
                var slotStart = start.AddMinutes(i * slotLength);
                var slotEnd = slotStart.AddMinutes(slotLength);
                int maxParticipants = AnsiConsole.Prompt(
                    new TextPrompt<int>($"Max participants for slot {slotStart:HH:mm} - {slotEnd:HH:mm}:").DefaultValue(10)
                );
                timeSlots.Add(new TimeSlot { Start = slotStart, End = slotEnd, MaxParticipants = maxParticipants });
            }
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
            CreatedBy = creator,
            TimeSlots = timeSlots
        };

        _eventService.Save(ev);

        var residents = new JsonDataService().LoadData<Resident>("Data/residents.json");
        var fullCreator = residents.FirstOrDefault(r => r.Id == currentUser.Id);
        if (fullCreator != null)
        {
            fullCreator.Commitments.Add(new Commitment { Resident = fullCreator, Event = ev, Attending = true });
            new JsonDataService().SaveData("Data/residents.json", residents);
        }

        AnsiConsole.MarkupLine("[green]✔ Event saved successfully![/]");
        AnsiConsole.MarkupLine("[grey]Press any key to return...[/]");
        Console.ReadKey();
    }

    public void SaveEventToCalendar(Event ev)
    {
        var residents = new JsonDataService().LoadData<Resident>("Data/residents.json");
        var _currentUser = Globals.CurrentUser != null
            ? residents.FirstOrDefault(r => r.Id == Globals.CurrentUser.Id)
            : null;
        if (_currentUser == null)
        {
            AnsiConsole.MarkupLine("[red]User not found. Cannot save event to calendar.[/]");
            return;
        }

        if (_currentUser.Commitments.Any(c => c.Event.Title == ev.Title && c.Event.Start == ev.Start))
        {
            AnsiConsole.MarkupLine("[yellow]Event is already in your calendar.[/]");
            return;
        }

        _currentUser.Commitments.Add(new Commitment { Resident = _currentUser, Event = ev, Attending = true });
        new JsonDataService().SaveData("Data/residents.json", residents);

        AnsiConsole.MarkupLine("[green]✔ Event saved to your calendar![/]");

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

        var currentUser = GetCurrentUser();
        var choices = events.Select(e =>
        {
            bool isAttending = _eventService.CurrentUserAttending(e);
            string attendingText = isAttending ? " ([green]Attending[/])" : "";
            return $"{e.Start:MMM dd, yyyy h:mm tt} — {e.Title}{attendingText}";
        }).ToList();

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

        if (ev.TimeSlots != null && ev.TimeSlots.Any())
        {
            AnsiConsole.MarkupLine("[bold]Time Slots:[/]");
            var slotLabels = ev.TimeSlots.Select((slot, idx) =>
                $"{idx + 1}. {slot.Start:MMM dd, yyyy HH:mm} to {slot.End:HH:mm} (Max: {slot.MaxParticipants})").ToList();

            var currentUser = GetCurrentUser();

            var alreadySignedUp = new HashSet<int>();
            if (currentUser != null)
            {
                for (int i = 0; i < ev.TimeSlots.Count; i++)
                {
                    var slot = ev.TimeSlots[i];
                    if (slot.Participants != null && slot.Participants.Any(p => p.Id == currentUser.Id))
                        alreadySignedUp.Add(i);
                }
            }

            var selectedSlots = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("Select time slots to [green]sign up[/] for (space to select, enter to confirm):")
                    .NotRequired()
                    .PageSize(10)
                    .InstructionsText("[grey](Press <space> to toggle a slot, <enter> to accept)[/]")
                    .AddChoices(slotLabels.Select((label, idx) =>
                        alreadySignedUp.Contains(idx) ? $"[green]{label} (Already Signed Up)[/]" : label)));

            if (currentUser != null)
            {
                for (int i = 0; i < ev.TimeSlots.Count; i++)
                {
                    var label = slotLabels[i];
                    bool shouldBeSignedUp = selectedSlots.Any(s => s.Contains(label));
                    var slot = ev.TimeSlots[i];

                    if (shouldBeSignedUp)
                    {
                        if (slot.Participants == null)
                            slot.Participants = new List<Resident>();
                        if (!slot.Participants.Any(p => p.Id == currentUser.Id))
                            slot.Participants.Add(currentUser);
                    }
                    else
                    {
                        if (slot.Participants != null)
                            slot.Participants.RemoveAll(p => p.Id == currentUser.Id);
                    }
                }

                _eventService.Update(ev);
                AnsiConsole.MarkupLine("[green]Your time slot selections have been updated![/]");
            }
        }

        if (AnsiConsole.Confirm("Save this event to your calendar?"))
        {
            SaveEventToCalendar(ev);
        }
    }
}