using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CarDealer.LeadAutomation.Contracts.Validators;

public enum EmailCheckResult
{
    Approved,
    Unapproved,
    Unknown
}

public class EmailValidator: IEmailValidator
{
    
    /// <summary>
    /// We first check against a local list of approved and a list of known disposable domains.
    /// This is to avoid unnecessary external calls for known good and bad emails.
    /// If the email is not found in either list, we will check it against an external API to see if it's a disposable email address.
    /// In a production environment, I would likely keep the json somewhere else and pull the results via REST endpoint so the list can be updated without requiring a PR.
    /// Also, I would likely want to cache the results of the external API calls to avoid hitting rate limits and improve performance, but for this example, I will keep it simple and just call the API directly.
    /// </summary> <param name="email">The email address to check.</param>
    /// <returns>True if the email is valid (approved or not found in unapproved and not disposable), false if it's unapproved or disposable.</returns>
    public async Task<bool> IsValidEmail(string email)
    {
        var localStatus = CheckLocalEmailLists(email);
        return localStatus switch
        {
            EmailCheckResult.Approved => true,
            EmailCheckResult.Unapproved => false,
            _ => await CheckEmailExternally(email)
        };
    }
    
    private async Task<bool> CheckEmailExternally(string email)
    {
        var domain = email.Split('@').LastOrDefault();
        var url = $"https://isfakemail.com/api/check?url={domain}";
        var httpClient = new HttpClient();
        try
        {
            var json = await httpClient.GetStringAsync(url);
            var obj = System.Text.Json.JsonDocument.Parse(json);
            var isDisposable = obj.RootElement.GetProperty("isDisposable").GetBoolean();
            return !isDisposable; // true = dis, false = disposable
        }
        catch
        {
            // if I didn't get a response, I will assume it's valid and let the local lists handle it
            return true;
        }
    }
    
    private EmailCheckResult CheckLocalEmailLists(string email)
    {
        var approved = LoadEmailList("Contracts/Validators/ApprovedEmails.json");
        var unapproved = LoadEmailList("Contracts/Validators/UnapprovedEmails.json");

        email = email.ToLowerInvariant();
        var domain = email.Split('@').LastOrDefault().Split('.').FirstOrDefault();

        if (approved.Contains(domain))
            return EmailCheckResult.Approved;

        if (unapproved.Contains(domain))
            return EmailCheckResult.Unapproved;

        return EmailCheckResult.Unknown;
    }
    private HashSet<string> LoadEmailList(string path)
    {
        var json = File.ReadAllText(path);
        var list = JsonSerializer.Deserialize<List<string>>(json);
        return new HashSet<string>(list.Select(e => e.ToLowerInvariant()));
    }
}