using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamOps.Domain.Tasks;

namespace TeamOps.Infrastructure.Persistence.Configurations;

public sealed class TaskConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("tasks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.ProjectId).IsRequired();

        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();

        builder.Property(x => x.Status).IsRequired();

        builder.Property(x => x.AssigneeId);
    }
}