namespace Infrastructure.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Todos;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class TodoConfiguration : IEntityTypeConfiguration<Todo>
{
    public void Configure(EntityTypeBuilder<Todo> builder)
    {
        builder.ToTable("Todos");

        builder.HasKey(todo => todo.Id);
        builder.Property(todo => todo.Id).ValueGeneratedNever();

        builder.Property(todo => todo.Title).IsRequired();
        builder.Property(todo => todo.Description).IsRequired();
        builder.Property(todo => todo.IsCompleted).IsRequired();
        builder.Property(todo => todo.CreatedAt).IsRequired();
        builder.Property(todo => todo.UpdatedAt).IsRequired();
    }
}
