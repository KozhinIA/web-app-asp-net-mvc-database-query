using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using WebAppAspNetMvcDatabaseQuery.Models;

namespace WebAppAspNetMvcDatabaseQuery.Controllers
{
    public class BooksController : Controller
    {
        public readonly string _connectionString = WebConfigurationManager.AppSettings["ConnectionString"];

        [HttpGet]
        public ActionResult Index()
        {           
            var books = SelectBooks();
            return View(books);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var book = new Book();

            return View(book);
        }

        [HttpPost]
        public ActionResult Create(Book model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.CreateAt = DateTime.Now;

            InsertBook(model);

            return RedirectPermanent("/Books/Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var book = SelectBooks().FirstOrDefault(x => x.Id == id);
            if (book == null)
                return RedirectPermanent("/Books/Index");

            DeleteBook(book);

            return RedirectPermanent("/Books/Index");
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var book = SelectBooks().FirstOrDefault(x => x.Id == id);
            if (book == null)
                return RedirectPermanent("/Books/Index");

            return View(book);
        }

        [HttpPost]
        public ActionResult Edit(Book model)
        {
            var book = SelectBooks().FirstOrDefault(x => x.Id == model.Id);
            if (book == null)
                ModelState.AddModelError("Id", "Книга не найдена");

            if (!ModelState.IsValid)
                return View(model);

            MappingBook(model, book);

            UpdateBook(book);

            return RedirectPermanent("/Books/Index");
        }

        private void MappingBook(Book sourse, Book destination)
        {
            destination.Name = sourse.Name;
            destination.Author = sourse.Author;
            destination.Isbn = sourse.Isbn;
            destination.Year = sourse.Year;
        }



        public void InsertBook(Book book)
        {
            IDbConnection connection = new SqlConnection(_connectionString);

            string cmdStr = $@"INSERT INTO [{connection.Database}].[dbo].[Books] ([Name], [Year], [Author], [Isbn], [CreateAt])  VALUES (@Name,@Year, @Author, @Isbn, @CreateAt)";
            IDbCommand cmd = new SqlCommand(cmdStr);
            cmd.Connection = connection;
            connection.Open();

            cmd.Parameters.Add(new SqlParameter("@Name", book.Name));
            cmd.Parameters.Add(new SqlParameter("@Year", book.Year));
            cmd.Parameters.Add(new SqlParameter("@Author", book.Author));
            cmd.Parameters.Add(new SqlParameter("@Isbn", book.Isbn));
            cmd.Parameters.Add(new SqlParameter("@CreateAt", book.CreateAt));

            cmd.ExecuteNonQuery();
            connection.Close();
        }
        public void UpdateBook(Book book)
        {
            IDbConnection connection = new SqlConnection(_connectionString);

            string cmdStr = $@"UPDATE [{connection.Database}].[dbo].[Books] SET [Name] = @Name, [Year] = @Year , [Author] = @Author, [Isbn] = @Isbn, [CreateAt] = @CreateAt WHERE Id = {book.Id}";
            IDbCommand cmd = new SqlCommand(cmdStr);
            cmd.Connection = connection;
            connection.Open();

            cmd.Parameters.Add(new SqlParameter("@Name", book.Name));
            cmd.Parameters.Add(new SqlParameter("@Year", book.Year));
            cmd.Parameters.Add(new SqlParameter("@Author", book.Author));
            cmd.Parameters.Add(new SqlParameter("@Isbn", book.Isbn));
            cmd.Parameters.Add(new SqlParameter("@CreateAt", book.CreateAt));

            cmd.ExecuteNonQuery();
            connection.Close();
        }
        public void DeleteBook(Book book)
        {
            IDbConnection connection = new SqlConnection(_connectionString);

            string cmdStr = $@"DELETE [{connection.Database}].[dbo].[Books] WHERE Id = {book.Id}";
            IDbCommand cmd = new SqlCommand(cmdStr);
            cmd.Connection = connection;
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }
        public List<Book> SelectBooks()
        {
            IDbConnection connection = new SqlConnection(_connectionString);

            string cmdStr = $"SELECT * FROM [{connection.Database}].[dbo].[Books]";
            IDbCommand cmd = new SqlCommand(cmdStr);
            cmd.Connection = connection;
            connection.Open();

            IDataReader read = cmd.ExecuteReader();
            var books = new List<Book>();
            while (read.Read())
            {
                var parser = read.GetRowParser<Book>(typeof(Book));
                var book = parser(read);
                books.Add(book);
            }

            connection.Close();
            return books;
        }
    }
}