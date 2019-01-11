using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using SamuraiApp.Data;
using SamuraiApp.Domain;

namespace SomeUI
{
  internal class Program
  {
    //do not use a staic dbcontext instead create a new instance within method
    private static SamuraiContext _context = new SamuraiContext();

    private static void Main(string[] args)
    {
            _context.Database.EnsureCreated();
            _context.GetService<ILoggerFactory>().AddProvider(new MyLoggerProvider());

            //InsertNewPkFkGraph();
            //InsertNewOneToOneGraph();
            //AddChildToExistingObjectWhileTracked();
            //AddOneToOneToExistingObjectWhileTracked();
            //AddBattles();
            //AddManyToManyWithFks();
            //AddManyToManyWithObjects();
            //EagerLoadWithInclude();
            //EagerLoadManyToManyAkaChildrenGrandchildren();
            //EagerLoadWithMultipleBranches();
            //EagerLoadWithFromSql();
            //AnonymousTypeViaProjection();
            //AnonymousTypeViaProjectionWithRelated();
            //ExplicitLoad();
            //ExplicitLoadWithChildFilter();
            UsingRelatedDataForFiltersAndMore();

            Console.ReadLine();
      #region inactive methods
      
      //EagerLoadAllOrNothingChildrenNope();
      //FilterAcrossRelationship();
      //EagerLoadViaProjectionNotQuite();
      //EagerLoadViaProjectionAndScalars();
      //FilteredEagerLoadViaProjectionNope();

      #endregion
    }

        private static void InsertNewPkFkGraph()
        {
            var samurai = new Samurai
            {
                Name = "Kambei Shimada",
                Quotes = new List<Quote>
                               {
                                 new Quote {Text = "I've come to save you"}
                               }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }
        private static void InsertNewOneToOneGraph()
        {
            var samurai = new Samurai
            {
                Name = "Shichiroji"
            };
            samurai.SecretIdentity = new SecretIdentity
            {
                RealName = "Julie"
            };
            _context.Add(samurai);
            _context.SaveChanges();
        }
        private static void AddChildToExistingObjectWhileTracked()
        {
            var samurai = _context.Samurais.First();
            samurai.Quotes.Add(
                new Quote
                {
                    Text = "I bet you're happy I've saved you!"
                });
            _context.SaveChanges();
        }
        private static void AddOneToOneToExistingObjectWhileTracked()
        {
            var samurai = _context.Samurais
                .FirstOrDefault(s => s.SecretIdentity == null);
            samurai.SecretIdentity = new SecretIdentity { RealName = "Sampson" };
            _context.SaveChanges();
        }
        private static void AddBattles()
        {
            _context.Battles.AddRange(
              new Battle { Name = "Battle of Shiroyama", StartDate = new DateTime(1877, 9, 24), EndDate = new DateTime(1877, 9, 24) },
              new Battle { Name = "Siege of Osaka", StartDate = new DateTime(1614, 1, 1), EndDate = new DateTime(1615, 12, 31) },
              new Battle { Name = "Boshin War", StartDate = new DateTime(1868, 1, 1), EndDate = new DateTime(1869, 1, 1) }
              );
            _context.SaveChanges();
        }
        private static void AddManyToManyWithFks()
        {
            _context = new SamuraiContext();
            var sb = new SamuraiBattle { SamuraiId = 1, BattleId = 1 };
            _context.SamuraiBattles.Add(sb);
            _context.SaveChanges();
        }
        private static void AddManyToManyWithObjects()
        {
            _context = new SamuraiContext();
            var samurai = _context.Samurais.FirstOrDefault();
            var battle = _context.Battles.FirstOrDefault();
            _context.SamuraiBattles.Add(
             new SamuraiBattle { Samurai = samurai, Battle = battle });
            _context.SaveChanges();
        }
        private static void EagerLoadWithInclude()
        {
            _context = new SamuraiContext();
            var samuraiWithQuotes = _context.Samurais.Include(s => s.Quotes).ToList();
        }
        private static void EagerLoadManyToManyAkaChildrenGrandchildren()
        {
            _context = new SamuraiContext();
            var samuraiWithBattles = _context.Samurais
              .Include(s => s.SamuraiBattles)
              .ThenInclude(sb => sb.Battle).ToList();
        }
        private static void EagerLoadWithMultipleBranches()
        {
            _context = new SamuraiContext();
            var samurais = _context.Samurais
              .Include(s => s.SecretIdentity)
              .Include(s => s.Quotes).ToList();
        }
        private static void EagerLoadWithFromSql()
        {
            var samurais = _context.Samurais.FromSql("select * from samurais")
              .Include(s => s.Quotes)
              .Include(s => s.SecretIdentity)
              .ToList();
        }
        private static void AnonymousTypeViaProjection()
        {
            _context = new SamuraiContext();
            var quotes = _context.Quotes
              .Select(q => new { q.Id, q.Text })
              .ToList();
        }

        private static void AnonymousTypeViaProjectionWithRelated()
        {
            _context = new SamuraiContext();
            var samurais = _context.Samurais
              .Select(s => new {
                  s.Id,
                  s.SecretIdentity.RealName,
                  QuoteCount = s.Quotes.Count
              })
              .ToList();
        }

        private static void RelatedObjectsFixUp()
        {
            _context = new SamuraiContext();
            var samurai = _context.Samurais.Find(1);
            var quotes = _context.Quotes.Where(q => q.SamuraiId == 1).ToList();
        }

        private static void ExplicitLoad()
        {
            _context = new SamuraiContext();
            var samurai = _context.Samurais.FirstOrDefault();
            _context.Entry(samurai).Collection(s => s.Quotes).Load();
            _context.Entry(samurai).Reference(s => s.SecretIdentity).Load();
        }

        private static void ExplicitLoadWithChildFilter()
        {
            _context = new SamuraiContext();
            var samurai = _context.Samurais.FirstOrDefault();

            // _context.Entry(samurai)
            //   .Collection(s => s.Quotes.Where(q=>q.Text.Contains("happy"))).Load();

            _context.Entry(samurai)
              .Collection(s => s.Quotes)
              .Query()
              .Where(q => q.Text.Contains("happy"))
              .Load();
        }
        private static void UsingRelatedDataForFiltersAndMore()
        {
            _context = new SamuraiContext();
            var samurais = _context.Samurais
              .Where(s => s.Quotes.Any(q => q.Text.Contains("happy")))
              .ToList();
        }

        #region Unrelated entity functions
        //private static void RawSqlCommandWithOutput()
        //{
        //    var procResult = new SqlParameter
        //    {
        //        ParameterName = "@procResult",
        //        SqlDbType = SqlDbType.VarChar,
        //        Direction = ParameterDirection.Output,
        //        Size = 50
        //    };
        //    _context.Database.ExecuteSqlCommand
        //        (
        //            "exec FindLongestName @procResult OUT",
        //            procResult
        //        );
        //    Console.WriteLine($"Longest name: {procResult.Value}");
        //}

        //private static void RawSqlCommand()
        //{
        //    var affected = _context.Database.ExecuteSqlCommand(
        //        "UPDATE samurais SET Name=REPLACE(Name, 'Jayden', 'Jay')");

        //    Console.WriteLine($"Affected rows {affected}");
        //}

        //private static void RawSqlQuery()
        //{
        //    //var samurais = _context.Samurais
        //    //    .FromSql("SELECT * FROM Samurais")
        //    //    .OrderByDescending(s=> s.Name)
        //    //    .ToList();
        //    var namePart = "Jay";
        //    var samurais = _context.Samurais
        //        .FromSql($"EXEC FilterSamuraiByNamePart {namePart}")
        //        .OrderByDescending(c=> c.Name)
        //        .ToList();
        //    samurais.ForEach(s => Console.WriteLine(s.Name));
        //    Console.WriteLine();
        //}

        //private static void RetrieveAndUpdateSamurai()
        //{
        //    throw new NotImplementedException();
        //}

        //private static void MoreQueries()
        //{
        //    var name = "Jayden";
        //    var samurais = _context.Samurais.FirstOrDefault(s => s.Name == name);
        //}

        //private static void InsertMultipleSamurais()
        //{
        //    var samurai = new Samurai { Name = "Jazz" };
        //    var samurai2 = new Samurai { Name = "Will" };
        //    using (var context = new SamuraiContext())
        //    {
        //        context.GetService<ILoggerFactory>().AddProvider(new MyLoggerProvider());
        //        context.Samurais.AddRange(new List<Samurai> { samurai, samurai2 });
        //        context.SaveChanges();
        //    }
        //}

        //private static void InsertSamurai()
        //{
        //    var samurai = new Samurai { Name = "Jayden"};
        //    using (var context = new SamuraiContext())
        //    {
        //        context.GetService<ILoggerFactory>().AddProvider(new MyLoggerProvider());
        //        context.Samurais.Add(samurai);
        //        context.SaveChanges();
        //    }
        //}

        //private static void SimpleSamuraiQuery()
        //{
        //    using (var context = new SamuraiContext())
        //    {
        //        var samurais = context.Samurais.ToList();
        //        var query = context.Samurais;
        //        //var samuraisAgain = query.ToList();
        //        foreach (var samurai in query)
        //        {
        //            Console.WriteLine(samurai.Name);
        //        }
        //    }
        //}
        #endregion
    }
}