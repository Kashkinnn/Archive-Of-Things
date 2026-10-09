using ArchiveOfThings.Data;
using ArchiveOfThings.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArchiveOfThings.Model.Features.Login;

public record LoginCommand(string Username, string Password);

public class LoginHandler
{
    private readonly ArchiveDbContext _db;
    private readonly SessionState _session;

    public LoginHandler(ArchiveDbContext db, SessionState session)
    {
        _db = db;
        _session = session;
    }

    public async Task<bool> HandleAsync(LoginCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Username) || string.IsNullOrWhiteSpace(command.Password))
            return false;

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Username == command.Username);

        if (user is null || !BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash))
            return false;

        _session.SignIn(user.Username);
        return true;
    }
}
