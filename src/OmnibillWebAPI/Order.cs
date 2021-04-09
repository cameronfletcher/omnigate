namespace Omnibill.WebAPI
{
    using System;
    using System.Collections.Generic;

    public record Order(string Type, string Reference, DateTime? ExecuteAt, OrderPrintInfo Print, OrderEmailInfo Email);
    public record OrderEmailInfo(string Name, string Address, EmailTemplate Template);
    public record EmailTemplate(string Id, string Language, string Subject);

    public record OrderPrintInfo(
        string Speed,
        string ApplyDMX4,
        string InstructionForReturn,
        string ReturnAdressID,
        string ConsignmentPurpose,
        string ClientInformation,
        List<string> Supplements);

    // need to consider links to documents
}
