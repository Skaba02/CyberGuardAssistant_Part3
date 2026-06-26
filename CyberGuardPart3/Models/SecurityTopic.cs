namespace CyberGuardAssistant.Models
{
    // Categories for grouping cybersecurity topics
    // Carried over from Part 1 - unchanged
    public enum SecurityCategory
    {
        Fundamentals,
        ThreatDetection,
        Prevention,
        IncidentResponse,
        GeneralInfo
    }

    // Represents a single cybersecurity topic with related search terms
    public class SecurityTopic
    {
        public string Keyword { get; set; }
        public SecurityCategory Category { get; set; }
        public string[] RelatedTerms { get; set; }

        public SecurityTopic(string keyword, SecurityCategory category, string[] relatedTerms)
        {
            Keyword = keyword;
            Category = category;
            RelatedTerms = relatedTerms;
        }
    }
}
