namespace Omnibill.WebAPI
{
    using System.Collections.Generic;

    public record Document(string Type, List<Page> Pages, string Reference, Recipient Recipient, DocumentPrintInfo Print);
    public record Page(int Number, int Type, string Duplex);
    public record Recipient(string BundlingId, string Name, Address Address, ToEmail email);
    public record Address(string Line1, string Line2, string Line3, string Line4, string Line5, string Line6, string Line7, string Country);
    public record ToEmail(string To, string CC, string BCC);
    public record DocumentPrintInfo(PostalPrepayment PostalPrepayment, Bundling Bundling, bool Folding);
    public record PostalPrepayment(int Zip, string City);
    public record Bundling(int SortNumber, bool RequiresCoversheet);
}