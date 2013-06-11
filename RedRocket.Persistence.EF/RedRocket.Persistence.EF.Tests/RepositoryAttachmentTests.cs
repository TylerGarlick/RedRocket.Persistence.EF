using System.Linq;
using FlitBit.Copy;
using FlitBit.IoC;
using FlitBit.Wireup;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedRocket.Persistence.EF;
using RedRocket.Repositories.EF.Tests.Entities;

namespace RedRocket.Repositories.EF.Tests
{
    [TestClass]
    public class RepositoryAttachmentTests
    {
        [TestInitialize]
        public void Init()
        {
            WireupCoordinator.SelfConfigure();
        }

        [TestMethod]
        public void Existing_Entity_Should_Update_Automatically()
        {
            using (var container = Create.SharedOrNewContainer())
            {
                SeedWithData();
                var humanRepository = Create.New<IRepository<Human>>();
                var chuck = humanRepository.All().FirstOrDefault(h => h.LastName.Equals("Norris"));
                var chuckDto = Create.NewInit<IHumanDto>().Init(chuck);
                Assert.AreEqual(chuck.FirstName, chuckDto.FirstName);
                Assert.AreEqual(chuck.LastName, chuckDto.LastName);
                Assert.AreEqual(chuck.Id, chuckDto.Id);

                chuckDto.LastName = "Garlick";
                chuck = Copier<Human>.CopyConstruct(chuckDto);
                chuck = humanRepository.Update(chuck);

                Assert.AreEqual(chuck.FirstName, chuckDto.FirstName);
                Assert.AreEqual(chuck.LastName, chuckDto.LastName);
                Assert.AreEqual(chuck.Id, chuckDto.Id);
            }
        }

        void SeedWithData()
        {
            var context = Create.New<TestDbContext>();

            foreach (var human in context.Humans)
            {
                context.Humans.Remove(human);
            }

            context.SaveChanges();

            context.Humans.Add(new Human()
            {
                FirstName = "Bob",
                LastName = "Sagot"
            });


            context.Humans.Add(new Human()
            {
                FirstName = "Chuck",
                LastName = "Norris"
            });
            context.Humans.Add(new Human()
            {
                FirstName = "Tommy",
                LastName = "Boy"
            });

            context.SaveChanges();
        }
    }
}