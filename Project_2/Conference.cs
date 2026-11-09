namespace Project_2;

public class Conference
{
    public string Title { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string Organizer { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string Deadline { get; set; } = string.Empty;
    public decimal Fee { get; set; }

    public Conference() { }

    public Conference(string title, string field, decimal fee)
    {
        Title = title;
        Field = field;
        Fee = fee;
    }

    public string GetInfo()
    {
        string orgText = string.IsNullOrWhiteSpace(Organizer) ? "Не вказано" : Organizer;
        string feeText = Fee > 0 ? $"{Fee} грн" : "Безкоштовно";

        return $"«{Title}» ; Галузь: {Field} ; Організатор: {orgText} ; Місце: {Location} ; Дати: {StartDate} ; Дедлайн: {Deadline} ; Вартість: {feeText}";
    }
}