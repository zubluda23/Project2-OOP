namespace  Project_2
{
    public class FilterCriteria
    {
        public string SelectedField { get; set; } = string.Empty;
        public string SelectedOrganizer { get; set; } = string.Empty;
        public string Keyword { get; set; } = string.Empty;

        public FilterCriteria(string field, string organizer, string keyword)
        {
            SelectedField = field;
            SelectedOrganizer = organizer;
            Keyword = keyword;
        }
    }
}