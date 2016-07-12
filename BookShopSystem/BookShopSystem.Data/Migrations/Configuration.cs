namespace BookShopSystem.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Models;

    public sealed class Configuration : DbMigrationsConfiguration<BookShopSystem.Data.ApplicationDbContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
            this.ContextKey = "BookShopSystem.Data.ApplicationDbContext";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var categoriesCount = context.Categories.Count();
            var athorsCount = context.Authors.Count();
            var booksCount = context.Books.Count();
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            var dirName = Path.GetDirectoryName(path) + "../../../";

            if (categoriesCount == 0)
            {
                AddCategories(context, dirName);
            }
            if (athorsCount == 0)
            {
                AddAuthors(context, dirName);
            }
            if (booksCount == 0)
            {
                AddBooks(context, dirName);
            }
        }

        private void AddBooks(ApplicationDbContext context, string directory)
        {
            string file = directory + "books.txt";
           using (var reader = new StreamReader(file))
            {
                Random random = new Random();
                var line = reader.ReadLine();
                line = reader.ReadLine();
                while (line != null)
                {
                    var data = line.Split(new[] { ' ' }, 6);
                    var authorIndex = random.Next(0, context.Authors.Count());
                    var author = context.Authors.ToList()[authorIndex];
                    var categoryIndex = random.Next(0, context.Categories.Count());
                    var category = context.Categories.ToList()[categoryIndex];
                    var edition = (Edition)int.Parse(data[0]);
                    var releaseDate = DateTime.ParseExact(data[1], "d/M/yyyy", CultureInfo.InvariantCulture);
                    var ageRestriction = (AgeRestriction) int.Parse(data[4]);
                    var copies = int.Parse(data[2]);
                    var price = decimal.Parse(data[3]);
                    var title = data[5];

                    context.Books.Add(new Book()
                    {
                        AuthorId = author.Id,
                        Author = author,
                        Edition = edition,
                        Copies = copies,
                        Price = price,
                        Title = title,
                        ReleaseDate = releaseDate,
                        AgeRestriction = ageRestriction,
                        Categories = new[] { category}
                    });

                    line = reader.ReadLine();
                }
                context.SaveChanges();
            }
        }

        private void AddAuthors(ApplicationDbContext context, string directory)
        {
           string file = directory + "authors.txt";
           using (var reader = new StreamReader(file))
           {
               var line = reader.ReadLine();
               line = reader.ReadLine();
               while (line != null)
               {
                   var data = line.Split(new[] { ' ' }, 2);
                   var firstName = data[0];
                   var lastName = data[1];

                   context.Authors.Add(new Author()
                   {
                       FirstName = firstName,
                       LastName = lastName,
                   });

                   line = reader.ReadLine();
               }
               context.SaveChanges();
           }
        }

        private void AddCategories(ApplicationDbContext context, string directory)
        {
            string file = directory + "categories.txt";
           
              using (var reader = new StreamReader(file))
              {
                  var line = reader.ReadLine();
                  //          line = reader.ReadLine();
                  while (line != null)
                  {
                      var data = line;

                      context.Categories.Add(new Category()
                      {
                          Name = data
                      });

                      line = reader.ReadLine();
                  }
                  context.SaveChanges();
              }
        }
    }
}
