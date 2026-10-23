using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.General;
using Farewell.Abstractions.Infrastructure;
using Farewell.Abstractions.Validation;
using Farewell.Debug.Tests.Validation;

namespace Farewell.Debug.Tests.ValidationInfrastructureTests;

public sealed class DbConstraintTests
{
    private readonly InMemoryQueryableRepository<UserEntity> _userRepo = new();
    private readonly InMemoryQueryableRepository<DepartmentEntity> _deptRepo = new();
    private readonly InMemoryQueryableRepository<OrderEntity> _orderRepo = new();

    private (IKeyedServiceProvider scope, IValidationProvider provider) BuildProvider()
    {
        return TestProviderFactory.Create(services =>
        {
            services.AddSingleton<IQueryableRepository<UserEntity>>(_userRepo);
            services.AddSingleton<IQueryableRepository<DepartmentEntity>>(_deptRepo);
            services.AddSingleton<IQueryableRepository<OrderEntity>>(_orderRepo);
        });
    }

    [Fact]
    public async Task DbExists_DepartmentExists_Passes()
    {
        _deptRepo.Seed(new DepartmentEntity { Id = 1, Name = "Engineering" });

        var (scope, provider) = BuildProvider();
        using var _ = scope;

        var cmd = new CreateUserCommand
        {
            Email = "new@test.com",
            Username = "newuser",
            DepartmentId = 1
        };

        var report = await provider.ValidateAsync(cmd, context: "DbCreate",
            ct: TestContext.Current.CancellationToken);
        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task DbExists_DepartmentMissing_FailsNotFound()
    {
        // No departments seeded

        var (scope, provider) = BuildProvider();
        using var _ = scope;

        var cmd = new CreateUserCommand
        {
            Email = "new@test.com",
            Username = "newuser",
            DepartmentId = 42
        };

        var report = await provider.ValidateAsync(cmd, context: "DbCreate",
            ct: TestContext.Current.CancellationToken);
        Assert.False(report.IsSuccess);
        Assert.Contains(report.Errors!, e =>
            e.Code == ResultCode.NotFound &&
            e.PropertyName == nameof(CreateUserCommand.DepartmentId));
    }

    [Fact]
    public async Task DbUnique_NoExisting_Passes()
    {
        _deptRepo.Seed(new DepartmentEntity { Id = 1 });

        var (scope, provider) = BuildProvider();
        using var _ = scope;

        var cmd = new CreateUserCommand
        {
            Email = "unique@test.com",
            Username = "uniqueuser",
            DepartmentId = 1
        };

        var report = await provider.ValidateAsync(cmd, context: "DbCreate",
            ct: TestContext.Current.CancellationToken);
        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task DbUnique_OneExisting_StillPasses()
    {
        // One existing = CountAsync returns 1, <= 1 passes
        _userRepo.Seed(new UserEntity { Id = 1, Email = "taken@test.com", Username = "other" });
        _deptRepo.Seed(new DepartmentEntity { Id = 1 });

        var (scope, provider) = BuildProvider();
        using var _ = scope;

        var cmd = new CreateUserCommand
        {
            Email = "taken@test.com",
            Username = "newuser",
            DepartmentId = 1
        };

        var report = await provider.ValidateAsync(cmd, context: "DbCreate",
            ct: TestContext.Current.CancellationToken);
        // Count == 1 for email, <= 1 passes
        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task DbUnique_TwoExisting_FailsNotUnique()
    {
        _userRepo.Seed(
            new UserEntity { Id = 1, Email = "dup@test.com", Username = "u1" },
            new UserEntity { Id = 2, Email = "dup@test.com", Username = "u2" });
        _deptRepo.Seed(new DepartmentEntity { Id = 1 });

        var (scope, provider) = BuildProvider();
        using var _ = scope;

        var cmd = new CreateUserCommand
        {
            Email = "dup@test.com",
            Username = "newuser",
            DepartmentId = 1
        };

        var report = await provider.ValidateAsync(cmd, context: "DbCreate",
            ct: TestContext.Current.CancellationToken);
        Assert.False(report.IsSuccess);
        Assert.Contains(report.Errors!, e =>
            e.Code == ResultCode.Conflict &&
            e.PropertyName == nameof(CreateUserCommand.Email));
    }

    [Fact]
    public async Task DbNotExists_NotTaken_Passes()
    {
        _deptRepo.Seed(new DepartmentEntity { Id = 1 });

        var (scope, provider) = BuildProvider();
        using var _ = scope;

        var cmd = new CreateUserCommand
        {
            Email = "new@test.com",
            Username = "fresh",
            DepartmentId = 1
        };

        var report = await provider.ValidateAsync(cmd, context: "DbCreate",
            ct: TestContext.Current.CancellationToken);
        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task DbNotExists_AlreadyTaken_FailsConflict()
    {
        _userRepo.Seed(new UserEntity { Id = 1, Email = "other@test.com", Username = "taken" });
        _deptRepo.Seed(new DepartmentEntity { Id = 1 });

        var (scope, provider) = BuildProvider();
        using var _ = scope;

        var cmd = new CreateUserCommand
        {
            Email = "new@test.com",
            Username = "taken",
            DepartmentId = 1
        };

        var report = await provider.ValidateAsync(cmd, context: "DbCreate",
            ct: TestContext.Current.CancellationToken);
        Assert.False(report.IsSuccess);
        Assert.Contains(report.Errors!, e =>
            e.Code == ResultCode.Conflict &&
            e.PropertyName == nameof(CreateUserCommand.Username));
    }

    [Fact]
    public async Task DbCount_WithinLimit_Passes()
    {
        _userRepo.Seed(new UserEntity { Id = 10 });
        _orderRepo.Seed(
            new OrderEntity { Id = 1, UserId = 10, IsActive = true },
            new OrderEntity { Id = 2, UserId = 10, IsActive = true });

        var (scope, provider) = BuildProvider();
        using var _ = scope;

        var cmd = new CreateOrderCommand { UserId = 10 };
        var report = await provider.ValidateAsync(cmd, context: "DbOrder",
            ct: TestContext.Current.CancellationToken);
        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task DbCount_ExceedsLimit_FailsOutOfRange()
    {
        _userRepo.Seed(new UserEntity { Id = 10 });
        for (var i = 1u; i <= 6; i++)
            _orderRepo.Seed(new OrderEntity { Id = i, UserId = 10, IsActive = true });

        var (scope, provider) = BuildProvider();
        using var _ = scope;

        var cmd = new CreateOrderCommand { UserId = 10 };
        var report = await provider.ValidateAsync(cmd, context: "DbOrder",
            ct: TestContext.Current.CancellationToken);
        Assert.False(report.IsSuccess);
        Assert.Contains(report.Errors!, e =>
            e.Code == ResultCode.ValidationOutOfRange &&
            e.PropertyName == nameof(CreateOrderCommand.UserId));
    }

    [Fact]
    public async Task DbAny_AdminExists_Passes()
    {
        _userRepo.Seed(new UserEntity { Id = 1, Role = "Admin" });

        var (scope, provider) = BuildProvider();
        using var _ = scope;

        var cmd = new AssignRoleCommand { Role = "Editor", AccountId = 5 };
        var report = await provider.ValidateAsync(cmd, context: "DbRole",
            ct: TestContext.Current.CancellationToken);
        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task DbAny_NoAdmins_FailsNotFound()
    {
        _userRepo.Seed(new UserEntity { Id = 1, Role = "Editor" });

        var (scope, provider) = BuildProvider();
        using var _ = scope;

        var cmd = new AssignRoleCommand { Role = "Editor", AccountId = 5 };
        var report = await provider.ValidateAsync(cmd, context: "DbRole",
            ct: TestContext.Current.CancellationToken);
        Assert.False(report.IsSuccess);
        Assert.Contains(report.Errors!, e =>
            e.Code == ResultCode.NotFound &&
            e.PropertyName == nameof(AssignRoleCommand.Role));
    }

    [Fact]
    public async Task DbNone_NoMatchingRecords_Passes()
    {
        _userRepo.Seed(new UserEntity { Id = 1, Role = "Admin" });
        // No active orders for userId 999

        var (scope, provider) = BuildProvider();
        using var _ = scope;

        var cmd = new AssignRoleCommand { Role = "Editor", AccountId = 5 };
        var report = await provider.ValidateAsync(cmd, context: "DbRole",
            ct: TestContext.Current.CancellationToken);
        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task DbNone_MatchingRecordsExist_FailsConflict()
    {
        _userRepo.Seed(new UserEntity { Id = 1, Role = "Admin" });
        _orderRepo.Seed(new OrderEntity { Id = 1, UserId = 999, IsActive = true });

        var (scope, provider) = BuildProvider();
        using var _ = scope;

        var cmd = new AssignRoleCommand { Role = "Editor", AccountId = 5 };
        var report = await provider.ValidateAsync(cmd, context: "DbRole",
            ct: TestContext.Current.CancellationToken);
        Assert.False(report.IsSuccess);
        Assert.Contains(report.Errors!, e =>
            e.Code == ResultCode.Conflict &&
            e.PropertyName == nameof(AssignRoleCommand.AccountId));
    }
}