using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Tests.Services
{
    public class TaskServiceTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateTask_ShouldAssignCorrectUser()
        {
            var context = GetDbContext();

            var service = new TaskService(context);

            var userId = 1;

            var task = await service.CreateTask("Test Task", "Test Description", userId);

            task.AssignedUserId.Should().Be(userId);
            task.Status.Should().Be(Models.TaskStatus.Open);
            task.Title.Should().Be("Test Task");
        }
    }
}
