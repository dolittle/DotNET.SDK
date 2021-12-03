using Dolittle.SDK.Events;

[EventType("7d0bf4d5-c614-4f60-b590-75b3e2479fbc")]
public record ChefCheckedOut(string Chef, int HoursWorked);