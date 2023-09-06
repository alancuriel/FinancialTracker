using Microsoft.AspNetCore.Identity;
using NSubstitute;
using NSubstitute.Core;

namespace FinancialTracker.Api.Tests;

public static class MockHelpers
{
    public static UserManager<TUser> MockUserManager<TUser>() where TUser : class
    {
        var store = Substitute.For<IUserStore<TUser>>();
        var mgr = Substitute.For<UserManager<TUser>>(store, null, null, null, null, null, null, null, null);
        mgr.UserValidators.Add(new UserValidator<TUser>());
        mgr.PasswordValidators.Add(new PasswordValidator<TUser>());
        return mgr;
    }

    public static RoleManager<TRole> MockRoleManager<TRole>() where TRole : class
    {
        var store = Substitute.For<IRoleStore<TRole>>();
        var mgr = Substitute.For<RoleManager<TRole>>(store, null, null, null, null);
        mgr.RoleValidators.Add(new RoleValidator<TRole>());
        return mgr;
    }

    public static ConfiguredCall TaskThrows<T>(this Task<T> task, Exception exception)
    {
        return task.Returns(Task.FromException<T>(exception));
    }
}
