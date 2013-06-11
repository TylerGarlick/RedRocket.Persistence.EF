using System;
using FlitBit.Dto;

namespace RedRocket.Repositories.EF.Tests.Entities
{
    public abstract class AbstractEntity
    {
        protected AbstractEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class Human : AbstractEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    
    [DTO]
    public interface IHumanDto
    {
        Guid Id { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
    }
}
