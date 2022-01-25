using Boggle.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Boggle.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(int count=4)
        {
            Word word = new Word(Http.RandomWord(count));
            word.count = count;
            Request.HttpContext.Session.SetString("Boggle", JsonConvert.SerializeObject(word));
            return View(word);
        }

        public JsonResult Answers()
        {
            Word word = JsonConvert.DeserializeObject<Word>(Request.HttpContext.Session.GetString("Boggle"));

            if(word.guess.Equals(word.original))
            {
                word.alert = "YOU GUESSED THE WORD!";
            }
            else if (word.answers.Contains(word.guess) || word.errors.Contains(word.guess))
            {
                word.alert = "Cannot use the same guess twice";
            }
            else
            {
                bool valid = Http.SearchWord(word.guess);
                word.alert = null;

                if (valid)
                {
                    word.answers_count++;
                    word.answers += (word.guess + "\r\n");
                }
                else
                {
                    word.errors_count++;
                    word.errors += (word.guess + "\r\n");
                }
            }
            word.guess = "";
            Request.HttpContext.Session.SetString("Boggle", JsonConvert.SerializeObject(word));
            return new JsonResult(word);
        }

        public JsonResult Select(string cell)
        {
            Word word = JsonConvert.DeserializeObject<Word>(Request.HttpContext.Session.GetString("Boggle"));

            word.guess += word.random.Substring(Int32.Parse(cell.Replace("R_cell_", "")), 1);
            word.cell = cell;
            Request.HttpContext.Session.SetString("Boggle", JsonConvert.SerializeObject(word));

            return new JsonResult(word);
        }

        public JsonResult Hint()
        {
            Word word = JsonConvert.DeserializeObject<Word>(Request.HttpContext.Session.GetString("Boggle"));
            word.guess = word.original.Substring(0, 1);
            Request.HttpContext.Session.SetString("Boggle", JsonConvert.SerializeObject(word));

            Hint hint = new Hint() { letter = word.guess[0], index = word.random.IndexOf(word.guess[0]) };
            return new JsonResult(hint);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
