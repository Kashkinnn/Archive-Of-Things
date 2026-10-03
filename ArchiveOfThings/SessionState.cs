namespace ArchiveOfThings;

public class SessionState
{
    public bool IsLoggedIn { get; private set; }
    public string Username { get; private set; } = "";
    public string Initial => Username.Length > 0 ? Username[..1].ToUpper() : "?";

    public void SignIn(string username)
    {
        IsLoggedIn = true;
        Username = username;
    }

    public void SignOut()
    {
        IsLoggedIn = false;
        Username = "";
    }
}
