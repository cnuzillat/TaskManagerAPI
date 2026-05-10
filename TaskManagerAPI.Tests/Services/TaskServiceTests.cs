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

        [Fact]
        public async Task GetTasksForUser_ShouldOnlyReturnTasksForSpecifiedUser()
        {
            var context = GetDbContext();

            context.Tasks.AddRange(
                new Models.TaskItem { Title = "User 1 Task", Description = "Description", AssignedUserId = 1, Status = Models.TaskStatus.Open },
                new Models.TaskItem { Title = "User 2 Task", Description = "Description", AssignedUserId = 2, Status = Models.TaskStatus.Open });

            await context.SaveChangesAsync();

            var service = new TaskService(context);

            var tasks = await service.GetTasksForUser(1);

            tasks.Should().HaveCount(1);

            tasks[0].AssignedUserId.Should().Be(1);
            tasks[0].Title.Should().Be("User 1 Task");
        }

        [Fact]
        public async Task GetTaskById_ShouldReturnNull_WhenUserDoesNotOwnTask()
        {
            var context = GetDbContext();

            var task = new Models.TaskItem { Title = "User 1 Task", Description = "Description", AssignedUserId = 1, Status = Models.TaskStatus.Open };
            context.Tasks.Add(task);

            await context.SaveChangesAsync();

            var service = new TaskService(context);

            var result = await service.GetTaskById(task.Id, 2);
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteTask_ShouldReturnFalse_WhenUserDoesNotOwnTask()
        {
            var context = GetDbContext();

            var task = new Models.TaskItem { Title = "User 1 Task", Description = "Description", AssignedUserId = 1, Status = Models.TaskStatus.Open };
            context.Tasks.Add(task);

            await context.SaveChangesAsync();

            var service = new TaskService(context);

            var result = await service.DeleteTask(task.Id, 2);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateTask_ShouldAllowValidStatusTransition()
        {
            var context = GetDbContext();

            var task = new Models.TaskItem { Title = "User 1 Task", Description = "Description", AssignedUserId = 1, Status = Models.TaskStatus.Open };
            context.Tasks.Add(task);

            await context.SaveChangesAsync();

            var service = new TaskService(context);

            var updatedTask = await service.UpdateTask(task.Id, "Updated Title", "Updated Description", Models.TaskStatus.InProgress, 1);

            updatedTask.Should().NotBeNull();

            updatedTask!.Status.Should().Be(Models.TaskStatus.InProgress);
        }

        [Fact]
        public async Task UpdateTask_ShouldThrowException_ForInvalidStatusTransition()
        {
            var context = GetDbContext();

            var task = new Models.TaskItem { Title = "User 1 Task", Description = "Description", AssignedUserId = 1, Status = Models.TaskStatus.Open };
            context.Tasks.Add(task);

            await context.SaveChangesAsync();

            var service = new TaskService(context);

            Func<Task> act = async () => await service.UpdateTask(task.Id, "Updated Title", "Updated Description", Models.TaskStatus.Completed, 1);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task UpdateTask_ShouldRejectChangesToCompletedTask()
        {
            var context = GetDbContext();

            var task = new Models.TaskItem { Title = "User 1 Task", Description = "Description", AssignedUserId = 1, Status = Models.TaskStatus.Completed };
            context.Tasks.Add(task);

            await context.SaveChangesAsync();

            var service = new TaskService(context);

            Func<Task> act = async () => await service.UpdateTask(task.Id, "Updated Title", "Updated Description", Models.TaskStatus.InProgress, 1);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
